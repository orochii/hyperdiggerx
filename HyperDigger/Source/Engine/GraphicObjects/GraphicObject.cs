using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperDigger
{
    class GraphicObject
    {
        public string Name = "";
        public Vector2 Position = Vector2.Zero;
        public int Depth = 0;
        public bool Visible = true;

        private Viewport _viewport = null;
        public Viewport Viewport { get {
                return _viewport;
            } set {
                if (_viewport != value)
                {
                    if (_viewport != null) _viewport.RemoveGO(this);
                    _viewport = value;
                    if (_viewport != null) _viewport.AddGO(this);
                }
            } 
        }

        public GraphicObject(Viewport _viewport = null) {
            Viewport = _viewport;
        }

        public virtual void Update(GameTime gameTime) { }

        public virtual void Draw() { }

        public virtual Vector2 GlobalPosition { 
            get { 
                if (_viewport != null) return _viewport.GlobalPosition + Position;
                return Position;
            } 
        }

        public override string ToString()
        {
            return "HyperDigger.Viewport " + Name;
        }
    }
}
