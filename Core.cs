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

    Player player; Player player2; 
    Enemy enemy;
    Texture Texture = new Texture();

    Texture2D myTextureBg;
    Rectangle backgroundRect;

    List<Texture2D> listTextures = new List<Texture2D>();
    List<Texture2D> listTexturesEnemy = new List<Texture2D>();

    Texture2D texturesPlatform;
    List<Rectangle> obstacles = new List<Rectangle>();
    Rectangle boxHitbox = new Rectangle(100, 400, 200, 50);
    Rectangle floorHitbox = new Rectangle(0, 510, 1280, 270);
    Texture2D pixel;

    int widthX;
    int heightY;

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

        texturesPlatform = Content.Load<Texture2D>("platform");
        obstacles.Add(boxHitbox);
        obstacles.Add(floorHitbox);

        MainFont = Content.Load<SpriteFont>("MyFont");
        myTextureBg = Content.Load<Texture2D>("Bg");

        player = new Player(listTextures, widthX, heightY,
            Gravity: 0.8f, JumpStrength: -10, Name: "Игрок",
            SpeedPlayer: 17,
            Statics: "left", PlayerX: 600,
            HitBoxPlayerX: 40, HitBoxPlayerY: 45,
        KeyRight: Keys.Right, KeyLeft: Keys.Left, KeyUp: Keys.Up); 

        player2 = new Player(listTextures, widthX, heightY,
            SpeedPlayer: 18, JumpStrength: -15,
            PlayerX: 200, SpeedUpdate: 5,
            HitBoxPlayerX: 40, HitBoxPlayerY: 45);
        //KeyLeft: Keys.D, KeyRight: Keys.W, KeyUp: Keys.A);

        enemy = new Enemy(listTexturesEnemy, widthX, SpeedEnemy: 15);
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape)) Exit();

        var mainyCoordinates1 = player.UpdatePlayer(obstacles);
        var mainyCoordinates2 = player2.UpdatePlayer(obstacles);

        enemy.UpdateEnemy(obstacles, [new Vector2(mainyCoordinates1.Item1, mainyCoordinates1.Item2), 
        new Vector2(mainyCoordinates2.Item1, mainyCoordinates2.Item2)]);

        // время кадра FPS
        float fps = 1 / (float)gameTime.ElapsedGameTime.TotalSeconds;

        // Выводим в заголовок окна 
        Window.Title = $"FPS: {fps:0}";
        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);
        SpriteBatch.Begin();

        SpriteBatch.Draw(myTextureBg, backgroundRect, Color.White);
        SpriteBatch.Draw(texturesPlatform, boxHitbox, Color.White);

        player.DrawPlayer(SpriteBatch, MainFont); player2.DrawPlayer(SpriteBatch, MainFont);

        enemy.DrawEnemy(SpriteBatch, MainFont);

        Color hitboxColor = new Color(255, 0, 0, 128);

        // Передаем текстуру пикселя, прямоугольник хитбокса и наш цвет
        // SpriteBatch.Draw(pixel, boxHitbox, hitboxColor);

        SpriteBatch.End();
        base.Draw(gameTime);
    }
}