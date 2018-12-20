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
        private int blackId;
        private int whiteId;

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
        public void init(int columns, int rows)
        {
            this.board = new int[columns, rows];
            var appSettings = ConfigurationManager.AppSettings;
            this.whiteId = Convert.ToInt32(appSettings["whiteId"]);
            this.blackId = Convert.ToInt32(appSettings["blackId"]);
            this.initBoard();
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
            int w = this.board.GetLength(0);
            int h = this.board.GetLength(1);
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