using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace HyperDigger
{
    class Viewport : GraphicObject
    {
        private List<GraphicObject> _graphicObjects = new List<GraphicObject>();

        public Viewport() {}
        public Viewport(Viewport _viewport = null)
        {
            Viewport = _viewport;
        }

        public void AddGO(GraphicObject go) {
            if (_graphicObjects.Contains(go)) return;
            _graphicObjects.Add(go);
            SortGO();
        }

        public void RemoveGO(GraphicObject go)
        {
            _graphicObjects.Remove(go);
        }

        public override void Update(GameTime gameTime)
        {
            foreach (GraphicObject obj in _graphicObjects)
            {
                obj.Update(gameTime);
            }
        }

        public override void Draw()
        {
            foreach (GraphicObject obj in _graphicObjects)
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

        private int CompareGOs(GraphicObject left, GraphicObject right)
        {
            return left.Depth - right.Depth;
        }

        internal void DebugPrint()
        {
            foreach(GraphicObject obj in _graphicObjects)
            {
                System.Console.WriteLine(obj.ToString());
            }
        }
    }
}
