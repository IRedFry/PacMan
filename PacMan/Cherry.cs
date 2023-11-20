using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PacMan
{
    [Serializable]
    class Cherry : GameEntity
    {
        public Cherry() : base()
        {
        }

        public override void OnSerialization()
        {
            
        }
        public override void Update(long dT)
        {
            UpdateAABB();
        }

        public override void ReactOnCollision(GameEntity collider)
        {
            if (collider.GetName() == "Player")
                EntityManager.GetInstance().EraseEntity(self_id);
        }
    }
}
