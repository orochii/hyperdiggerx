using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
namespace HyperDigger
{
    public class GameObject
    {
        public string Name = "";
        public Vector2 Position = Vector2.Zero;
        private int _depth;
        public int Depth { 
            get { return _depth; } 
            set {
                _depth = value;
                if (_container != null) _container.SortGO();
            }
        }
        public bool Visible = true;
        public TilemapWorld World;
        public bool Destroyed;

        private Container _container = null;
        public Container Container { get {
                return _container;
            } set {
                if (_container != value)
                {
                    if (_container != null) _container.RemoveGO(this);
                    _container = value;
                    if (_container != null) _container.AddGO(this);
                }
            } 
        }

        public GameObject(Container _container = null) {
            Container = _container;
        }

        public virtual void Update(GameTime gameTime) { }

        public virtual void Draw(SpriteBatch spriteBatch) { }

        public virtual void Predraw() { }

        public virtual Vector2 GlobalPosition { 
            get { 
                if (_container != null) return _container.GlobalPosition + Position;
                return Position;
            }
        }

        public override string ToString()
        {
            return "HyperDigger.Container " + Name;
        }
    }
}
