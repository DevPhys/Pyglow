using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Vector2 = Microsoft.Xna.Framework.Vector2;

internal class calculationForPlayerCoordinates (int SpeedPlayer, int WidthX, int HitBoxX, int HitBoxY, float Gravity, float JumpStrength, string Statics, int PlayerX, int PlayerY)
{
    int playerX = PlayerX; int playerY = PlayerY;
    int speedPlayer = SpeedPlayer; int widthX = WidthX;

    int hitBoxx = HitBoxX; int hitBoxy = HitBoxY;

    string statics = Statics;
    List<Rectangle> obstacles;

    Vector2 position;
    float verticalVelocity = 0; // Текущая вертикальная скорость
    float gravity = Gravity;       // Сила, тянущая вниз
    float jumpStrength = JumpStrength;  // Сила прыжка (минус, потому что вверх)
    bool isGround = true;      // Стоим ли мы на земле

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

            if (nextX < widthX - 30 && CanMove(nextX, playerY, hitBoxx, hitBoxy))
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

            if (nextX > 2 && CanMove(nextX, playerY, hitBoxx, hitBoxy))
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
        if (CanMove(playerX, futureY, hitBoxx, hitBoxy))
        {
            // Если впереди пусто — падаем
            playerY = futureY;
            verticalVelocity += gravity;
        }
        else
        {
            Rectangle futureHitbox = new Rectangle(playerX, futureY, hitBoxx, hitBoxy);

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
                        playerY = obstacle.Top - hitBoxy; // Встаем на платформу
                        verticalVelocity = 0;
                        isGround = true;
                    }
                    break;
                }
            }
        }
        return (playerX, playerY);
    }
}