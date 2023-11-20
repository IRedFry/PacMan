using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PacMan
{
    [Serializable]
    class UI
    {
        [NonSerialized]
        private System.Windows.Forms.Label[] StaticInfo = new System.Windows.Forms.Label[3];
        [NonSerialized]
        private System.Windows.Forms.Label[] DynamicInfo = new System.Windows.Forms.Label[3];
        private bool setText = false;

        public UI()
        {
            for (int i = 0; i < 3; ++i)
            {
                StaticInfo[i] = new System.Windows.Forms.Label();
                DynamicInfo[i] = new System.Windows.Forms.Label();

                StaticInfo[i].Font = new System.Drawing.Font("ArcadeClassic", 20.0f);
                DynamicInfo[i].Font = new System.Drawing.Font("ArcadeClassic", 20.0f);

                StaticInfo[i].Location = new System.Drawing.Point((int)((10 + 260 * i) * Sprite.ResizeSize.x + Sprite.offset.x), (int)(650 * Sprite.ResizeSize.y + Sprite.offset.y));
                DynamicInfo[i].Location = new System.Drawing.Point(StaticInfo[i].Bounds.X + StaticInfo[i].Bounds.Width + 15, (int)(650 * Sprite.ResizeSize.y + Sprite.offset.y));

                StaticInfo[i].ForeColor = System.Drawing.Color.White;
                StaticInfo[i].BackColor = System.Drawing.Color.Black;

                DynamicInfo[i].ForeColor = System.Drawing.Color.White;
                DynamicInfo[i].BackColor = System.Drawing.Color.Black;

                StaticInfo[i].AutoSize = true;
                DynamicInfo[i].AutoSize = true;
            }

            StaticInfo[0].Text = "SCORE ";
            StaticInfo[1].Text = "LEVEL ";
            StaticInfo[2].Text = "LIVES ";

            EntityManager.GetInstance().GetEntity(0, out var g);
            DynamicInfo[2].Text = ((Player)g).GetLives().ToString();
            DynamicInfo[1].Text = Map.GetInstance().GetMapName();

            Game.LivesChanged += HandleLivesChanged;
            Game.ScoreChanged += HandleScoreChanged;
            Game.MapNameChanged += HandleMapNameChanged;
            Game.ButtonPressed += Game_ButtonPressed;

            Form1.Resize += Form1_Resize;
        }

        private void Form1_Resize(float x, float y)
        {
            for (int i = 0; i < 3; ++i)
            {
                StaticInfo[i].Location = new System.Drawing.Point((int)((10 + 260 * i) * Sprite.ResizeSize.x + Sprite.offset.x), (int)(650 * Sprite.ResizeSize.y + Sprite.offset.y));
                DynamicInfo[i].Location = new System.Drawing.Point(StaticInfo[i].Bounds.X + StaticInfo[i].Bounds.Width + 15, (int)(650 * Sprite.ResizeSize.y + Sprite.offset.y));
            }
        }

        public void OnSerialization()
        {
            setText = false;
            Game.LivesChanged += HandleLivesChanged;
            Game.ScoreChanged += HandleScoreChanged;
            Game.MapNameChanged += HandleMapNameChanged;
            Game.ButtonPressed += Game_ButtonPressed;
            Form1.Resize += Form1_Resize;

            StaticInfo = new System.Windows.Forms.Label[3];
            DynamicInfo = new System.Windows.Forms.Label[3];

            for (int i = 0; i < 3; ++i)
            {
                StaticInfo[i] = new System.Windows.Forms.Label();
                DynamicInfo[i] = new System.Windows.Forms.Label();

                StaticInfo[i].Font = new System.Drawing.Font("ArcadeClassic", 20.0f);
                DynamicInfo[i].Font = new System.Drawing.Font("ArcadeClassic", 20.0f);

                StaticInfo[i].Location = new System.Drawing.Point((int)((10 + 260 * i) * Sprite.ResizeSize.x + Sprite.offset.x), (int)(650 * Sprite.ResizeSize.y + Sprite.offset.y));
                DynamicInfo[i].Location = new System.Drawing.Point(StaticInfo[i].Bounds.X + StaticInfo[i].Bounds.Width + 15, (int)(650 * Sprite.ResizeSize.y + Sprite.offset.y));

                StaticInfo[i].ForeColor = System.Drawing.Color.White;
                StaticInfo[i].BackColor = System.Drawing.Color.Black;

                DynamicInfo[i].ForeColor = System.Drawing.Color.White;
                DynamicInfo[i].BackColor = System.Drawing.Color.Black;

                StaticInfo[i].AutoSize = true;
                DynamicInfo[i].AutoSize = true;
            }

            StaticInfo[0].Text = "SCORE ";
            StaticInfo[1].Text = "LEVEL ";
            StaticInfo[2].Text = "LIVES ";

            EntityManager.GetInstance().GetEntity(0, out var g);
            DynamicInfo[2].Text = ((Player)g).GetLives().ToString();
            DynamicInfo[1].Text = Map.GetInstance().GetMapName();
            DynamicInfo[0].Text = ((Player)g).GetScore().ToString();
        }

        private void Game_ButtonPressed(SceneType type, int arg, string string_args)
        {
            if (type == SceneType.Pause && arg == 1)
            {
                for (int i = 0; i < 3; ++i)
                {
                    StaticInfo[i].Visible = false;
                    DynamicInfo[i].Visible = false;
                }
            }
            else if (type == SceneType.Menu && arg == 0)
            {
                for (int i = 0; i < 3; ++i)
                {
                    StaticInfo[i].Visible = true;
                    DynamicInfo[i].Visible = true;
                }
            }
        }

        public void Draw()
        {
            if (!setText && (Form1.ActiveForm != null))
                for (int i = 0; i < 3; ++i)
                {
                    Form1.ActiveForm.Controls.Add(StaticInfo[i]);
                    Form1.ActiveForm.Controls.Add(DynamicInfo[i]);
                    setText = true;
                }
        }

        private void HandleLivesChanged(SceneType type, int arg, string string_args)
        {
            if (type == SceneType.Main_Game)
            {
                if (arg < 0)
                  arg = 0;

                DynamicInfo[2].Text = arg.ToString();
            }
        }

        private void HandleScoreChanged(SceneType type, int arg, string string_args)
        {
            if (type == SceneType.Main_Game)
            {
              DynamicInfo[0].Text = arg.ToString();
            }
        }

        private void HandleMapNameChanged(SceneType type, int arg, string string_args)
        {
            if (type == SceneType.Main_Game)
            {
               DynamicInfo[1].Text = string_args;
            }
        }



        public void Destroy()
        {

            Game.LivesChanged -= HandleLivesChanged;
            Game.ScoreChanged -= HandleScoreChanged;
            Game.MapNameChanged -= HandleMapNameChanged;
            Game.ButtonPressed -= Game_ButtonPressed;
            Form1.Resize -= Form1_Resize;

            for (int i = 0; i < 3; ++i)
            {
                StaticInfo[i].Visible = false;
                DynamicInfo[i].Visible = false;
            }
        }
    }
}
