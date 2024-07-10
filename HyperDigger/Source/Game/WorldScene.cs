using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace HyperDigger
{
    internal class WorldScene : BaseScene
    {
        Player Player;
        TilemapWorld World;
        Container UI;

        public override void Initialize()
        {
            // Create world
            World = new TilemapWorld(Container, "Maps/startMap", 0);
            World.ActivateLevel("Level_0");
            // Create player
            Player = new Player(World.InnerContainer);
            Player.Position = new Vector2(64, 176);
            World.AddEntity(Player);
            World.ViewTarget = Player;
            // Create UI
            UI = new Container(Container);
            Sprite uiBack = new Sprite(UI);
            uiBack.Texture = Globals.Cache.LoadTexture("Graphics/System/HUD");

            // Play musics
            Globals.Audio.PlayBGM("That Zen Moment.mp3");
        }

        public override void Update(GameTime gameTime)
        {
            //Player.Update(gameTime);
            World.Update(gameTime);
            
        }

        public override void Dispose()
        {
            
        }
    }
}
