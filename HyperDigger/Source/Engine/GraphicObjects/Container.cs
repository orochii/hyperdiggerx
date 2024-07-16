using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace HyperDigger
{
    public class Container : GameObject
    {
        private List<GameObject> _graphicObjects = new List<GameObject>();
        private SpriteBatch _innerSpriteBatch;
        private RenderTarget2D _innerTarget;
        protected Rectangle _rectangle;

        public Rectangle Rectangle
        {
            get { return _rectangle; }
            set
            {
                _rectangle = value;
                _innerTarget = new RenderTarget2D(_innerSpriteBatch.GraphicsDevice, _rectangle.Width, _rectangle.Height);
            }
        }

        public Container(Rectangle r, Container _container = null)
        {
            Container = _container;
            CreatePrivateMembers(r);
        }
        public Container(Container _container = null)
        {
            Container = _container;
            CreatePrivateMembers(new Rectangle(new Point(0, 0), new Point(Global.Graphics.Width, Global.Graphics.Height)));
        }

        private void CreatePrivateMembers(Rectangle rectangle)
        {
            _rectangle = rectangle;
            _innerSpriteBatch = Global.Graphics.CreateSpriteBatch();
            _innerTarget = new RenderTarget2D(_innerSpriteBatch.GraphicsDevice, _rectangle.Width, _rectangle.Height);
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

        public override void Predraw()
        {
            if (!Visible) return;

            foreach (GameObject obj in _graphicObjects)
            {
                obj.Predraw();
            }

            _innerSpriteBatch.GraphicsDevice.SetRenderTarget(_innerTarget);
            _innerSpriteBatch.GraphicsDevice.Clear(Color.Transparent);
            _innerSpriteBatch.Begin(SpriteSortMode.Deferred, Global.Graphics.BlendMode, SamplerState.PointClamp);
            foreach (GameObject obj in _graphicObjects)
            {
                obj.Draw(_innerSpriteBatch);
            }
            _innerSpriteBatch.End();
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (!Visible) return;

            // Draw container
            spriteBatch.Draw(_innerTarget, new Vector2(_rectangle.X, _rectangle.Y), Color.White);
        }

        public void SortGO()
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
