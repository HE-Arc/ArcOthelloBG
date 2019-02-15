using ArcOthelloBG.Exceptions;
using ArcOthelloBG.Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArcOthelloBG
{
    /// <summary>
    /// Class that handle undo redo of a board state
    /// </summary>
    class UndoRedo
    {
        private Stack<OthelloState> actionDone;
        private Stack<OthelloState> actionToRedo;

        /// <summary>
        /// constructoe
        /// </summary>
        public UndoRedo()
        {
            this.actionDone = new Stack<OthelloState>();
            this.actionToRedo = new Stack<OthelloState>();
        }
        
        /// <summary>
        /// do an action and stack it
        /// </summary>
        /// <param name="state">state of the board to stack</param>
        public void DoAction(OthelloState state)
        {
            this.actionDone.Push(state);
            this.actionToRedo.Clear();
        }

        /// <summary>
        /// Undo an action
        /// </summary>
        /// <returns>state of the board after action undon</returns>
        public OthelloState Undo(OthelloState stateUndone)
        {
            if(this.actionDone.Count > 0)
            {
                OthelloState state = this.actionDone.Pop();
                this.actionToRedo.Push(stateUndone);

                return state;
            }
            else
            {
                throw new StackEmptyException("no action done");
            }
            
        }

        /// <summary>
        /// Redo the first undone actions
        /// </summary>
        /// <returns>state of the board of the action</returns>
        public OthelloState Redo(OthelloState state)
        {
            if (this.actionToRedo.Count > 0)
            {
                OthelloState stateToRedo = this.actionToRedo.Pop();
                this.actionDone.Push(state);

                return stateToRedo;
            }
            else
            {
                throw new StackEmptyException("no action to redo");
            }
        }

        /// <summary>
        /// get the number of actions already done
        /// </summary>
        /// <returns>nb of actions</returns>
        public int GetNbUndoActions()
        {
            return this.actionDone.Count;
        }

        /// <summary>
        /// get the nb of actions undone
        /// </summary>
        /// <returns>nb of actions</returns>
        public int GetNbRedoActions()
        {
            return this.actionToRedo.Count;
        }
    }
}
