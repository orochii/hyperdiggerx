using Microsoft.Xna.Framework;

namespace HyperDigger
{
    internal class BaseScene
    {
        public Viewport Viewport;

        public BaseScene() {
            Viewport = new Viewport();
            Viewport.Name = "BaseSceneViewport";
        }
        public void Draw()
        {
            Viewport.Draw();
        }

        /* public void Setup(params) {} */
        public virtual void Initialize() { }
        public virtual void Update(GameTime gameTime) { }
        public virtual void Dispose() { }
    }
}
