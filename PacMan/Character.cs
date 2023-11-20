using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PacMan
{
    [Serializable]
    enum Direction {Right = 0, Down, Left, Up};

    [Serializable]
    abstract class Character : GameEntity
    {
        protected float speed;
        protected Direction dir, dirOld, dirPred;
        public Character() : base()
        {
            speed = 80;
            dir = Direction.Right;
            dirPred = dirOld = dir;
        }

        public float GetSpeed()
        {
            return speed;
        }

        public Direction GetDir()
        {
            return dir;
        }

        protected bool CheckDirection(Direction dir)
        {

            if (dir == Direction.Up)
            {
                if (position.y - sprite.GetGlobalBounds().height / 2 <= 0)
                    return false;

                Map.GetInstance().GetTile(new Vector2<float>(position.x, position.y - 17), out Tile t1);
                Map.GetInstance().GetTile(new Vector2<float>(position.x - 15, position.y - 17), out Tile t2);
                Map.GetInstance().GetTile(new Vector2<float>(position.x + 15, position.y - 17), out Tile t3);

                if (t1 != null)
                {
                    float errY = (t1.tileSprite.GetGlobalBounds().y + 16) - (position.y - 16);
                    MoveEnt(0, errY);
                }

                if ((t1 == null) && (t2 == null) && (t3 == null))
                    return true;
                return false;
            }
            else if (dir == Direction.Down)
            {
                if (position.y + sprite.GetGlobalBounds().height / 2 >= Form1.WindowSize.height)
                    return false;

                Map.GetInstance().GetTile(new Vector2<float>(position.x, position.y + 17), out Tile t1);
                Map.GetInstance().GetTile(new Vector2<float>(position.x - 15, position.y + 17), out Tile t2);
                Map.GetInstance().GetTile(new Vector2<float>(position.x + 15, position.y + 17), out Tile t3);

                if (t1 != null)
                {
                    float errY = (t1.tileSprite.GetGlobalBounds().y) - (position.y + 16);
                    MoveEnt(0, errY);
                }

                if ((t1 == null) && (t2 == null) && (t3 == null))
                    return true;
                return false;
            }
            else if (dir == Direction.Right)
            {
                if (position.y + sprite.GetGlobalBounds().width / 2 >= Form1.WindowSize.width)
                    return false;

                Map.GetInstance().GetTile(new Vector2<float>(position.x + 17, position.y), out Tile t1);
                Map.GetInstance().GetTile(new Vector2<float>(position.x + 17, position.y - 15), out Tile t2);
                Map.GetInstance().GetTile(new Vector2<float>(position.x + 17, position.y + 15), out Tile t3);

                if (t1 != null)
                {
                    float errX = (t1.tileSprite.GetGlobalBounds().x) - (position.x + 16);
                    MoveEnt(errX,0);
                }

                if ((t1 == null) && (t2 == null) && (t3 == null))
                    return true;
                return false;
            }
            else if (dir == Direction.Left)
            {
                if (position.y - sprite.GetGlobalBounds().width / 2 <= 0)
                    return false;

                Map.GetInstance().GetTile(new Vector2<float>(position.x - 17, position.y), out Tile t1);
                Map.GetInstance().GetTile(new Vector2<float>(position.x - 17, position.y - 15), out Tile t2);
                Map.GetInstance().GetTile(new Vector2<float>(position.x - 17, position.y + 15), out Tile t3);

                if (t1 != null)
                {
                    float errX = (t1.tileSprite.GetGlobalBounds().x + 16) - (position.x - 16);
                    MoveEnt(errX,0);
                }

                if ((t1 == null) && (t2 == null) && (t3 == null))
                    return true;
                return false;
            }

            return false;
        }

        public void Go(Direction dir, float speed)
        {
            switch (dir)
            {
                case Direction.Up:
                    {
                        sprite.SetRotation(this.dirOld, Direction.Up);
                        MoveEnt(0, -speed);
                        break;
                    }
                case Direction.Down:
                    {
                        sprite.SetRotation(this.dirOld, Direction.Down);
                        MoveEnt(0, speed);
                        break;
                    }
                case Direction.Right:
                    {
                        sprite.SetRotation(this.dirOld, Direction.Right);
                        MoveEnt(speed, 0);
                        break;
                    }
                case Direction.Left:
                    {
                        sprite.SetRotation(this.dirOld, Direction.Left);
                        MoveEnt(-speed,0);
                        break;
                    }
            }
        }

    }
}
