using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq; 

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

    Random rand = new Random(); // Переменная класса Random

    List<Enemy> enemies = new List<Enemy>(); // Создаем список обьектов ботов (врагов)
    List<Player> players = new List<Player>(); // Создаем список обьектов игрока 

    int numEnemy = 10000; // Количество ботов
    int numPlayers = 2; // Количество игроков

    int widthX; // Длина окна
    int heightY; // Ширина окна

    int spawnY = 300; // Спавн по вертикали Y

    float jumpStrength = -14.0f; // Текущая сила прыжка
    float jumpStrengthMax = -15.0f; // Максимальная сила прыжка (лимит)

    string namePlayer = "игрок"; // Имя игрока
    bool collision = false;

    Texture Texture = new Texture(); // Создаем новый класс Texture

    Texture2D texturesPlatform; // Переменная для текстур платформ
    Texture2D pixelTexture; // Пиксель

    Texture2D myTextureBg; // Задний фон
    Texture2D pixel; // Второй пиксель

    Rectangle backgroundRect; // Хитбокс для заднего фона
    Rectangle floorHitbox = new Rectangle(0, 510, 1280, 270); // Хитбокс пола

    Rectangle hitboxPlayer; // Обьевляем переменную хитбокса игрока
    Rectangle hitboxEnemy; // Обьевляем переменную хитбокса бота

    List <Texture2D> listTextures = new List<Texture2D>(); // Список спрайтов для Игрока
    List<Texture2D> listTexturesEnemy = new List<Texture2D>(); // Список спрайтов для Бота

    List<Rectangle> listRectangleEnemy = new List<Rectangle>(); // Список хитбоксов ботов
    List<Rectangle> listRectanglePlayer = new List<Rectangle>(); // Список хитбоксов игроков
    List <Rectangle> listREctPlatforms = new List<Rectangle>(); // Список хитбоксов платформ

    List<Dictionary<string, Keys>> listKeysPlayer = new List<Dictionary<string, Keys>>(); // Список клавишь клавеатуры

    // Единая база данных всей игры
    Dictionary<string, int> worldData = new Dictionary<string, int>();

    /// <param name="title">The title to display in the title bar of the game window.</param>
    /// <param name="width">The initial width, in pixels, of the game window.</param>
    /// <param name="height">The initial height, in pixels, of the game window.</param>
    /// <param name="fullScreen">Indicates if the game should start in fullscreen mode.</param>
    public Core(string title, int width, int height, bool fullScreen)
    {
        // Настраиваем окно
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

        pixelTexture = new Texture2D(GraphicsDevice, 1, 1);
        pixelTexture.SetData(new Color[] { Color.White });
    }
    protected override void LoadContent()
    {
        // Добавляем пусой хитбок, чтобы не вылетила ошибка
        listRectangleEnemy = [new Rectangle(0, 0, 0, 0)];

        // загружаем все текстуры
        listTextures = [
            Content.Load<Texture2D>("left_1"),
            Content.Load<Texture2D>("left_2"),
            Content.Load<Texture2D>("right_1"),
            Content.Load<Texture2D>("right_2"),
            Content.Load<Texture2D>("bullet")];
        listTexturesEnemy = [
            Content.Load<Texture2D>("Enemy_1"),
            Content.Load<Texture2D>("Enemy_2"),
            Texture.FlipTextureHorizontally(Content.Load<Texture2D>("Enemy_1"), base.GraphicsDevice),
            Texture.FlipTextureHorizontally(Content.Load<Texture2D>("Enemy_2"), base.GraphicsDevice),
            Content.Load<Texture2D>("bullet")];
        // Создаем хитбоксы платформ
        listREctPlatforms = [
            new Rectangle(100, 400, 200, 50),
            new Rectangle(500, 300, 200, 50),
            new Rectangle(850, 400, 200, 50),
            new Rectangle(150, 200, 200, 50),
            new Rectangle(800, 200, 200, 50)];
        // Добавляем в конец списка хитбокс пола
        listREctPlatforms.Add(floorHitbox);

        // Загружаем отдельно
        texturesPlatform = Content.Load<Texture2D>("platform");
        MainFont = Content.Load<SpriteFont>("MyFont");
        myTextureBg = Content.Load<Texture2D>("Bg");

        // Создаем словарь для игрока
        Dictionary<string, Keys> keysPlayer = new Dictionary<string, Keys>
        {
            {"Left", Keys.Left },
            {"Right", Keys.Right },
            {"Up", Keys.Up }
        };
        Dictionary<string, Keys> keysPlayer2 = new Dictionary<string, Keys>
        {
            {"Left", Keys.A },
            {"Right", Keys.D },
            {"Up", Keys.W }
        };
        // Передаем готовый словарь в список
        listKeysPlayer = [keysPlayer, keysPlayer2];

        // Если в списке listKeysPlayer недостаточно словарей
        if (numPlayers > listKeysPlayer.Count)
            for (int i = listKeysPlayer.Count; i < numPlayers; i++)
            {
                listKeysPlayer.Add(listKeysPlayer[0]);
            }

        // Проверка лимита прыжка 
        if (jumpStrengthMax > jumpStrength)
            jumpStrength = jumpStrengthMax;

        // Через цикл for пополняем список Игроков
        for (int i = 0; i < numPlayers; i++)
        {
            players.Add(new Player(
                listTextures,
                widthX, heightY, i,
                PlayerX: rand.Next(0, widthX),
                PlayerY: spawnY,
                SpeedPlayer: rand.Next(14, 17), JumpStrength: jumpStrength,
                Name: namePlayer,
                HitBoxPlayerX: 40, HitBoxPlayerY: 45,
                KeyLeft: listKeysPlayer[i]["Left"],
                KeyRight: listKeysPlayer[i]["Right"],
                KeyUp: listKeysPlayer[i]["Up"],
                Collision: collision
            ));
        }

        // Через цикл for пополняем список Ботов
        for (int i = 0; i < numEnemy; i++)
        {
            enemies.Add(new Enemy(
                listTexturesEnemy,
                widthX, i, namePlayer,
                EnemyX: rand.Next(0, widthX), 
                EnemyY: spawnY,
                SpeedEnemy: rand.Next(14, 17), JumpStrength: jumpStrength,
                RangeEnemy: 45,
                Collision: collision
            ));
        }

        // Заносим общее кол-во игроков и ботов
        worldData["numPlayers"] = numPlayers;
        worldData["numEnemy"] = numEnemy;
    }
    protected override void Update(GameTime gameTime)
    {
        // Проверка для выхода из игры
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape)) Exit();

        // Очищаем список
        listRectanglePlayer.Clear(); 
        // через цикл for вызываем метод UpdatePlayer()
        for (int i = 0; i < players.Count; i++)
        {
            hitboxPlayer = players[i].UpdatePlayer(listREctPlatforms, listRectangleEnemy, worldData);
            listRectanglePlayer.Add(hitboxPlayer);
        }

        // Очищаем список для новых хитбоксов
        listRectangleEnemy.Clear();
        // Вызывем метод UpdateEnemy() ботов через цикл
        for (int i = 0; i < enemies.Count; i++)
        {
            hitboxEnemy = enemies[i].UpdateEnemy(listREctPlatforms, listRectanglePlayer, worldData);
            listRectangleEnemy.Add(hitboxEnemy);
        }

        // обновляем кол-во игроков и ботов
        worldData["numPlayers"] = players.Count;
        worldData["numEnemy"] = enemies.Count;

        float fps = 1 / (float)gameTime.ElapsedGameTime.TotalSeconds; // Считаем ФПС
        Window.Title = $"FPS: {(int)fps}"; // Выводим ФПС в заголовок окна

        base.Update(gameTime);
    }
    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);
        SpriteBatch.Begin();

        // Рисуем задний фон
        SpriteBatch.Draw(myTextureBg, backgroundRect, Color.White);

        // Рисуем самого игрока и полоску здоровья
        for (int i = 0; i < players.Count; i++)
        {
            players[i].DrawPlayer(SpriteBatch, MainFont);
            players[i].DrawPlayerHP(SpriteBatch, pixelTexture);
        }

        // Через цикл for делаем тоже самое для ботов
        for (int i = 0; i < enemies.Count; i++)
        {
            enemies[i].DrawEnemy(SpriteBatch, MainFont);
            enemies[i].DrawEnemyHP(SpriteBatch, pixelTexture);
        }

        // Отрисовываем через цикл for все платформы
        for (int i = 0; listREctPlatforms.Count - 1 > i; i++)
        {
            SpriteBatch.Draw(texturesPlatform, listREctPlatforms[i], Color.White);
        }

        SpriteBatch.End();
        base.Draw(gameTime);
    }
}