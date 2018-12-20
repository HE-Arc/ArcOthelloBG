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


        public Playable()
        {
            game = Game.Instance;
            this.readSettings();
            game.init(this.rows, this.columns, this.whiteId, this.blackId);
        }

        int IPlayable.IPlayable.GetBlackScore()
        {
           return game.GetBlackScore();
        }
  
        int IPlayable.IPlayable.GetWhiteScore()
        {
            return game.GetWhiteScore();
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
            throw new NotImplementedException();
        }

        bool IPlayable.IPlayable.PlayMove(int column, int line, bool isWhite)
        {
            throw new NotImplementedException();
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
            }
            catch (ConfigurationErrorsException)
            {
                Console.WriteLine("Error reading app settings");
            }
        }
    }
}
