using ArcOthelloBG.EventHandling;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        private int lastPlayed;
        private int whiteId;
        private int blackId;
        private List<Vector2> possibleMoves;
        private bool hasSkipped;

        public event EventHandler<SkipTurnEventArgs> TurnSkipped;
        public event EventHandler<WinEventArgs> Won;


        // METHODS

        /// <summary>
        /// singleton constructor, so private
        /// </summary>
        private Game()
        { }

        /// <summary>
        /// property for the white score, only getter, and it's a computed value
        /// </summary>
        public int WhiteScore
        {
            get
            {
                int score = 0;
                for (int i = 0; i < board.GetLength(0); i++)
                {
                    for (int j = 0; j < board.GetLength(1); j++)
                    {
                        if (board[i, j] == this.whiteId)
                        {
                            score++;
                        }
                    }
                }

                return score;
            }
        }

        /// <summary>
        /// property for the black score, only getter, and it's a computed value
        /// </summary>
        public int BlackScore
        {
            get
            {
                int score = 0;
                for (int i = 0; i < board.GetLength(0); i++)
                {
                    for (int j = 0; j < board.GetLength(1); j++)
                    {
                        if (board[i, j] == this.blackId)
                        {
                            score++;
                        }
                    }
                }

                return score;
            }
        }

        public int LastPlayed
        {
            get
            {
                return this.lastPlayed;
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

        /// <summary>
        /// Init the grid
        /// </summary>
        /// <param name="width">width of the grid</param>
        /// <param name="height">height of the grid</param>
        public void init(int columns, int rows, int whiteId, int blackId)
        {
            this.board = new int[columns, rows];
            this.lastPlayed = whiteId;
            this.whiteId = whiteId;
            this.blackId = blackId;
            this.buildPossibleDirections();
            this.hasSkipped = false;

            this.initBoard();
        }

        /// <summary>
        /// Play a move
        /// </summary>
        /// <param name="position">Position to put a pawn</param>
        /// <param name="isWhite">color of the pawn</param>
        /// <returns>positions that changed</returns>
        public List<Vector2> play(Vector2 position, int idToPlay)
        {

            if (this.isPlayable(position, idToPlay))
            {
                var initialPosition = new Vector2(position);
                var changedPositions = new List<Vector2>();

                var directions = this.getValidMoves(position, idToPlay);

                foreach (var direction in directions)
                {
                    position = initialPosition;
                    do
                    {
                        this.putPawn(position, idToPlay);
                        changedPositions.Add(position);
                        position = position.add(direction);
                    } while (this.isInBoundaries(position) && this.getColor(position) != idToPlay);
                }

                // if it has 
                this.checkSkipAndSkip(idToPlay);
                
                return changedPositions;
            }
            else
            {
                throw new ArgumentException("This move isn't possible");
            }
        }

        private void checkSkipAndSkip(int playerPlayedId)
        {
            var otherPlayer = this.lastPlayed;

            if(this.checkSkipTurn(otherPlayer))
            {
                // check if the other player has played
                this.checkSkipAndSkip(otherPlayer);

                // if already skipped, a player won
                if(hasSkipped)
                {
                    Won(this, new WinEventArgs(this.getWinner()));
                }
                else
                {
                    TurnSkipped(this, new SkipTurnEventArgs(this.lastPlayed));
                    hasSkipped = true;
                }
            }
            else
            {
                //can play, so do not skip
                this.lastPlayed = playerPlayedId;
                hasSkipped = false;
            }
        }

        private void SkipTurn(int playerId)
        {
            this.lastPlayed = playerId;
            TurnSkipped(this, new SkipTurnEventArgs(playerId));
        }

        private bool checkSkipTurn(int idPlayer)
        {
            return this.getPositionsAvailable(idPlayer).Count == 0;
        }

        private int getWinner()
        {
            return 0;
        }

        /// <summary>
        /// Check if a move is possible
        /// </summary>
        /// <param name="position">Position to put a pawn</param>
        /// <param name="isWhite">Color of the pawn</param>
        /// <returns>move is playable or not</returns>
        public bool isPlayable(Vector2 position, int idToPlay)
        {
            return !(
                this.lastPlayed == idToPlay ||
                (this.isInBoundaries(position) && this.getColor(position) != 0) ||
                this.getValidMoves(position, idToPlay).Count == 0
            );

        }

        /// <summary>
        /// Get the directions which make the move position
        /// </summary>
        /// <param name="position">position of the move</param>
        /// <param name="isWhite">color of the pawns</param>
        /// <returns>list of the directions possible</returns>
        private List<Vector2> getValidMoves(Vector2 position, int idToPlay)
        {
            var validMoves = new List<Vector2>();

            foreach (Vector2 move in this.possibleMoves)
            {
                if (this.checkLine(position, move, idToPlay))
                {
                    Vector2 validMove = new Vector2(move);
                    validMoves.Add(move);
                }
            }

            return validMoves;
        }

        /// <summary>
        /// check there is another pawn of the same color on the same line
        /// </summary>
        /// <param name="position">position to start from</param>
        /// <param name="direction">direction to go</param>
        /// <param name="idToPlay">color of the pawn</param>
        /// <returns>if there is a pawn of the same color</returns>
        private bool checkLine(Vector2 position, Vector2 direction, int idToPlay)
        {
            int i = 0;
            int colorPosition = 0;
            do
            {
                position = position.add(direction);
                
                if (!this.isInBoundaries(position))
                {
                    return false;
                }
                colorPosition = this.getColor(position);


                if (colorPosition == idToPlay && i != 0)
                {
                    return true;
                }

                i++;
            } while (colorPosition != idToPlay && colorPosition != 0);

            return false;
        }

        /// <summary>
        /// Get all the move a player can do
        /// </summary>
        /// <param name="playerColor">id of the player</param>
        /// <returns>list of position playable</returns>
        public List<Vector2> getPositionsAvailable(int playerColor)
        {
            var positionsAvailable = new List<Vector2>();

            for(int i = 0; i < this.board.GetLength(0); i++)
            {
                for(int j = 0; j < this.board.GetLength(1); j++)
                {
                    var position = new Vector2(i, j);

                    if (this.isPlayable(position, playerColor) && this.getColor(position) == 0)
                    {
                        positionsAvailable.Add(position);
                    }
                }
            }

            return positionsAvailable;
        }



        /// <summary>
        /// check a position is in the board
        /// </summary>
        /// <param name="position">position to check</param>
        /// <returns>is in the board or not</returns>
        private bool isInBoundaries(Vector2 position)
        {
            return position.X < this.board.GetLength(0) && position.Y < this.board.GetLength(1)
                && position.X >= 0 && position.Y >= 0;
        }

        /// <summary>
        /// Check which neighbors allows this move
        /// TODO USEFUL?
        /// </summary>
        /// <param name="position">position of the neighbors</param>
        /// <param name="isWhite">colors of the pawn played</param>
        /// <returns>neighbor playable or not</returns>
        public bool isNeighborValid(Vector2 neighborPosition, int idToPlay)
        {
            return this.isInBoundaries(neighborPosition) && this.getColor(neighborPosition) != idToPlay;
        }

        /// <summary>
        /// get the color of a position (used to shortened the code)
        /// </summary>
        /// <param name="position">position of the pawns</param>
        /// <returns>Color of the pawns</returns>
        public int getColor(Vector2 position)
        {
            return this.board[position.X, position.Y];
        }

        public void putPawn(Vector2 position, int idColor)
        {
            this.board[position.X, position.Y] = idColor;
        }

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
        /// init the board with the right numbers
        /// </summary>
        private void initBoard()
        {
            int w = this.board.GetLength(0);
            int h = this.board.GetLength(1);

            for (int i = 0; i < w; i++)
            {
                for (int j = 0; j < h; j++)
                {
                    int color = 0;

                    if (i == w / 2 && j == h / 2 || i == w / 2 - 1 && j == h / 2 - 1)
                    {
                        color = this.whiteId;
                    }
                    else if (i == w / 2 - 1 && j == h / 2 || i == w / 2  && j == h / 2 - 1)
                    {
                        color = this.blackId;
                    }

                    this.putPawn(new Vector2(i, j), color);
                }
            }
        }

        /// <summary>
        /// build the list of Direction possible to play
        /// </summary>
        private void buildPossibleDirections()
        {
            this.possibleMoves = new List<Vector2>();

            //list is always the same, see if we can make it elsewhere
            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    if (i != 0 || j != 0)
                    {
                        this.possibleMoves.Add(new Vector2(i, j));
                    }
                }
            }
        }
    }
}