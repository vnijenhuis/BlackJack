using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlackJackLibrary
{
    public class PlayerCollection
    {
        public List<Player> List { get; set; }

        public PlayerCollection()
        {
            this.List = new List<Player>();
        }

        //public void Sort()
        //{
        //    this.List.Sort(delegate (Player player1, Player player2) { return player1.HandValue.CompareTo(player2.HandValue); });
        //}

        public void LinqSort()
        {
            this.List = this.List.OrderByDescending(i => i.HandValue).ToList();
        }

        public void AddPlayer(Player player)
        {
            this.List.Add(player);
        }

    }
}
