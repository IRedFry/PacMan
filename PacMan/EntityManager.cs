using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PacMan
{
    using Entities = Dictionary<int, GameEntity>;
    using EntityParameters = Dictionary<string, Rect<int>>;
    using EntityFactory = Dictionary<string, Func<GameEntity>>;
    [Serializable]
    class EntityManager
    {
        private EntityParameters data = new EntityParameters();
        private Entities entities = new Entities();
        private EntityFactory factory = new EntityFactory();
        private System.Drawing.Bitmap entityTexture;
        private int id = 2;

        private bool GameState = false;

        private static EntityManager instance;

        private long innerTimer = 0;
        private int endCounter = 0;
        private void RegisterEntity<T>(string name) where T : new()
        {
            factory[name] = () =>
            {
                return (GameEntity)Activator.CreateInstance(typeof(T));
            };
        }

        private void CheckCollisions()
        {
            if (entities.Count > 1)
            {
                int size = entities.Count;

                GetEntity(0, out GameEntity g);
                Player player = (Player)g;

                if (player != null)
                {
                    foreach (KeyValuePair<int, GameEntity> ent in entities)
                    {

                        if (player.GetAABB().intersects(ent.Value.GetAABB()))
                        {
                            player.ReactOnCollision(ent.Value);
                            if (ent.Value.GetName() == "Blinky" || ent.Value.GetName() == "Pinky" || ent.Value.GetName() == "Inky" || ent.Value.GetName() == "Clyde")
                            {
                                ((Ghost)ent.Value).ReactOnCollision(player);
                            }
                            else if (ent.Value.GetName() == "Cherry")
                            {
                                ((Cherry)ent.Value).ReactOnCollision(player);
                            }

                            if (entities.Count != size)
                            {
                                break;
                            }
                        }
                    }
                }
            }
        }

        private EntityManager()
        {
            entityTexture = Properties.Resources.entities;
            LoadEntities();

            // Registrations
            RegisterEntity<Player>("Player");
            RegisterEntity<Ghost>("Blinky");
            RegisterEntity<Ghost>("Pinky");
            RegisterEntity<Ghost>("Inky");
            RegisterEntity<Ghost>("Clyde");
            RegisterEntity<Cherry>("Cherry");

            Game.PlayerDied += HandlePlayerDied;
            Game.LivesChanged += HandleLivesChanged;
            Game.CherryEaten += Game_CherryEaten;
        }

        private void Game_CherryEaten(SceneType type, int arg, string string_args)
        {
            if (type == SceneType.Main_Game)
            {
                GameState = true;
            }
        }

        public void Destroy()
        {
            Game.PlayerDied -= HandlePlayerDied;
            Game.LivesChanged -= HandleLivesChanged;
            instance = null;
        }
        private void HandlePlayerDied(SceneType type, int arg, string string_args)
        {
            if (type == SceneType.Main_Game)
            {
              AddEntity("Player", Map.GetInstance().GetPlayerStart());
              GetEntity(0, out GameEntity g);
              ((Player)g).ChangeScore(arg);
            }
        }
        private void HandleLivesChanged(SceneType type, int arg, string string_args)
        {
            if (type == SceneType.Main_Game)
            { 
               GetEntity(0, out GameEntity g);
               ((Player)g).ChangeLives(arg - 3);
            }
        }

        public static EntityManager GetInstance()
        {
            if (instance == null)
                instance = new EntityManager();
            
            return instance;
        }

        public void OnSerialization()
        {
            instance = this;

            Game.PlayerDied += HandlePlayerDied;
            Game.LivesChanged += HandleLivesChanged;
            Game.CherryEaten += Game_CherryEaten;

            foreach (var e in entities)
                e.Value.OnSerialization();
        }

        public void GetEntity(int id, out GameEntity g)
        {
            entities.TryGetValue(id, out g);

        }

        public void AddEntity(string name, Vector2<float> coords)
        {
            if (factory.TryGetValue(name, out var entCreate))
            {
                GameEntity ent = entCreate.Invoke();

                if (data.TryGetValue(name, out var _data))
                {
                    int resid;

                    if (name == "Player")
                        resid = 0;
                    else if (name == "Blinky")
                        resid = 1;
                    else
                        resid = id++;

                    ent.Create(ref entityTexture, _data, coords, name, resid);
                    entities.Add(resid, ent);

                    if (name == "Blinky")
                        ((Ghost)ent).SetType(GhostType.Blinky);
                    else if (name == "Pinky")
                        ((Ghost)ent).SetType(GhostType.Pinky);
                    else if (name == "Inky")
                        ((Ghost)ent).SetType(GhostType.Inky);
                    else if (name == "Clyde")
                        ((Ghost)ent).SetType(GhostType.Clyde);
                }
                else
                    return;
            }
            else
                return;

        }

        public void SetEntityTextureOrigin(GameEntity g)
        {
            data.TryGetValue(g.GetName(), out var _data);

            g.Create(ref entityTexture, _data, g.GetPos(), g.GetName(), g.GetId());
        }

        public void EraseEntity(int id)
        {
            entities.Remove(id);
        }

        public void LoadEntities()
        {
            string[] lines = (Properties.Resources.entities1.Split('\n'));

           foreach(string line in lines)
            {
                string[] devided = line.Split(' ');

                string name = devided[0];

                Rect<int> coords = new Rect<int>(int.Parse(devided[1]), int.Parse(devided[2]), int.Parse(devided[3]), int.Parse(devided[4]));

                data.Add(name, coords);
            }
        }

        public void DrawEntities(System.Windows.Forms.PaintEventArgs e)
        {
            foreach (KeyValuePair<int, GameEntity> ent in entities)
            {
                ent.Value.Draw(e);
            }
        }

        public bool GetGameState()
        {
            return GameState;
        }

        public void Update(long dT)
        {
            if (GameState)
            {
                innerTimer += dT;
                if (innerTimer >= 100)
                {
                    if (endCounter < 80)
                    {
                        Game.GetInstance().AddEvent(SceneType.Main_Game, EventType.Blink);
                        Console.WriteLine("Blink - {0}",endCounter);
                        endCounter++;
                        innerTimer = 0;
                    }
                    else
                    {
                        GameState = false;
                        innerTimer = 0;
                        endCounter = 0;

                        Game.GetInstance().AddEvent(SceneType.Main_Game, EventType.StateEnd);
                    }

                }
            }
            foreach (KeyValuePair<int, GameEntity> ent in entities)
            {
                ent.Value.Update(dT);
            }
            CheckCollisions();
        }
    }
}
