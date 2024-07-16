using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace HyperDigger
{
    class Graphics
    {
        public int Scale { get { return _scale; } set { SetScale(value); } }

        private int _nativeWidth = 320;
        private int _nativeHeight = 240;
        private Color _backgroundColor = Color.DarkGray;
        public int Width => _nativeWidth;
        public int Height => _nativeHeight;

        private GraphicsDeviceManager _graphics;
        private Game _game;
        private RenderTarget2D _nativeRenderTarget;
        private int _scale = 2;
        private Rectangle _actualScreenRectangle;
        public SpriteBatch SpriteBatch { get {  return _spriteBatch; } }

        public BlendState BlendMode = BlendState.AlphaBlend;

        private SpriteBatch _spriteBatch;
        Texture2D _dotTexture;

        public Graphics(Game g)
        {
            _game = g;
            _graphics = new GraphicsDeviceManager(_game);
        }

        public void Initialize()
        {
            SetScale(_scale);
            _spriteBatch = new SpriteBatch(_game.GraphicsDevice);
            // For drawing operations.
            _dotTexture = new Texture2D(_game.GraphicsDevice, 1, 1);
            _dotTexture.SetData(new Color[] { Color.White });
        }

        public SpriteBatch CreateSpriteBatch() { 
            return new SpriteBatch(_game.GraphicsDevice);
        }
        public Texture2D GetScreenshot()
        {
            var _screenshot = new Texture2D(_game.GraphicsDevice, _nativeRenderTarget.Width, _nativeRenderTarget.Height);
            var _data = new Color[_nativeRenderTarget.Width * _nativeRenderTarget.Height];
            _nativeRenderTarget.GetData(_data);
            _screenshot.SetData(_data);
            return _screenshot;
        }

        internal void StartRender()
        {
            _game.GraphicsDevice.SetRenderTarget(_nativeRenderTarget);
            _game.GraphicsDevice.Clear(_backgroundColor);
            _spriteBatch.Begin(SpriteSortMode.Deferred, BlendMode);
        }
        internal void EndRender()
        {
            _game.GraphicsDevice.SetRenderTarget(_nativeRenderTarget);
            _spriteBatch.End();
            // Redraw all but scaled
            _game.GraphicsDevice.SetRenderTarget(null);
            _spriteBatch.Begin(samplerState: SamplerState.PointClamp);
            _spriteBatch.Draw(_nativeRenderTarget, _actualScreenRectangle, Color.White);
            _spriteBatch.End();
        }

        private void SetScale(int targetScale)
        {
            _scale = targetScale;
            _nativeRenderTarget = new RenderTarget2D(_game.GraphicsDevice, _nativeWidth, _nativeHeight);
            _actualScreenRectangle = new Rectangle(0, 0, _nativeWidth * _scale, _nativeHeight * _scale);
            _graphics.PreferredBackBufferWidth = _actualScreenRectangle.Width;
            _graphics.PreferredBackBufferHeight = _actualScreenRectangle.Height;
            _graphics.ApplyChanges();
        }

        public void DrawDot(SpriteBatch batch, Vector2 position, Color color)
        {
            batch.Draw(_dotTexture, position, color);
        }

        public void DrawRectangle(SpriteBatch batch, Rectangle rect, Color color)
        {
            batch.Draw(_dotTexture, rect, color);
        }
    }
}
