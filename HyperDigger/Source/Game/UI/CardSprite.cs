using Assimp;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace HyperDigger
{
    internal class CardSprite : GameObject
    {
        const int CARD_WIDTH = 24;
        const int CARD_HEIGHT = 32;

        public float Angle;
        public int CardIdx = -1;
        public float Percent = 0f;

        Texture2D texture;
        Texture2D _burnTexture;
        float burnAnimState = 0f;
        int _cardCols = 1;

        public CardSprite(Container container) : base(container) {
            _burnTexture = Global.Cache.LoadTexture("Graphics/Cards/burn_anim");
            texture = Global.Cache.LoadTexture("Graphics/Cards/cards");
            _cardCols = texture.Width / CARD_WIDTH;
            if (_cardCols < 1 ) _cardCols = 1;
        }

        public override void Update(GameTime gameTime)
        {
            float d = (float)gameTime.ElapsedGameTime.TotalSeconds;
            burnAnimState += d * 16;
            if (burnAnimState >= 4) burnAnimState -= 4;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            Percent = Math.Clamp(Percent, 0f, 1f);

            if (CardIdx < 0 ) return;
            if (!Visible) return;
            if (texture == null) return;

            Vector2 origPosition = GlobalPosition;
            Vector2 intPosition = new Vector2((int)origPosition.X, (int)origPosition.Y);

            int cardX = CARD_WIDTH * (CardIdx % _cardCols);
            int cardY = CARD_HEIGHT * (CardIdx / _cardCols);
            if (Angle != 0)
            {
                Vector2 middleOrigin = new Vector2(CARD_WIDTH / 2, CARD_HEIGHT / 2);
                Vector2 offsetPosition = intPosition + middleOrigin;
                Rectangle bottomSrc = new Rectangle(cardX, cardY, CARD_WIDTH, CARD_HEIGHT);
                spriteBatch.Draw(texture, offsetPosition, bottomSrc, Color.White, Angle, middleOrigin, Vector2.One, SpriteEffects.None, 0);
            }
            else
            {
                int divide = (int)(CARD_HEIGHT * Percent);
                // Draw bottom part
                Rectangle bottomSrc = new Rectangle(cardX, cardY, CARD_WIDTH, divide);
                spriteBatch.Draw(texture, intPosition, bottomSrc, Color.DarkSlateGray, 0, Vector2.Zero, 1, SpriteEffects.None, 0);
                // Draw top part
                var topStart = intPosition + new Vector2(0, divide);
                Rectangle topSrc = new Rectangle(bottomSrc.X, bottomSrc.Y + divide, CARD_WIDTH, CARD_HEIGHT - divide);
                spriteBatch.Draw(texture, topStart, topSrc, Color.White, 0, Vector2.Zero, Vector2.One, SpriteEffects.None, 0);
                // Draw burning texture
                if (Percent > 0f)
                {
                    int f = (int)burnAnimState;
                    Rectangle burnSrc = new Rectangle(0,f*4,32,4);
                    var burnPos = intPosition + new Vector2(burnSrc.Width / 2 - 4, divide);
                    spriteBatch.Draw(_burnTexture, burnPos, burnSrc, Color.White, 0, new Vector2(burnSrc.Width / 2, burnSrc.Height / 2), 1, SpriteEffects.None, 0);
                }
            }
            
        }
    }
}
