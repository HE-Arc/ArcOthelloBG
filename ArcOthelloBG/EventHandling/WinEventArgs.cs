using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArcOthelloBG.EventHandling
{
    class WinEventArgs
    {
        private int playerId;

        public WinEventArgs(int playerId)
        {
            this.playerId = playerId;
        }

        public int PlayerId
        {
            get
            {
                return this.playerId;
            }
        }
    }
}
