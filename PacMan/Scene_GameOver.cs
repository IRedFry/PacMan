using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PacMan
{
    
    class Scene_GameOver : Scene
    {
        private int playerScore;
        private TextEnter tEnter = new TextEnter();
        private System.Windows.Forms.Label[] over = new System.Windows.Forms.Label[3];
        private bool setOver = false;

        public Scene_GameOver() : base()
        {
            playerScore = 0;
            transDraw = true;
            Game.MapComplete += HandleMapComplete;
            Game.NameEntered += HandleNameEntered;

            string res = "", sc = "", cont = "";
            if (Map.GetInstance().GetCompleted())
                res += "YOU WIN!!!";
            else
                res += "YOU LOSE!!!";

            sc = "Your score ";
            cont = "press enter your name";

            for (int i = 0; i < 3; ++i)
            {
                over[i] = new Label();
                over[i].Font = new System.Drawing.Font("ArcadeClassic", 20.0f);
                over[i].ForeColor = System.Drawing.Color.White;
                over[i].BackColor = System.Drawing.Color.Transparent;
                over[i].AutoSize = true;

                over[i].Location = new System.Drawing.Point((int)(200 * Sprite.ResizeSize.x + Sprite.offset.x), (int)((150 + 27 * i) * Sprite.ResizeSize.y + Sprite.offset.y));
            }

            over[0].Text = res;
            over[1].Text = sc;
            over[2].Text = cont;

            Form1.Resize += Form1_Resize;
        }

        private void Form1_Resize(float x, float y)
        {
            for (int i = 0; i < 3; ++i)
            {
                over[i].Location = new System.Drawing.Point((int)(200 * Sprite.ResizeSize.x + Sprite.offset.x), (int)((150 + 27 * i) * Sprite.ResizeSize.y + Sprite.offset.y));
            }
        }

        private void HandleMapComplete(SceneType type, int arg, string string_args)
        {
            if (type == SceneType.GameOver)
            {
              playerScore = arg;
              over[1].Text += playerScore.ToString();
            }
            
        }

        private void HandleNameEntered(SceneType type, int arg, string string_args)
        {
            if (type == SceneType.GameOver)
            {
              SceneManager.GetInstance().Remove(SceneType.Main_Game);
              SceneManager.GetInstance().ChangeSceneTo(SceneType.Highscores);
              tEnter.LabelOff();
              for (int i = 0; i < 3; ++i)
                 over[i].Visible = false;

              SceneManager.GetInstance().Remove(SceneType.GameOver);
            }

        }

        public override void Draw(PaintEventArgs e)
        {
            e.Graphics.FillRectangle(new System.Drawing.SolidBrush(System.Drawing.Color.FromArgb(120, System.Drawing.Color.Black)), 0, 0, Form1.WindowSize.width, Form1.WindowSize.height);


            if (!setOver)
            {
                if (Form1.ActiveForm != null)
                {
                    Form1.ActiveForm.Controls.Add(over[0]);
                    Form1.ActiveForm.Controls.Add(over[1]);
                    Form1.ActiveForm.Controls.Add(over[2]);
                    setOver = true;
                }
            }

            tEnter.Draw(e);
        }

        public override void Update(long dT)
        {
            
        }

        public override void Destroy()
        {
            Game.GetInstance().AddEvent(SceneType.Highscores, EventType.NewRecord, playerScore, tEnter.GetName());

            tEnter.Destroy();
            tEnter = null;
            Game.MapComplete -= HandleMapComplete;
            Game.NameEntered -= HandleNameEntered;
            Form1.Resize -= Form1_Resize;
        }
    }
}
