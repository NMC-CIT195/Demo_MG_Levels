using System; // add to allow Windows message box
using System.Runtime.InteropServices; // add to allow Windows message box

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using System.Collections.Generic;

namespace Demo_MG_Levels
{
    public enum GameAction
    {
        None,
        StartGame,
        PlayerRight,
        PlayerLeft,
        PlayerUp,
        PlayerDown,
        Quit
    }

    public enum GameState
    {
        OpeningScreen,
        Playing,
        ClosingScreen
    }

    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class MazeGame : Game
    {
        // add code to allow Windows message boxes when running in a Windows environment
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern uint MessageBox(IntPtr hWnd, String text, String caption, uint type);

        // set the cell size in pixels
        public static int CELL_WIDTH = 64;
        public static int CELL_HEIGHT = 64;
        public static int CELL_WIDTH_SMALL_SPRITE_OFFSET = 8;
        public static int CELL_HEIGHT_SMALL_SPRITE_OFFSET = 8;

        // set the map size in cells
        public static int MAP_CELL_ROW_COUNT = 9;
        private const int MAP_CELL_COLUMN_COUNT = 9;

        // set game info display
        public static int GAME_INFO_DISPLAY_X_POSITION = 0;
        public static int GAME_INFO_DISPLAY_Y_POSITION = MAP_CELL_ROW_COUNT * CELL_HEIGHT;
        public static int GAME_INFO_DISPLAY_HEIGHT = 192;

        // set the window size
        public static int WINDOW_WIDTH = MAP_CELL_COLUMN_COUNT * CELL_WIDTH;
        public static int WINDOW_HEIGHT = MAP_CELL_ROW_COUNT * CELL_HEIGHT + GAME_INFO_DISPLAY_HEIGHT;

        // set game parameters
        public static int NUMBER_OF_LEVELS = 2;

        // level state
        private bool initializeNextLevel = true;

        // game state
        private GameState currentGameState = GameState.OpeningScreen;

        // wall objects
        private List<Wall> walls;

        // SpriteFont for the info screen
        private SpriteFont infoFont;

        // background for the info area
        private Texture2D backgroundInfoArea;

        // opening screen
        private Texture2D openingScreen;

        // game status variables
        private int level = 1;
        private int health = 100;
        private int lives = 5;
        private int score = 0;

        // background tile
        private Texture2D backgroundTile;

        // portal object
        private Portal portal;

        // map array
        private int[,] map;

        // player object
        private Player player;

        // player starting position
        private Vector2 playerStartingPosition;

        // variable to hold the player's current game action
        private GameAction playerGameAction;

        // keyboard state objects to track a single keyboard press
        private KeyboardState newState;
        private KeyboardState oldState;

        // declare a GraphicsDeviceManager object
        private GraphicsDeviceManager graphics;

        // declare a SpriteBatch object
        private SpriteBatch spriteBatch;

        public MazeGame()
        {
            graphics = new GraphicsDeviceManager(this);

            // set the window size 
            graphics.PreferredBackBufferWidth = WINDOW_WIDTH;
            graphics.PreferredBackBufferHeight = WINDOW_HEIGHT;

            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // add floors, walls, and ceilings
            walls = new List<Wall>();

            // add the player
            playerStartingPosition = new Vector2(1 * CELL_WIDTH, 1 * CELL_HEIGHT);
            player = new Player(Content, playerStartingPosition);
            player.Active = true;

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

            // load font for info screen
            infoFont = Content.Load<SpriteFont>("info_font");

            // load opening screen
            openingScreen = Content.Load<Texture2D>("splash_screen");

            // load background graphics
            backgroundTile = Content.Load<Texture2D>("background_tile");
            backgroundInfoArea = Content.Load<Texture2D>("background_info_area");

            // Note: wall, portal, and player sprites loaded when instantiated
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            playerGameAction = GetKeyboardEvents();
            ManageGameActions(playerGameAction);

            switch (currentGameState)
            {
                case GameState.OpeningScreen:
                    break;

                case GameState.Playing:
                    ManageGameLevel();
                    ManageGameStatus();
                    ManageGameObjects();
                    break;

                case GameState.ClosingScreen:
                    break;

                default:
                    break;
            }



            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            //GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();

            switch (currentGameState)
            {
                case GameState.OpeningScreen:
                    DrawOpeningScreen();
                    break;

                case GameState.Playing:
                    DrawBackground();
                    DrawGameObjects();
                    DrawGameInfo();
                    player.Draw(spriteBatch);
                    break;

                case GameState.ClosingScreen:
                    break;

                default:
                    break;
            }

            spriteBatch.End();

            base.Draw(gameTime);
        }

        private void ManageGameLevel()
        {
            if (initializeNextLevel)
            {
                switch (level)
                {
                    // Level 1 initialization
                    case (1):
                        // set the player's initial speed
                        player.SpeedHorizontal = 5;
                        player.SpeedVertical = 5;

                        // add everything to Level 1
                        BuildMapLevel1();

                        // reset new level flag
                        initializeNextLevel = false;
                        break;

                    // Level 2 initialization
                    case (2):
                        // set the player's initial speed
                        player.SpeedHorizontal = 5;
                        player.SpeedVertical = 5;

                        // set the player's starting position
                        player.Position = new Vector2(3 * CELL_WIDTH, CELL_HEIGHT);

                        // reset wall objects from previous level
                        walls = new List<Wall>();

                        // add everything to Level 1
                        BuildMapLevel2();

                        // reset new level flag
                        initializeNextLevel = false;
                        break;
                    default:
                        break;
                }
            }
        }

        private void ManageGameStatus()
        {
            // has player collided with the portal
            if (player.BoundingRectangle.Intersects(portal.BoundingRectangle))
            {
                if (level != NUMBER_OF_LEVELS)
                {
                    level++;
                    initializeNextLevel = true;
                }
                else
                {
                    // TODO display closing screen
                    Environment.Exit(1);
                }
            }
        }

        /// <summary>
        /// manage all player game actions
        /// </summary>
        /// <param name="gameAction">player game action choice</param>
        private void ManageGameActions(GameAction gameAction)
        {
            switch (playerGameAction)
            {
                case GameAction.None:
                    break;

                // start game
                case GameAction.StartGame:
                    currentGameState = GameState.Playing;
                    break;

                // move player right
                case GameAction.PlayerRight:
                    player.PlayerDirection = Player.Direction.Right;

                    // only move player if allowed
                    if (CanMove())
                    {
                        player.Position = new Vector2(player.Position.X + player.SpeedHorizontal, player.Position.Y);
                    }
                    break;

                //move player left
                case GameAction.PlayerLeft:
                    player.PlayerDirection = Player.Direction.Left;

                    // only move player if allowed
                    if (CanMove())
                    {
                        player.Position = new Vector2(player.Position.X - player.SpeedHorizontal, player.Position.Y);
                    }

                    break;

                // move player up
                case GameAction.PlayerUp:
                    player.PlayerDirection = Player.Direction.Up;

                    // only move player if allowed
                    if (CanMove())
                    {
                        player.Position = new Vector2(player.Position.X, player.Position.Y - player.SpeedVertical);
                    }
                    break;

                // move player down
                case GameAction.PlayerDown:
                    player.PlayerDirection = Player.Direction.Down;

                    // only move player if allowed
                    if (CanMove())
                    {
                        player.Position = new Vector2(player.Position.X, player.Position.Y + player.SpeedVertical);
                    }
                    break;

                // quit game
                case GameAction.Quit:
                    Exit();
                    break;

                default:
                    break;
            }
        }

        /// <summary>
        /// Draw all game info in game info area
        /// </summary>
        private void DrawGameInfo()
        {
            // draw score
            spriteBatch.DrawString(infoFont, score.ToString(), new Vector2(GAME_INFO_DISPLAY_X_POSITION + 430, GAME_INFO_DISPLAY_Y_POSITION + 130), Color.Black);

            // draw health
            spriteBatch.DrawString(infoFont, health.ToString(), new Vector2(GAME_INFO_DISPLAY_X_POSITION + 230, GAME_INFO_DISPLAY_Y_POSITION + 70), Color.Black);

            // draw lives
            spriteBatch.DrawString(infoFont, lives.ToString(), new Vector2(GAME_INFO_DISPLAY_X_POSITION + 230, GAME_INFO_DISPLAY_Y_POSITION + 130), Color.Black);

            // draw current level
            spriteBatch.DrawString(infoFont, level.ToString(), new Vector2(GAME_INFO_DISPLAY_X_POSITION + 430, GAME_INFO_DISPLAY_Y_POSITION + 70), Color.Black);
        }

        /// <summary>
        /// manage the interaction of all game objects
        /// </summary>
        private void ManageGameObjects()
        {

        }


        /// <summary>
        /// get keyboard events
        /// </summary>
        /// <returns>GameAction</returns>
        private GameAction GetKeyboardEvents()
        {
            GameAction playerGameAction = GameAction.None;

            newState = Keyboard.GetState();

            if (CheckKey(Keys.Right) == true)
            {
                playerGameAction = GameAction.PlayerRight;
            }
            else if (CheckKey(Keys.Left) == true)
            {
                playerGameAction = GameAction.PlayerLeft;
            }
            else if (CheckKey(Keys.Up) == true)
            {
                playerGameAction = GameAction.PlayerUp;
            }
            else if (CheckKey(Keys.Down) == true)
            {
                playerGameAction = GameAction.PlayerDown;
            }
            else if (CheckKey(Keys.Enter) == true)
            {
                playerGameAction = GameAction.StartGame;
            }
            else if (CheckKey(Keys.Escape) == true)
            {
                playerGameAction = GameAction.Quit;
            }

            oldState = newState;

            return playerGameAction;
        }

        /// <summary>
        /// check the current state of the keyboard against the previous state
        /// </summary>
        /// <param name="theKey">bool new key press</param>
        /// <returns></returns>
        private bool CheckKey(Keys theKey)
        {
            // allows the key to be held down
            return newState.IsKeyDown(theKey);

            // player must continue to tap the key
            //return oldState.IsKeyDown(theKey) && newState.IsKeyUp(theKey); 
        }

        /// <summary>
        /// check to confirm that player movement is allowed
        /// </summary>
        /// <returns></returns>
        private bool CanMove()
        {
            bool canMove = true;

            // do not allow movement into wall
            foreach (Wall wall in walls)
            {
                if (WallCollision(wall))
                {
                    canMove = false;
                    continue;
                }
            }

            return canMove;
        }

        /// <summary>
        /// test for player collision with a wall object
        /// </summary>
        /// <param name="wall">wall object to test</param>
        /// <returns>true if collision</returns>
        private bool WallCollision(Wall wall)
        {
            bool wallCollision = false;

            // create a Rectangle object for the new move's position
            Rectangle newPlayerPosition = player.BoundingRectangle;

            // test the new move's position for a collision with the wall
            switch (player.PlayerDirection)
            {
                case Player.Direction.Left:
                    // set the position of the new move's rectangle
                    newPlayerPosition.Offset(-player.SpeedHorizontal, 0);

                    // test for a collision with the new move and the wall
                    if (newPlayerPosition.Intersects(wall.BoundingRectangle))
                    {
                        wallCollision = true;

                        // move player next to wall
                        player.Position = new Vector2(wall.BoundingRectangle.Right, player.Position.Y);
                    }
                    break;

                case Player.Direction.Right:
                    // set the position of the new move's rectangle
                    newPlayerPosition.Offset(player.SpeedHorizontal, 0);

                    // test for a collision with the new move and the wall
                    if (newPlayerPosition.Intersects(wall.BoundingRectangle))
                    {
                        wallCollision = true;

                        // move player next to wall
                        player.Position = new Vector2(wall.BoundingRectangle.Left - player.BoundingRectangle.Width, player.Position.Y);
                    }
                    break;

                case Player.Direction.Up:
                    // set the position of the new move's rectangle
                    newPlayerPosition.Offset(0, -player.SpeedVertical);

                    // test for a collision with the new move and the wall
                    if (newPlayerPosition.Intersects(wall.BoundingRectangle))
                    {
                        wallCollision = true;

                        // move player next to wall
                        player.Position = new Vector2(player.Position.X, wall.BoundingRectangle.Bottom);
                    }
                    break;

                case Player.Direction.Down:
                    // set the position of the new move's rectangle
                    newPlayerPosition.Offset(0, player.SpeedVertical);

                    // test for a collision with the new move and the wall
                    if (newPlayerPosition.Intersects(wall.BoundingRectangle))
                    {
                        wallCollision = true;

                        // move player next to wall
                        player.Position = new Vector2(player.Position.X, wall.BoundingRectangle.Top - player.BoundingRectangle.Height);
                    }
                    break;

                default:
                    break;
            }

            return wallCollision;
        }

        /// <summary>
        /// build the Level 1 map
        /// </summary>
        private void BuildMapLevel1()
        {
            // Note: initialized array size must equal the MAP_CELL_COLUMN_COUNT and MAP_CELL_ROW_COUNT
            //
            map = new int[,]
            {
                { 1, 1, 1, 1, 1, 1, 1, 1, 1 },
                { 1, 0, 0, 0, 0, 0, 0, 0, 1 },
                { 1, 0, 1, 1, 0, 1, 1, 0, 1 },
                { 1, 0, 1, 1, 0, 1, 1, 0, 1 },
                { 1, 0, 0, 0, 0, 0, 0, 0, 3 },
                { 1, 0, 1, 1, 0, 1, 1, 0, 1 },
                { 1, 0, 1, 1, 0, 1, 1, 0, 1 },
                { 1, 0, 0, 0, 0, 0, 0, 0, 1 },
                { 1, 1, 1, 1, 1, 1, 1, 1, 1 }
            };

            for (int row = 0; row < MAP_CELL_ROW_COUNT; row++)
            {
                for (int column = 0; column < MAP_CELL_COLUMN_COUNT; column++)
                {
                    // add walls
                    if (map[row, column] == 1)
                    {
                        walls.Add(new Wall(Content, "wall", new Vector2(column * CELL_HEIGHT, row * CELL_WIDTH)));
                    }

                    // add portal
                    if (map[row, column] == 3)
                    {
                        portal = new Portal(Content, "portal", new Vector2(column * CELL_HEIGHT, row * CELL_WIDTH));
                    }
                }
            }
        }

        /// <summary>
        /// build the Level 2 map
        /// </summary>
        private void BuildMapLevel2()
        {
            // Note: initialized array size must equal the MAP_CELL_COLUMN_COUNT and MAP_CELL_ROW_COUNT
            //
            map = new int[,]
            {
                { 1, 1, 1, 1, 1, 1, 1, 1, 1 },
                { 1, 0, 0, 0, 0, 0, 0, 0, 1 },
                { 1, 0, 1, 1, 0, 1, 1, 0, 1 },
                { 1, 0, 1, 1, 0, 1, 1, 0, 1 },
                { 3, 0, 0, 0, 0, 0, 0, 0, 1 },
                { 1, 0, 1, 1, 0, 1, 1, 0, 1 },
                { 1, 0, 1, 1, 0, 1, 1, 0, 1 },
                { 1, 0, 0, 0, 0, 0, 0, 0, 1 },
                { 1, 1, 1, 1, 1, 1, 1, 1, 1 }
            };

            for (int row = 0; row < MAP_CELL_ROW_COUNT; row++)
            {
                for (int column = 0; column < MAP_CELL_COLUMN_COUNT; column++)
                {
                    // add walls
                    if (map[row, column] == 1)
                    {
                        walls.Add(new Wall(Content, "wall", new Vector2(column * CELL_HEIGHT, row * CELL_WIDTH)));
                    }

                    // add portal
                    if (map[row, column] == 3)
                    {
                        portal = new Portal(Content, "portal", new Vector2(column * CELL_HEIGHT, row * CELL_WIDTH));
                    }
                }
            }
        }


        /// <summary>
        /// draw the background for each level
        /// </summary>
        /// <param name="spriteBatch"></param>
        private void DrawBackground()
        {
            // draw tiled background for map
            for (int row = 0; row < MAP_CELL_ROW_COUNT; row++)
            {
                for (int column = 0; column < MAP_CELL_COLUMN_COUNT; column++)
                {
                    // add a background tile to each cell of the map
                    spriteBatch.Draw(backgroundTile, new Vector2(column * CELL_HEIGHT, row * CELL_WIDTH), Color.White);
                }
            }

            // draw info area background
            spriteBatch.Draw(backgroundInfoArea, new Vector2(0, MAP_CELL_ROW_COUNT * CELL_HEIGHT), Color.White);
        }


        /// <summary>
        /// draw the opening screen
        /// </summary>
        /// <param name="spriteBatch"></param>
        private void DrawOpeningScreen()
        {
            // draw opening screen
            spriteBatch.Draw(openingScreen, new Vector2(0, 0), Color.White);
        }


        /// <summary>
        /// draw all game objects on map
        /// </summary>
        /// <param name="spriteBatch">spriteBatch object</param>
        private void DrawGameObjects()
        {
            // draw walls
            foreach (Wall wall in walls)
            {
                wall.Draw(spriteBatch);
            }

            portal.Draw(spriteBatch);
        }


    }
}
