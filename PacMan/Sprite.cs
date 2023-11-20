using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
namespace PacMan
{
    [Serializable]
    class Sprite
    {
        private Rect<float> bounds;
        public static Rect<int> StartSize;
        public static Vector2<float> offset = new Vector2<float>(0,0);
        public static Vector2<float> ResizeSize = new Vector2<float>(1, 1);
        private Vector2<float> origin;
        private Bitmap sprite;
        private int opacity;
        public Sprite()
        {
            sprite = null;
            opacity = 255;

            Form1.Resize += Form1_Resize;
        }
        ~Sprite()
        {
            Form1.Resize += Form1_Resize;
        }
        private void Form1_Resize(float x, float y)
        {
            float Sy = y / StartSize.height;
            float Sx = x / StartSize.width;
            if (Sx < Sy)
            {
                offset.x = 0;
                offset.y = (y - StartSize.height * Sx) / 2.0f;
                Sy = Sx;
            }
            else
            {
                offset.x = (x - StartSize.width * Sy) / 2.0f;
                offset.y = 0;
                Sx = Sy;
            }

            ResizeSize.x = Sx;
            ResizeSize.y = Sy;


        }

        public void SetTexture(ref Bitmap texture, Rect<int> rect)
        {
            sprite = texture.Clone(new Rectangle(rect.x, rect.y, rect.width, rect.height), texture.PixelFormat);

            bounds = new Rect<float>(0, 0, rect.width, rect.height);

            origin = new Vector2<float>(0, 0);

        }

        public void SetOrigin(float x, float y)
        {
            origin = new Vector2<float>(x, y);
        }
        public void SetOrigin(Vector2<float> newOrigin)
        {
            origin = newOrigin;
        }

        public Rect<float> GetGlobalBounds()
        {
            return bounds;
        }

        public Vector2<float> GetPosition()
        {
            return new Vector2<float>(bounds.x + origin.x, bounds.y + origin.y);
        }

        public void SetPosition(float x, float y)
        {
            bounds.x = (x - origin.x);// * ResizeSize.x;
            bounds.y = (y - origin.y);// * ResizeSize.y;
        }

        public void SetPosition(Vector2<float> pos)
        {
            bounds.x = (pos.x - origin.x);// * ResizeSize.x;
            bounds.y = (pos.y - origin.y);// * ResizeSize.y;
        }

        public void SetOppacity(int op)
        {
            opacity = op;
            Bitmap bmp = new Bitmap(sprite.Width, sprite.Height);
            Color c, v;
            for (int x = 0; x < sprite.Width; ++x)
            {
                for (int y = 0; y < sprite.Height; ++y)
                {
                    c = sprite.GetPixel(x, y);
                    if (c.A == 0)
                        v = Color.FromArgb(0, c.R, c.G, c.B);
                    else
                        v = Color.FromArgb(op, c.R, c.G, c.B);

                    bmp.SetPixel(x, y, v);
                }
            }

            sprite = bmp.Clone(new Rectangle(0,0,sprite.Width,sprite.Height),sprite.PixelFormat);
            
        }

        public int GetOpacity()
        {
            return opacity;
        }

        public void SetRotation(Direction start, Direction end)
        { 
            if (start == end)
                return;


            switch (start)
            {
                case Direction.Up:
                    sprite.RotateFlip(RotateFlipType.Rotate90FlipNone);
                    break;
                case Direction.Down:
                    sprite.RotateFlip(RotateFlipType.Rotate270FlipNone);
                    break;
                case Direction.Right:
                    break;
                case Direction.Left:
                    sprite.RotateFlip(RotateFlipType.RotateNoneFlipX);
                    break;
            }

            switch (end)
            {
                case Direction.Up:
                    sprite.RotateFlip(RotateFlipType.Rotate270FlipNone);
                    break;
                case Direction.Down:
                    sprite.RotateFlip(RotateFlipType.Rotate90FlipNone);
                    break;
                case Direction.Right:
                    break;
                case Direction.Left:
                    sprite.RotateFlip(RotateFlipType.RotateNoneFlipX);
                    break;
            }
        }

        public void DrawSprite(System.Windows.Forms.PaintEventArgs e)
        {

            e.Graphics.DrawImage(sprite, bounds.x * ResizeSize.x + offset.x, bounds.y * ResizeSize.y + offset.y, bounds.width * ResizeSize.x, bounds.height * ResizeSize.y);
        }
    }
}
