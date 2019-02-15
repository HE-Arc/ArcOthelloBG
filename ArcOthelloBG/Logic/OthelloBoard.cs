using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IPlayable;
using System.Configuration;

namespace ArcOthelloBG.Logic
{
    /// <summary>
    /// Implementation of the IPlayable
    /// </summary>
    class OthelloBoard : IPlayable.IPlayable
    {
        Game game;
        private int rows;
        private int columns;
        private int whiteId;
        private int blackId;
        private int emptyId;

        /// <summary>
        /// constructor
        /// </summary>
        public OthelloBoard()
        {
            this.game = Game.Instance;
            this.readSettings();
            Console.WriteLine(this.rows);
            Console.WriteLine(this.columns);
            this.game.Init(this.rows, this.columns, this.whiteId, this.blackId,this.emptyId);
        }

        /// <summary>
        /// get the black score
        /// </summary>
        /// <returns>score</returns>
        int IPlayable.IPlayable.GetBlackScore()
        {
           return game.BlackScore;
        }
  
        /// <summary>
        /// get the white score
        /// </summary>
        /// <returns>score</returns>
        int IPlayable.IPlayable.GetWhiteScore()
        {
            return game.WhiteScore;
        }

        /// <summary>
        /// get the board
        /// </summary>
        /// <returns></returns>
        int[,] IPlayable.IPlayable.GetBoard()
        {
            return game.Board;
        }

        /// <summary>
        /// get the name of the game
        /// </summary>
        /// <returns></returns>
        string IPlayable.IPlayable.GetName()
        {
            return "ArcOthelloBG";
            
        }

        /// <summary>
        /// Get the next moves
        /// </summary>
        /// <param name="game">board</param>
        /// <param name="level">level</param>
        /// <param name="whiteTurn">is it white turn or black</param>
        /// <returns></returns>
        Tuple<int, int> IPlayable.IPlayable.GetNextMove(int[,] game, int level, bool whiteTurn)
        {
            return this.game.GetPositionsAvailable()[0].toTuplesintint();
        }

        /// <summary>
        /// if a move is playable
        /// </summary>
        /// <param name="column">x coordinate of the move</param>
        /// <param name="line">y coordinate of the move</param>
        /// <param name="isWhite">is white or black</param>
        /// <returns></returns>
        bool IPlayable.IPlayable.IsPlayable(int column, int line, bool isWhite)
        {
            return this.game.IsPlayable(new Vector2(column, line), this.getIdFromBool(isWhite));
        }

        /// <summary>
        /// Play a move at the position given
        /// </summary>
        /// <param name="column">nb of the column</param>
        /// <param name="line">nb of the line</param>
        /// <param name="isWhite">is white or black</param>
        /// <returns></returns>
        bool IPlayable.IPlayable.PlayMove(int column, int line, bool isWhite)
        {
            try
            {
                this.game.Play(new Vector2(column, line), this.getIdFromBool(isWhite));
                return true;
            }
            catch(ArgumentException)
            {
                return false;
            }
        }

        /// <summary>
        /// return the id corresponding to the bool
        /// </summary>
        /// <param name="isWhite">boolean to check</param>
        /// <returns>the id of the corresponding color</returns>
        private int getIdFromBool(bool isWhite)
        {
            return isWhite ? this.whiteId : this.blackId;
        }

        /// <summary>
        /// read the app.config file
        /// </summary>
        private void readSettings()
        {
            Console.WriteLine("Error reading app settings");
            this.columns = 7;
            this.rows = 9;
            this.whiteId = 0;
            this.blackId = 1;
            this.emptyId = -1;
        }
    }
}
