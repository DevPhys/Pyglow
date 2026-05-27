using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;
using Vector2 = Microsoft.Xna.Framework.Vector2;

class StockAi (int HitBoxX, int HitBoxY, int RengeAi, int SpeedAi, int WidthX)
{
    Texture texture = new Texture();

    Vector2 positionAi = new Vector2();
    Vector2 positionPlayer = new Vector2();

    bool goLeft = false, goRight = false, goUp = false, goBypass = false;
    public (bool, bool, bool, bool) Update(Vector2 PositionAi, Vector2 PositionPlayer, List<Rectangle> obstacles)
    {
        positionAi = PositionAi;
        positionPlayer = PositionPlayer;

        texture.CanMoveUpdate(obstacles);

        goLeft = false;
        goRight = false;
        goUp = false;

        if (goBypass)
        {
            Bypass();
        }
        // Если бот с игроком на одном уровне по высоте Y (в пределах радиуса)
        else if (positionAi.Y - positionPlayer.Y < RengeAi && positionPlayer.Y - positionAi.Y < RengeAi)
        {
            OneLevel();
        }
        // Если бот выше игрока
        else if (positionPlayer.Y - positionAi.Y > RengeAi)
        {
            AiHigher();
        }
        // Если бот ниже игрока
        else if (positionAi.Y - positionPlayer.Y > RengeAi * 2)
        {
            AiBelow();
        }
        // Если игрок подпрыгнул
        else if (positionAi.Y - positionPlayer.Y > -20 && !(positionAi.Y - positionPlayer.Y > RengeAi * 2))
        {
            AiJumped();
        }

        return (goLeft, goRight, goUp, goBypass);
    }
    public void OneLevel()
    {
        if (!texture.CanMove((int)positionAi.X, (int)positionAi.Y + HitBoxY + 5, HitBoxX, HitBoxY))
        {
            if (positionAi.X - positionPlayer.X > RengeAi)
            {
                goLeft = true;
                if (!texture.CanMove((int)positionAi.X - (SpeedAi * 2), (int)positionAi.Y - 5, HitBoxX, HitBoxY))
                {
                    goUp = true;
                    goLeft = true;
                }
            }
            else if (positionPlayer.X - positionAi.X > RengeAi)
            {
                goRight = true;
                if (!texture.CanMove((int)positionAi.X + (SpeedAi * 2), (int)positionAi.Y - 5, HitBoxX, HitBoxY))
                {
                    goUp = true;
                    goRight = true;
                }
            }
        }
    }
    public void AiHigher()
    {
        if (positionAi.X - positionPlayer.X > RengeAi)
        {
            goLeft = true;
        }
        else if (positionPlayer.X - positionAi.X > RengeAi)
        {
            goRight = true;
        }
    }
    public void AiBelow()
    {
        bool isOnGround = !texture.CanMove((int)positionAi.X, (int)positionAi.Y + HitBoxY + 5, HitBoxX, HitBoxY);
        if (isOnGround && positionAi.X > WidthX / 2 - RengeAi * 2 && positionAi.X < WidthX / 2 + RengeAi * 2)
        {
            goBypass = false;
        }
        else
        {
            goBypass = true;
        }
    }
    public void AiJumped()
    {
        goUp = true;
    }
    public void Bypass()
    {
        bool isOnGround = !texture.CanMove((int)positionAi.X, (int)positionAi.Y + HitBoxY + 5, HitBoxX, HitBoxY);
        if (isOnGround && positionAi.X > WidthX / 2 - RengeAi * 2 && positionAi.X < WidthX / 2 + RengeAi * 2)
        {
            goBypass = false;
        }

        if (positionAi.X < WidthX / 2)
        {
            goRight = true;

            if (texture.CanMove((int)positionAi.X + SpeedAi, (int)positionAi.Y + HitBoxY + 1, HitBoxX, HitBoxY))
            {
                goUp = true;
            }

            if (!texture.CanMove((int)positionAi.X + (SpeedAi * 2), (int)positionAi.Y - 5, HitBoxX, HitBoxY))
            {
                goUp = true;
                goRight = true;
            }
        }
        else if (positionAi.X > WidthX / 2)
        {
            goLeft = true;

            if (texture.CanMove((int)positionAi.X - SpeedAi, (int)positionAi.Y + HitBoxY + 1, HitBoxX, HitBoxY))
            {
                goUp = true;
            }

            if (!texture.CanMove((int)positionAi.X - (SpeedAi * 2), (int)positionAi.Y - 5, HitBoxX, HitBoxY))
            {
                goUp = true;
                goLeft = true;
            }
        }
    }
}