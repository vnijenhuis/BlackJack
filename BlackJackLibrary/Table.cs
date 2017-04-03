using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlackJackLibrary
{
    public class Table
    {
        public String TableName { get; set; }
        public PlayerCollection PlayersAtTable = new PlayerCollection();
        public Table(String tableName, PlayerCollection playerCollection)
        {
            this.TableName = tableName;
            this.PlayersAtTable = playerCollection;
        }
    }
}
