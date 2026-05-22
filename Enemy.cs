using Microsoft.VisualBasic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using Vector2 = Microsoft.Xna.Framework.Vector2;
class Enemy (List<Texture2D> ListTextures, int WidthX, int EnemyX = 600, int EnemyY = 100, string Name = "Enemy",
    int SpeedEnemy = 10, int HitBoxX = 44, int HitBoxY = 73, float Gravity = 0.5f, float JumpStrength = -20f, 
    string Statics = "left", int SpeedUpdate = 10)
{
    CharacterPhysics CP = new CharacterPhysics(SpeedEnemy,
        WidthX, HitBoxX, HitBoxY, Gravity, JumpStrength,
        Statics, PlayerX: EnemyX, PlayerY: EnemyY, SpeedUpdate: SpeedUpdate);

    List<Texture2D> listTextures = ListTextures;
    int enemyX = EnemyX; int enemyY = EnemyY;

    string name = Name;
    int index = 0; 

    bool goLeft = false, goRight = false, goUp = false;

    List<Vector2> vectorPlayerCoordinates;

    public void UpdateEnemy(List<Rectangle> obstacles, List<Vector2> VectorPlayerCoordinates)
    {
        CP.Update(obstacles);
        vectorPlayerCoordinates = VectorPlayerCoordinates;

        goLeft = false;
        goRight = false;

        int playerX = CP.FindingTheNearestPlayer(vectorPlayerCoordinates);

        if (enemyX - playerX > 100) // Игрок далеко слева
            goLeft = true;
        else if (playerX - enemyX > 100) // Игрок далеко справа (разница в другую сторону!)
            goRight = true;

        var main = CP.MoveLeft(goLeft);
        main = CP.MoveRight(goRight);

        index = CP.AnimateIndex(goLeft, goRight);

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