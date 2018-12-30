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

        private enum Direction {LEFT, TOP, RIGHT, BOTTOM };


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


        public void play(Tuple<int,int> position, bool isWhite)
        {
            if(this.isPlayable(position, isWhite))
            {

            }
            else
            {
                 throw new ArgumentException("This move isn't possible");
            }
        }

        public bool isPlayable(Tuple<int,int> position, bool isWhite)
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

        private List<Direction> getValidMoves(Tuple<int,int> position, bool isWhite)
        {
            var validMoves = new List<Direction>();
            var possibleMoves = new List<Tuple<int, int>>();

            possibleMoves.Add(new Tuple<int, int>(position.Item1 - 1, position.Item2)); // left
            possibleMoves.Add(new Tuple<int, int>(position.Item1, position.Item2 - 1)); // top
            possibleMoves.Add(new Tuple<int, int>(position.Item1 + 1, position.Item2)); // right
            possibleMoves.Add(new Tuple<int, int>(position.Item1, position.Item2 + 1)); // bottom

            for(int i = 0; i < possibleMoves.Count; i++)
            {
                if (this.isNeighborValid(possibleMoves[i], isWhite))
                {
                    validMoves.Add((Direction)i);
                }
            }

            return validMoves;
        }

        public bool isNeighborValid(Tuple<int,int> position, bool isWhite)
        {
            if (position.Item1 >= 0 && position.Item1 < this.board.GetLength(1)
                && position.Item2 >= 0 && position.Item2 < this.board.GetLength(0)
                && (isWhite && this.getColor(position) == this.blackId 
                    || !isWhite && this.getColor(position) == this.whiteId
                    )
                )
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public int getColor(Tuple<int,int> position)
        {
            return this.board[position.Item2, position.Item1];
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