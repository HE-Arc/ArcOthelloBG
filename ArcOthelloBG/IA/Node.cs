using ArcOthelloBG.Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ArcOthelloBG.IA
{
    /// <summary>
    /// Class of a node of the tree
    /// </summary>
    class Node
    {
        private AlphaBetaAgentBG agent;
        private OthelloState state;
        private int score;
        private Vector2 op;
        private bool turnSkipped;
        private Node parent;


        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="agent">alphabeta agent</param>
        /// <param name="parent">parent node</param>
        /// <param name="op">operation</param>
        /// <param name="score">score to add, default 0</param>
        public Node(AlphaBetaAgentBG agent, Node parent, Vector2 op, int score = 0)
        {
            this.agent = agent;
            this.state = agent.IAGame.BoardState; // state of the board for the board
            this.op = op; // operation made to arrive to this state

            this.score = score; //score is the number of point lost or won
            this.turnSkipped = false;
            this.parent = parent;

            this.agent = agent;
            // get the state of the board for this child
            this.state = agent.IAGame.BoardState;
            this.score = 0; //initial score at the beginning of the game
        }

        /// <summary>
        /// is this a final move or not
        /// </summary>
        /// <returns>is final</returns>
        public bool Final()
        {
            //The final state is reached if a turn was skipped two consecutive times
            bool final = this.parent != null && this.parent.turnSkipped && this.turnSkipped;
            return final;
        }

        public int Eval()
        {
            int positionScore = 0;
            int malusSkipTurn = 0;

            // if skip a turn, move isn't good
            if(this.op.X == -1 && this.op.Y == -1)
            {
                malusSkipTurn = -10;
            }
            else
            {
                // if in the corner
                if (
                    this.op.X == 0 && this.op.Y == 0 ||
                    this.op.X == this.state.Board.GetLength(0) - 1 && this.op.Y == this.state.Board.GetLength(1) - 1 ||
                    this.op.X == 0 && this.op.Y == this.state.Board.GetLength(1) - 1 ||
                    this.op.X == this.state.Board.GetLength(0) - 1 && this.op.Y == 0
                    )
                {
                    positionScore = 20;
                }
                // if on the side
                else if (
                    this.op.X == 0 || this.op.Y == 0 ||
                    this.op.X == this.state.Board.GetLength(0) - 1 || this.op.Y == this.state.Board.GetLength(1) - 1
                    )
                {
                    positionScore = 10;
                }
                // if line before side;
                else if (
                    this.op.X == 1 || this.op.Y == 1 ||
                    this.op.X == this.state.Board.GetLength(0) - 2 || this.op.Y == this.state.Board.GetLength(1) - 2
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
        public List<Vector2> Ops()
        {
            return this.state.AvailablePositions.Select(item => (Vector2)item.Clone()).ToList();
        }

        /// <summary>
        /// Apply an op to the node
        /// </summary>
        /// <param name="op">OPeration to apply</param>
        /// <returns>New node</returns>
        public Node Apply(Vector2 op)
        {
            if (op.X != -1 && op.X != -1)
            {
                Game.Instance.Play(op, this.state.PlayerId);
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

            // create the child
            Node newNode = new Node(agent, this, op, score);

            if (op.X == -1 && op.Y == -1)
            {
                newNode.turnSkipped = true;
            }

            return newNode;
        }
    }
}
