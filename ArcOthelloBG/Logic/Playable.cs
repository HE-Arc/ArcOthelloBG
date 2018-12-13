using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IPlayable;

namespace ArcOthelloBG.Logic
{
    class Playable : IPlayable.IPlayable

    {
        int IPlayable.IPlayable.GetBlackScore()
        {
            throw new NotImplementedException();
        }

        int[,] IPlayable.IPlayable.GetBoard()
        {
            throw new NotImplementedException();
        }

        string IPlayable.IPlayable.GetName()
        {
            throw new NotImplementedException();
        }

        Tuple<int, int> IPlayable.IPlayable.GetNextMove(int[,] game, int level, bool whiteTurn)
        {
            throw new NotImplementedException();
        }

        int IPlayable.IPlayable.GetWhiteScore()
        {
            throw new NotImplementedException();
        }

        bool IPlayable.IPlayable.IsPlayable(int column, int line, bool isWhite)
        {
            throw new NotImplementedException();
        }

        bool IPlayable.IPlayable.PlayMove(int column, int line, bool isWhite)
        {
            throw new NotImplementedException();
        }
    }
}
