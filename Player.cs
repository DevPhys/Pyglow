using Microsoft.VisualBasic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.Linq;
public class Player(List<Texture2D> ListTextures,
        int Width, int Height, int Number,
        float Gravity = 0.5f, float JumpStrength = -20f,
        int PlayerX = 200, int PlayerY = 0,
        int SpeedPlayer = 10, int SpeedUpdate = 10,
        string Name = "Name", string Statics = "right",
        Keys KeyUp = Keys.W, Keys KeyLeft = Keys.A, Keys KeyRight = Keys.D,
        int HitBoxPlayerX = 30, int HitBoxPlayerY = 30,
        int PlayerHP = 100, int Damage = 15,
        bool Collision = false) // Обьявляем переменный по умолчанию в конструкторе класса
{
    CharacterPhysics CFPC = new CharacterPhysics(SpeedPlayer, Width, HitBoxPlayerX, HitBoxPlayerY, 
        Gravity, JumpStrength, Statics, PlayerX, PlayerY, SpeedUpdate: SpeedUpdate); // Инициализируем обьект для физики

    int playerHP = PlayerHP, playerHPMax = PlayerHP, barMaxWidth = 50; // Обьявляем переменные здоровья
    int damage = Damage; // Наносимый урон

    int height = Height;
    int playerX = PlayerX, playerY = PlayerY; // Координаты

    int currentTextureIndex = 0; // Индекс для списка анимации

    string name = Name; // Имя игрока

    Rectangle hitbox = new Rectangle(PlayerX, PlayerY, HitBoxPlayerX, HitBoxPlayerY); // Хитбокс игрока

    Keys keyLeft = KeyLeft, keyRight = KeyRight, keyUp = KeyUp; 
    KeyboardState oldState;

    List<Texture2D> listTextures = ListTextures; // Список текстур
    List<Rectangle> allObstaclesBuffer = new List<Rectangle>();
    public Rectangle UpdatePlayer(List<Rectangle> obstacles, List<Rectangle> Characters, Dictionary<string, int> WorldData)
    {
        // Очищаем список
        allObstaclesBuffer.Clear();

        // Обьединяем все списки
        if (Collision) allObstaclesBuffer.AddRange(Characters);
        allObstaclesBuffer.AddRange(obstacles);

        // Обновляем kState и получаемые хитбоксы на случай изменений
        var kState = Keyboard.GetState();
        CFPC.Update(allObstaclesBuffer);

        // Обновляем проверку нажатий клавиш
        var main = CFPC.MoveRight(kState.IsKeyDown(keyRight));
        main = CFPC.MoveLeft(kState.IsKeyDown(keyLeft));
        // Достаем перменную playerX, обновляя ее
        playerX = (int)main.Item1.X;

        // Обновляем гравитацию
        var mainGravity = CFPC.Gravity(kState.IsKeyDown(keyUp), blOld: !oldState.IsKeyDown(keyUp));
        // Достаем и обновляем координаты
        playerX = mainGravity.Item1; playerY = mainGravity.Item2;

        // Обновляем координаты хитбокса
        hitbox.X = playerX; hitbox.Y = playerY;
        // Обновляем в самом конце логики старое значение клавеатуры
        oldState = kState;

        // Обновляем индекс
        currentTextureIndex = CFPC.AnimateIndex(kState.IsKeyDown(keyLeft), kState.IsKeyDown(keyRight));

        // Перезаписываем информация в общий словарь извне
        WorldData[$"{name}{Number}_X"] = playerX;
        WorldData[$"{name}{Number}_Y"] = playerY;
        WorldData[$"{name}{Number}_HP"] = playerHP;
        WorldData[$"{name}{Number}_Damage"] = damage;

        // Возвращаем хитбокс игрока
        return hitbox;
    }
    public void DrawPlayer(SpriteBatch SpriteBatch, SpriteFont MainFont)
    {
        // Рисуем текстуру
        SpriteBatch.Draw(listTextures[currentTextureIndex], new Vector2(playerX, playerY), Color.White);
        SpriteBatch.DrawString(MainFont, name, new Vector2(playerX, playerY - 33), Color.Black);

        // SpriteBatch.Draw(debugTexture, Hitbox, Color.Red * 0.5f);
    }
    public void DrawPlayerHP(SpriteBatch spriteBatch, Texture2D pixelTexture)
    {
        // Считаем ширину по формуле
        int currentBarWidth = (int)(((float)playerHP / playerHPMax) * barMaxWidth);

        // Рисуем задний фон полоски (например, черный прямоугольник над головой бота)
        Rectangle backgroundRect = new Rectangle(playerX - 5, playerY - 15, barMaxWidth, 5);
        spriteBatch.Draw(pixelTexture, backgroundRect, Color.Black);

        // Рисуем полоску здоровья 
        Rectangle hpRect = new Rectangle(playerX - 5, playerY - 15, currentBarWidth, 5);
        spriteBatch.Draw(pixelTexture, hpRect, Color.Red);
    }
}