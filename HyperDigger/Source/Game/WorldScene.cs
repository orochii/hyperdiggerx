using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Linq;

namespace HyperDigger
{
    internal class WorldScene : BaseScene
    {
        Player Player;
        TilemapWorld World;

        public override void Initialize()
        {
            // Create player
            Player = new Player(Viewport);
            // Let's add a dumb tilemap
            World = new TilemapWorld(Viewport, "Maps/startMap", 0);
            World.ActivateLevel("Level_0");

            Player.World = World;
            Player.Position = new Vector2(64, 176);

            Viewport.DebugPrint();
        }

        public override void Update(GameTime gameTime)
        {
            Player.Update(gameTime);
            World.Update(gameTime);
            World.SetActiveLevelByBoundaries((int)Player.Position.X, (int)Player.Position.Y);
            // Follow player
            Viewport.Position.X = (Globals.Graphics.Width / 2) - Player.Position.X;
            Viewport.Position.Y = (Globals.Graphics.Height / 2) - Player.Position.Y;
            ClampViewToBoundaries();
        }

        private void ClampViewToBoundaries()
        {
            var boundaries = World.GetCurrentBoundaries();
            if (boundaries.Width == 0) return;

            var minX = boundaries.Left;
            var minY = boundaries.Top;
            var maxX = boundaries.Right - Globals.Graphics.Width;
            var maxY = boundaries.Bottom - Globals.Graphics.Height;
            maxX = Math.Max(minX, maxX);
            maxY = Math.Max(minY, maxY);
            Viewport.Position.X = Math.Clamp(Viewport.Position.X, -maxX, -minX);
            Viewport.Position.Y = Math.Clamp(Viewport.Position.Y, -maxY, -minY);
        }

        public override void Dispose()
        {
            
        }
    }
}
