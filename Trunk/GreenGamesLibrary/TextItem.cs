using System;
using System.Collections.Generic;
using System.Linq;
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
    public class TextItem : Sprite
    {
        /// <summary>
        /// The text that should be displayed
        /// </summary>
        public string text = "";

        /// <summary>
        /// The font that the text is in
        /// </summary>
        public SpriteFont font;

        /// <summary>
        /// Makes a new TextItem
        /// </summary>
        /// <param name="loadedFont">The SpriteFont that the text item should have</param>
        public TextItem(SpriteFont loadedFont) : base(loadedFont)
        {
            font = loadedFont;
            rect = new Rectangle((int)pos.X, (int)pos.Y, (int)font.MeasureString(text).X, (int)font.MeasureString(text).Y);
        }

        /// <summary>
        /// Makes a new TextItem and sets all usefull properties
        /// </summary>
        /// <param name="loadedFont">The SpriteFont that the text item should have</param>
        /// <param name="loadedText">The text that should be displayed</param>
        /// <param name="position">The position that the text item should initiall have, use the GetDefaultLocation <see cref="Menu.cs"/> </param>
        public TextItem(SpriteFont loadedFont, string loadedText, Vector2 position) : base(loadedFont)
        {
            font = loadedFont;
            rect = new Rectangle((int)pos.X, (int)pos.Y, (int)font.MeasureString(text).X, (int)font.MeasureString(text).Y);
            text = loadedText;
            pos = position;
        }

        /// <summary>
        /// Measures the text item, easier and shorter than spritefont.MeasureString(text)
        /// </summary>
        /// <returns>MeasureString from the SpriteFont</returns>
        public Vector2 MeasureString()
        {
            return font.MeasureString(text);
        }
    }
}
