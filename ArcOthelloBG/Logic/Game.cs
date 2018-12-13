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
        public void init(int width, int height)
        {
            this.board = new int[width, height];
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

                if (instance.board == null)
                {
                    return new InvalidOperationException;
                }

                return instance;
            }
        }

        public int[,] Board { get; }
    }
}
