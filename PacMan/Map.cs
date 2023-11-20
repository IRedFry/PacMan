using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace PacMan
{
    using Tiles = Dictionary<int, Tile>;
    using TileParameters = Dictionary<string, Rect<int>>;
    using Stars = Dictionary<int, Star>;
    [Serializable]
    class Tile
    {
        public Sprite tileSprite;
        public Tile(Vector2<int> pos, Sprite sp)
        {
            tileSprite = sp;
            tileSprite.SetPosition(new Vector2<float>(pos.x * Map.TileSize, pos.y * Map.TileSize));
        }
    }
    [Serializable]
    class Star
    {
        public Sprite starSprite;
        public Star(Vector2<int> pos, Sprite sp)
        {
            starSprite = sp;
            starSprite.SetPosition(pos.x, pos.y);
        }
    }



    [Serializable]
    class Map
    {
        private static Map instance;

        public const int TileSize = 16;
        public static Vector2<int> MapSize = new Vector2<int>(50, 40);

        private TileParameters tileParam = new TileParameters();
        private string mapName;
        private Vector2<float> playerStart;
        private System.Drawing.Bitmap tileTexture;
        private Tiles tiles = new Tiles();
        private Stars stars = new Stars();
        bool isComplete;

        public void OnSerialization()
        {
            instance = this;
        }

        private int ConvertCoordinate(Vector2<int> cor)
        {
            return cor.x * MapSize.x + cor.y;
        }

        private Vector2<int> ConvertCoordinate(int cor)
        {
            return new Vector2<int>(cor / MapSize.x, cor % MapSize.x);
        }
        private Map()
        {

            tileTexture = Properties.Resources.tileTexture;
            LoadTiles();
        }
        public void Destroy()
        {
            instance = null;
        }
        public static Map GetInstance()
        {
            if (instance == null)
                instance = new Map();

            return instance;
        }

        public void LoadTiles()
        {
            string[] lines = (Properties.Resources.tiles.Split('\n'));

            foreach (string line in lines)
            {
                string[] devided = line.Split(' ');

                string name = devided[0];

                Rect<int> coords = new Rect<int>(int.Parse(devided[1]) * TileSize, int.Parse(devided[2]) * TileSize, TileSize, TileSize);

                tileParam.Add(name, coords);
            }
        }

        public void LoadMap(string _name)
        {
            string[] lines = ((string)Properties.Resources.ResourceManager.GetObject(_name)).Split('\n');
            foreach (string line in lines)
            {
                string[] devided = line.Split(' ');

                if (devided[0] == "TILE")
                {
                    string name = devided[1];

                    tileParam.TryGetValue(name, out Rect<int> tileRect);
                    Sprite sp = new Sprite();
                    sp.SetTexture(ref tileTexture, tileRect);

                    Vector2<int> pos = new Vector2<int>(Int32.Parse(devided[2]), Int32.Parse(devided[3]));

                    Tile tile = new Tile(pos, sp);
                    tiles.Add(ConvertCoordinate(pos), tile);
                }
                else if (devided[0] == "PLAYER")
                {
                    playerStart.x = Int32.Parse(devided[1]);
                    playerStart.y = Int32.Parse(devided[2]);

                    EntityManager.GetInstance().AddEntity("Player", playerStart);
                }
                else if (devided[0] == "BLINKY")
                {
                    Vector2<float> pos = new Vector2<float>(Int32.Parse(devided[1]), Int32.Parse(devided[2]));

                    EntityManager.GetInstance().AddEntity("Blinky", pos);
                }
                else if (devided[0] == "PINKY")
                {
                    Vector2<float> pos = new Vector2<float>(Int32.Parse(devided[1]), Int32.Parse(devided[2]));

                    EntityManager.GetInstance().AddEntity("Pinky", pos);
                }
                else if (devided[0] == "INKY")
                {
                    Vector2<float> pos = new Vector2<float>(Int32.Parse(devided[1]), Int32.Parse(devided[2]));

                    EntityManager.GetInstance().AddEntity("Inky", pos);
                }
                else if (devided[0] == "CLYDE")
                {
                    Vector2<float> pos = new Vector2<float>(Int32.Parse(devided[1]), Int32.Parse(devided[2]));

                    EntityManager.GetInstance().AddEntity("Clyde", pos);
                }
                else if (devided[0] == "STAR")
                {
                    Vector2<int> pos = new Vector2<int>(Int32.Parse(devided[1]), Int32.Parse(devided[2]));

                    tileParam.TryGetValue("STAR", out Rect<int> starRect);
                    Sprite sp = new Sprite();
                    sp.SetTexture(ref tileTexture, starRect);

                    Star star = new Star(pos, sp);

                    stars.Add(ConvertCoordinate(new Vector2<int>(pos.x / TileSize, pos.y / TileSize)), star);
                }
                else if (devided[0] == "NAME")
                {
                    mapName = devided[1];
                }
                else if (devided[0] == "CHERRY")
                {
                    Vector2<float> pos = new Vector2<float>(Int32.Parse(devided[1]), Int32.Parse(devided[2]));

                    EntityManager.GetInstance().AddEntity("Cherry", pos);
                }
            }
        }
        public void GetTile(int coord, out Tile t)
        {
            if (tiles.TryGetValue(coord, out t))
                return;

            t = null;
            return; 
        }
        public void GetTile(Vector2<float> cor, out Tile t)
        {
            Vector2<int> newCoord = new Vector2<int>((int)(cor.x / TileSize), (int)(cor.y / TileSize));
            GetTile(ConvertCoordinate(newCoord), out t);
        }

        public bool GetStar(Vector2<float> cor)
        {
            Vector2<int> newCoord = new Vector2<int>((int)((cor.x -8) / TileSize), (int)((cor.y - 8) / TileSize));
            return stars.TryGetValue(ConvertCoordinate(newCoord), out Star s);
        }

        public void EatStar(Vector2<float> cor)
        {
            Vector2<int> newCoord = new Vector2<int>((int)((cor.x - 8) / TileSize), (int)((cor.y - 8) / TileSize));

            stars.Remove(ConvertCoordinate(newCoord));
        }

        public string GetMapName()
        {
            return mapName;
        }

        public Vector2<float> GetPlayerStart()
        {
            return playerStart;
        }

        public bool GetCompleted()
        {
            return isComplete;
        }

        public void DrawMap(System.Windows.Forms.PaintEventArgs e)
        {
            foreach (KeyValuePair<int, Tile> t in tiles)
            {
                t.Value.tileSprite.DrawSprite(e);
            }

            foreach (KeyValuePair<int, Star> t in stars)
            {
                t.Value.starSprite.DrawSprite(e);
            }
        }

        public void Update()
        {
            if (stars.Count == 0)
            {
                isComplete = true;

                EntityManager.GetInstance().GetEntity(0, out var g);
                int score = ((Player)g).GetScore();
                Game.GetInstance().AddEvent(SceneType.Main_Game, EventType.MapComplete, score);

                Game.GetInstance().AddEvent(SceneType.GameOver, EventType.MapComplete, score);
            }
        }
    }
}
