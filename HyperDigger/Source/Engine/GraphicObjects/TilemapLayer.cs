using Microsoft.Xna.Framework;
using LDtk;
using LDtk.Renderer;

namespace HyperDigger
{
    class TilemapLayer : GraphicObject
    {
        public TilemapLayer() { }
        public TilemapLayer(Viewport _viewport = null)
        {
            Viewport = _viewport;
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
