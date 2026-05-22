using Microsoft.VisualBasic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
public class Player(List<Texture2D> ListTextures,
        int Width, int Height,
        float Gravity = 0.5f, float JumpStrength = -20f,
        int PlayerX = 200, int PlayerY = 0,
        int SpeedPlayer = 10, int SpeedUpdate = 10,
        string Name = "Name", string Statics = "right",
        Keys KeyUp = Keys.W, Keys KeyLeft = Keys.A, Keys KeyRight = Keys.D,
        int HitBoxPlayerX = 30, int HitBoxPlayerY = 30)
{
    CharacterPhysics CFPC = new CharacterPhysics(SpeedPlayer, Width, HitBoxPlayerX, HitBoxPlayerY, 
        Gravity, JumpStrength, Statics, PlayerX, PlayerY, SpeedUpdate: SpeedUpdate);

    string name = Name, statics = Statics;
    Keys keyLeft = KeyLeft, keyRight = KeyRight, keyUp = KeyUp;

    int heightY = Height, widthX = Width;
    int playerX = PlayerX, playerY = PlayerY;

    int speedPlayer = SpeedPlayer, speedUpdate = SpeedUpdate, oldSpeedUpdate = SpeedUpdate;

    List<Texture2D> listTextures = ListTextures;

    int hitBoxX = HitBoxPlayerX, hitBoxY = HitBoxPlayerY;
    Rectangle hitbox = new Rectangle(PlayerX, PlayerY, HitBoxPlayerX, HitBoxPlayerY);

    int currentTextureIndex = 0;
    KeyboardState oldState;
    public (int, int) UpdatePlayer(List<Rectangle> obstacles)
    {
        var kState = Keyboard.GetState();
        CFPC.Update(obstacles);

        var main = CFPC.MoveRight(kState.IsKeyDown(keyRight));
        main = CFPC.MoveLeft(kState.IsKeyDown(keyLeft));

        currentTextureIndex =  CFPC.AnimateIndex(kState.IsKeyDown(keyLeft), kState.IsKeyDown(keyRight));

        // Извлекаем новые X из возвращенного вектора
        playerX = (int)main.Item1.X;

        var mainGravity = CFPC.Gravity(kState.IsKeyDown(keyUp), blOld: !oldState.IsKeyDown(keyUp));
        playerX = mainGravity.Item1; playerY = mainGravity.Item2;

        hitbox.X = playerX; hitbox.Y = playerY;
        oldState = kState;
        return (playerX, playerY);
    }

    public void DrawPlayer(SpriteBatch SpriteBatch, SpriteFont MainFont)
    {
        // Рисуем текстуру
        SpriteBatch.Draw(listTextures[currentTextureIndex], new Vector2(playerX, playerY), Color.White);
        SpriteBatch.DrawString(MainFont, name, new Vector2(playerX, playerY - 20), Color.Black);

        // SpriteBatch.Draw(debugTexture, Hitbox, Color.Red * 0.5f);
    }
}