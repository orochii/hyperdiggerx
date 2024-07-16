using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperDigger
{
    public abstract class PhysicsBody : Sprite
    {
        public Collider Collider;

        public PhysicsBody(Container container) : base(container) {
            Collider = new Collider(this);
        }

        public bool CollidesWith(int x, int y, Collider other)
        {
            return Collider.CollidesWith(x, y, other);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
            // Draw debug colliders.
            if (!Global.DebugDraw) return;
            // Draw collider
            if (Collider != null)
            {
                var sx = Collider.OffsetX(GlobalPosition.X) - Collider.BoundarySize.X;
                var sy = Collider.OffsetY(GlobalPosition.Y) - Collider.BoundarySize.Y;
                var w = Collider.BoundarySize.X * 2;
                var h = Collider.BoundarySize.Y * 2;
                var rect = new Rectangle((int)sx, (int)sy, (int)w, (int)h);
                Global.Graphics.DrawRectangle(spriteBatch, rect, new Color(64, 160, 224, 64));
            }
            // Draw origin
            Global.Graphics.DrawDot(spriteBatch, GlobalPosition, Color.IndianRed);
        }
    }
}
