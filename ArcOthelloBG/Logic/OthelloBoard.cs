using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IPlayable;
using System.Configuration;
using ArcOthelloBG.IA;

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
        private Node node;
        private AlphaBetaAgentBG agent;

        /// <summary>
        /// constructor
        /// </summary>
        public OthelloBoard()
        {
            this.game = Game.Instance;
            this.readSettings();
            this.game.Init(this.columns, this.rows, this.whiteId, this.blackId, this.emptyId);
            this.agent = new AlphaBetaAgentBG(columns, rows, whiteId, blackId, emptyId);
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
            if (this.game.PlayerToPlay != this.getIdFromBool(whiteTurn))
            {
                this.game.NextTurn();
            }

            if (this.game.GetPositionsAvailable().Count > 0)
            {
                if (this.game.GetPositionsAvailable().Count == 1)
                {
                    return this.game.GetPositionsAvailable()[0].toTuplesintint();
                }

                node = new Node(agent, null, new Vector2(-1,-1));

                int minOrMax = 1;
                int initValue = minOrMax * -int.MaxValue;
                int depth = level;
                Tuple<int, Vector2> ab = agent.alphabeta(node, depth, minOrMax, initValue);

                return ab.Item2.toTuplesintint();
            }
            else
            {
                return new Tuple<int, int>(-1, -1);
            }
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
                if (this.game.PlayerToPlay != this.getIdFromBool(isWhite))
                {
                    this.game.NextTurn();
                }

                this.game.Play(new Vector2(column, line), this.getIdFromBool(isWhite));
                return true;
            }
            catch (ArgumentException)
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
            this.columns = 9;
            this.rows = 7;
            this.whiteId = 0;
            this.blackId = 1;
            this.emptyId = -1;
        }
    }
}
