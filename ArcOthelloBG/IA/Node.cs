using ArcOthelloBG.Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ArcOthelloBG.IA
{
    class Node
    {
        private AlphaBetaAgentBG agent;
        private OthelloState state;
        private int score;
        private Tuple<int, int> op;
        private bool turnSkipped;
        private Node parent;


        public Node(AlphaBetaAgentBG agent, Node parent, Tuple<int,int> op, int score = 0)
        {
            this.agent = agent;
            this.state = agent.IAGame.BoardState; // state of the board for the board
            this.op = op; // operation made to arrive to this state

            this.score = score; //score is the number of point lost or won
            this.turnSkipped = false;
            this.parent = parent;

            this.agent = agent;
            //We use the same Game instance for every node, we load the state and playerToPlay before playing/computing next moves
            this.state = agent.IAGame.BoardState;
            this.score = 2; //initial score at the beginning of the game
        }

        public bool final()
        {
            //The final state is reached if a turn was skipped two consecutive times
            bool final = this.parent != null && this.parent.turnSkipped && this.turnSkipped;
            //Console.WriteLine($"final: {final}");
            return final;
        }

        public int Eval()
        {
            int positionScore = 0;
            int malusSkipTurn = 0;

            // if skip a turn, move isn't good
            if(this.op.Item1 == -1 && this.op.Item2 == -1)
            {
                malusSkipTurn = -10;
            }
            else
            {
                // if in the corner
                if (
                    this.op.Item1 == 0 && this.op.Item2 == 0 ||
                    this.op.Item1 == this.state.Board.GetLength(0) - 1 && this.op.Item2 == this.state.Board.GetLength(1) - 1 ||
                    this.op.Item1 == 0 && this.op.Item2 == this.state.Board.GetLength(1) - 1 ||
                    this.op.Item1 == this.state.Board.GetLength(0) - 1 && this.op.Item2 == 0
                    )
                {
                    positionScore = 20;
                }
                // if on the side
                else if (
                    this.op.Item1 == 0 || this.op.Item2 == 0 ||
                    this.op.Item1 == this.state.Board.GetLength(0) - 1 || this.op.Item2 == this.state.Board.GetLength(1) - 1
                    )
                {
                    positionScore = 10;
                }
                // if line before side;
                else if (
                    this.op.Item1 == 1 || this.op.Item2 == 1 ||
                    this.op.Item1 == this.state.Board.GetLength(0) - 2 || this.op.Item2 == this.state.Board.GetLength(1) - 2
                    )
                {
                    positionScore = -5; // malus because it allow the other player to put a good move on the side
                }
            }
            
            return score + positionScore + malusSkipTurn;
        }

        /// <summary>
        /// list of moves available
        /// </summary>
        /// <returns>list of moves available</returns>
        public List<Tuple<int, int>> Ops()
        {
            List<Tuple<int, int>> children = new List<Tuple<int, int>>();

            this.state.AvailablePositions.ForEach(el => children.Add(el.toTuplesintint()));

            return children;
        }

        /// <summary>
        /// Apply an op to the node
        /// </summary>
        /// <param name="op">OPeration to apply</param>
        /// <returns>New node</returns>
        public Node Apply(Tuple<int, int> op)
        {
            if (op.Item1 != -1 && op.Item2 != -1)
            {
                Game.Instance.Play(new Vector2(op), this.state.PlayerId);
            }
            else
            {
                Game.Instance.NextTurn();
            }
            int score = 0;

            //compute the loss or win of point for move
            if (this.state.PlayerId == agent.blackId)
            {
                score = Game.Instance.BlackScore - this.state.BlackScore;
            }
            else
            {
                score += Game.Instance.WhiteScore - this.state.WhiteScore;
            }

            Node newNode = new Node(agent, this, op, score);

            if (op.Item1 == -1 && op.Item2 == -1)
            {
                newNode.turnSkipped = true;
            }

            return newNode;
        }
    }
}
