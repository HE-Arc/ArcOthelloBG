﻿using System;
using System.Collections.Generic;

namespace ArcOthelloBG.Logic
{
    [Serializable]
    class OthelloState
    {
        private int[,] board;
        private List<Vector2> availablePositions;
        private int playerId;
        private int emptyId;
        private int whiteScore;
        private int blackScore;
        private Dictionary<Vector2, List<Vector2>> listAvailableDirectionsForPosition;
        private List<Vector2> possibleDirections;


        public int[,] Board
        {
            get { return this.board; }
        }
        public List<Vector2> AvailablePositions
        {
            get { return this.availablePositions; }
        }
        public int PlayerId
        {
            get { return this.playerId; }
        }
        public Dictionary<Vector2, List<Vector2>> ListAvailableDirectionsForPosition
        {
            get { return this.listAvailableDirectionsForPosition; }
        }

        public int EmptyId
        {
            get { return this.emptyId;  }
        }

        public int WhiteScore
        {
            get { return this.whiteScore; }
        }

        public int BlackScore
        {
            get { return this.blackScore; }
        }

        public int WhiteTime { get; set; }
        public int BlackTime { get; set; }

        public OthelloState(int[,] board, int playerId, List<Vector2> possibleDirections, int emptyId, int whiteScore, int blackScore)
        {
            this.board = (int[,])board.Clone();
            this.playerId = playerId;
            this.emptyId = emptyId;
            this.whiteScore = whiteScore;
            this.blackScore = blackScore;
            this.availablePositions = new List<Vector2>();
            this.listAvailableDirectionsForPosition = new Dictionary<Vector2, List<Vector2>>();
            this.possibleDirections = possibleDirections;
            this.computeAvailablePositions();
        }

        private void computeAvailablePositions()
        {
            for (int i = 0; i < this.board.GetLength(0); i++)
            {
                for (int j = 0; j < this.board.GetLength(1); j++)
                {
                    var position = new Vector2(i, j);

                    if (this.isPlayableWithCompute(position))
                    {
                        this.availablePositions.Add(position);
                    }
                }
            }
        }

        private bool isPlayableWithCompute(Vector2 position)
        {
            this.computeValidDirection(position);

            return this.isPlayable(position);
        }

        public bool isPlayable(Vector2 position)
        {
            return this.isInBoundaries(position) && this.getColor(position) == this.emptyId && this.listAvailableDirectionsForPosition.ContainsKey(position);        
        }

        private List<Vector2> computeValidDirection(Vector2 position)
        {
            var validMoves = new List<Vector2>();

            foreach (Vector2 move in this.possibleDirections)
            {
                if (this.checkLine(position, move))
                {
                    if(!this.listAvailableDirectionsForPosition.ContainsKey(position))
                    {
                        this.ListAvailableDirectionsForPosition.Add(position, new List<Vector2>());
                    }
                    
                    this.ListAvailableDirectionsForPosition[position].Add(move);
                }
            }

            return validMoves;
        }

        private bool checkLine(Vector2 position, Vector2 direction)
        {
            int i = 0;
            int colorPosition = this.emptyId;

            do
            {
                position = position.add(direction);

                if (!this.isInBoundaries(position))
                {
                    return false;
                }
                colorPosition = this.getColor(position);


                if (colorPosition == this.playerId && i != 0)
                {
                    return true;
                }

                i++;
            } while (colorPosition != this.playerId && colorPosition != this.emptyId);

            return false;
        }
            
        public int getColor(Vector2 position)
        {
            return this.board[position.X, position.Y];
        }

        public bool isInBoundaries(Vector2 position)
        {
            return position.X < this.board.GetLength(0) && position.Y < this.board.GetLength(1)
                && position.X >= 0 && position.Y >= 0;
        }

        public List<Vector2> getValidDirections(Vector2 position)
        {
            return this.listAvailableDirectionsForPosition[position];
        }
    }
}
