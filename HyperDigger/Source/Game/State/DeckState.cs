using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace HyperDigger
{
    class DeckState
    {
        const int MAX_HAND = 4;
        const int MAX_DECK = 40;

        public struct HandSlot
        {
            public int cardIdx;
            public float state;
            public bool burning;
            public HandSlot(int i)
            {
                cardIdx = i;
                state = 0;
                burning = false;
            }

            public float Percent { get {
                    if (cardIdx < 0) return 0;
                    Card card = Global.Database.Cards.Get(cardIdx);
                    if (card == null) return 0;
                    if (card.Energy < 0) return 0;
                    return state / card.Energy;
                } }
        }
        public class HandClass
        {
            public int Count {
                get {
                    int c = 0;
                    foreach (var s in Slots) if (s.cardIdx >= 0) c++;
                    return c;
                }
            }
            public int Size => MAX_HAND;

            public int this[int key]
            {
                get => Slots[key].cardIdx;
                set
                {
                    Slots[key] = new HandSlot(value);
                }
            }

            HandSlot[] Slots = new HandSlot[MAX_HAND];

            public HandClass() {
                for (int i = 0; i < MAX_HAND; i++) Slots[i] = new HandSlot(-1);
            }

            public HandSlot Get(int i)
            {
                return Slots[i];
            }
            public void Set(int i, HandSlot c)
            {
                Slots[i] = c;
            }

            public bool Add(int c)
            {
                for (int i = 0; i < Slots.Length; i++)
                {
                    if (Slots[i].cardIdx <= -1)
                    {
                        Slots[i] = new HandSlot(c);
                        return true;
                    }
                }
                return false;
            }

            public int Mill(int i)
            {
                var idx = Slots[i].cardIdx;
                Slots[i] = new HandSlot(-1);
                return idx;
            }

            internal bool UpdateSlot(int i, float d)
            {
                if (Slots[i].cardIdx < 0) return false;
                if (Slots[i].burning) Slots[i].state += d;
                Card card = Global.Database.Cards.Get(Slots[i].cardIdx);
                if (card == null) return false;
                return Slots[i].state > card.Energy;
            }
        }

        public HandClass Hand;
        List<int> deck;
        List<int> discard;

        public DeckState()
        {
            Hand = new HandClass();
            deck = new List<int>();
            discard = new List<int>();
            deck.AddRange(new int[]{ 0,1,2,3,0,1,2,3 });

            Shuffle();
        }

        public void Update(GameTime gameTime)
        {
            var d = (float)gameTime.ElapsedGameTime.TotalSeconds;
            for (int i = 0; i < Hand.Size; i++)
            {
                if (Hand.UpdateSlot(i, d))
                {
                    Mill(i);
                }
            }
        }

        public int TopCard {
            get
            {
                if (deck.Count == 0)
                {
                    if (discard.Count != 0) Reshuffle();
                    else return -1;
                }

                return deck[deck.Count - 1];
            }
        }

        public bool CanDraw()
        {
            if (Hand.Count == MAX_HAND) return false;
            if (deck.Count == 0 && discard.Count == 0) return false;
            return true;
        }

        public void Draw()
        {
            if (Hand.Count == MAX_HAND) return; //Mill(0);
            var c = TopCard;
            if (Hand.Add(c))
                deck.RemoveAt(deck.Count - 1);
        }

        public Card Use(int i)
        {
            var c = Hand.Get(i);
            if (c.cardIdx == -1) return null;
            // Get card data.
            Card card = Global.Database.Cards.Get(c.cardIdx);
            if (card == null)
            {
                Mill(i);
                return null;
            }
            // Spend.
            switch (card.Type)
            {
                case Card.EType.ATTACK:
                    c.state += 1;
                    if (c.state >= card.Energy) Mill(i);
                    else Hand.Set(i, c);
                    break;
                case Card.EType.SUMMON:
                    if (c.burning) return null;
                    c.burning = true;
                    Hand.Set(i, c);
                    break;
            }
            
            return card;
        }

        public void Mill(int i)
        {
            var c = Hand.Mill(i);
            if (c != -1) discard.Add(c);
        }

        public void Reshuffle()
        {
            deck.AddRange(discard);
            discard.Clear();
            Shuffle();
        }

        public void Shuffle()
        {
            var rand = new Random();
            var shuffled = new List<int>();
            while (deck.Count > 0)
            {
                var r = rand.Next(deck.Count);
                var c = deck[r];
                deck.RemoveAt(r);
                shuffled.Add(c);
            }
            deck = shuffled;
        }

        public bool AddCard(Card card)
        {
            if (deck.Count >= MAX_DECK) return false;

            deck.Add(card.Idx);
            return true;
        }

        public bool RemoveCard(int idx)
        {
            if (deck.Count <= 0) return false;

            deck.RemoveAt(idx);
            return true;
        }

        public int GetDeckRemainingCards()
        {
            return deck.Count;
        }

        public int GetDeckTotalCards()
        {
            return deck.Count + discard.Count + Hand.Count;
        }
        
        public string GetStateString()
        {
            int total = GetDeckTotalCards();
            return string.Format("{0}/{1}", deck.Count, total);
        }
    }
}
