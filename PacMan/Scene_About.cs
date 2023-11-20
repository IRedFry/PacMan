using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PacMan
{
    class Scene_About : Scene
    {
        private System.Drawing.Bitmap bgText;
        private Sprite bg = new Sprite();

        public Scene_About() : base()
        {
            bgText = Properties.Resources.ABOUT;
            bg.SetTexture(ref bgText, new Rect<int>(0, 0, bgText.Width, bgText.Height));
            Form1.KeyboardHit += KeyboardHit; ;
        }

        private void KeyboardHit(SceneType type, KeyEventArgs ek)
        {
            if (ek.KeyCode == Keys.Escape)
            {
                SceneManager.GetInstance().ChangeSceneTo(SceneType.Menu);
                SceneManager.GetInstance().Remove(SceneType.About);
            }
        }

        public override void Destroy()
        {
            Form1.KeyboardHit -= KeyboardHit;
        }

        public override void Update(long dT)
        {
            
        }

        public override void Draw(PaintEventArgs e)
        {
            bg.DrawSprite(e);
        }
    }
}
