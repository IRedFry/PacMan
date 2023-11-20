using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PacMan
{
    [Serializable]
    abstract class GameEntity
    {
        protected Sprite sprite;
        protected Vector2<float> position, positionOld;
        protected Rect<float> AABB;
        protected string name;
        protected int self_id;

        public GameEntity()
        {
            sprite = new Sprite();
        }

        public abstract void Update(long dT);
        public abstract void OnSerialization();
        public virtual void Draw(System.Windows.Forms.PaintEventArgs e)
        {
            sprite.DrawSprite(e);
        }

        public void Create(ref System.Drawing.Bitmap texture, Rect<int> spriteRect, Vector2<float> pos, string name, int id)
        {
            this.name = name;
            sprite.SetTexture(ref texture, spriteRect);
            sprite.SetOrigin(sprite.GetGlobalBounds().width / 2, sprite.GetGlobalBounds().height / 2);
            position = pos;
            sprite.SetPosition(pos);
            AABB = sprite.GetGlobalBounds();
            self_id = id;
        }

        public string GetName()
        {
            return name;
        }
        public Vector2<float> GetPos()
        {
            return position; 
        }
        public Rect<float> GetAABB()
        {
            return AABB;
        }
        public int GetId()
        {
            return self_id;
        }

        protected void MoveEnt(float x, float y)
        {
            positionOld = position;
            position.x += x;
            position.y += y;
            sprite.SetPosition(position);
        }
        protected void UpdateAABB()
        {
            AABB = sprite.GetGlobalBounds();
        }
        public abstract void ReactOnCollision(GameEntity collider);
    }
}
