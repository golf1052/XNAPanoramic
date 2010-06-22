using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using Microsoft.Xna.Framework.Media;

namespace GreenGamesLibrary
{
    public class Sprite
    {
        public Texture2D tex;
        public bool alive;
        public int health;
        public Vector2 pos = Vector2.Zero;
        public Vector2 vel = Vector2.Zero;
        public Rectangle rect;
        public Color color = Color.White;
        public Color[] colorData;
        public Bullet[] bullets;

        public Sprite(Texture2D loadedTex)
        {
            tex = loadedTex;
            colorData = new Color[tex.Width * tex.Height];
            tex.GetData(colorData);
            rect = new Rectangle((int)pos.X, (int)pos.Y, tex.Width, tex.Height);
        }

        public Sprite(SpriteFont loadedFont)
        {
        }

        public void UpdateRect()
        {
            rect = new Rectangle((int)pos.X, (int)pos.Y, tex.Width, tex.Height);
        }

        public void FireBullet(Sprite sprite, Vector2 bulletVelocity)
        {
            foreach (Bullet bullet in bullets)
            {
                if (bullet.alive == false)
                {
                    bullet.alive = true;
                    bullet.pos = new Vector2(sprite.pos.X + (sprite.tex.Width / 2), sprite.pos.Y);
                    bullet.vel += bulletVelocity;
                    return;
                }
            }
        }

        public void UpdateBullet(GraphicsDevice graphics, GameTime gameTime)
        {
            foreach (Bullet bullet in bullets)
            {
                if (bullet.alive == true)
                {
                    bullet.pos += bullet.vel;
                    bullet.rect = new Rectangle((int)bullet.pos.X, (int)bullet.pos.Y, bullet.tex.Width, bullet.tex.Height);
                    if (bullet.pos.Y <= 0.0f)
                    {
                        bullet.alive = false;
                        continue;
                    }
                }
            }
        }

        public void DrawSprite(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(tex, pos, color);
        }
    }
}
