using System;

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
