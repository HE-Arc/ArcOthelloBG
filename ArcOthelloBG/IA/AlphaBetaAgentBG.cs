using ArcOthelloBG.Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArcOthelloBG.IA
{
    class AlphaBetaAgentBG
    {
        public Game IAGame;

        public int whiteId;

        public int blackId;

        public int columns;

        public int rows;

        public AlphaBetaAgentBG(int columns, int rows, int whiteId, int blackId, int emptyId)
        {
            IAGame = Game.Instance;
            IAGame.Init(columns, rows, whiteId, blackId, emptyId);
            this.whiteId = whiteId;
            this.blackId = blackId;
            this.rows = rows;
            this.columns = columns;
        }

        //Implemented from the code example in the pdf of the IA course
        //the Tuple<int, int> contains in the following order: move X coordinate, move Y coordinate
        public Tuple<int, Tuple<int, int>> alphabeta(Node root, int depth, int minOrMax, int parentValue)
        {
            if (depth == 0 || root.final())
            {
                return new Tuple<int, Tuple<int, int>>(root.eval(), null);
            }

            int optVal = minOrMax * -int.MaxValue;

            Tuple<int, int> optOp = null;

            OthelloState initialState = Game.Instance.BoardState;

            var ops = root.ops();

            if(ops.Count == 0)
            {
                ops.Add(new Tuple<int, int>(-1, -1));
            }

            foreach (Tuple<int, int> op in ops)
            {
                Game.Instance.LoadState(initialState);
                Node newNode = root.apply(op);
                //Console.WriteLine($"minormax : {minOrMax}");
                //Console.WriteLine($"optVal : {optVal}");
                Tuple<int, Tuple<int, int>> result = alphabeta(newNode, depth - 1, -1 * minOrMax, optVal);
                
                int val = result.Item1;

                if (val * minOrMax > optVal * minOrMax)
                {
                    optVal = val;
                    optOp = op;

                    if (optVal * minOrMax > parentValue * minOrMax)
                    {
                        break;
                    }
                }
            }

            Game.Instance.LoadState(initialState);

            return new Tuple<int, Tuple<int, int>>(optVal, optOp);
        }
    }
}
