using ArcOthelloBG.EventHandling;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ArcOthelloBG.Logic
{
    /// <summary>
    /// Class that implements the rules of the game
    /// </summary>
    class Game
    {
        // MEMBERS
        private int[,] board;
        private static Game instance = null;
        private int playerToPlay;
        private int whiteId;
        private int blackId;
        private List<Vector2> possibleDirections;
        private int turn;
        private int emptyId;
        private OthelloState boardState;
        private int blackScore;
        private int whiteScore;

        //public event EventHandler<SkipTurnEventArgs> TurnSkipped;
        //public event EventHandler<WinEventArgs> Won;

        // GETTERS AND SETTERS

        /// <summary>
        /// Getters for the singleton instance
        /// </summary>
        public static Game Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new Game();
                }

                return instance;
            }
        }

        /// <summary>
        /// singleton constructor, so private
        /// </summary>
        private Game()
        {
            this.boardState = null;
        }

        /// <summary>
        /// property for the white score, only getter, and it's a computed value
        /// </summary>
        public int WhiteScore
        {
            get
            {
                return this.whiteScore;
            }
        }

        /// <summary>
        /// property for the black score, only getter, and it's a computed value
        /// </summary>
        public int BlackScore
        {
            get
            {
                return this.blackScore;
            }
        }

        public int PlayerToPlay
        {
            get
            {
                return this.playerToPlay;
            }
        }

        /// <summary>
        /// getter for the board
        /// </summary>
        public int[,] Board
        {
            get
            {
                if (instance.board == null)
                {
                    throw new InvalidOperationException("board not init");
                }

                return board;
            }
        }

        public OthelloState BoardState
        {
            get
            {
                return this.boardState;
            }

        }

        /// <summary>
        /// Init the grid
        /// </summary>
        /// <param name="width">width of the grid</param>
        /// <param name="height">height of the grid</param>
        public void Init(int columns, int rows, int whiteId, int blackId, int emptyId)
        {
            this.board = new int[columns, rows];
            this.playerToPlay = whiteId;
            this.whiteId = whiteId;
            this.blackId = blackId;
            this.emptyId = emptyId;
            this.BuildPossibleDirections();
            this.blackScore = 0;
            this.whiteScore = 0;

            this.InitBoard();

            this.turn = -1;
            this.NextTurn();
        }

        /// <summary>
        /// load a board state and changes to match it
        /// </summary>
        /// <param name="state">state to use</param>
        public void LoadState(OthelloState state)
        {
            this.board = (int[,])state.Board.Clone();
            this.emptyId = state.EmptyId;
            this.whiteScore = state.WhiteScore;
            this.blackScore = state.BlackScore;
            this.playerToPlay = state.PlayerId;
            this.boardState = state;
        }

        /// <summary>
        /// Play a move
        /// </summary>
        /// <param name="position">Position to put a pawn</param>
        /// <param name="isWhite">color of the pawn</param>
        /// <returns>positions that changed</returns>
        public List<Vector2> Play(Vector2 position, int idToPlay)
        {
            if (this.IsPlayable(position, idToPlay))
            {
                var initialPosition = new Vector2(position);
                var changedPositions = new List<Vector2>();

                var directions = this.boardState.getValidDirections(position);

                foreach (var direction in directions)
                {
                    position = initialPosition;
                    do
                    {
                        this.PutPawn(position, idToPlay);
                        changedPositions.Add(position);
                        position = position.add(direction);
                    } while (this.boardState.isInBoundaries(position) && this.GetColor(position) != idToPlay);
                }

                this.NextTurn();

                return changedPositions;
            }
            else
            {
                Console.WriteLine("Not valid");
                throw new ArgumentException("This move isn't possible");
            }

        }

        /// <summary>
        /// Check if a move is possible
        /// </summary>
        /// <param name="position">Position to put a pawn</param>
        /// <param name="isWhite">Color of the pawn</param>
        /// <returns>move is playable or not</returns>
        public bool IsPlayable(Vector2 position, int idToPlay)
        {
            return this.playerToPlay == idToPlay && this.boardState.isPlayable(position);
        }

        /// <summary>
        /// get a list of positions where a player can play
        /// </summary>
        /// <returns>list of position</returns>
        public List<Vector2> GetPositionsAvailable()
        {
            return this.boardState.AvailablePositions;
        }

        /// <summary>
        /// get the color of a position (used to shortened the code)
        /// </summary>
        /// <param name="position">position of the pawns</param>
        /// <returns>Color of the pawns</returns>
        public int GetColor(Vector2 position)
        {
            return this.board[position.X, position.Y];
        }

        /// <summary>
        /// handle the changes for changing turn
        /// </summary>
        /// <param name="hasSkipped">if the transition was because the turn skipped, useful for detecing the end of the game</param>
        public void NextTurn(bool hasSkipped = false)
        {
            this.playerToPlay = this.GetNextPlayer();

            this.turn++;
            this.boardState = new OthelloState(this.board, this.playerToPlay, this.possibleDirections, this.emptyId, this.whiteScore, this.blackScore);

            //if(this.GetPositionsAvailable().Count == 0)
            //{
            //    if (hasSkipped)
            //    {
            //        Won?.Invoke(this, new WinEventArgs(this.GetWinner()));
            //        return; 
            //    }

            //    int previousPlayer = this.playerToPlay;
            //    this.NextTurn(true);
            //    TurnSkipped?.Invoke(this, new SkipTurnEventArgs(previousPlayer));
            //}
        }

        /// <summary>
        /// Get the next player to play
        /// </summary>
        /// <returns>id of the player</returns>
        private int GetNextPlayer()
        {
            return this.playerToPlay == this.whiteId ? this.blackId : this.whiteId;
        }


        /// <summary>
        /// get the winner of the game
        /// </summary>
        /// <returns>id of the winner</returns>
        private int GetWinner()
        {
            return this.whiteScore > this.blackScore ? this.whiteId : this.blackId;
        }

        /// <summary>
        /// put a pawn at a position and handle score channges
        /// </summary>
        /// <param name="position">position of the pawn</param>
        /// <param name="idColor">color of the pawn</param>
        private void PutPawn(Vector2 position, int idColor)
        {
            this.IncrementScore(idColor, position);
            this.board[position.X, position.Y] = idColor;
        }



        /// <summary>
        /// init the board with the right numbers
        /// </summary>
        private void InitBoard()
        {
            int w = this.board.GetLength(0);
            int h = this.board.GetLength(1);

            for (int i = 0; i < w; i++)
            {
                for (int j = 0; j < h; j++)
                {
                    int color = this.emptyId;

                    if (i == w / 2 && j == h / 2 + 1 || i == w / 2 - 1 && j == h / 2)
                    {
                        color = this.whiteId;
                    }
                    else if (i == w / 2 - 1 && j == h / 2 + 1 || i == w / 2 && j == h / 2)
                    {
                        color = this.blackId;
                    }

                    this.PutPawn(new Vector2(i, j), color);
                }
            }
        }

        /// <summary>
        /// build the list of Direction possible to play
        /// </summary>
        private void BuildPossibleDirections()
        {
            this.possibleDirections = new List<Vector2>();

            //list is always the same, see if we can make it elsewhere
            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    if (i != 0 || j != 0)
                    {
                        this.possibleDirections.Add(new Vector2(i, j));
                    }
                }
            }
        }

        /// <summary>
        /// Increment the score of a player
        /// </summary>
        /// <param name="playerId">player to add a point</param>
        /// <param name="position">position, to check if the position had another pawn already</param>
        private void IncrementScore(int playerId, Vector2 position)
        {
            int weight = 1;

            int w = this.board.GetLength(0);
            int h = this.board.GetLength(1);

            if (position.X == 0 || position.X == w - 1 || position.Y == 0 || position.Y == h - 1)
                weight *= 4;

            if ((position.X == 0 || position.X == w - 1) && (position.Y == 0 || position.Y == h - 1))
                weight *= 100;

            if (position.X == 1 || position.X == w - 2 || position.Y == 1 || position.Y == h - 2)
                weight *= -25;


            if (playerId == this.whiteId && this.GetColor(position) != this.whiteId)
            {
                this.whiteScore += weight;

                if (this.GetColor(position) == this.blackId)
                {
                    this.blackScore -= weight;
                }
            }
            else if (playerId == this.blackId && this.GetColor(position) != this.blackId)
            {
                this.blackScore += weight;

                if (this.GetColor(position) == this.whiteId)
                {
                    this.whiteScore -= weight;
                }
            }
        }

    }
}