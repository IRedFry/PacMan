using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PacMan
{
    
    class Scene_Pause : Scene
    {
        private Sprite[] buttons = new Sprite[2];
        private System.Drawing.Bitmap mainText;

        public Scene_Pause() : base()
        {
            mainText = Properties.Resources.MAIN_TEXTURE;
            transDraw = true;

            for (int i = 0; i < 2; ++i)
            {
                buttons[i] = new Sprite();
                buttons[i].SetTexture(ref mainText, new Rect<int>(306, 116 + 71 * i, 306, 71));
                buttons[i].SetOrigin(new Vector2<float>(buttons[i].GetGlobalBounds().x + buttons[i].GetGlobalBounds().width / 2, buttons[i].GetGlobalBounds().y + buttons[i].GetGlobalBounds().height / 2));
                buttons[i].SetPosition(400, 300 + 110 * i);
            }

            Form1.MouseClick += HandleMouseEvent;
            Game.ButtonPressed += HandleButtonPressed;
        }

        public override void Update(long dT)
        {

        }

        public override void Draw(PaintEventArgs e)
        {
            e.Graphics.FillRectangle(new System.Drawing.SolidBrush(System.Drawing.Color.FromArgb(120, System.Drawing.Color.Black)),0,0,Form1.WindowSize.width, Form1.WindowSize.height);

            for (int i = 0; i < 2; ++i)
            {
                buttons[i].DrawSprite(e);
            }
        }

        private void HandleMouseEvent(SceneType type, MouseEventArgs em)
        {
            if (type == SceneType.Pause)
            {
                Vector2<float> mousePos = new Vector2<float>(em.X, em.Y);
                for (int i = 0; i < 2; ++i)
                {
                    Console.WriteLine("{0} | {1} | {2} | {3}", buttons[0].GetGlobalBounds().x, buttons[0].GetGlobalBounds().y, buttons[0].GetGlobalBounds().width, buttons[0].GetGlobalBounds().height);
                    if (buttons[i].GetGlobalBounds().contains(new Vector2<float>((mousePos.x - Sprite.offset.x) / Sprite.ResizeSize.x, (mousePos.y - Sprite.offset.y) / Sprite.ResizeSize.y)))
                    {
                        if (i == 0)
                            Game.GetInstance().AddEvent(SceneType.Pause, EventType.ButtonPressed,0);
                        else if (i == 1)
                            Game.GetInstance().AddEvent(SceneType.Pause, EventType.ButtonPressed, 1);
                    }

                }
            }
        }

        private void HandleButtonPressed(SceneType type, int int_arg, string string_arg)
        {
            if (type == SceneType.Pause)
            {
              if (int_arg == 0)
                 SceneManager.GetInstance().ChangeSceneTo(SceneType.Main_Game);
              else if (int_arg == 1)
                 SceneManager.GetInstance().ChangeSceneTo(SceneType.Menu);
            }
        }

        public override void Destroy()
        {
            Form1.MouseClick -= HandleMouseEvent;
            Game.ButtonPressed -= HandleButtonPressed;
        }

    }
    
}
