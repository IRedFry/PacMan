using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PacMan
{
    [Serializable]
    class Scene_Game : Scene
    {
        private Map gameMap;
        private EntityManager entityManager;
        private UI gameUI;
        public Scene_Game() : base()
        {
            gameMap = Map.GetInstance();
            gameMap.LoadMap("map");

            entityManager = EntityManager.GetInstance();
            gameUI = new UI();


            Form1.KeyboardHit += KeyBoardEvent;
            Game.MapComplete += HandleMapComplete;
        }

        public void OnSerialization()
        {
            Form1.KeyboardHit += KeyBoardEvent;
            Game.MapComplete += HandleMapComplete;

            entityManager.OnSerialization();
            gameMap.OnSerialization();
            gameUI.OnSerialization();
        }

        private void HandleMapComplete(SceneType type, int arg, string string_args)
        {
            if (type == SceneType.Main_Game)
            {
                System.IO.File.Delete("save.bin");
              SceneManager.GetInstance().ChangeSceneTo(SceneType.GameOver);
            }
        }
        private void KeyBoardEvent(SceneType type, KeyEventArgs ek)
        {
            if (type == SceneType.Main_Game)
            {
                if (ek.KeyCode == Keys.Escape)
                    SceneManager.GetInstance().ChangeSceneTo(SceneType.Pause);
                else if (ek.KeyCode == Keys.D || ek.KeyCode == Keys.S || ek.KeyCode == Keys.A || ek.KeyCode == Keys.W)
                {
                    EntityManager.GetInstance().GetEntity(0, out GameEntity g);
                    if (g != null)
                    {
                        Player p = (Player)g;

                        switch (ek.KeyCode)
                        {
                            case Keys.D:
                                p.SetDir(Direction.Right);
                                break;
                            case Keys.W:
                                p.SetDir(Direction.Up);
                                break;
                            case Keys.A:
                                p.SetDir(Direction.Left);
                                break;
                            case Keys.S:
                                p.SetDir(Direction.Down);
                                break;
                        }
                    }
                }
            }           
        }

        public override void Update(long dT)
        {
            entityManager.Update(dT);
            gameMap.Update();
        }

        public override void Draw(PaintEventArgs e)
        {
            gameMap.DrawMap(e);
            entityManager.DrawEntities(e);
            gameUI.Draw();
        }

        public override void Destroy()
        {
            Form1.KeyboardHit -= KeyBoardEvent;
            Game.MapComplete -= HandleMapComplete;

            entityManager.Destroy();
            entityManager = null;
            gameMap.Destroy();
            gameMap = null;
            gameUI.Destroy();
        }
    }
}
