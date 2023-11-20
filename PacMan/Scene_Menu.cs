using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace PacMan
{
    
    class Scene_Menu : Scene
    {
        private Sprite[] buttons = new Sprite[5];
        private Sprite bg, logo;
        private System.Drawing.Bitmap mainText, bgText;
        private bool hasSave = false;
        public Scene_Menu() : base()
        {
            mainText = Properties.Resources.MAIN_TEXTURE;
            bgText = Properties.Resources.BG_MAIN;

            buttons[0] = new Sprite();
            buttons[0].SetTexture(ref mainText, new Rect<int>(306, 116, 306, 71));
            buttons[0].SetOrigin(new Vector2<float>(buttons[0].GetGlobalBounds().x + buttons[0].GetGlobalBounds().width / 2, buttons[0].GetGlobalBounds().y + buttons[0].GetGlobalBounds().height / 2));
            buttons[0].SetPosition(400, 250);

            if (!System.IO.File.Exists("save.bin"))
            {
                hasSave = false;
                buttons[0].SetOppacity(120);
            }
            else
                hasSave = true;

            for (int i = 1; i < 5; ++i)
            {
                buttons[i] = new Sprite();

                buttons[i].SetTexture(ref mainText, new Rect<int>(0, 116 + 71 * (i - 1), 306, 71));
                buttons[i].SetOrigin(new Vector2<float>(buttons[i].GetGlobalBounds().x + buttons[i].GetGlobalBounds().width / 2, buttons[i].GetGlobalBounds().y + buttons[i].GetGlobalBounds().height / 2));
                buttons[i].SetPosition(400, 250 + 90 * i);
            }

            logo = new Sprite();
            logo.SetTexture(ref mainText, new Rect<int>(0, 0, 608, 116));
            logo.SetPosition(85, 2);

            bg = new Sprite();
            bg.SetTexture(ref bgText, new Rect<int>(0, 0, bgText.Width, bgText.Height));

            Form1.MouseClick += HandleMouseEvent;
            Game.ButtonPressed += HandleButtonPressed;
            Game.MapComplete += Game_MapComplete;
            
        }

        private void Game_MapComplete(SceneType type, int arg, string string_args)
        {
            buttons[1].SetTexture(ref mainText, new Rect<int>(0, 116, 306, 71));
            buttons[1].SetOrigin(new Vector2<float>(buttons[1].GetGlobalBounds().x + buttons[1].GetGlobalBounds().width / 2, buttons[1].GetGlobalBounds().y + buttons[1].GetGlobalBounds().height / 2));
            buttons[1].SetPosition(400, 340);

            buttons[0].SetTexture(ref mainText, new Rect<int>(306, 116, 306, 71));
            buttons[0].SetOrigin(new Vector2<float>(buttons[0].GetGlobalBounds().x + buttons[0].GetGlobalBounds().width / 2, buttons[0].GetGlobalBounds().y + buttons[0].GetGlobalBounds().height / 2));
            buttons[0].SetPosition(400, 250);
            buttons[0].SetOppacity(120);
            hasSave = false;

        }

        private void HandleButtonPressed(SceneType type, int int_arg, string string_args)
        {
            if (type == SceneType.Menu)
            {
                if (int_arg == 0)
                {
                    if (SceneManager.GetInstance().HasScene(SceneType.Main_Game))
                    {
                        SceneManager.GetInstance().ChangeSceneTo(SceneType.Main_Game);
                        hasSave = false;
                    }
                    else if (hasSave)
                    {
                        SceneManager.GetInstance().SerOn();
                        SceneManager.GetInstance().ChangeSceneTo(SceneType.Main_Game);
                    }
                }
                else if (int_arg == 1)
                {
                    SceneManager.GetInstance().Remove(SceneType.Main_Game);
                    SceneManager.GetInstance().ChangeSceneTo(SceneType.Main_Game);
                    buttons[0].SetTexture(ref mainText, new Rect<int>(306, 116, 306, 71));
                    buttons[0].SetOrigin(new Vector2<float>(buttons[0].GetGlobalBounds().x + buttons[0].GetGlobalBounds().width / 2, buttons[0].GetGlobalBounds().y + buttons[0].GetGlobalBounds().height / 2));
                    buttons[0].SetPosition(400, 250);
                }
                else if (int_arg == 2)
                {
                    SceneManager.GetInstance().ChangeSceneTo(SceneType.Highscores);
                }
                else if (int_arg == 3)
                {
                    SceneManager.GetInstance().ChangeSceneTo(SceneType.About);
                }
                else if (int_arg == 4)
                {
                    if (SceneManager.GetInstance().HasScene(SceneType.Main_Game))
                    {
                        SceneManager.GetInstance().Serialization();
                    }
                    Application.Exit();
                }
            }
        }

        public override void Update(long dT)
        {


        }

        public override void Draw(PaintEventArgs e)
        {
            bg.DrawSprite(e);
            for (int i = 0; i < buttons.Length; ++i)
            {
                buttons[i].DrawSprite(e);
            }

            logo.DrawSprite(e);
        }

        private void HandleMouseEvent(SceneType type, MouseEventArgs em)
        {
            if (type == SceneType.Menu)
            {    
                Vector2<float> mousePos = new Vector2<float>(em.X, em.Y);
                for (int i = 0; i < 5; ++i)
                {
                    Console.WriteLine("{0} | {1} | {2} | {3}", buttons[0].GetGlobalBounds().x, buttons[0].GetGlobalBounds().y, buttons[0].GetGlobalBounds().width, buttons[0].GetGlobalBounds().height);
                    if (buttons[i].GetGlobalBounds().contains(new Vector2<float>((mousePos.x - Sprite.offset.x) / Sprite.ResizeSize.x, (mousePos.y - Sprite.offset.y) / Sprite.ResizeSize.y)))
                    {
                        Game.GetInstance().AddEvent(SceneType.Menu, EventType.ButtonPressed, i);
                    }

                }
            }
        }

        public override void Destroy()
        {
            Form1.MouseClick -= HandleMouseEvent;
            Game.ButtonPressed -= HandleButtonPressed;
            Game.MapComplete -= Game_MapComplete;
        }
    }
}
