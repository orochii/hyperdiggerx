using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace HyperDigger
{
    internal class SceneStack
    {
        List<BaseScene> sceneStack = new List<BaseScene>();

        internal void Update(GameTime gameTime)
        {
            CurrentScene().Update(gameTime);
        }

        internal void Draw()
        {
            if (!IsFree()) CurrentScene().Draw();
        }

        public BaseScene CurrentScene() { return sceneStack[sceneStack.Count-1]; }

        public void AddScene(BaseScene scene) { 
            sceneStack.Add(scene);
            scene.Initialize();
        }
        public void RemoveScene(BaseScene scene) {
            scene.Dispose();
            sceneStack.Remove(scene);
        }
        public void Pop() {
            var scene = sceneStack[sceneStack.Count-1];
            scene.Dispose();
            sceneStack.RemoveAt(sceneStack.Count-1);
        }

        public bool IsFree() { 
            return sceneStack.Count == 0;
        }

    }
}
