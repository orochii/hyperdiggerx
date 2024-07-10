using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace HyperDigger
{
    class Container : GameObject
    {
        private List<GameObject> _graphicObjects = new List<GameObject>();

        public Container() {}
        public Container(Container _containerr = null)
        {
            Container = _containerr;
        }

        public void AddGO(GameObject go) {
            if (_graphicObjects.Contains(go)) return;
            _graphicObjects.Add(go);
            SortGO();
        }

        public void RemoveGO(GameObject go)
        {
            _graphicObjects.Remove(go);
        }

        public override void Update(GameTime gameTime)
        {
            foreach (GameObject obj in _graphicObjects)
            {
                obj.Update(gameTime);
            }
        }

        public override void Draw()
        {
            foreach (GameObject obj in _graphicObjects)
            {
                //System.Console.WriteLine(obj.ToString());
                obj.Draw();
            }
            //System.Console.WriteLine("END");
        }

        private void SortGO()
        {
            _graphicObjects.Sort(CompareGOs);
        }

        private int CompareGOs(GameObject left, GameObject right)
        {
            return left.Depth - right.Depth;
        }

        internal void DebugPrint()
        {
            foreach(GameObject obj in _graphicObjects)
            {
                System.Console.WriteLine(obj.ToString());
            }
        }
    }
}
