using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperDigger
{
    internal class WorldmapScene : BaseScene
    {
        Sprite background;
        Container worldmap;
        TilemapWorld World;
        Player Player;
        Sprite[] levelSprites;
        Sprite playerSprite;
        Label someTitleText;
        Label someRandomText;

        Vector2 min;
        Vector2 max;
        Vector2 current;

        public WorldmapScene(Player player, TilemapWorld world) : base()
        {
            Player = player;
            World = world;
        }

        public override void Initialize()
        {
            background = new Sprite(Container);
            background.Texture = Global.Graphics.GetScreenshot();
            background.Color = Color.DarkGray;

            var r = new Rectangle(0, 40, Global.Graphics.Width, Global.Graphics.Height - 80);
            worldmap = new Container(r, Container);

            levelSprites = new Sprite[World.AllLevels.Length];
            for (int i = 0; i < World.AllLevels.Length; i++)
            {
                var l = World.AllLevels[i];
                var s = new Sprite(worldmap);

                s.Texture = l.GetMinimap();
                s.Position = l.GetPosition() / 16;
                levelSprites[i] = s;

                var _max = s.Position + new Vector2(s.Texture.Width, s.Texture.Height);
                if (s.Position.X < min.X) min.X = s.Position.X;
                if (s.Position.Y < min.Y) min.Y = s.Position.Y;
                if (max.X < _max.X) max.X = _max.X;
                if (max.Y < _max.Y) max.Y = _max.Y;
            }

            var offset = new Vector2(worldmap.Rectangle.Width, worldmap.Rectangle.Height) / 2;
            current = (Player.Position / 16);
            worldmap.Position = offset - current;

            playerSprite = new Sprite(worldmap);
            playerSprite.Texture = Global.Cache.LoadTexture("Graphics/System/mmap_loc");
            playerSprite.Origin = new Vector2(playerSprite.Texture.Width / 2, playerSprite.Texture.Height / 2);
            playerSprite.Position = current + new Vector2(0,-1);

            someTitleText = new Label(Container);
            someTitleText.Font = Global.Cache.LoadFont("Fonts/Title");
            someTitleText.Text = "Lost Savior";
            someTitleText.Position = new Vector2(Global.Graphics.Width / 2, 40);
            someTitleText.HAlign = Label.EHAlign.Middle;
            someTitleText.Shadow = new Color(0,0,0,128);

            someRandomText = new Label(Container);
            someRandomText.Font = Global.Cache.LoadFont("Fonts/Small");
            someRandomText.Text = "Lost Savior" + System.Environment.NewLine + "Another line?";
            someRandomText.Position = new Vector2(8, 64);
        }

        public override void Update(GameTime gameTime)
        {
            var horz = Global.Input.GetHorz();
            var vert = Global.Input.GetVert();

            current += new Vector2(horz, vert);
            current.X = Math.Clamp(current.X, min.X, max.X);
            current.Y = Math.Clamp(current.Y, min.Y, max.Y);

            var offset = new Vector2(worldmap.Rectangle.Width, worldmap.Rectangle.Height) / 2;
            worldmap.Position = offset - current;

            if (Global.Input.IsTriggered(Input.Button.MENU))
            {
                Global.SceneStack.Pop();
            }
        }
    }
}
