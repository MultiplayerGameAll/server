using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace server
{
    [Serializable]
    public class Game
    {
        public Player owner;

        public List<Player> player = new List<Player>();

    }
}
