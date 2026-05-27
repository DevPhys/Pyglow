using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Threading.Channels;
using Vector2 = Microsoft.Xna.Framework.Vector2;

internal class CharacterPhysics(int SpeedPlayer, int WidthX, int HitBoxX, int HitBoxY, float Gravity, 
    float JumpStrength, string Statics, int PlayerX, int PlayerY,int SpeedUpdate = 10)
{
    int playerX = PlayerX; int playerY = PlayerY;
    int speedPlayer = SpeedPlayer; int widthX = WidthX;

    int hitBoxX = HitBoxX; int hitBoxY = HitBoxY;

    string statics = Statics;
    List<Rectangle> obstacles;

    Vector2 position;
    float verticalVelocity = 0; // Текущая вертикальная скорость
    float gravity = Gravity;       // Сила, тянущая вниз
    float jumpStrength = JumpStrength;  // Сила прыжка (минус, потому что вверх)
    bool isGround = true;      // Стоим ли мы на земле

    int index = 0;
    int speedUpdate = SpeedUpdate, oldSpeedUpdate = SpeedUpdate;

    public void Update (List<Rectangle> Obstacles)
    {
        obstacles = Obstacles;
    }
    public bool CanMove (int newX, int newY, int HitboxX, int HitboxY)
    {
        // Создаем воображаемый хитбокс в новой позиции
        Rectangle futureHitbox = new Rectangle(newX, newY, HitboxX, HitboxY);

        foreach (var obstacle in obstacles)
        {
            // Если воображаемый хитбокс пересекается с препятствием — ходить нельзя
            if (futureHitbox.Intersects(obstacle))
            {
                return false;
            }
        }
        return true; // Путь свободен
    }
    public (Vector2, string) MoveRight (bool blKey)
    {
        if (blKey)
        {
            statics = "right";
            int nextX = playerX + (speedPlayer - 5);

            if (nextX < widthX - 30 && CanMove(nextX, playerY, hitBoxX, hitBoxY))
            {
                playerX = nextX;
            }
        }
        return (new Vector2(playerX, playerY), statics);
    }
    public (Vector2, string) MoveLeft (bool blKey)
    {
        if (blKey)
        {
            statics = "left";

            int nextX = playerX - (speedPlayer - 5);

            if (nextX > 2 && CanMove(nextX, playerY, hitBoxX, hitBoxY))
            {
                playerX = nextX;
            }
        }
        return (new Vector2(playerX, playerY), statics);
    }

    public (int, int) Gravity (bool bl, bool blOld = true)
    {
        position = new Vector2(playerX, playerY);
        if (bl && blOld && isGround)
        {
            verticalVelocity = jumpStrength;
            isGround = false;
        }

        int futureY = playerY + (int)verticalVelocity;
        if (CanMove(playerX, futureY, hitBoxX, hitBoxY))
        {
            // Если впереди пусто — падаем
            playerY = futureY;
            verticalVelocity += gravity;
        }
        else
        {
            Rectangle futureHitbox = new Rectangle(playerX, futureY, hitBoxX, hitBoxY);

            foreach (var obstacle in obstacles)
            {
                if (futureHitbox.Intersects(obstacle))
                {
                    // ПРОВЕРКА: Летим ВВЕРХ (удар головой снизу платформы)
                    if (verticalVelocity < 0)
                    {
                        playerY = obstacle.Bottom; // Стукаемся головой и остаемся под ней
                        verticalVelocity = 0;       // Обнуляем скорость взлета, чтобы начать падать
                    }
                    // Иначе: Летим ВНИЗ (приземление на платформу)
                    else if (verticalVelocity >= 0)
                    {
                        playerY = obstacle.Top - hitBoxY; // Встаем на платформу
                        verticalVelocity = 0;
                        isGround = true;
                    }
                    break;
                }
            }
        }
        return (playerX, playerY);
    }
    public int AnimateIndex(bool GoLeft, bool GoRight)
    {
        if (statics == "left" && GoLeft)
        {
            if (speedUpdate == 0)
            {
                if (index < 1) index += 1;
                else index = 0;
                speedUpdate = oldSpeedUpdate;
            }
            else speedUpdate -= 1;
        }

        if (statics == "right" && GoRight)
        {
            if (speedUpdate == 0)
            {
                if (index < 3) index += 1;
                else index = 2;
                speedUpdate = oldSpeedUpdate;
            }
            else speedUpdate -= 1;
        }

        if (!GoRight && !GoLeft)
        {
            if (statics == "left") index = 0;
            if (statics == "right") index = 2;
        }
        return index;
    }
    public (int, int) FindingTheNearestPlayer(List<Vector2> VectorPlayerCoordinates)
    {
        int minDistance = int.MaxValue;
        int closestPlayerX = playerX; // По умолчанию цель — сам враг (никуда не идем)
        int closestPlayerY = playerY;

        for (int i = 0; i < VectorPlayerCoordinates.Count; i++)
        {
            int currentTargetX = (int)VectorPlayerCoordinates[i].X;
            int currentTargetY = (int)VectorPlayerCoordinates[i].Y;

            int distance = Math.Abs(playerX - currentTargetX);

            if (distance < minDistance)
            {
                minDistance = distance;
                closestPlayerX = currentTargetX;
                closestPlayerY = currentTargetY;
            }
        }
        return (closestPlayerX, closestPlayerY);
    }
}
class Texture
{
    List<Rectangle> obstacles = new List<Rectangle>();
    public Texture2D FlipTextureHorizontally(Texture2D original, GraphicsDevice graphicsDevice)
    {
        // 1. Создаем массив, куда скопируем цвета всех пикселей
        Color[] originalData = new Color[original.Width * original.Height];
        original.GetData(originalData);

        // 2. Создаем массив для новой перевернутой текстуры
        Color[] flippedData = new Color[original.Width * original.Height];

        // 3. Алгоритм переворота пикселей по горизонтали
        for (int y = 0; y < original.Height; y++)
        {
            for (int x = 0; x < original.Width; x++)
            {
                int originalIndex = x + y * original.Width;
                int flippedIndex = (original.Width - 1 - x) + y * original.Width;
                flippedData[flippedIndex] = originalData[originalIndex];
            }
        }

        // 4. Создаем новую текстуру на видеокарте и заливаем в нее перевернутые пиксели
        Texture2D flippedTexture = new Texture2D(graphicsDevice, original.Width, original.Height);
        flippedTexture.SetData(flippedData);

        return flippedTexture;
    }
    public void CanMoveUpdate(List<Rectangle> Obstacles)
    {
        obstacles = Obstacles;
    }
    public bool CanMove(int newX, int newY, int HitboxX, int HitboxY)
    {
        // Создаем воображаемый хитбокс в новой позиции
        Rectangle futureHitbox = new Rectangle(newX, newY, HitboxX, HitboxY);

        foreach (var obstacle in obstacles)
        {
            // Если воображаемый хитбокс пересекается с препятствием — ходить нельзя
            if (futureHitbox.Intersects(obstacle))
            {
                return false;
            }
        }
        return true; // Путь свободен
    }
}