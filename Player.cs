using Microsoft.VisualBasic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
public class Player
{
    string name;

    List<Texture2D> listTextures = new List<Texture2D>();
    int currentTextureIndex = 0;
    string statics;

    int widthX, heightY;

    int playerX, playerY;
    int speedPlayer, speedUpdate, oldSpeedUpdate;
    KeyboardState oldState;

    Keys keyUp, keyLeft, keyRight;
    Rectangle Hitbox;
    int hitBoxX, hitBoxY;

    calculationForPlayerCoordinates CFPC;

    public void PlayerSettings(List<Texture2D> ListTextures, 
        int Width, int Height, 
        float Gravity = 0.5f, float JumpStrength = -20f,
        int PlayerX = 200, int PlayerY = 0, 
        int SpeedPlayer = 10, int SpeedUpdate = 10,  
        string Name = "NoneName", string Statics = "right",
        Keys KeyUp = Keys.W, Keys KeyLeft = Keys.A, Keys KeyRight = Keys.D,
        int HitBoxPlayerX = 30, int HitBoxPlayerY = 30)
    {
        name = Name; statics = Statics;
        keyLeft = KeyLeft; keyRight = KeyRight; keyUp = KeyUp;

        heightY = Height; widthX = Width;
        playerX = PlayerX; playerY = PlayerY;

        speedPlayer = SpeedPlayer;
        speedUpdate = SpeedUpdate;
        oldSpeedUpdate = SpeedUpdate;

        listTextures = ListTextures;

        hitBoxX = HitBoxPlayerX; hitBoxY = HitBoxPlayerY; 
        Hitbox = new Rectangle(playerX, playerY, hitBoxX, hitBoxY);

        CFPC = new calculationForPlayerCoordinates(speedPlayer, widthX, hitBoxX, hitBoxY, Gravity, JumpStrength, statics, playerX, playerY);
    }
    public void UpdatePlayer(List<Rectangle> obstacles)
    {
        var kState = Keyboard.GetState();
        CFPC.Update(obstacles);

        var main = CFPC.MoveRight(kState.IsKeyDown(keyRight));
        statics = main.Item2;
        if (statics == "right" && kState.IsKeyDown(keyRight))
        {
            if (speedUpdate == 0)
            {
                if (currentTextureIndex < 3) currentTextureIndex += 1;
                else currentTextureIndex = 2;
                speedUpdate = oldSpeedUpdate;
            }
            else speedUpdate -= 1;
        }

        main = CFPC.MoveLeft(kState.IsKeyDown(keyLeft));
        statics = main.Item2;
        if (statics == "left" && kState.IsKeyDown(keyLeft))
        {
            if (speedUpdate == 0)
            {
                if (currentTextureIndex < 1) currentTextureIndex += 1;
                else currentTextureIndex = 0;
                speedUpdate = oldSpeedUpdate;
            }
            else speedUpdate -= 1;
        }

        if (!kState.IsKeyDown(keyLeft) && !kState.IsKeyDown(keyRight)) 
        {
            if (statics == "left") currentTextureIndex = 0;
            if (statics == "right") currentTextureIndex = 2;
        }

        // Извлекаем новые X из возвращенного вектора
        playerX = (int)main.Item1.X;
        CFPC.Update(obstacles);

        var mainGravity = CFPC.Gravity(kState.IsKeyDown(keyUp), blOld: !oldState.IsKeyDown(keyUp));
        playerX = mainGravity.Item1; playerY = mainGravity.Item2;

        Hitbox.X = playerX; Hitbox.Y = playerY;
        oldState = kState;
    }

    public void DrawPlayer(SpriteBatch SpriteBatch, SpriteFont MainFont)
    {
        // Рисуем текстуру
        SpriteBatch.Draw(listTextures[currentTextureIndex], new Vector2(playerX, playerY), Color.White);
        SpriteBatch.DrawString(MainFont, name, new Vector2(playerX, playerY - 20), Color.Black);

        // SpriteBatch.Draw(debugTexture, Hitbox, Color.Red * 0.5f);

    }
}