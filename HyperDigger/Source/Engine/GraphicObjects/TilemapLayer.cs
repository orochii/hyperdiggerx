using Microsoft.Xna.Framework;
using LDtk;
using LDtk.Renderer;

namespace HyperDigger
{
    class TilemapLayer : GameObject
    {
        public TilemapLayer() { }
        public TilemapLayer(Container _container = null)
        {
            Container = _container;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public override void Draw()
        {
            
        }
    }
}
