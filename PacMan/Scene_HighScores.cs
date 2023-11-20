using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.Serialization.Formatters.Binary;

namespace PacMan
{

    class Scene_HighScores : Scene
    {
        private List<KeyValuePair<string,int>> scores = new List<KeyValuePair<string, int>>();
        private List<KeyValuePair<Label, Label>> records = new List<KeyValuePair<Label, Label>>();

        private System.Drawing.Bitmap bgText;
        private Sprite bg = new Sprite();
        private bool setLabel = false;

        public Scene_HighScores() : base()
        {
            bgText = Properties.Resources.BG_HS;
            bg.SetTexture(ref bgText, new Rect<int>(0, 0, bgText.Width, bgText.Height));

            if (System.IO.File.Exists("HS.bin"))
            {
                Stream openFileStream = File.OpenRead("HS.bin");
                BinaryFormatter deserializer = new BinaryFormatter();
                scores = (List<KeyValuePair<string, int>>)deserializer.Deserialize(openFileStream);
                openFileStream.Close();
            }

            for (int i = 0; i < scores.Count; ++i)
            {
                Label l = new Label();
                Label l2 = new Label();

                l.Text = scores[i].Key;
                l2.Text = scores[i].Value.ToString();

                l.Font = new System.Drawing.Font("ArcadeClassic", 45.0f);
                l.ForeColor = System.Drawing.Color.White;
                l.BackColor = System.Drawing.Color.Black;
                l.Scale(new System.Drawing.SizeF(2.0f, 2.0f));
                l.AutoSize = true;
                l.Location = new System.Drawing.Point((int)(80 * Sprite.ResizeSize.x + Sprite.offset.x), (int)((100 + 75 * i) * Sprite.ResizeSize.y + Sprite.offset.y));

                l2.Font = new System.Drawing.Font("ArcadeClassic", 45.0f);
                l2.ForeColor = System.Drawing.Color.White;
                l2.BackColor = System.Drawing.Color.Black;
                l2.Scale(new System.Drawing.SizeF(2.0f, 2.0f));
                l2.AutoSize = true;
                l2.Location = new System.Drawing.Point((int)(575 * Sprite.ResizeSize.x + Sprite.offset.x), (int)((100 + 75 * i) * Sprite.ResizeSize.y + Sprite.offset.y));

                records.Add(new KeyValuePair<Label, Label>(l,l2));
            }

            Form1.KeyboardHit += KeyboardHit;
            Game.NewRecord += Game_NewRecord;
            Form1.Resize += Form1_Resize;
        }

        private void Form1_Resize(float x, float y)
        {
            for (int i = 0; i < scores.Count; ++i)
            {
                records[i].Key.Location = new System.Drawing.Point((int)(80 * Sprite.ResizeSize.x + Sprite.offset.x), (int)((100 + 75 * i) * Sprite.ResizeSize.y + Sprite.offset.y));
                records[i].Value.Location = new System.Drawing.Point((int)(575 * Sprite.ResizeSize.x + Sprite.offset.x), (int)((100 + 75 * i) * Sprite.ResizeSize.y + Sprite.offset.y));
            }
        }

        private void Game_NewRecord(SceneType type, int arg, string string_args)
        {
            int resId = scores.Count;
            for (int i = 0; i < scores.Count; ++i)
            {
                if (scores[i].Value > arg)
                {
                    resId = i;
                    break;
                }

            }

            if (scores.Count < 9 || scores.Count > 8 && resId != 0 )
            {
                scores.Insert(resId, new KeyValuePair<string, int>(string_args, arg));

                if (scores.Count > 8)
                {
                    for (int i = 0; i < 8; ++i)
                        scores[i] = scores[i + 1];

                    scores.RemoveAt(8);
                }

                Stream SaveFileStream = File.Create("HS.bin");
                BinaryFormatter serializer = new BinaryFormatter();

                serializer.Serialize(SaveFileStream, scores);
                SaveFileStream.Close();

                records.Clear();

                for (int i = 0; i < scores.Count; ++i)
                {
                    Label l = new Label();
                    Label l2 = new Label();

                    l.Text = scores[i].Key;
                    l2.Text = scores[i].Value.ToString();

                    l.Font = new System.Drawing.Font("ArcadeClassic", 45.0f);
                    l.ForeColor = System.Drawing.Color.White;
                    l.BackColor = System.Drawing.Color.Black;
                    l.Scale(new System.Drawing.SizeF(2.0f, 2.0f));
                    l.AutoSize = true;
                    l.Location = new System.Drawing.Point((int)(80 * Sprite.ResizeSize.x + Sprite.offset.x), (int)((100 + 75 * i) * Sprite.ResizeSize.y + Sprite.offset.y));

                    l2.Font = new System.Drawing.Font("ArcadeClassic", 45.0f);
                    l2.ForeColor = System.Drawing.Color.White;
                    l2.BackColor = System.Drawing.Color.Black;
                    l2.Scale(new System.Drawing.SizeF(2.0f, 2.0f));
                    l2.AutoSize = true;
                    l2.Location = new System.Drawing.Point((int)(575 * Sprite.ResizeSize.x + Sprite.offset.x), (int)((100 + 75 * i) * Sprite.ResizeSize.y + Sprite.offset.y));

                    records.Add(new KeyValuePair<Label, Label>(l, l2));
                }
            }
        }

        private void KeyboardHit(SceneType type, KeyEventArgs ek)
        {
            if (ek.KeyCode == Keys.Escape)
            {
                SceneManager.GetInstance().ChangeSceneTo(SceneType.Menu);
                SceneManager.GetInstance().Remove(SceneType.Highscores);
            }
        }

        public override void Update(long dT)
        {
            
        }

        public override void Draw(PaintEventArgs e)
        {
            bg.DrawSprite(e);
            if (!setLabel)
            {
                if (Form1.ActiveForm != null)
                {
                    for (int i = 0; i < records.Count; ++i)
                    {
                        Form1.ActiveForm.Controls.Add(records[i].Key);
                        Form1.ActiveForm.Controls.Add(records[i].Value);
                    }
                    setLabel = true;
                }
            }
        }

        public override void Destroy()
        {
            setLabel = false;

            if (Form1.ActiveForm != null)
            {
                for (int i = 0; i < records.Count; ++i)
                {
                    records[i].Key.Visible = false;
                    records[i].Value.Visible = false;
                }
            }

            Form1.KeyboardHit -= KeyboardHit;
            Game.NewRecord -= Game_NewRecord;
        }
    }
    
}
