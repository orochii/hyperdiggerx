using HyperDigger.Source.Game.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace HyperDigger
{
    class WorldHUD : Container
    {
        static Vector2 DECK_POS = new Vector2(8, 200);

        public Player Player;
        Sprite uiBack;
        Minimap minimap = null;
        LevelNameLabel LevelName;
        Label deckInfo;
        List<CardSprite> cardHand;

        // Pulling animation.
        CardSprite drawingCard;
        float drawingState;
        Vector2 drawingTarget;

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
            cardHand[0].Position = new Vector2(240, 192);
            cardHand[1].Position = new Vector2(264, 168);
            cardHand[2].Position = new Vector2(264, 200);
            cardHand[3].Position = new Vector2(288, 184);

            drawingCard = new CardSprite(this);
            drawingCard.Visible = false;
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
                if (i < hand.Size)
                {
                    var slot = hand.Get(i);
                    
                    cardHand[i].CardIdx = slot.cardIdx;
                    cardHand[i].Percent = slot.Percent;
                }
                else cardHand[i].CardIdx = -1;
            }

            // Pull support.
            if (drawingCard.Visible) UpdatePullAnimation(gameTime);
            if (Player.IsPulling) DoPull();
        }

        public void DoPull()
        {
            Player.IsPulling = false;
            if (drawingCard.Visible) return;
            // Prepare for drawing card
            drawingCard.CardIdx = Global.State.Deck.TopCard;
            if (drawingCard.CardIdx < 0) return;
            drawingCard.Position = DECK_POS;
            drawingCard.Visible = true;
            drawingState = 0;

            drawingTarget = GetDrawingTarget();
        }

        private Vector2 GetDrawingTarget()
        {
            foreach (var card in cardHand)
            {
                if (card.CardIdx < 0) return card.Position;
            }
            return cardHand[0].Position;
        }

        public void UpdatePullAnimation(GameTime gameTime)
        {
            var d = (float)gameTime.ElapsedGameTime.TotalSeconds;
            drawingState += d * 5;
            if (drawingState >= 1f)
            {
                Global.State.Deck.Draw();
                drawingCard.Visible = false;
            } 
            else
            {
                drawingCard.Position = Vector2.Lerp(DECK_POS, drawingTarget, drawingState);
                drawingCard.Angle += d * 1080;
            }
        }
    }
}
