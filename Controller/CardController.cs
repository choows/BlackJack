using System;
using System.Collections.Generic;
using System.Linq;

namespace BlackJack
{
    public class CardController
    {
        private int MinimumCardNumber = 15;
        List<Suits> suitList = new List<Suits>() { Suits.Spade , Suits.Heart , Suits.Club , Suits.Diamond};
        List<string> Alphaberts = new List<string>() { "A", "2", "3", "4", "5", "6", "7", "8", "9", "10", "J", "Q", "K" };
        List<Card> cards;
        /**
         * Initialize Card Set 
         * 
         * Each Card Set have 52 cards 
         */
        public CardController(int MincardNum)
        {
            this.MinimumCardNumber = MincardNum;
            cards = new List<Card>(); 
            
            foreach(Suits suit in suitList)
            {
                foreach(string Alp in Alphaberts)
                {
                    cards.Add(new Card()
                    {
                        suits = suit,
                        Numeric = Alp
                    });
                }
            }
        }
        
        /**
         * Shuffle will be used to shuffle the card set 
         * 
         * by draw out the random number of cards from the set 
         * and then put it onto the top 
         */
        public void Shuffle(int ShuffleFrequency)
        {
            Random random = new Random();
            for(int freq = 0; freq < ShuffleFrequency; freq++)
            {
                int StartIdx = random.Next(0, cards.Count + 1);
                int NumOfCardDrawed = cards.Count - StartIdx;
                List<Card> drawed = cards.GetRange(StartIdx , NumOfCardDrawed);
                cards.RemoveRange(StartIdx, NumOfCardDrawed);
                cards.Reverse();
                cards.AddRange(drawed);
                cards.Reverse();
            }
        }
        public void Reshuffle(List<Card> TrashCard , out List<Card> return_card)
        {
            if(cards.Count <= MinimumCardNumber)
            {
                cards.AddRange(TrashCard);
                return_card = new List<Card> ();
                Shuffle(20);
            }
            else
            {
                return_card = TrashCard;
            }
        }
        public Card DrawCard()
        {
            Card DrawedCard = cards.FirstOrDefault();
            cards.Remove(DrawedCard);
            return DrawedCard;
        }
    }
}
