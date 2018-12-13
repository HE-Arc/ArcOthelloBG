using System;
using System.Collections.Generic;
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

        // METHODS

        /// <summary>
        /// singleton constructor, so private
        /// </summary>
        private Game()
        { }


        /// <summary>
        /// Init the grid
        /// </summary>
        /// <param name="width">width of the grid</param>
        /// <param name="height">height of the grid</param>
        public void init(int columns, int rows)
        {
            this.board = new int[columns, rows];
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
    }
}