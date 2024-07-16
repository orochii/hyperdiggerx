using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace HyperDigger
{
    class Label : GameObject
    {
        public enum EVAlign { Top, Middle, Bottom }
        public enum EHAlign { Left, Middle, Right }

        public SpriteFont Font;
        public string Text;
        public Color Color = Color.White;
        public Color Shadow = Color.Transparent;
        public Vector2 ShadowDisplace = Vector2.One;
        public float Angle = 0f;
        public EVAlign VAlign = EVAlign.Top;
        public EHAlign HAlign = EHAlign.Left;
        public Vector2 Scale = new Vector2(1, 1);
        public SpriteEffects Effects = SpriteEffects.None;

        public Label() { }
        public Label(Container _container = null)
        {
            Container = _container;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (!Visible) return;
            if (Font == null) return;
            if (Text.Length == 0) return;

            Vector2 origPosition = GlobalPosition;
            Vector2 intPosition = new Vector2((int)origPosition.X, (int)origPosition.Y);
            Vector2 size = Font.MeasureString(Text);
            Vector2 origin = new Vector2(size.X * (.5f * (int)HAlign), size.Y * (.5f * (int)VAlign));

            if (Shadow.A != 0)
            {
                var shadowPos = intPosition + ShadowDisplace;
                var alpha = Shadow.A * Color.A / 255;
                var shadowColor = new Color(Shadow.R, Shadow.G, Shadow.B, alpha);
                spriteBatch.DrawString(Font, Text, shadowPos, shadowColor, Angle, origin, Scale, Effects, 0);
            }

            spriteBatch.DrawString(Font, Text, intPosition, Color, Angle, origin, Scale, Effects, 0);
        }
    }
}
