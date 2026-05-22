using Microsoft.VisualBasic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using Vector2 = Microsoft.Xna.Framework.Vector2;
class Enemy (List<Texture2D> ListTextures, int WidthX, int EnemyX = 600, int EnemyY = 100, string Name = "Enemy",
    int SpeedEnemy = 10, int HitBoxX = 44, int HitBoxY = 73, float Gravity = 0.5f, float JumpStrength = -20f, 
    string Statics = "left", int SpeedUpdate = 10, int RangeEnemy = 100)
{
    CharacterPhysics CP = new CharacterPhysics(SpeedEnemy,
        WidthX, HitBoxX, HitBoxY, Gravity, JumpStrength,
        Statics, PlayerX: EnemyX, PlayerY: EnemyY, SpeedUpdate: SpeedUpdate);

    Random rand = new Random();

    List<Texture2D> listTextures = ListTextures;
    int enemyX = EnemyX; int enemyY = EnemyY; 
    int oldEnemyX = EnemyX; int oldEnemyY = EnemyY;

    string name = Name;
    int index = 0;
    int playerX, playerY;

    bool goLeft = false, goRight = false, goUp = false;
    int rengeEnemy = RangeEnemy;

    List<Vector2> vectorPlayerCoordinates;

    public void UpdateEnemy(List<Rectangle> obstacles, List<Vector2> VectorPlayerCoordinates)
    {
        CP.Update(obstacles);
        vectorPlayerCoordinates = VectorPlayerCoordinates;

        goLeft = false;
        goRight = false;
        goUp = false;

        oldEnemyX = enemyX; oldEnemyY = enemyY;

        var mainVar = CP.FindingTheNearestPlayer(vectorPlayerCoordinates);
        playerX = mainVar.Item1; playerY = mainVar.Item2;

        if (enemyY - playerY > rengeEnemy)
            goUp = true;
        else if (playerY - enemyY > rengeEnemy + HitBoxY * 2) // Игрок внизу
        {
            goRight = true;
            goLeft = false;
        }
        if (playerY - enemyY < rengeEnemy || enemyX + rengeEnemy != playerX)
        {
            if (enemyX - playerX > rengeEnemy) // Игрок далеко слева
                goLeft = true;
            else if (playerX - enemyX > rengeEnemy) // Игрок далеко справа (разница в другую сторону!)
                goRight = true;
        }

        var main = CP.MoveLeft(goLeft);
        main = CP.MoveRight(goRight);

        index = CP.AnimateIndex(goLeft, goRight);

        var mainGravity = CP.Gravity(goUp);
        enemyX = mainGravity.Item1;
        enemyY = mainGravity.Item2;

        if ((goLeft || goRight) && enemyX == oldEnemyX)
        {
            // Перезапускаем гравитацию с флагом прыжка, чтобы бот подпрыгнул на препятствие
            mainGravity = CP.Gravity(bl: true);
            enemyX = mainGravity.Item1;
            enemyY = mainGravity.Item2;
        }
    }
    public void DrawEnemy(SpriteBatch SpriteBatch, SpriteFont MainFont) 
    {
        SpriteBatch.Draw(listTextures[index], new Vector2(enemyX, enemyY), Color.White);
        SpriteBatch.DrawString(MainFont, name, new Vector2(enemyX, enemyY - 20), Color.Black);
    }
}