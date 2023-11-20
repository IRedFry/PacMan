using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PacMan
{
    [Serializable]
    enum GhostType { Blinky, Pinky, Inky, Clyde };
    [Serializable]
    class Ghost : Character
    {
        bool escaping;
        private delegate Vector2<float> ResultFunc();
        ResultFunc func;
        private Vector2<float> destination;
        private Vector2<float> defaultPosition;
        private bool hasDestination;
        private Vector2<float> block;
        GhostType self_type;
        private long innerTimer;
        public Ghost() : base()
        {
            escaping = false;
            hasDestination = false;
            speed = 100;
            innerTimer = 0;

            Game.CherryEaten += Game_CherryEaten;
            Game.Blink += Game_Blink;
            Game.StateEnd += Game_StateEnd;


        }

        public override void OnSerialization()
        {
            Game.CherryEaten += Game_CherryEaten;
            Game.Blink += Game_Blink;
            Game.StateEnd += Game_StateEnd;
        }

        private void Game_StateEnd(SceneType type, int arg, string string_args)
        {
            if (type == SceneType.Main_Game)
            {
                escaping = false;

                switch (self_type)
                {
                    case GhostType.Blinky:
                        func = CalculateDestinationRed;
                        break;
                    case GhostType.Pinky:
                        func = CalculateDestinationPink;
                        break;
                    case GhostType.Inky:
                        func = CalculateDestinationBlue;
                        break;
                    case GhostType.Clyde:
                        func = CalculateDestinationYellow;
                        break;
                }

                hasDestination = false;

                innerTimer = 0;

                var texture = Properties.Resources.entities;


                EntityManager.GetInstance().SetEntityTextureOrigin(this);
            }
        }

        private void Game_Blink(SceneType type, int arg, string string_args)
        {
            if (sprite.GetOpacity() == 255)
                sprite.SetOppacity(150);
            else
                sprite.SetOppacity(255);
        }

        private void Game_CherryEaten(SceneType type, int arg, string string_args)
        {
            if (type == SceneType.Main_Game)
            {
                escaping = true;
                func = GetDefaultPosition;
                hasDestination = false;
                var texture = Properties.Resources.entities;

                
                sprite.SetTexture(ref texture, new Rect<int>(32, 32, 32, 32));
                sprite.SetOrigin(16,16);
                sprite.SetPosition(positionOld);
            }
        }

        public void SetType(GhostType type)
        {
            self_type = type;
            switch (type)
            {
                case GhostType.Blinky:                 
                    func = CalculateDestinationRed;
                    defaultPosition = new Vector2<float>(96, 32);
                    break;
                case GhostType.Pinky:
                    func = CalculateDestinationPink;
                    defaultPosition = new Vector2<float>(96, Map.MapSize.y * 16 - 32);
                    break;
                case GhostType.Inky:
                    func = CalculateDestinationBlue;
                    defaultPosition = new Vector2<float>(Map.MapSize.x * 16 - 96, 32);
                    break;
                case GhostType.Clyde:
                    func = CalculateDestinationYellow;
                    defaultPosition = new Vector2<float>(Map.MapSize.x * 16 - 96, Map.MapSize.y * 16 - 32);
                    break;
            }

        }
        public override void Update(long dT)
        {
            innerTimer += dT;
  
            if (innerTimer > 20000.0f && func != GetDefaultPosition && !escaping)
            {
                if (self_type == GhostType.Blinky)
                    speed += 5;

                func = GetDefaultPosition;
                innerTimer = 0;

                Console.WriteLine("CHANGE TO SCATTER");
            }
            else if (innerTimer > 12000.0f && func == GetDefaultPosition && !escaping)
            {
                if (self_type == GhostType.Blinky)
                    speed += 10;

                Console.WriteLine("CHANGE TO HUNT");
                switch (self_type)
                {
                    case GhostType.Blinky:
                        func = CalculateDestinationRed;
                        break;
                    case GhostType.Pinky:
                        func = CalculateDestinationPink;
                        break;
                    case GhostType.Inky:
                        func = CalculateDestinationBlue;
                        break;
                    case GhostType.Clyde:
                        func = CalculateDestinationYellow;
                        break;
                }

                innerTimer = 0;
            }


            if (!hasDestination)
            {
                destination = func();
                Console.WriteLine("{0} : {1} ||| {2}", destination.x,destination.y, self_type);
                block = position;
                hasDestination = true;
            }
            else
            {

                if (position != block)
                {
                    float dx = block.x - position.x;
                    dx = (dx < 0 ? dx = -1 : dx > 0 ? dx = 1 : dx = 0);
                    float dy = block.y - position.y;
                    dy = (dy < 0 ? dy = -1 : dy > 0 ? dy = 1 : dy = 0);


                    if (dx < 0)
                    {
                        if (position.x + (dT / 1000.0f * speed * dx) < block.x)
                            MoveEnt(block.x - position.x, 0);
                        else
                            MoveEnt(dT / 1000.0f * speed * dx, 0);
                    }
                    else if (dx > 0)
                    {
                        if (position.x + (dT / 1000.0f * speed * dx) > block.x)
                            MoveEnt(block.x - position.x, 0);
                        else
                            MoveEnt(dT / 1000.0f * speed * dx, 0);
                    }

                    if (dy < 0)
                    {
                        if (position.y + (dT / 1000.0f * speed * dy) < block.y)
                            MoveEnt(0, block.y - position.y);
                        else
                            MoveEnt(0, dT / 1000.0f * speed * dy);
                    }
                    else if (dy > 0)
                    {
                        if (position.y + (dT / 1000.0f * speed * dy) > block.y)
                            MoveEnt(0, block.y - position.y);
                        else
                            MoveEnt(0, dT / 1000.0f * speed * dy);
                    }

                }
                else
                {
                    CalculateDirection();
                }

                if (position == destination)
                {
                    if (destination == defaultPosition)
                        destination = func();
                    else
                        hasDestination = false;
                }
            }

            UpdateAABB();
        }

        private void CalculateDirection()
        {

            Vector2<float> nextBlock = new Vector2<float>();
            Vector2<float> futureBlock = new Vector2<float>();

            int d = ((int)dir + 2) % 4;
            switch (d)
            {
                case 3:
                    {
                        futureBlock.x = position.x;
                        futureBlock.y = position.y - 16;
                        break;
                    }
                case 1:
                    {
                        futureBlock.x = position.x;
                        futureBlock.y = position.y + 16;
                        break;
                    }
                case 0:
                    {
                        futureBlock.x = position.x + 16;
                        futureBlock.y = position.y;
                        break;
                    }
                case 2:
                    {
                        futureBlock.x = position.x - 16;
                        futureBlock.y = position.y;
                        break;
                    }
            }


            int distance = 2505;
            int futureDirection = ((int)dir + 2) % 4;
            for (int i = 0; i < 4; ++i)
            {
                if (i == ((int)dir + 2) % 4)
                    continue;

                switch (i)
                {
                    case 3:
                        {
                            nextBlock.x = position.x;
                            nextBlock.y = position.y - 16;
                            break;
                        }
                    case 1:
                        {
                            nextBlock.x = position.x;
                            nextBlock.y = position.y + 16;
                            break;
                        }
                    case 0:
                        {
                            nextBlock.x = position.x + 16;
                            nextBlock.y = position.y;
                            break;
                        }
                    case 2:
                        {
                            nextBlock.x = position.x - 16;
                            nextBlock.y = position.y;
                            break;
                        }
                }


                if (CheckDirectionAI((Direction)i,position))
                {
                    int semiDistance = (int)Math.Sqrt((destination.x - nextBlock.x) * (destination.x - nextBlock.x) + (destination.y - nextBlock.y) * (destination.y - nextBlock.y));
                    if (semiDistance < distance)
                    {
                        distance = semiDistance;
                        futureBlock = new Vector2<float>((int)(nextBlock.x / 16) * 16, (int)(nextBlock.y / 16) * 16);
                        futureDirection = i;
                    }
                }
            }

            dir = (Direction)futureDirection;
            block = futureBlock;

        }

        public override void ReactOnCollision(GameEntity g)
        {
            if (escaping && g.GetName() == "Player")
            {
                EntityManager.GetInstance().EraseEntity(self_id);
            }
        }

        public override void Draw(System.Windows.Forms.PaintEventArgs e)
        {

            base.Draw(e);
            /*
            if (destination != null)
            {
              e.Graphics.FillRectangle(new System.Drawing.SolidBrush(System.Drawing.Color.Red), destination.x, destination.y, 16, 16);
                e.Graphics.FillRectangle(new System.Drawing.SolidBrush(System.Drawing.Color.LightPink), block.x, block.y, 16, 16);
            }
            */
            
    }

        private bool CheckDirectionAI(Direction dir, Vector2<float> pos)
        {

            if (dir == Direction.Up)
            {
                if (pos.y - sprite.GetGlobalBounds().height / 2 <= 0)
                    return false;

                Map.GetInstance().GetTile(new Vector2<float>(pos.x, pos.y - 17), out Tile t1);
                Map.GetInstance().GetTile(new Vector2<float>(pos.x - 15, pos.y - 17), out Tile t2);
                Map.GetInstance().GetTile(new Vector2<float>(pos.x + 15, pos.y - 17), out Tile t3);

                if ((t1 == null) && (t2 == null) && (t3 == null))
                    return true;

                return false;
            }
            else if (dir == Direction.Down)
            {
                if (pos.y + sprite.GetGlobalBounds().height / 2 >= Form1.WindowSize.height)
                    return false;

                Map.GetInstance().GetTile(new Vector2<float>(pos.x, pos.y + 17), out Tile t1);
                Map.GetInstance().GetTile(new Vector2<float>(pos.x - 15, pos.y + 17), out Tile t2);
                Map.GetInstance().GetTile(new Vector2<float>(pos.x + 15, pos.y + 17), out Tile t3);

                if ((t1 == null) && (t2 == null) && (t3 == null))
                    return true;

                return false;
            }
            else if (dir == Direction.Right)
            {
                if (pos.y + sprite.GetGlobalBounds().width / 2 >= Form1.WindowSize.width)
                    return false;

                Map.GetInstance().GetTile(new Vector2<float>(pos.x + 17, pos.y), out Tile t1);
                Map.GetInstance().GetTile(new Vector2<float>(pos.x + 17, pos.y - 15), out Tile t2);
                Map.GetInstance().GetTile(new Vector2<float>(pos.x + 17, pos.y + 15), out Tile t3);

                if ((t1 == null) && (t2 == null) && (t3 == null))
                    return true;

                return false;
            }
            else if (dir == Direction.Left)
            {
                if (pos.y - sprite.GetGlobalBounds().width / 2 <= 0)
                    return false;

                Map.GetInstance().GetTile(new Vector2<float>(pos.x - 17, pos.y), out Tile t1);
                Map.GetInstance().GetTile(new Vector2<float>(pos.x - 17, pos.y - 15), out Tile t2);
                Map.GetInstance().GetTile(new Vector2<float>(pos.x - 17, pos.y + 15), out Tile t3);

                if ((t1 == null) && (t2 == null) && (t3 == null))
                    return true;

                return false;
            }

            return false;
        }

        private Vector2<float> CalculateDestinationRed()
        {
            Vector2<float> dest;
            EntityManager.GetInstance().GetEntity(0, out var p);
            Vector2<float> p_pos = p.GetPos();


            dest.x = (int)((p_pos.x - 8) / 16);
            dest.y = (int)((p_pos.y - 8) / 16);


            return new Vector2<float>(dest.x * 16 + 16, dest.y * 16 + 16);
        }
        private Vector2<float> CalculateDestinationPink()
        {
            Vector2<float> dest  = new Vector2<float>();
            EntityManager.GetInstance().GetEntity(0, out var p);
            Vector2<float> p_pos = p.GetPos();
            Direction d = ((Player)p).GetDir();

            for (int i = 4; i > 0; ++i)
            {
                switch (d)
                {
                    case Direction.Up:
                        dest.x = (int)((p_pos.x - 8) / 16);
                        dest.y = (int)((p_pos.y - 8) / 16) - i;
                        break;

                    case Direction.Right:
                        dest.x = (int)((p_pos.x - 8) / 16) + i;
                        dest.y = (int)((p_pos.y - 8) / 16);
                        break;

                    case Direction.Down:
                        dest.x = (int)((p_pos.x - 8) / 16);
                        dest.y = (int)((p_pos.y - 8) / 16) + i;
                        break;

                    case Direction.Left:
                        dest.x = (int)((p_pos.x - 8) / 16) - i;
                        dest.y = (int)((p_pos.y - 8) / 16);
                        break;
                }

                Map.GetInstance().GetTile(new Vector2<float>(dest.x * 16 + 16, dest.y * 16 + 16), out Tile t);

                if (t == null)
                {
                    Map.GetInstance().GetTile(new Vector2<float>(dest.x * 16, dest.y * 16 + 16), out t);
                    if (t != null)
                        dest.x += 1;

                    Map.GetInstance().GetTile(new Vector2<float>(dest.x * 16 + 16, dest.y * 16), out t);
                    if (t != null)
                        dest.y += 1;

                    return new Vector2<float>(dest.x * 16 + 16, dest.y * 16 + 16);
                }
            }

            return p_pos;
        }
        private Vector2<float> CalculateDestinationBlue()
        {
            Vector2<float> dest = new Vector2<float>();
            EntityManager.GetInstance().GetEntity(0, out var p);
            Vector2<float> p_pos = p.GetPos();
            Direction d = ((Player)p).GetDir();

            switch (d)
            {
                case Direction.Up:
                    dest.x = (int)((p_pos.x - 8) / 16);
                    dest.y = (int)((p_pos.y - 8) / 16) - 2;
                    break;

                case Direction.Right:
                    dest.x = (int)((p_pos.x - 8) / 16) + 2;
                    dest.y = (int)((p_pos.y - 8) / 16);
                    break;

                case Direction.Down:
                    dest.x = (int)((p_pos.x - 8) / 16);
                    dest.y = (int)((p_pos.y - 8) / 16) + 2;
                    break;

                case Direction.Left:
                    dest.x = (int)((p_pos.x - 8) / 16) - 2;
                    dest.y = (int)((p_pos.y - 8) / 16);
                    break;
            }

            EntityManager.GetInstance().GetEntity(1, out GameEntity g);

            if (g == null)
                return CalculateDestinationYellow();

            Vector2<float> g_pos = g.GetPos();

            dest.x = (g_pos.x + 2 * (dest.x * 16 + 16  - g_pos.x));
            dest.y = (g_pos.y + 2 * (dest.y * 16 + 16  - g_pos.y));

            dest.x = (int)(dest.x / 16) * 16;
            dest.y = (int)(dest.y / 16) * 16;

            Map.GetInstance().GetTile(dest, out Tile t);

            if (t != null || dest.x > Map.MapSize.x * 16 - 16 || dest.y > Map.MapSize.y * 16 - 16 || dest.x < 16 || dest.y < 16)
            {
                dest = defaultPosition;
                return dest;
            }

            Map.GetInstance().GetTile(new Vector2<float>(dest.x - 16 , dest.y), out t);
            if (t != null)
                dest.x += 16;

            Map.GetInstance().GetTile(new Vector2<float>(dest.x, dest.y - 16), out t);
            if (t != null)
                dest.y += 16;

            return dest;
        }
        private Vector2<float> CalculateDestinationYellow()
        {
            EntityManager.GetInstance().GetEntity(0, out var p);
            Vector2<float> p_pos = p.GetPos();

            int semiDistance = (int)Math.Sqrt((position.x - p_pos.x) * (position.x - p_pos.x) + (position.y - p_pos.y) * (position.y - p_pos.y));

            if (semiDistance >= 8 * 16)
                return CalculateDestinationRed();

            return defaultPosition;

        }

        private Vector2<float> GetDefaultPosition()
        {
            return defaultPosition;
        }
    }

}
