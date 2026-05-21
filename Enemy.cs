using Microsoft.VisualBasic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using Vector2 = Microsoft.Xna.Framework.Vector2;
class Enemy (List<Texture2D> ListTextures, int EnemyX = 600, int EnemyY = 100, string Name = "Enemy")
{
    CharacterPhysics CP = new CharacterPhysics(SpeedPlayer: 10,
        WidthX: 1280, HitBoxX: 40, HitBoxY: 50, Gravity: 0.5f, JumpStrength: -20f,
        Statics: "left", PlayerX: EnemyX, PlayerY: EnemyY);

    List<Texture2D> listTextures = ListTextures;
    int enemyX = EnemyX; int enemyY = EnemyY;

    string name = Name;
    int index = 0; int indexCoordinates = 1; 

    bool goLeft = false, goRight = false;

    List<Vector2> vectorPlayerCoordinates;

    public void UpdateEnemy(List<Rectangle> obstacles, List<Vector2> VectorPlayerCoordinates)
    {
        CP.Update(obstacles);

        vectorPlayerCoordinates = VectorPlayerCoordinates;
        int playerX = (int)vectorPlayerCoordinates[indexCoordinates].X;
        int playerY = (int)vectorPlayerCoordinates[indexCoordinates].Y;

        goLeft = false;
        goRight = false;

        int minDistance = int.MaxValue;
        int closestPlayerX = enemyX; // По умолчанию цель — сам враг (никуда не идем)

        for (int i = 0; i < vectorPlayerCoordinates.Count; i++)
        {
            int currentTargetX = (int)vectorPlayerCoordinates[i].X;
            int distance = Math.Abs(enemyX - currentTargetX);

            if (distance < minDistance)
            {
                minDistance = distance;
                closestPlayerX = currentTargetX;
            }
        }
        playerX = closestPlayerX;

        if (enemyX - playerX > 100) // Игрок далеко слева
        {
            goLeft = true;
        }
        else if (playerX - enemyX > 100) // Игрок далеко справа (разница в другую сторону!)
        {
            goRight = true;
        }

        var main = CP.MoveLeft(goLeft);
        main = CP.MoveRight(goRight);

        var mainGravity = CP.Gravity(false);
        enemyX = mainGravity.Item1;
        enemyY = mainGravity.Item2;
    }
    public void DrawEnemy(SpriteBatch SpriteBatch, SpriteFont MainFont) 
    {
        SpriteBatch.Draw(listTextures[index], new Vector2(enemyX, enemyY), Color.White);
        SpriteBatch.DrawString(MainFont, name, new Vector2(enemyX, enemyY - 20), Color.Black);
    }
}

