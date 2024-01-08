using System;

namespace Liquid.CommonUtils
{
    /// <summary>
    /// Base class of all custom FSMState classes.
    /// </summary>
    public abstract class FSMState
    {
        protected Type sourceStateType;
        private FSM linkedFSM;

        protected FSM LinkedFSM
        {
            get => linkedFSM;
        }

        protected void Init(FSM fsm)
        {
            linkedFSM = fsm;
        }

        /// <summary>
        /// Called when the state enters.
        /// </summary>
        /// <param name="fromState">State of the source.</param>
        public virtual void EnterState(Type fromState)
        {
            sourceStateType = fromState;
        }

        /// <summary>
        /// Called when the state exits
        /// </summary>
        public virtual void ExitState(Type nextState) {}

        /// <summary>
        /// Reset data of state.
        /// </summary>
        public virtual void Reset() 
        {
            sourceStateType = null;
        }

        /// <summary>
        /// Handle event code and control whether to transfer state.
        /// </summary>
        /// <param name="eventCode">Event code.</param>
        /// <returns>
        /// A type of FSM state to transition after transfer.
        /// If you don't want to transfer state, you can return null.
        /// </returns>
        public abstract Type Transition(Enum eventCode);
    }
}