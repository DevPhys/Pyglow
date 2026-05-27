using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;
using Vector2 = Microsoft.Xna.Framework.Vector2;
class Enemy (List<Texture2D> ListTextures, int WidthX, int Number, string NamePlayer, int EnemyX = 600, int EnemyY = 100, string Name = "Enemy",
    int SpeedEnemy = 10, int HitBoxX = 44, int HitBoxY = 73, float Gravity = 0.5f, float JumpStrength = -20f, 
    string Statics = "left", int SpeedUpdate = 10, int RangeEnemy = 100, int EnemyHP = 100, int Damage = 15, bool Collision = false)
{
    CharacterPhysics CP = new CharacterPhysics(SpeedEnemy,
        WidthX, HitBoxX, HitBoxY, Gravity, JumpStrength,
        Statics, PlayerX: EnemyX, PlayerY: EnemyY, SpeedUpdate: SpeedUpdate);

    StockAi Ai = new StockAi(HitBoxX, HitBoxY, RangeEnemy, SpeedEnemy, WidthX);

    int enemyHP = EnemyHP, enemyHPMax = EnemyHP, barMaxWidth = 50, damage = Damage;
    int enemyX = EnemyX; int enemyY = EnemyY;

    int index = 0;
    int playerX, playerY;

    string name = Name;

    bool goLeft = false, goRight = false, goUp = false;

    Rectangle hitbox = new Rectangle(0, 0, HitBoxX, HitBoxY);

    List<Texture2D> listTextures = ListTextures;

    List<Vector2> vectorPlayerCoordinates = new List<Vector2>();
    List<Rectangle> allObstaclesBuffer = new List<Rectangle>();
    public Rectangle UpdateEnemy(List<Rectangle> obstacles, List<Rectangle> Characters, Dictionary<string, int> WorldData)
    {
        // Очищаем список
        allObstaclesBuffer.Clear();

        // Обьединяем все списки
        if (Collision) allObstaclesBuffer.AddRange(Characters);
        allObstaclesBuffer.AddRange(obstacles);

        // Обновляем параметры
        CP.Update(allObstaclesBuffer);

        // Очищаем список
        vectorPlayerCoordinates.Clear();
        // Вычисляем все координаты игроков
        for (int i = 0; i < WorldData["numPlayers"]; i++)
        {
            vectorPlayerCoordinates.Add(new Vector2(WorldData[$"{NamePlayer}{i}_X"], WorldData[$"{NamePlayer}{i}_Y"]));
        }

        // Находим самого ближнего игрока для выбора цели
        var mainVar = CP.FindingTheNearestPlayer(vectorPlayerCoordinates);
        // Определяем цель 
        playerX = mainVar.Item1; playerY = mainVar.Item2;

        // Обновляем ИИ
        var mainBool = Ai.Update(new Vector2(enemyX, enemyY), new Vector2(playerX, playerY), allObstaclesBuffer);
        // Обновляем флаги
        goLeft = mainBool.Item1; goRight = mainBool.Item2; goUp = mainBool.Item3;

        // Обновляем движения вправо и влево
        CP.MoveLeft(goLeft);
        CP.MoveRight(goRight);

        // Обновляем индекс
        index = CP.AnimateIndex(goLeft, goRight);

        var mainGravity = CP.Gravity(goUp); // Обновляем гравитацию
        enemyX = mainGravity.Item1; enemyY = mainGravity.Item2; // Обновляем координаты бота
        hitbox.X = enemyX; hitbox.Y = enemyY; // Обновляем координаты хитбокса

        // Обновляем данные
        WorldData[$"{name}{Number}_HP"] = enemyHP;
        WorldData[$"{name}{Number}_Damage"] = damage;

        // Возвращаем хитбокс
        return hitbox;
    }
    public void DrawEnemy(SpriteBatch SpriteBatch, SpriteFont MainFont) 
    {
        SpriteBatch.Draw(listTextures[index], new Vector2(enemyX, enemyY), Color.White); // Рисуем текстуру
        SpriteBatch.DrawString(MainFont, name, new Vector2(enemyX, enemyY - 33), Color.Black); // Выводим имя на экран
    }
    public void DrawEnemyHP(SpriteBatch spriteBatch, Texture2D pixelTexture)
    {
        // Считаем ширину по формуле
        int currentBarWidth = (int)(((float)enemyHP / enemyHPMax) * barMaxWidth);

        // Рисуем задний фон полоски (например, черный прямоугольник над головой бота)
        Rectangle backgroundRect = new Rectangle(enemyX - 5, enemyY - 15, barMaxWidth, 5);
        spriteBatch.Draw(pixelTexture, backgroundRect, Color.Black);

        // Рисуем полоску здоровья (зеленый прямоугольник, ширина которого меняется)
        Rectangle hpRect = new Rectangle(enemyX - 5, enemyY - 15, currentBarWidth, 5);
        spriteBatch.Draw(pixelTexture, hpRect, Color.Red);
    }
}