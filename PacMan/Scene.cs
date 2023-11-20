using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PacMan
{
    [Serializable]
    abstract class Scene
    {
        protected bool transDraw, transUpdate;
        public Scene()
        {
            transDraw = false;
            transUpdate = false;
        }

        public abstract void Draw(System.Windows.Forms.PaintEventArgs e);
        public abstract void Update(long dT);

        public bool GetTransDraw()
        {
            return transDraw;
        }
        public void SetTransDraw(bool t)
        {
            transDraw = t;
        }
        public bool GetTransUpdate()
        {
            return transUpdate;
        }
        public void SetTransUpdate(bool t)
        {
            transUpdate = t;
        }


        public abstract void Destroy();
    }
}
