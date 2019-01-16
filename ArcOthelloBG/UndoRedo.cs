using ArcOthelloBG.Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArcOthelloBG
{
    class UndoRedo
    {
        private Stack<BoardState> actionDone;
        private Stack<BoardState> actionToRedo;

        public UndoRedo()
        {
            this.actionDone = new Stack<BoardState>();
            this.actionToRedo = new Stack<BoardState>();
        }
        
        public void DoAction(BoardState state)
        {
            this.actionDone.Push(state);
            this.actionToRedo.Clear();
        }

        public BoardState Undo()
        {
            BoardState state = this.actionDone.Pop();
            this.actionToRedo.Push(state);

            return state;
        }

        public BoardState Redo()
        {
            BoardState state = this.actionToRedo.Pop();
            this.actionDone.Push(state);

            return state;
        }

        public int GetNbUndoActions()
        {
            return this.actionDone.Count;
        }

        public int GetNbRedoActions()
        {
            return this.actionToRedo.Count;
        }
    }
}
