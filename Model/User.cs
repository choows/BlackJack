using System.Collections.Generic;

namespace BlackJack
{
    internal class User
    {
        public string Name { get; set; }
        public int Point { get; set; }
        public List<Card> HandCard { get; set; }
    }
}
