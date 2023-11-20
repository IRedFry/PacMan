using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.Runtime.InteropServices;

namespace PacMan
{
    [Serializable]
    public struct Vector2<T>
    {
        public T x, y;

        public Vector2(T x, T y)
        {
            this.x = x;
            this.y = y;
        }

        public static bool operator ==(Vector2<T> obj1, Vector2<T> obj2)
        {
            if (EqualityComparer<T>.Default.Equals(obj1.x, obj2.x) && EqualityComparer<T>.Default.Equals(obj1.y, obj2.y))
                    return true;
            else
                return false;
        }

        public static bool operator !=(Vector2<T> obj1, Vector2<T> obj2)
        {
            return !(obj1 == obj2);
        }
    }
    [Serializable]
    public struct Rect<T>
    {
        public T x, y, width, height;

        public Rect(T x, T y, T width, T height)
        {
            this.x = x;
            this.y = y;
            this.width = width;
            this.height = height;
        }

        public bool contains(Vector2<T> point)
        {
            if ((Comparer<T>.Default.Compare(point.x, x) >= 0) && (Comparer<T>.Default.Compare(point.y, y) >= 0) && (Comparer<T>.Default.Compare(point.y, Add(y, height)) <= 0) && (Comparer<T>.Default.Compare(point.x, Add(x, width)) <= 0))
                return true;
            else
                return false;
        }

        public bool intersects(Rect<T> inter)
        {
            if (this.contains(new Vector2<T>(inter.x,inter.y)) || this.contains(new Vector2<T>(Add(inter.x,inter.width), inter.y)) || this.contains(new Vector2<T>(inter.x, Add(inter.y,inter.height))) || this.contains(new Vector2<T>(Add(inter.x,inter.width), Add(inter.y,inter.height))))
                return true;
            else
                return false;
        }

        private T Add(T one, T two)
        {
            dynamic a = one;
            dynamic b = two;
            return a + b ;
        }
    }

    public partial class Form1 : Form
    {
        private Game maingame;
        private System.Diagnostics.Stopwatch sw;
        public static Rect<int> WindowSize;
        
        public static event HandleKeyboardEvent KeyboardHit;
        public static event HandleMouseEvent MouseClick;

        public delegate void TwoFloat(float x, float y);
        public static event TwoFloat Resize;

        public Form1()
        {
            InitializeComponent();
        }
                [System.Runtime.InteropServices.DllImport("gdi32.dll")]
        private static extern IntPtr AddFontMemResourceEx(IntPtr pbFont, uint cbFont,IntPtr pdv, [System.Runtime.InteropServices.In] ref uint pcFonts);

        private System.Drawing.Text.PrivateFontCollection fonts = new System.Drawing.Text.PrivateFontCollection();

        public static Font myFont;


        private void Form1_Load(object sender, System.EventArgs e)
        {

            byte[] fontData = Properties.Resources.overFont;
            IntPtr fontPtr = System.Runtime.InteropServices.Marshal.AllocCoTaskMem(fontData.Length);
            System.Runtime.InteropServices.Marshal.Copy(fontData, 0, fontPtr, fontData.Length);
            uint dummy = 0;
            fonts.AddMemoryFont(fontPtr, Properties.Resources.overFont.Length);
            AddFontMemResourceEx(fontPtr, (uint)Properties.Resources.overFont.Length, IntPtr.Zero, ref dummy);
            System.Runtime.InteropServices.Marshal.FreeCoTaskMem(fontPtr);

            DoubleBuffered = true;
            maingame =  Game.GetInstance();

            Application.Idle += Game_Update;


            sw = new System.Diagnostics.Stopwatch();
            sw.Start();

            WindowSize = new Rect<int>(0, 0, ClientSize.Width, ClientSize.Height);

            Sprite.StartSize = new Rect<int>(0,0,WindowSize.width,WindowSize.height);
        }

        private void Game_Update(object sender, System.EventArgs e)
        {
                maingame.Update(sw.ElapsedMilliseconds);
                sw.Restart();     
        }
        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            maingame.Draw(e);
            
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (KeyboardHit != null)
                KeyboardHit(SceneManager.GetInstance().GetCurrentScene(), e);
        }

        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            if (MouseClick != null)
                MouseClick(SceneManager.GetInstance().GetCurrentScene(), e);
        }

        private void Form1_ClientSizeChanged(object sender, System.EventArgs e)
        {
            WindowSize = new Rect<int>(0, 0, ClientSize.Width, ClientSize.Height);
            Resize(WindowSize.width, WindowSize.height);
        }
    }
}
