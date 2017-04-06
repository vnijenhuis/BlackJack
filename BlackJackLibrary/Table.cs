using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlackJackLibrary
{
    public class Table
    {
        public Int32 TableId { get; set; }
        public PlayerCollection PlayersAtTable = new PlayerCollection();
        public Dealer Dealer { get; set; }

        public Table(Int32 tableId, PlayerCollection playerCollection)
        {
            this.TableId = tableId;
            this.PlayersAtTable = playerCollection;
            this.CheckTableId();
        }

        private void CheckTableId()
        {
            if (this.TableId > 8 || this.TableId < 1)
            {
                this.Dealer = new Dealer();
            } else
            {
                switch (this.TableId)
                {
                    case 1:
                        this.Dealer = new Dealer(20);
                        break;
                    case 2:
                        this.Dealer = new Dealer(50);
                        break;
                    case 3:
                        this.Dealer = new Dealer(75);
                        break;
                    case 4:
                        this.Dealer = new Dealer(75);
                        break;
                    case 5:
                        this.Dealer = new Dealer(50);
                        break;
                    case 6:
                        this.Dealer = new Dealer(20);
                        break;
                    case 7:
                        this.Dealer = new Dealer(50);
                        break;
                    case 8:
                        this.Dealer = new Dealer(10);
                        break;
                    default:
                        this.Dealer = new Dealer(100);
                        break;
                }
            }
        }
    }
}
