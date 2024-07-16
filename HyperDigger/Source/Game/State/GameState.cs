using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperDigger
{
    class GameState
    {
        public DeckState Deck;
        public int Health;
        public int MaxHealth = 20;

        public GameState()
        {
            Deck = new DeckState();
            Health = MaxHealth;
        }
    }
}
