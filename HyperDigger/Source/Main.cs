using Microsoft.Xna.Framework;

namespace HyperDigger
{
    public class Main : Game
    {
        public Main()
        {
            Global.Initialize(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            Global.Graphics.Initialize();
            Global.Audio.Initialize();
            base.Initialize();
        }

        protected override void LoadContent()
        {
            Global.Cache.content = Content;
            WorldScene startScene = new WorldScene();
            Global.SceneStack.AddScene(startScene);
        }

        protected override void Update(GameTime gameTime)
        {
            Global.Input.Update(gameTime);
            if (Global.SceneStack.IsFree())
                Exit();
            else
            {
                Global.SceneStack.Update(gameTime);
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            Global.Graphics.StartRender();
            Global.SceneStack.Draw();
            Global.Graphics.EndRender();

            base.Draw(gameTime);
        }
        protected override void UnloadContent()
        {
            Global.Cache.FlushAll();
            base.UnloadContent();
        }
    }
}
