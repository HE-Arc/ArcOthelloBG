using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IPlayable;
using System.Configuration;

namespace ArcOthelloBG.Logic
{
    class Playable : IPlayable.IPlayable
    {
        Game game;
        private int rows;
        private int columns;
        private int whiteId;
        private int blackId;
        private int emptyId;


        public Playable()
        {
            this.game = Game.Instance;
            this.readSettings();
            this.game.Init(this.rows, this.columns, this.whiteId, this.blackId,this.emptyId);
        }

        int IPlayable.IPlayable.GetBlackScore()
        {
           return game.BlackScore;
        }
  
        int IPlayable.IPlayable.GetWhiteScore()
        {
            return game.WhiteScore;
        }

        int[,] IPlayable.IPlayable.GetBoard()
        {
            return game.Board;
        }

        string IPlayable.IPlayable.GetName()
        {
            throw new NotImplementedException();
        }

        Tuple<int, int> IPlayable.IPlayable.GetNextMove(int[,] game, int level, bool whiteTurn)
        {
            throw new NotImplementedException();
        }

        bool IPlayable.IPlayable.IsPlayable(int column, int line, bool isWhite)
        {
            return this.game.IsPlayable(new Vector2(column, line), this.getIdFromBool(isWhite));
        }

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
            try
            {
                var appSettings = ConfigurationManager.AppSettings;
                this.rows = Convert.ToInt32(appSettings["rows"]);
                this.columns = Convert.ToInt32(appSettings["columns"]);
                this.whiteId = Convert.ToInt32(appSettings["whiteId"]);
                this.blackId = Convert.ToInt32(appSettings["blackId"]);
                this.emptyId = Convert.ToInt32(appSettings["emptyId"]);
            }
            catch (ConfigurationErrorsException)
            {
                Console.WriteLine("Error reading app settings");
                this.columns = 9;
                this.rows = 7;
                this.whiteId = 2;
                this.blackId = 1;
                this.emptyId = -1;
            }
        }
    }
}
