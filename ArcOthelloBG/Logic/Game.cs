using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArcOthelloBG.Logic
{
    class Game
    {
        // MEMBERS
        private int[,] board;
        private static Game instance = null;
        private int lastPlayed;
        private int whiteId;
        private int blackId;


        // METHODS

        /// <summary>
        /// singleton constructor, so private
        /// </summary>
        private Game()
        { }

        // TODO : Transform into property
        public int GetWhiteScore()
        {
            int score = 0;
            for(int i = 0; i < board.GetLength(0); i++)
            {
                for(int j = 0; j < board.GetLength(1); j++)
                {
                    if(board[i,j] == this.whiteId)
                    {
                        score++;
                    }
                }
            }

            return score;
        }

        // TODO : Transform into property
        public int GetBlackScore()
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

            this.initBoard();
        }

        /// <summary>
        /// Play a move
        /// </summary>
        /// <param name="position">Position to put a pawn</param>
        /// <param name="isWhite">color of the pawn</param>
        public void play(Vector2 position, bool isWhite)
        {
            if(this.isPlayable(position, isWhite))
            {
                int idToplay = isWhite ? this.whiteId : this.blackId;

                var directions = this.getValidMoves(position, isWhite);

                foreach (Vector2 direction in directions)
                {
                    do
                    {
                        this.board[position.X, position.Y] = idToplay;
                        position = position.add(direction);
                    } while (this.board[position.X, position.Y] == idToplay);
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
        public bool isPlayable(Vector2 position, bool isWhite)
        {
            if(this.lastPlayed == this.whiteId && isWhite || this.lastPlayed == this.blackId && !isWhite)
            {
                return false;
            }

            if(this.getValidMoves(position, isWhite).Count == 0)
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
        private List<Vector2> getValidMoves(Vector2 position, bool isWhite)
        {
            var validMoves = new List<Vector2>();
            var possibleMoves = new List<Vector2>();

            for(int i = -1; i <= 1; i++ )
            {
                for(int j = -1; j <= 1; j++)
                {
                    if(i != 0 && j != 0)
                    {
                        possibleMoves.Add(new Vector2(i, j));
                    }
                }
            }

            for(int i = 0; i < possibleMoves.Count; i++)
            {
                if (this.isNeighborValid(possibleMoves[i], isWhite))
                {
                    validMoves.Add(possibleMoves[i]);
                }
            }

            return validMoves;
        }

        /// <summary>
        /// Check which neighbors allows this move
        /// </summary>
        /// <param name="position">position of the neighbors</param>
        /// <param name="isWhite">colors of the pawn played</param>
        /// <returns>neighbor playable or not</returns>
        public bool isNeighborValid(Vector2 position, bool isWhite)
        {
            // TODO CORRECT, ONLY CHECK NEIGHBORS AND NOT THE END OF THE LINE
            return position.X >= 0 && position.X < this.board.GetLength(1)
                && position.Y >= 0 && position.Y < this.board.GetLength(0)
                && 
                (
                    isWhite && this.getColor(position) == this.blackId
                    || !isWhite && this.getColor(position) == this.whiteId
                )
            ;
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
    }
}