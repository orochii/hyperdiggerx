using HyperDigger.Source.Game.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace HyperDigger
{
    class WorldHUD : Container
    {
        public Player Player;
        Sprite uiBack;
        Minimap minimap = null;
        LevelNameLabel LevelName;
        Label deckInfo;
        List<CardSprite> cardHand;

        public WorldHUD(Container container) : base(container) {
            uiBack = new Sprite(this);
            uiBack.Texture = Global.Cache.LoadTexture("Graphics/System/HUD");
            //minimap = new Minimap(this);

            deckInfo = new Label(this);
            deckInfo.Font = Global.Cache.LoadFont("Fonts/Small");
            deckInfo.HAlign = Label.EHAlign.Middle;
            deckInfo.Position = new Vector2(24, Global.Graphics.Height - 16);
            
            LevelName = new LevelNameLabel(this);

            // Cards
            cardHand = new List<CardSprite>();
            cardHand.Add(new CardSprite(this));
            cardHand.Add(new CardSprite(this));
            cardHand.Add(new CardSprite(this));
            cardHand.Add(new CardSprite(this));
            cardHand[0].Position = new Vector2(240,192);
            cardHand[1].Position = new Vector2(264, 168);
            cardHand[2].Position = new Vector2(264, 200);
            cardHand[3].Position = new Vector2(288, 184);
        }

        public void SetLevel(TilemapLevel _level)
        {
            if(minimap != null) minimap.SetMinimap(_level);
            LevelName.Set(_level.DisplayName);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            deckInfo.Text = Global.State.Deck.GetStateString();
            if (minimap != null)
            {
                minimap.Player = Player;
                minimap.Update(gameTime);
            }
            LevelName.Update(gameTime);

            // Update hand
            var hand = Global.State.Deck.Hand;
            for (int i = 0; i < cardHand.Count; i++)
            {
                if (i < hand.Count)
                {
                    cardHand[i].CardIdx = hand[i];
                }
                else cardHand[i].CardIdx = -1;
            }
        }
    }
}
