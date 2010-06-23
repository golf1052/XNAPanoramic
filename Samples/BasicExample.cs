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
using GreenGamesLibrary;

namespace GreenGamesLibraryExamples
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        //Text Items
        //Title TextItem
        TextItem titleTextItem;

        //Section Items
        TextItem section2DItem;
        TextItem example1;
        TextItem example2;
        TextItem exitItem;

        //Menu Items
        TextItem spritesItem;
        TextItem exitMenuItem;

        //List of TextItems under section2D TextItem
        List<TextItem> example2DList = new List<TextItem>();

        Sprite background;

        //Menu Handler
        Menu menuHandler = new Menu();

        //Touch Stuff
        TouchPanelCapabilities touchCaps = TouchPanel.GetCapabilities();
        bool touchHasPressure = false;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            // Frame rate is 30 fps by default for Windows Phone.
            TargetElapsedTime = TimeSpan.FromTicks(333333);

            // Pre-autoscale settings.
            graphics.PreferredBackBufferWidth = 480;
            graphics.PreferredBackBufferHeight = 800;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            //Touch Stuff
            touchHasPressure = touchCaps.IsConnected;

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            //First set the main section (the first one)
            menuHandler.currentSection = Menu.Sections.Examples2D;

            //Load the fonts
            menuHandler.LoadBasicFonts(Content);

            //Add the sections you are going to use
            menuHandler.sectionList.Add(Menu.Sections.Examples2D, 0);
            menuHandler.sectionList.Add(Menu.Sections.Example1, 1);
            menuHandler.sectionList.Add(Menu.Sections.Example2, 2);
            menuHandler.sectionList.Add(Menu.Sections.ExitSection, 3);

            //Set up the TextItems
            titleTextItem = new TextItem(menuHandler.titleFont, "green games library", menuHandler.GetDefaultLocation(Menu.TextLocation.Title));

            section2DItem = new TextItem(menuHandler.sectionFont, "2d examples", menuHandler.GetDefaultLocation(Menu.TextLocation.Section, titleTextItem, Menu.Sections.Examples2D, graphics.GraphicsDevice));
            example1 = new TextItem(menuHandler.sectionFont, "example", menuHandler.GetDefaultLocation(Menu.TextLocation.Section, titleTextItem, Menu.Sections.Example1, graphics.GraphicsDevice));
            example2 = new TextItem(menuHandler.sectionFont, "example", menuHandler.GetDefaultLocation(Menu.TextLocation.Section, titleTextItem, Menu.Sections.Example2, graphics.GraphicsDevice));
            exitItem = new TextItem(menuHandler.sectionFont, "exit", menuHandler.GetDefaultLocation(Menu.TextLocation.Section, titleTextItem, Menu.Sections.ExitSection, graphics.GraphicsDevice));

            spritesItem = new TextItem(menuHandler.menuFont, "sprites test", menuHandler.GetDefaultLocation(Menu.TextLocation.Menu, section2DItem, 10.0f));
            exitMenuItem = new TextItem(menuHandler.menuFont, "exit", menuHandler.GetDefaultLocation(Menu.TextLocation.Menu, exitItem, 10.0f));

            //Add the menu items to their appropriate lists
            example2DList.Add(spritesItem);
            menuHandler.exitItemsList.Add(exitMenuItem);

            //Add those lists to menu items
            menuHandler.menuItems.Add(Menu.Sections.Examples2D, example2DList);
            menuHandler.menuItems.Add(Menu.Sections.ExitSection, menuHandler.exitItemsList);

            //Add the sections to their corresponding section TextItem
            menuHandler.sectionItems.Add(Menu.Sections.Examples2D, section2DItem);
            menuHandler.sectionItems.Add(Menu.Sections.Example1, example1);
            menuHandler.sectionItems.Add(Menu.Sections.Example2, example2);
            menuHandler.sectionItems.Add(Menu.Sections.ExitSection, exitItem);

            //Generate the numbered section list
            menuHandler.GenerateNumberedSectionList();

            background = new Sprite(Content.Load<Texture2D>("ILWP"));
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            GamePadState gamePadState = GamePad.GetState(PlayerIndex.One);

            if (gamePadState.Buttons.Back == ButtonState.Pressed)
            {
                if (menuHandler.currentSection != Menu.Sections.GameMenu)
                {
                    this.Exit();
                }
                else if (menuHandler.currentSection == Menu.Sections.GameMenu)
                {
                    menuHandler.currentSection = Menu.Sections.Examples2D;
                }
            }

            TouchCollection touchCollection = TouchPanel.GetState();

            foreach (TouchLocation touchLocation in touchCollection)
            {
                Point touchPoint = new Point((int)touchLocation.Position.X, (int)touchLocation.Position.Y);
                TouchLocation previousLocation;

                if (touchLocation.State == TouchLocationState.Pressed)
                {
                    if (spritesItem.rect.Contains(touchPoint))
                    {
                        menuHandler.currentSection = Menu.Sections.GameMenu;
                    }

                    if (exitMenuItem.rect.Contains(touchPoint))
                    {
                        this.Exit();
                    }
                }

                if (touchLocation.State == TouchLocationState.Moved)
                {
                    //If the sections are allowed to scroll
                    if (menuHandler.canMove == true)
                    {
                        //If the sections are active (at the menu screen) 
                        if (menuHandler.currentSection != Menu.Sections.GameMenu)
                        {
                            //Try to get the start of the movement
                            touchLocation.TryGetPreviousLocation(out previousLocation);

                            //If the start of the movement is less than the end of the movement
                            if (touchLocation.Position.X < previousLocation.Position.X)
                            {
                                //Scroll through the sections forwards
                                menuHandler.ScrollThroughSections(Menu.Direction.Forwards);
                            }

                            //If the start of the movement is greater than the end of the movement
                            if (touchLocation.Position.X > previousLocation.Position.X)
                            {
                                //Scroll through the sections backwards
                                menuHandler.ScrollThroughSections(Menu.Direction.Backwards);
                            }
                        }
                    }
                }
            }

            if (menuHandler.currentSection != Menu.Sections.GameMenu)
            {
                //Update TextItems
                menuHandler.UpdatePanorama(menuHandler.currentSection, titleTextItem, background);

                //Update TextItem rectangles
                menuHandler.UpdatePanoramaRects(titleTextItem, background);
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();
            if (menuHandler.currentSection != Menu.Sections.GameMenu)
            {
                //Draw the panorama
                menuHandler.DrawPanorama(spriteBatch, titleTextItem, background);
            }
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
