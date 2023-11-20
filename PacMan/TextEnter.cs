using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PacMan
{
    
    class TextEnter
    {
        private bool setLabel = false;

        private static string charSet = "ABCDEFGHIGKLMNOPQRSTUVWXYZ";
        private static Vector2<float> posA = new Vector2<float>(150 + 103 - 3 - 3, 150 +201 - 3 - 3);
        private static Vector2<float> charSize = new Vector2<float>(18 + 13, 18 + 16);

        private System.Drawing.Bitmap texture;
        private Sprite keyboard = new Sprite(), frame = new Sprite();
        private System.Windows.Forms.Label name = new System.Windows.Forms.Label();

        private string s_name = "";

        private int curChar;


        public TextEnter()
        {
            curChar = 0;
            texture = Properties.Resources.GameOver;

            keyboard.SetTexture(ref texture, new Rect<int>(0, 0, texture.Width, texture.Height - 29));
            keyboard.SetPosition(150, 150);

            frame.SetTexture(ref texture, new Rect<int>(0, texture.Height - 29, 32, 29));
            frame.SetPosition(posA);



            name.Font = new System.Drawing.Font("ArcadeClassic", 45.0f);
            name.ForeColor = System.Drawing.Color.Black;
            name.BackColor = System.Drawing.Color.White;
            name.Scale(new System.Drawing.SizeF(2.0f, 2.0f));
            name.AutoSize = true;
            name.Location = new System.Drawing.Point((int)(280 * Sprite.ResizeSize.x + Sprite.offset.x), (int)(260 * Sprite.ResizeSize.y + Sprite.offset.y));

            Form1.KeyboardHit += KeyboardEvent;

            Form1.Resize += Form1_Resize;
        }

        private void Form1_Resize(float x, float y)
        {
            name.Location = new System.Drawing.Point((int)(280 * Sprite.ResizeSize.x + Sprite.offset.x), (int)(260 * Sprite.ResizeSize.y + Sprite.offset.y));
        }

        public void Destroy()
        {
           Form1.KeyboardHit -= KeyboardEvent;
           Form1.Resize -= Form1_Resize;
        }
        public void Draw(System.Windows.Forms.PaintEventArgs e)
        {
            keyboard.DrawSprite(e);
            frame.DrawSprite(e);

            if (!setLabel)
            {
                if (Form1.ActiveForm != null)
                {
                    Form1.ActiveForm.Controls.Add(name);
                    setLabel = true;
                }
            }
        }

        private void KeyboardEvent(SceneType type, System.Windows.Forms.KeyEventArgs ek)
        {
            if (type == SceneType.GameOver)
            {
                switch (ek.KeyCode)
                {
                    case System.Windows.Forms.Keys.Left:
                        if (curChar % 10 != 0)
                        {
                            curChar -= 1;
                            frame.SetPosition(frame.GetPosition().x - charSize.x, frame.GetPosition().y);
                        }
                        else
                        {
                            curChar += 9;
                            frame.SetPosition(frame.GetPosition().x + 9 * charSize.x, frame.GetPosition().y);
                        }
                        System.Threading.Thread.Sleep(100);
                        break;
                    case System.Windows.Forms.Keys.Right:
                        if (curChar == 26)
                        {
                            curChar -= 6;
                            frame.SetPosition(frame.GetPosition().x - 6 * charSize.x, frame.GetPosition().y);
                        }
                        else if (curChar % 10 != 9)
                        {
                            curChar += 1;
                            frame.SetPosition(frame.GetPosition().x + charSize.x, frame.GetPosition().y);
                        }
                        else
                        {
                            curChar -= 9;
                            frame.SetPosition(frame.GetPosition().x - 9 * charSize.x, frame.GetPosition().y);
                        }
                        System.Threading.Thread.Sleep(100);
                        break;
                    case System.Windows.Forms.Keys.Up:
                        if (curChar / 10 != 0)
                        {
                            curChar -= 10;
                            frame.SetPosition(frame.GetPosition().x, frame.GetPosition().y - charSize.y);
                        }
                        else
                        {
                            curChar += 20;
                            frame.SetPosition(frame.GetPosition().x, frame.GetPosition().y + 2 * charSize.y);
                        }
                        System.Threading.Thread.Sleep(100);
                        break;
                    case System.Windows.Forms.Keys.Down:
                        if (curChar / 10 != 2)
                        {
                            frame.SetPosition(frame.GetPosition().x, frame.GetPosition().y + charSize.y);
                            curChar += 10;
                        }
                        else
                        {
                            curChar -= 20;
                            frame.SetPosition(frame.GetPosition().x, frame.GetPosition().y - 2 * charSize.y);
                        }
                        System.Threading.Thread.Sleep(100);
                        break;

                    case System.Windows.Forms.Keys.Enter:
                        if (curChar == 26)
                        {
                            if (s_name.Length > 0)
                            {
                                Game.GetInstance().AddEvent(SceneType.GameOver, EventType.NameEntered, 0, s_name);
                            }
                        }
                        else if (s_name.Length < 6)
                        {
                            s_name += charSet[curChar];
                            name.Text = s_name;
                        }
                        System.Threading.Thread.Sleep(100);
                        break;

                    case System.Windows.Forms.Keys.Back:
                        if (s_name.Length > 0)
                        {
                            s_name = s_name.Remove(s_name.Length - 1, 1);
                            name.Text = s_name;
                        }
                        System.Threading.Thread.Sleep(100);
                        break;
                }

                if (curChar >= 26)
                {
                    curChar = 26;
                    frame.SetTexture(ref texture,new Rect<int>(32, texture.Height- 29, 110, 29));
                    frame.SetPosition(posA.x + charSize.x * 6, posA.y + 2 * charSize.y);
                }
                else
                {
                    Vector2<float> pos = frame.GetPosition();
                    frame.SetTexture(ref texture, new Rect<int>(0, texture.Height - 29, 32, 29));
                    frame.SetPosition(pos);
                }
            }
        }

        public string GetName()
        {
            return s_name;
        }

        public void LabelOff()
        {
            name.Visible = false;
            name.Text = "";
        }
    }
}
