using Microsoft.Xna.Framework;

namespace HyperDigger
{
    public class Main : Game
    {
        public Main()
        {
            Globals.Initialize(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            Globals.Graphics.Initialize();
            Globals.Audio.Initialize();
            base.Initialize();
        }

        protected override void LoadContent()
        {
            Globals.Cache.content = Content;
            WorldScene startScene = new WorldScene();
            Globals.SceneStack.AddScene(startScene);
        }

        protected override void Update(GameTime gameTime)
        {
            Globals.Input.Update(gameTime);
            if (Globals.SceneStack.IsFree())
                Exit();
            else
            {
                Globals.SceneStack.Update(gameTime);
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            Globals.Graphics.StartRender();
            Globals.SceneStack.Draw();
            Globals.Graphics.EndRender();

            base.Draw(gameTime);
        }
        protected override void UnloadContent()
        {
            Globals.Cache.FlushAll();
            base.UnloadContent();
        }
    }
}
