using Microsoft.Xna.Framework;

namespace HyperDigger
{
    internal class Collider
    {
        public const int BOUND_UNIT = 4;

        enum EColliderType { BOX }

        public GameObject Parent;
        public Vector2 Offset;
        public Vector2 BoundarySize;

        public Collider(GameObject parent)
        {
            Parent = parent;
        }

        public bool CollidesWith(int x, int y, Collider other)
        {
            // TODO: Collision between different types of colliders.
            return BoxCollidesWithBox(x, y, other);
        }

        public bool BoxCollidesWithBox(int x, int y, Collider other)
        {
            float otherSizeX = other.BoundarySize.X;
            float otherSizeY = other.BoundarySize.Y;
            float xOther = other.OffsetX(x);
            float yOther = other.OffsetY(y);
            float thisSizeX = BoundarySize.X;
            float thisSizeY = BoundarySize.Y;
            float xThis = OffsetX(Parent.Position.X);
            float yThis = OffsetY(Parent.Position.Y);
            float deltaX = xOther - xThis;
            float deltaY = yOther - yThis;
            return (deltaX < otherSizeX + thisSizeX) || (deltaY < otherSizeY + thisSizeY);
        }

        public float OffsetX(float x)
        {
            return x + Offset.X;
        }
        public float OffsetY(float y)
        {
            return y + Offset.Y;
        }
    }
}
