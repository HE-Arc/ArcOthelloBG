using ArcOthelloBG.Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArcOthelloBG.IA
{
    class Node
    {
        private AlphaBetaAgentBG agent;
        private OthelloState state;
        public int score;

        public Node(AlphaBetaAgentBG agent)
        {
            this.agent = agent;
            //We use the same Game instance for every node, we load the state and playerToPlay before playing/computing next moves
            this.state = agent.IAGame.BoardState;
            this.score = 2; //initial score at the beginning of the game
        }

        public bool final()
        {
            bool terminalNode = false;

            // Game over?

            return terminalNode;
        }

        public int eval()
        {
            //The score is evaluated after the node creation

            return score;
        }

        public List<Tuple<int, int>> ops()
        {
            List<Tuple<int, int>> children = new List<Tuple<int, int>>();

            this.state.AvailablePositions.ForEach(el => children.Add(el.toTuplesintint()));

            return children;
        }

        public Node apply(Tuple<int, int> op)
        {
            if (op.Item1 != -1 && op.Item2 != -1)
            {
                 Game.Instance.Play(new Vector2(op), this.state.PlayerId);
            }
            else
            {
                Game.Instance.NextTurn();
            }

            Node newNode = new Node(agent);


            //The score calculation was modified in the Game Class (IncrementScore method) to give more/less score to pawns on some strategic positions
            if (Game.Instance.PlayerToPlay == agent.blackId)
            {
                newNode.score = Game.Instance.WhiteScore;
            }
            else 
            {
                newNode.score = Game.Instance.BlackScore;
            }

            return newNode;
        }
    }
}
