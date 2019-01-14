using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArcOthelloBG.EventHandling
{
    class SkipTurnEventArgs : EventArgs
    {
        private int playerIdSkipped;

        public SkipTurnEventArgs(int playerIdSkipped)
        {
            this.playerIdSkipped = playerIdSkipped;
        }

        public int PlayerIdSkipped {
            get
            {
                return this.playerIdSkipped;
            }
        }
    }
}
