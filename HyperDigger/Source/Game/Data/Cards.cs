﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace HyperDigger
{
    public class Card
    {
        public enum EType { ATTACK, SUMMON }

        public int Idx;
        public string Name;
        public string Description;
        public EType Type;
        public string EntityName;
        public Vector2 Offset;
        public int Energy;
    }

    class Cards
    {
        Card[] _cards;

        public Cards()
        {
            _cards = new Card[] {
                new Card() { Name = "Tower", 
                    Description = "", 
                    Type = Card.EType.SUMMON, 
                    EntityName = "Tower",
                    Offset = new Vector2(32, 16),
                    Energy = 5},
                new Card() { Name = "TowerTest",
                    Description = "",
                    Type = Card.EType.ATTACK,
                    EntityName = "Tower",
                    Offset = new Vector2(32, 16),
                    Energy = 5},
            };
            for (int i = 0; i < _cards.Length; i++)
            {
                _cards[i].Idx = i;
            }
        }

        internal Card Get(int cardIdx)
        {
            if (cardIdx < _cards.Length && cardIdx >= 0)
            {
                return _cards[cardIdx];
            }
            return null;
        }
    }
}
