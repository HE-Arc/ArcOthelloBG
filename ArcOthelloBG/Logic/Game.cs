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
            get {
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

        /// <summary>
        /// Init the grid
        /// </summary>
        /// <param name="width">width of the grid</param>
        /// <param name="height">height of the grid</param>
        public void init(int columns, int rows, int whiteId, int blackId)
        {
            this.board = new int[columns, rows];
            this.lastPlayed = blackId;
            this.whiteId = whiteId;
            this.blackId = blackId;
            this.buildPossibleDirections();

            this.initBoard();
        }

        /// <summary>
        /// Play a move
        /// </summary>
        /// <param name="position">Position to put a pawn</param>
        /// <param name="isWhite">color of the pawn</param>
        public void play(Vector2 position, int idToPlay)
        {
            if(this.isPlayable(position, idToPlay))
            {
                var directions = this.getValidMoves(position, idToPlay);

                foreach (Vector2 direction in directions)
                {
                    do
                    {
                        this.putPawn(position, idToPlay);
                        position = position.add(direction);
                    } while (this.getColor(position) == idToPlay && this.isInBoundaries(position));
                }
            }
            else
            {
                 throw new ArgumentException("This move isn't possible");
            }
        }

        /// <summary>
        /// Check if a move is possible
        /// </summary>
        /// <param name="position">Position to put a pawn</param>
        /// <param name="isWhite">Color of the pawn</param>
        /// <returns>move is playable or not</returns>
        public bool isPlayable(Vector2 position, int idToPlay)
        {
            if(this.lastPlayed != idToPlay)
            {
                return false;
            }

            if(this.getValidMoves(position, idToPlay).Count == 0)
            {
                return false;
            }

            return true;
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
            
            foreach(Vector2 move in this.possibleMoves)
            {
                if (this.isNeighborValid(move, idToPlay) && this.checkLine(position, move, idToPlay))
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
            do
            {
                position = position.add(direction);

                if (this.getColor(position) == idToPlay)
                {
                    return true;
                }
            } while (this.getColor(position) == idToPlay && this.isInBoundaries(position));

            return false;
        }



        /// <summary>
        /// check a position is in the board
        /// </summary>
        /// <param name="position">position to check</param>
        /// <returns>is in the board or not</returns>
        private bool isInBoundaries(Vector2 position)
        {
            return position.X < this.board.GetLength(1) && position.Y < this.board.GetLength(0) 
                && position.X >= 0 && position.Y >= 0;
        }

        /// <summary>
        /// Check which neighbors allows this move
        /// </summary>
        /// <param name="position">position of the neighbors</param>
        /// <param name="isWhite">colors of the pawn played</param>
        /// <returns>neighbor playable or not</returns>
        public bool isNeighborValid(Vector2 position, int idToPlay)
        {
            return this.isInBoundaries(position) && this.getColor(position) == idToPlay;
                
            ;
        }

        /// <summary>
        /// get the color of a position (used to shortened the code)
        /// </summary>
        /// <param name="position">position of the pawns</param>
        /// <returns>Color of the pawns</returns>
        public int getColor(Vector2 position)
        {
            return this.board[position.Y, position.X];
        }

        public void putPawn(Vector2 position, int idColor)
        {
            this.board[position.Y, position.X] = idColor;
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
        /// init the board with the right numbers
        /// </summary>
        private void initBoard()
        {
            int w = this.board.GetLength(1);
            int h = this.board.GetLength(0);

            for (int i = 0; i < this.board.GetLength(0); i++)
            {
                for (int j = 0; j < board.GetLength(1); j++)
                {
                    if(i == w / 2 - 1 && j == h / 2 - 1 || i == w / 2 && j == h / 2)
                    {
                        this.board[i, j] = this.whiteId;
                    }
                    else if(i == w / 2  && j == h / 2 - 1 || i == w / 2 - 1 && j == h / 2)
                    {
                        this.board[i, j] = this.blackId;
                    }
                    else
                    {
                        this.board[i, j] = 0;
                    }
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
                    if (i != 0 && j != 0)
                    {
                        this.possibleMoves.Add(new Vector2(i, j));
                    }
                }
            }
        }
    }
}