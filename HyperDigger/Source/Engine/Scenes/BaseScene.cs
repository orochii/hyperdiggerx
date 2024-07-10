using Microsoft.Xna.Framework;

namespace HyperDigger
{
    internal class BaseScene
    {
        public Container Container;

        public BaseScene() {
            Container = new Container();
            Container.Name = "BaseSceneContainer";
        }
        public void Draw()
        {
            Container.Draw();
        }

        /* public void Setup(params) {} */
        public virtual void Initialize() { }
        public virtual void Update(GameTime gameTime) { }
        public virtual void Dispose() { }
    }
}
