using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace MonoGameLibrary;

public class Core : Game
{
    internal static Core s_instance;
    public static Core Instance => s_instance;
    public static GraphicsDeviceManager Graphics { get; private set; }
    public static new GraphicsDevice GraphicsDevice { get; private set; }
    public static SpriteBatch SpriteBatch { get; private set; }
    public static new ContentManager Content { get; private set; }
    public static SpriteFont MainFont { get; private set; }

    Random rand = new Random();
    Player player; Player player2;
    List<Enemy> enemies = new List<Enemy>();
    int numEnemy = 1;

    Texture Texture = new Texture();

    Texture2D myTextureBg;
    Rectangle backgroundRect;

    List<Texture2D> listTextures = new List<Texture2D>();
    List<Texture2D> listTexturesEnemy = new List<Texture2D>();

    List<Vector2> listVector = new List<Vector2>();

    Texture2D texturesPlatform;

    List<Rectangle> listREctPlatforms = new List<Rectangle>();

    Rectangle floorHitbox = new Rectangle(0, 510, 1280, 270);
    Texture2D pixel;

    int widthX;
    int heightY;

    int spawnY = 400;
    float jumpStrength = -12.0f;
    float jumpStrengthMax = -12.0f;

    /// <param name="title">The title to display in the title bar of the game window.</param>
    /// <param name="width">The initial width, in pixels, of the game window.</param>
    /// <param name="height">The initial height, in pixels, of the game window.</param>
    /// <param name="fullScreen">Indicates if the game should start in fullscreen mode.</param>
    public Core(string title, int width, int height, bool fullScreen)
    {

        widthX = width;
        heightY = height;

        if (s_instance != null)
        {
            throw new InvalidOperationException($"Only a single Core instance can be created");
        }

        s_instance = this;
        Graphics = new GraphicsDeviceManager(this);

        // Set the graphics defaults.
        Graphics.PreferredBackBufferWidth = width;
        Graphics.PreferredBackBufferHeight = height;
        Graphics.IsFullScreen = fullScreen;

        Graphics.ApplyChanges();
        Window.Title = title;

        Content = base.Content;
        Content.RootDirectory = "Content";
        IsMouseVisible = true;

        //TargetElapsedTime = TimeSpan.FromSeconds(1.0 / 45.0);
        //IsFixedTimeStep = true;
    }

    protected override void Initialize()
    {
        base.Initialize();

        GraphicsDevice = base.GraphicsDevice;
        SpriteBatch = new SpriteBatch(GraphicsDevice);
        backgroundRect = new Rectangle(0, 0, widthX, heightY);

        // Создаем текстуру 1x1 пиксель
        pixel = new Texture2D(GraphicsDevice, 1, 1);

        // Заполняем этот один пиксель чистым белым цветом
        pixel.SetData(new[] { Color.White });
    }
    protected override void LoadContent()
    {
        listTextures = [
            Content.Load<Texture2D>("left_1"),
            Content.Load<Texture2D>("left_2"),
            Content.Load<Texture2D>("right_1"),
            Content.Load<Texture2D>("right_2")];

        listTexturesEnemy = [
            Content.Load<Texture2D>("Enemy_1"),
            Content.Load<Texture2D>("Enemy_2")];

        Texture2D enemyRight1 = Texture.FlipTextureHorizontally(listTexturesEnemy[0], base.GraphicsDevice);
        Texture2D enemyRight2 = Texture.FlipTextureHorizontally(listTexturesEnemy[1], base.GraphicsDevice);
        listTexturesEnemy.Add(enemyRight1);
        listTexturesEnemy.Add(enemyRight2);

        listREctPlatforms = [
            new Rectangle(100, 400, 200, 50),
            new Rectangle(500, 300, 200, 50),
            new Rectangle(850, 400, 200, 50),
            new Rectangle(150, 200, 200, 50)];

        texturesPlatform = Content.Load<Texture2D>("platform");
        listREctPlatforms.Add(floorHitbox);

        MainFont = Content.Load<SpriteFont>("MyFont");
        myTextureBg = Content.Load<Texture2D>("Bg");

        if (jumpStrengthMax > jumpStrength)
            jumpStrength = jumpStrengthMax;

        player = new Player(listTextures, widthX, heightY,
            JumpStrength: -15.0f, Name: "Игрок",
            SpeedPlayer: 13, PlayerY: spawnY,
            Statics: "left", PlayerX: 600,
            HitBoxPlayerX: 40, HitBoxPlayerY: 45);
        //KeyRight: Keys.Right, KeyLeft: Keys.Left, KeyUp: Keys.Up); 

        player2 = new Player(listTextures, widthX, heightY,
            SpeedPlayer: 20, JumpStrength: -12.0f,
            PlayerX: 100, SpeedUpdate: 5, PlayerY: spawnY,
            HitBoxPlayerX: 40, HitBoxPlayerY: 45);
        //KeyLeft: Keys.D, KeyRight: Keys.W, KeyUp: Keys.A);

        for (int i = 0; i < numEnemy; i++)
        {
            enemies.Add(new Enemy(
                listTexturesEnemy,
                widthX,
                EnemyX: rand.Next(0, widthX), // Каждая копия появится чуть правее
                EnemyY: spawnY,
                SpeedEnemy: rand.Next(13, 18), JumpStrength: jumpStrength,
                RangeEnemy: rand.Next(50, 100)
            ));
        }
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape)) Exit();

        var mainyCoordinates1 = player.UpdatePlayer(listREctPlatforms);
        //var mainyCoordinates2 = player2.UpdatePlayer(listREctPlatforms);

        listVector = [new Vector2(mainyCoordinates1.Item1, mainyCoordinates1.Item2)];
        // new Vector2(mainyCoordinates2.Item1, mainyCoordinates2.Item2)];

        for (int i = 0; i < enemies.Count; i++)
        {
            Window.Title = enemies[i].UpdateEnemy(listREctPlatforms, listVector);
        }

        // время кадра FPS
        float fps = 1 / (float)gameTime.ElapsedGameTime.TotalSeconds;

        // Выводим в заголовок окна 
        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);
        SpriteBatch.Begin();

        SpriteBatch.Draw(myTextureBg, backgroundRect, Color.White);

        for (int i = 0; listREctPlatforms.Count - 1 > i; i++)
        {
            SpriteBatch.Draw(texturesPlatform, listREctPlatforms[i], Color.White);
        }

        player.DrawPlayer(SpriteBatch, MainFont); //player2.DrawPlayer(SpriteBatch, MainFont);

        for (int i = 0; i < enemies.Count; i++)
        {
            enemies[i].DrawEnemy(SpriteBatch, MainFont);
        }

        Color hitboxColor = new Color(255, 0, 0, 128);

        // Передаем текстуру пикселя, прямоугольник хитбокса и наш цвет
        // SpriteBatch.Draw(pixel, boxHitbox, hitboxColor);

        SpriteBatch.End();
        base.Draw(gameTime);
    }
}