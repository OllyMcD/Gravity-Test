using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace TopDownGame
{
    public class Enemy
    {
        public Texture2D Texture { get; private set; }
        public Vector2 Position { get; private set; }

        public Enemy(Texture2D texture, Vector2 position)
        {
            Texture = texture;
            Position = position;
        }

        public void Update(GameTime gameTime, Vector2 playerPosition)
        {
            // Example: Implement enemy movement logic towards player
            // This is placeholder code; actual logic depends on your game design
            Vector2 direction = playerPosition - Position;
            direction.Normalize();
            Position += direction * 100f * (float)gameTime.ElapsedGameTime.TotalSeconds; // Adjust enemy speed
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture, Position, Color.White);
        }
    }
}



