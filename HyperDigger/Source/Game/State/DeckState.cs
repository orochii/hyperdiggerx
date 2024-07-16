using System;
using System.Collections.Generic;

namespace HyperDigger
{
    class DeckState
    {
        const int MAX_HAND = 4;
        const int MAX_DECK = 40;

        public List<int> Hand;
        List<int> deck;
        List<int> discard;

        public DeckState()
        {
            Hand = new List<int>();
            deck = new List<int>();
            discard = new List<int>();
            deck.Add(0);
            deck.Add(0);
            //deck.Add(0);
            //deck.Add(0);

            Shuffle();
        }

        public void PullCard()
        {
            if (Hand.Count == MAX_HAND) Mill(0);
            if (deck.Count == 0)
            {
                if (discard.Count != 0) Reshuffle();
                else return;
            }

            var c = deck[deck.Count - 1];
            deck.RemoveAt(deck.Count - 1);
            Hand.Add(c);
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

        public void Mill(int i)
        {
            var c = Hand[i];
            Hand.RemoveAt(i);
            discard.Add(c);
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

        internal string GetStateString()
        {
            int total = deck.Count + discard.Count + Hand.Count;
            return string.Format("{0}/{1}", deck.Count, total);
        }
    }
}
