using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperDigger
{
    class Minimap : Container
    {
        const int MARGIN = 8;
        const int WIDTH = 40;
        const int HEIGHT = 30;
        const int SCALE = 2;

        Color MMAP_OPAC_NORMAL = new Color(Color.White, 0.8f);
        Color MMAP_OPAC_HOVER = new Color(Color.Gray, 0.5f);
        static Point Size = new Point(WIDTH, HEIGHT);
        static Point Pos = new Point(Global.Graphics.Width - MARGIN - WIDTH, MARGIN);

        public Player Player;
        Sprite minimapSprite;
        Sprite minimapLoc;
        TilemapLevel level;

        public Minimap(Container container) : 
            base(new Rectangle(Pos, Size), container) {

            Depth = 2;

            minimapSprite = new Sprite(this);
            minimapSprite.Scale = new Vector2(SCALE, SCALE);
            minimapSprite.Color = MMAP_OPAC_NORMAL;

            minimapLoc = new Sprite(this);
            minimapLoc.Texture = Global.Cache.LoadTexture("Graphics/System/mmap_loc");
            minimapLoc.Origin = new Vector2(minimapLoc.Texture.Width / 2, minimapLoc.Texture.Height / 2);
        }

        public void SetMinimap(TilemapLevel _level)
        {
            level = _level;
            minimapSprite.Texture = level.GetMinimap();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (Player != null)
            {
                minimapLoc.Visible = true;
                var pos = Player.Position - level.GetPosition();
                pos = (pos / 16 * minimapSprite.Scale);
                var offset = new Vector2(0, -1);
                minimapLoc.Position = pos + offset;

                // 
                var screenPos = Player.GlobalPosition;
                var start = new Vector2(_rectangle.Location.X, _rectangle.Location.Y);
                var end = start + (new Vector2(minimapSprite.Texture.Width, minimapSprite.Texture.Height) * minimapSprite.Scale);
                var startCheck = start - new Vector2(16, 16);
                var endCheck = end + new Vector2(16, 16);
                if (screenPos.X > startCheck.X && screenPos.X < endCheck.X
                    && screenPos.Y > startCheck.Y && screenPos.Y < endCheck.Y)
                {
                    minimapSprite.Color = MMAP_OPAC_HOVER;
                }
                else
                {
                    minimapSprite.Color = MMAP_OPAC_NORMAL;
                }
                var _ts = new Vector2(minimapSprite.Texture.Width, minimapSprite.Texture.Height) * minimapSprite.Scale;
                var _s = new Vector2(Size.X, Size.Y);
                var _p = (_s / 2) - minimapLoc.Position;
                var maxX = Math.Max(0, _ts.X - _s.X);
                var maxY = Math.Max(0, _ts.Y - _s.Y);
                _p.X = Math.Clamp(_p.X, -maxX, 0);
                _p.Y = Math.Clamp(_p.Y, -maxY, 0);
                Position = _p;
            }
            else
            {
                minimapLoc.Visible = false;
                minimapSprite.Color = MMAP_OPAC_NORMAL;
            }
        }
    }
}
