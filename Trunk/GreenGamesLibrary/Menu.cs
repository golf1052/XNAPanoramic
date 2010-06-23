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
    public class Menu
    {
        // For Metro UI
        // Panorama Specifications
        // Title
            //Font - Segoe UI(WP) Light
            //Size - 140
        // Section Title
            //Font - Segoe UI(WP) Semilight
            //Size - 54
        // Menu Items
            //Font - Segoe UI(WP) Semilight
            //Size - 24
        // Image about Panoramic Info
        // http://i50.tinypic.com/2ld7wia.jpg

        /// <summary>
        /// The sections and menus used in a game, feel free to add your own
        /// </summary>
        public enum Sections
        {
            MainSection,
            SettingsSection1,
            SettingsSection2,
            HelpSection,
            GameMenu,
            ExitSection,
            Examples2D,
            None,
            Example1,
            Example2
        }

        /// <summary>
        /// The direction the sections can scroll
        /// </summary>
        public enum Direction
        {
            Forwards,
            Backwards
        }

        /// <summary>
        /// The location of a text item
        /// </summary>
        public enum TextLocation
        {
            Title,
            Section,
            Menu
        }

        /// <summary>
        /// The current section, the active section
        /// </summary>
        public Sections currentSection = Sections.None;

        /// <summary>
        /// The previous section, the previously active section
        /// </summary>
        public Sections previousSection = Sections.MainSection;

        /// <summary>
        /// The sections in a panoramic menu and the order they go in, start with 0 (like an array)
        /// </summary>
        public Dictionary<Sections, int> sectionList = new Dictionary<Sections, int>();

        /// <summary>
        /// The section list but in reverse, numbers first then sections, is auto generated by GenerateNumberedSectionList method
        /// </summary>
        public Dictionary<int, Sections> numberedSectionList = new Dictionary<int, Sections>();

        /// <summary>
        /// The sections in a panoramic menu and the text item it is linked to
        /// </summary>
        public Dictionary<Sections, TextItem> sectionItems = new Dictionary<Sections, TextItem>();

        /// <summary>
        /// The menu items located in a section
        /// </summary>
        public Dictionary<Sections, List<TextItem>> menuItems = new Dictionary<Sections, List<TextItem>>();

        // The items located under the menu section, add your own lists for sections you make your main class
        public List<TextItem> menuItemsList = new List<TextItem>();
        public List<TextItem> settingsItemList1 = new List<TextItem>();
        public List<TextItem> settingsItemList2 = new List<TextItem>();
        public List<TextItem> exitItemsList = new List<TextItem>();

        /// <summary>
        /// Determins if the user can scroll through the menus
        /// </summary>
        public bool canMove = true;

        /// <summary>
        /// The speed the background image should move. Change this based upon your image size and your panorama length. Should move slower than title text item.
        /// </summary>
        public float backgroundSpeed = 4.5f;

        /// <summary>
        /// The speed the title text item should move, should move slower than the section and menu text items. Change this based upon your panorama length. 
        /// </summary>
        public float titleItemSpeed = 7.0f;

        /// <summary>
        /// The speed the section and menu text items should move, should move faster than the title text item. Change this based upon your panorama length.
        /// </summary>
        public float menuItemSpeed = 12.0f;

        public SpriteFont titleFont;
        public SpriteFont sectionFont;
        public SpriteFont menuFont;

        public Menu()
        {
        }

        /// <summary>
        /// Gets the default location for text based upon Microsoft's UI panorama specifications, use this one for the title item
        /// </summary>
        /// <param name="location">Where it is located in the panorama, a title, a section, or a menu text</param>
        /// <returns>A Vector2 with where the text should be located</returns>
        public Vector2 GetDefaultLocation(TextLocation location)
        {
            if (location == TextLocation.Title)
            {
                return new Vector2(10.0f, -50.0f);
            }
            return Vector2.Zero;
        }

        /// <summary>
        /// Gets the default location for text based upon Microsoft's UI panorama specifications, use this one for section items
        /// </summary>
        /// <param name="location">Where it is located in the panorama, a title, a section, or a menu text</param>
        /// <param name="titleItem">What is the title text item</param>
        /// <param name="sectionMenu">The section the text item should be located</param>
        /// <param name="graphics">graphics.GraphicsDevice from main class</param>
        /// <returns>A Vector2 with where the text should be located</returns>
        public Vector2 GetDefaultLocation(TextLocation location, TextItem titleItem, Sections sectionMenu, GraphicsDevice graphics)
        {
            if (location == TextLocation.Section)
            {
                int position;
                sectionList.TryGetValue(sectionMenu, out position);
                int menuPositioning = 24;

                if (position == 0)
                {
                    return new Vector2(10.0f, titleItem.MeasureString().Y - 100.0f);
                }
                else
                {
                    return new Vector2((graphics.Viewport.Width * position) - (menuPositioning * position) + 10, titleItem.MeasureString().Y - 100.0f);
                }
            }
            return Vector2.Zero;
        }

        /// <summary>
        /// Gets the default location for text based upon Microsoft's UI panorama specifications, use this one for the first menu item in a list (use PlaceUnderText for subsequent text items)
        /// </summary>
        /// <param name="location">Where it is located in the panorama, a title, a section, or a menu text</param>
        /// <param name="textItem">What text item the current text item should go under</param>
        /// <param name="spacing">The spacing between the bottom of the specified text item and the current text item</param>
        /// <returns>A Vector2 with where the text should be located</returns>
        public Vector2 GetDefaultLocation(TextLocation location, TextItem textItem, float spacing)
        {
            if (location == TextLocation.Menu)
            {
                return new Vector2(textItem.pos.X, textItem.pos.Y + textItem.MeasureString().Y + spacing);
            }
            return Vector2.Zero;
        }

        /// <summary>
        /// Loads the default fonts for a panoramic menu
        /// </summary>
        /// <param name="Content">Content from main class</param>
        public void LoadBasicFonts(ContentManager Content)
        {
            titleFont = Content.Load<SpriteFont>("TitleFont");
            sectionFont = Content.Load<SpriteFont>("SectionFont");
            menuFont = Content.Load<SpriteFont>("MenuFont");
        }

        /// <summary>
        /// Moves the text left and right based upon the current menu
        /// </summary>
        /// <param name="selectedMenu">The currently active menu</param>
        /// <param name="titleItem">The title text item</param>
        public void UpdatePanorama(Sections selectedMenu, TextItem titleItem)
        {
            TextItem currentTextItem;
            TextItem previousTextItem;

            sectionItems.TryGetValue(selectedMenu, out currentTextItem);
            sectionItems.TryGetValue(previousSection, out previousTextItem);

            if (currentTextItem.pos.X > 0.0f)
            {
                canMove = false;
                foreach (KeyValuePair<Sections, TextItem> sectionItem in sectionItems)
                {
                    sectionItem.Value.vel.X = -menuItemSpeed;
                    sectionItem.Value.pos += sectionItem.Value.vel;
                }

                foreach (KeyValuePair<Sections, List<TextItem>> menuItem in menuItems)
                {
                    foreach (TextItem item in menuItem.Value)
                    {
                        item.vel.X = -menuItemSpeed;
                        item.pos += item.vel;
                    }
                }
                titleItem.vel.X = -titleItemSpeed;
                titleItem.pos += titleItem.vel;
                sectionItems.TryGetValue(selectedMenu, out currentTextItem);

                if (currentTextItem.pos.X <= 10.0f)
                {
                    canMove = true;
                    foreach (KeyValuePair<Sections, TextItem> sectionItem in sectionItems)
                    {
                        sectionItem.Value.vel.X = 0.0f;
                    }

                    foreach (KeyValuePair<Sections, List<TextItem>> menuItem in menuItems)
                    {
                        foreach (TextItem item in menuItem.Value)
                        {
                            item.vel.X = 0.0f;
                        }
                    }
                    titleItem.vel.X = 0.0f;
                    previousSection = currentSection;
                }
            }

            if (currentTextItem.pos.X < 0.0f)
            {
                canMove = false;
                foreach (KeyValuePair<Sections, TextItem> sectionItem in sectionItems)
                {
                    sectionItem.Value.vel.X = menuItemSpeed;
                    sectionItem.Value.pos += sectionItem.Value.vel;
                }

                foreach (KeyValuePair<Sections, List<TextItem>> menuItem in menuItems)
                {
                    foreach (TextItem item in menuItem.Value)
                    {
                        item.vel.X = menuItemSpeed;
                        item.pos += item.vel;
                    }
                }
                titleItem.vel.X = titleItemSpeed;
                titleItem.pos += titleItem.vel;
                sectionItems.TryGetValue(selectedMenu, out currentTextItem);

                if (currentTextItem.pos.X >= 10.0f)
                {
                    canMove = true;
                    foreach (KeyValuePair<Sections, TextItem> sectionItem in sectionItems)
                    {
                        sectionItem.Value.vel.X = 0.0f;
                    }

                    foreach (KeyValuePair<Sections, List<TextItem>> menuItem in menuItems)
                    {
                        foreach (TextItem item in menuItem.Value)
                        {
                            item.vel.X = 0.0f;
                        }
                    }
                    titleItem.vel.X = 0.0f;
                    previousSection = currentSection;
                }
            }
        }

        /// <summary>
        /// Moves the panorama left and right based upon the current menu
        /// </summary>
        /// <param name="selectedMenu">The currently active menu</param>
        /// <param name="titleItem">The title text item</param>
        /// <param name="background">The background image</param>
        public void UpdatePanorama(Sections selectedMenu, TextItem titleItem, Sprite background)
        {
            TextItem currentTextItem;
            TextItem previousTextItem;

            sectionItems.TryGetValue(selectedMenu, out currentTextItem);
            sectionItems.TryGetValue(previousSection, out previousTextItem);

            if (currentTextItem.pos.X > 0.0f)
            {
                canMove = false;
                foreach (KeyValuePair<Sections, TextItem> sectionItem in sectionItems)
                {
                    sectionItem.Value.vel.X = -menuItemSpeed;
                    sectionItem.Value.pos += sectionItem.Value.vel;
                }

                foreach (KeyValuePair<Sections, List<TextItem>> menuItem in menuItems)
                {
                    foreach (TextItem item in menuItem.Value)
                    {
                        item.vel.X = -menuItemSpeed;
                        item.pos += item.vel;
                    }
                }
                titleItem.vel.X = -titleItemSpeed;
                titleItem.pos += titleItem.vel;

                background.vel.X = -backgroundSpeed;
                background.pos += background.vel;

                sectionItems.TryGetValue(selectedMenu, out currentTextItem);

                if (currentTextItem.pos.X <= 10.0f)
                {
                    canMove = true;
                    foreach (KeyValuePair<Sections, TextItem> sectionItem in sectionItems)
                    {
                        sectionItem.Value.vel.X = 0.0f;
                    }

                    foreach (KeyValuePair<Sections, List<TextItem>> menuItem in menuItems)
                    {
                        foreach (TextItem item in menuItem.Value)
                        {
                            item.vel.X = 0.0f;
                        }
                    }
                    titleItem.vel.X = 0.0f;

                    background.vel.X = 0.0f;
                    previousSection = currentSection;
                }
            }

            if (currentTextItem.pos.X < 0.0f)
            {
                canMove = false;
                foreach (KeyValuePair<Sections, TextItem> sectionItem in sectionItems)
                {
                    sectionItem.Value.vel.X = menuItemSpeed;
                    sectionItem.Value.pos += sectionItem.Value.vel;
                }

                foreach (KeyValuePair<Sections, List<TextItem>> menuItem in menuItems)
                {
                    foreach (TextItem item in menuItem.Value)
                    {
                        item.vel.X = menuItemSpeed;
                        item.pos += item.vel;
                    }
                }
                titleItem.vel.X = titleItemSpeed;
                titleItem.pos += titleItem.vel;

                background.vel.X = backgroundSpeed;
                background.pos += background.vel;

                sectionItems.TryGetValue(selectedMenu, out currentTextItem);

                if (currentTextItem.pos.X >= 10.0f)
                {
                    canMove = true;
                    foreach (KeyValuePair<Sections, TextItem> sectionItem in sectionItems)
                    {
                        sectionItem.Value.vel.X = 0.0f;
                    }

                    foreach (KeyValuePair<Sections, List<TextItem>> menuItem in menuItems)
                    {
                        foreach (TextItem item in menuItem.Value)
                        {
                            item.vel.X = 0.0f;
                        }
                    }
                    titleItem.vel.X = 0.0f;

                    background.vel.X = 0.0f;

                    previousSection = currentSection;
                }
            }
        }

        /// <summary>
        /// Draw all panoramic text
        /// </summary>
        /// <param name="spriteBatch">SpriteBatch from main class</param>
        /// <param name="titleItem">The title text item</param>
        public void DrawPanorama(SpriteBatch spriteBatch, TextItem titleItem)
        {
            spriteBatch.DrawString(titleItem.font, titleItem.text, titleItem.pos, titleItem.color);

            foreach (KeyValuePair<Sections, TextItem> sectionItem in sectionItems)
            {
                spriteBatch.DrawString(sectionItem.Value.font, sectionItem.Value.text, sectionItem.Value.pos, sectionItem.Value.color);
            }

            foreach (KeyValuePair<Sections, List<TextItem>> menuItem in menuItems)
            {
                foreach (TextItem item in menuItem.Value)
                {
                    spriteBatch.DrawString(item.font, item.text, item.pos, item.color);
                }
            }
        }

        /// <summary>
        /// Draw all panoramic text
        /// </summary>
        /// <param name="spriteBatch">SpriteBatch from main class</param>
        /// <param name="titleItem">The title text item</param>
        /// <param name="background">The background image</param>
        public void DrawPanorama(SpriteBatch spriteBatch, TextItem titleItem, Sprite background)
        {
            spriteBatch.Draw(background.tex, background.pos, background.color);

            spriteBatch.DrawString(titleItem.font, titleItem.text, titleItem.pos, titleItem.color);

            foreach (KeyValuePair<Sections, TextItem> sectionItem in sectionItems)
            {
                spriteBatch.DrawString(sectionItem.Value.font, sectionItem.Value.text, sectionItem.Value.pos, sectionItem.Value.color);
            }

            foreach (KeyValuePair<Sections, List<TextItem>> menuItem in menuItems)
            {
                foreach (TextItem item in menuItem.Value)
                {
                    spriteBatch.DrawString(item.font, item.text, item.pos, item.color);
                }
            }
        }

        /// <summary>
        /// Updates the text item rectangles
        /// </summary>
        /// <param name="titleItem">The title text item</param>
        public void UpdatePanoramaRects(TextItem titleItem)
        {
            titleItem.rect = new Rectangle((int)titleItem.pos.X, (int)titleItem.pos.Y, (int)titleItem.font.MeasureString(titleItem.text).X, (int)titleItem.font.MeasureString(titleItem.text).X);

            foreach (KeyValuePair<Sections, TextItem> sectionItem in sectionItems)
            {
                sectionItem.Value.rect = new Rectangle((int)sectionItem.Value.pos.X, (int)sectionItem.Value.pos.Y, (int)sectionItem.Value.font.MeasureString(sectionItem.Value.text).X, (int)sectionItem.Value.font.MeasureString(sectionItem.Value.text).X);
            }

            foreach (KeyValuePair<Sections, List<TextItem>> menuItem in menuItems)
            {
                foreach (TextItem item in menuItem.Value)
                {
                    item.rect = new Rectangle((int)item.pos.X, (int)item.pos.Y, (int)item.font.MeasureString(item.text).X, (int)item.font.MeasureString(item.text).X);
                }
            }
        }

        /// <summary>
        /// Updates the panorama rectangles
        /// </summary>
        /// <param name="titleItem">The title text item</param>
        /// <param name="background">The background image</param>
        public void UpdatePanoramaRects(TextItem titleItem, Sprite background)
        {
            background.rect = new Rectangle((int)background.pos.X, (int)background.pos.Y, background.tex.Width, background.tex.Height);

            titleItem.rect = new Rectangle((int)titleItem.pos.X, (int)titleItem.pos.Y, (int)titleItem.font.MeasureString(titleItem.text).X, (int)titleItem.font.MeasureString(titleItem.text).X);

            foreach (KeyValuePair<Sections, TextItem> sectionItem in sectionItems)
            {
                sectionItem.Value.rect = new Rectangle((int)sectionItem.Value.pos.X, (int)sectionItem.Value.pos.Y, (int)sectionItem.Value.font.MeasureString(sectionItem.Value.text).X, (int)sectionItem.Value.font.MeasureString(sectionItem.Value.text).X);
            }

            foreach (KeyValuePair<Sections, List<TextItem>> menuItem in menuItems)
            {
                foreach (TextItem item in menuItem.Value)
                {
                    item.rect = new Rectangle((int)item.pos.X, (int)item.pos.Y, (int)item.font.MeasureString(item.text).X, (int)item.font.MeasureString(item.text).X);
                }
            }
        }

        /// <summary>
        /// Places the current text item under the specified text item, useful for a list of menu items
        /// </summary>
        /// <param name="textItem">The specified text item that the current text item should go under</param>
        /// <param name="spacing">The spacing between the bottom of the specified text item and the current text item</param>
        /// <returns>A Vector2 with the position of the current text item</returns>
        public Vector2 PlaceUnderText(TextItem textItem, float spacing)
        {
            return new Vector2(textItem.pos.X, textItem.pos.Y + textItem.MeasureString().Y + spacing);
        }

        /// <summary>
        /// Recolors all the text in a panoramic menu
        /// </summary>
        /// <param name="titleItem">The title text item</param>
        /// <param name="color">The color all text should be recolored to</param>
        public void RecolorAllText(TextItem titleItem, Color color)
        {
            titleItem.color = color;

            foreach (KeyValuePair<Sections, TextItem> sectionItem in sectionItems)
            {
                sectionItem.Value.color = color;
            }

            foreach (KeyValuePair<Sections, List<TextItem>> menuItem in menuItems)
            {
                foreach (TextItem item in menuItem.Value)
                {
                    item.color = color;
                }
            }
        }

        /// <summary>
        /// Generates the numbered section list from the section list
        /// </summary>
        public void GenerateNumberedSectionList()
        {
            foreach (KeyValuePair<Sections, int> menu in sectionList)
            {
                numberedSectionList.Add(menu.Value, menu.Key);
            }
        }

        /// <summary>
        /// Gets the next or previous section item based upon the direction scrolled
        /// </summary>
        /// <param name="direction">The direction the sections should move</param>
        /// <returns>The previous or next section, makes that the current section</returns>
        public Sections ScrollThroughSections(Direction direction)
        {
            int menuNumber;
            sectionList.TryGetValue(currentSection, out menuNumber);
            if (direction == Direction.Forwards)
            {
                if (menuNumber + 1 >= numberedSectionList.Count)
                {
                    menuNumber = 0;
                }
                else
                {
                    menuNumber += 1;
                }
                numberedSectionList.TryGetValue(menuNumber, out currentSection);
            }
            if (direction == Direction.Backwards)
            {
                if (menuNumber - 1 < 0)
                {
                    menuNumber = numberedSectionList.Count - 1;
                }
                else
                {
                    menuNumber -= 1;
                }
                numberedSectionList.TryGetValue(menuNumber, out currentSection);
            }
            return currentSection;
        }
    }
}
