using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;

namespace TopDownGame
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private Texture2D playerTexture;
        private Texture2D enemyTexture;
        private Vector2 playerPosition;
        private Vector2 jiggleOffset;
        private Vector2 velocity;
        private float playerSpeed = 200f; // Adjust player speed
        private float jiggleStrength = 10f;
        private float damping = 0.9f;
        private float springConstant = 0.3f;
        private List<Enemy> enemies;
        private float enemySpawnTimer = 0f;
        private float enemySpawnInterval = 1.5f; // Adjust spawn interval as needed
        private int playerHealth = 100;
        private int maxPlayerHealth = 100;
        private int score = 0;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this)
            {
                PreferredBackBufferWidth = 800,
                PreferredBackBufferHeight = 600
            };
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            playerPosition = new Vector2(100, 100);
            jiggleOffset = Vector2.Zero;
            velocity = Vector2.Zero;
            enemies = new List<Enemy>();
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            playerTexture = Content.Load<Texture2D>("walta"); // Replace with your player texture
            enemyTexture = Content.Load<Texture2D>("waltawife"); // Replace with your enemy texture
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // Handle enemy spawning
            enemySpawnTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (enemySpawnTimer >= enemySpawnInterval)
            {
                // Spawn enemy
                SpawnEnemy();
                enemySpawnTimer = 0f; // Reset timer
            }

            // Update Walter (player) movement and jiggle physics (as before)
            var state = Keyboard.GetState();
            var deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            var movement = Vector2.Zero;

            if (state.IsKeyDown(Keys.W))
                movement.Y -= playerSpeed * deltaTime;
            if (state.IsKeyDown(Keys.S))
                movement.Y += playerSpeed * deltaTime;
            if (state.IsKeyDown(Keys.A))
                movement.X -= playerSpeed * deltaTime;
            if (state.IsKeyDown(Keys.D))
                movement.X += playerSpeed * deltaTime;

            var newPosition = playerPosition + movement;

            // Collision detection with window borders
            if (newPosition.X >= 0 && newPosition.X <= _graphics.PreferredBackBufferWidth - playerTexture.Width)
                playerPosition.X = newPosition.X;
            if (newPosition.Y >= 0 && newPosition.Y <= _graphics.PreferredBackBufferHeight - playerTexture.Height)
                playerPosition.Y = newPosition.Y;

            // Jiggle physics
            var jiggleForce = -springConstant * jiggleOffset;
            velocity += jiggleForce;
            velocity *= damping;
            jiggleOffset += velocity * deltaTime;

            if (movement != Vector2.Zero)
            {
                jiggleOffset += movement * jiggleStrength * deltaTime;
            }

            // Update enemies
            foreach (var enemy in enemies.ToList())
            {
                enemy.Update(gameTime, playerPosition); // Update enemy movement towards Walter

                // Collision detection between player and enemies
                Rectangle playerRect = new Rectangle(
                    (int)(playerPosition.X + jiggleOffset.X),
                    (int)(playerPosition.Y + jiggleOffset.Y),
                    (int)(playerTexture.Width * 0.5f),
                    (int)(playerTexture.Height * 0.5f)
                );

                Rectangle enemyRect = new Rectangle(
                    (int)enemy.Position.X,
                    (int)enemy.Position.Y,
                    enemy.Texture.Width,
                    enemy.Texture.Height
                );

                if (playerRect.Intersects(enemyRect))
                {
                    // Calculate direction vector from player to enemy
                    Vector2 direction = enemy.Position - playerPosition;

                    // Normalize direction
                    if (direction != Vector2.Zero)
                        direction.Normalize();

                    // Apply a force to push the player away from the enemy
                    const float pushForce = 200f;
                    velocity += -direction * pushForce;

                    // Example: Player loses health when colliding with an enemy
                    playerHealth -= 10; // Adjust damage amount
                    if (playerHealth <= 0)
                    {
                        // Game over logic
                        // Example: Reset game or show game over screen
                    }

                    // Remove enemy upon collision
                    enemies.Remove(enemy);

                    // Increase score when an enemy is defeated
                    score += 10; // Adjust score increment
                }
            }

            base.Update(gameTime);
        }

        private void SpawnEnemy()
        {
            // Example: Spawn enemy at a random position outside the screen area
            Random random = new Random();
            Vector2 spawnPosition = new Vector2(
                _graphics.PreferredBackBufferWidth + enemyTexture.Width, // X position outside screen
                random.Next(0, _graphics.PreferredBackBufferHeight - enemyTexture.Height) // Random Y position within screen
            );

            Enemy newEnemy = new Enemy(enemyTexture, spawnPosition);
            enemies.Add(newEnemy);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin();

            // Draw enemies
            foreach (var enemy in enemies)
            {
                enemy.Draw(_spriteBatch);
            }

            // Draw Walter (player) with jiggle effect
            _spriteBatch.Draw(playerTexture, playerPosition + jiggleOffset, null, Color.White, 0f, Vector2.Zero, 0.5f, SpriteEffects.None, 0f);

            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}



















