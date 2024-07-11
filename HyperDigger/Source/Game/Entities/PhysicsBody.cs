using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperDigger
{
    abstract class PhysicsBody : Sprite
    {
        public Collider Collider;

        public PhysicsBody(Container container) : base(container) {
            Collider = new Collider(this);
        }

        public bool CollidesWith(int x, int y, Collider other)
        {
            return Collider.CollidesWith(x, y, other);
        }

    }
}
