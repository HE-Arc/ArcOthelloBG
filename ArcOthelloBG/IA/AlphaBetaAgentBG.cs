using ArcOthelloBG.Logic;
using System;

namespace ArcOthelloBG.IA
{
    class AlphaBetaAgentBG
    {
        public Game IAGame;

        public int whiteId;

        public int blackId;

        public int columns;

        public int rows;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="columns">number of columns of the board</param>
        /// <param name="rows">number of rows of the board</param>
        /// <param name="whiteId">white int id</param>
        /// <param name="blackId">black int id</param>
        /// <param name="emptyId">empty int id</param>
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
        public Tuple<int, Vector2> alphabeta(Node root, int depth, int minOrMax, int parentValue)
        {
            // check if end of the depth or if the move is final
            if (depth == 0 || root.Final())
            {
                return new Tuple<int, Vector2>(root.Eval(), null);
            }

            int optVal = minOrMax * -int.MaxValue;

            Vector2 optOp = null;

            OthelloState initialState = Game.Instance.BoardState; // get the state to come back to it

            var ops = root.Ops();

            // if there is no move available, add a skip turn move
            if(ops.Count == 0)
            {
                ops.Add(new Vector2(-1, -1));
            }

            foreach (Vector2 op in ops)
            {
                Game.Instance.LoadState(initialState); // load the initial state to compute the brother

                Node newNode = root.Apply(op);

                Tuple<int, Vector2> result = alphabeta(newNode, depth - 1, -1 * minOrMax, optVal);
                
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

            Game.Instance.LoadState(initialState); // load the initial state

            return new Tuple<int, Vector2>(optVal, optOp);
        }
    }
}
