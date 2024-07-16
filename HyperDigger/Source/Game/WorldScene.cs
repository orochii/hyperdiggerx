using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace HyperDigger
{
    internal class WorldScene : BaseScene
    {
        Player Player;
        TilemapWorld World;
        WorldHUD UI;

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
            UI = new WorldHUD(Container);
            UI.SetLevel(World.CurrentLevel);
            UI.Player = Player;
            World.OnCurrentLevelChange += OnCurrentLevelChange;
            // Play musics
            Global.Audio.PlayBGM("That Zen Moment.mp3");
        }

        private void OnCurrentLevelChange(TilemapLevel l)
        {
            UI.SetLevel(l);
        }

        public override void Update(GameTime gameTime)
        {
            //Player.Update(gameTime);
            World.Update(gameTime);
            UI.Update(gameTime);

            if (Global.Input.IsTriggered(Input.Button.MENU))
            {
                Global.SceneStack.AddScene(new WorldmapScene(Player, World));
            }
        }

        public override void Dispose()
        {
            
        }
    }
}
