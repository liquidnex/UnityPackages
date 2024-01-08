using System;

namespace Liquid.CommonUtils
{
    /// <summary>
    /// A symbol for alphabet in FSM.
    /// </summary>
    /// <typeparam name="OriginalT">Original data type.</typeparam>
    public class FSMSymbol<OriginalT>
    {
        private Predicate<OriginalT> valueJudgeCallback;

        private FSM linkedFSM;
        private Enum linkedEvent;
        private OriginalT originalValue;

        /// <summary>
        /// Construct a new symbol.
        /// </summary>
        /// <param name="fsm">Linked finite state machine.</param>
        /// <param name="eventCode">Event sent when triggered.</param>
        /// <param name="initialValue">Initial value of the symbol.</param>
        /// <param name="valueJudge">Trigger judgement.</param>
        public FSMSymbol(FSM fsm, Enum eventCode, OriginalT initialValue, Predicate<OriginalT> valueJudge)
        {
            valueJudgeCallback = valueJudge;

            linkedFSM = fsm;
            linkedEvent = eventCode;
            originalValue = initialValue;
        }

        /// <summary>
        /// Set a new value to the symbol.
        /// </summary>
        /// <param name="newValue">A new symbol value.</param>
        public void Set(OriginalT newValue)
        {
            originalValue = newValue;

            if (valueJudgeCallback == null ||
                !valueJudgeCallback(originalValue))
                return;

            if (linkedFSM != null &&
                linkedEvent != null)
            {
                linkedFSM.OnEvent(linkedEvent);
            }
        }

        /// <summary>
        /// Get the value of symbol.
        /// </summary>
        /// <returns>Symbol value.</returns>
        public OriginalT Get()
        {
            return originalValue;
        }

        public static implicit operator OriginalT(FSMSymbol<OriginalT> s)
        {
            return s.Get();
        }
    }
}