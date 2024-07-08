using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace HyperDigger
{
    class Sprite : GraphicObject
    {
        public Texture2D Texture = null;
        public Rectangle SourceRect = new Rectangle(0, 0, 0, 0);
        public Color Color = Color.White;
        public float Angle = 0f;
        public Vector2 Origin = Vector2.Zero;
        public Vector2 Scale = new Vector2(1, 1);
        public SpriteEffects Effects = SpriteEffects.None;

        public Sprite() { }
        public Sprite(Viewport _viewport = null)
        {
            Viewport = _viewport;
        }

        public override void Draw()
        {
            if (!Visible) return;
            if (Texture == null) return;

            Vector2 origPosition = GlobalPosition;
            Vector2 intPosition = new Vector2((int)origPosition.X, (int)origPosition.Y);

            if (SourceRect.Width == 0 || SourceRect.Height == 0)
            {
                Globals.Graphics.SpriteBatch.Draw(Texture, intPosition, null, Color, Angle, Origin, Scale, Effects, 0);
            }
            else
            {
                Globals.Graphics.SpriteBatch.Draw(Texture, intPosition, SourceRect, Color, Angle, Origin, Scale, Effects, 0);
            }
        }

        public override string ToString()
        {
            if (Viewport != null)
            {
                return "HyperDigger.Sprite " + Name + "(on " + Viewport.Name + ")";
            }
            return "HyperDigger.Sprite " + Name;
        }
    }
}
