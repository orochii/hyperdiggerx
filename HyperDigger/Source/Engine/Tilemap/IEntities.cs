using Microsoft.Xna.Framework;

namespace HyperDigger
{
    internal interface IEntities
    {
        const int BOUND_UNIT = 4;

        public abstract bool CollidesWith(int x, int y, Vector2 boundarySize);

        public abstract void DoUpdate(GameTime gameTime);
    }
}
