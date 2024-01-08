using System;
using System.Collections.Generic;
using System.Reflection;

namespace Liquid.CommonUtils
{
    /// <summary>
    /// Finite State Machine.
    /// The managed FSM state MUST be one of the fields of the state machine.
    /// </summary>
    public class FSM
    {
        protected FSMState currentState;
        private List<FSMState> registeredStates = new List<FSMState>();

        /// <summary>
        /// Current FSM state.
        /// </summary>
        public FSMState CurrentState
        {
            get => currentState;
        }

        /// <summary>
        /// Construct a FSM directly.
        /// </summary>
        public FSM() {}

        /// <summary>
        /// Register a state into FSM.
        /// </summary>
        /// <typeparam name="T">State to register.</typeparam>
        /// <returns>Returns true if success, otherwise returns false.</returns>
        public bool Register<T>()
            where T : FSMState, new()
        {
            Type registerType = typeof(T);
            if (registerType == null)
                return false;

            if (registeredStates == null)
                registeredStates = new List<FSMState>();

            int idx = registeredStates.FindIndex(s => s.GetType() == registerType);
            if (idx != -1)
                return false;

            T newState = new T();
            var initMethod =
                registerType.GetMethod("Init", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            if (initMethod == null)
                return false;

            initMethod.Invoke(newState, new object[] { this });
            registeredStates.Add(newState);

            return true;
        }

        /// <summary>
        /// Register a state into FSM.
        /// </summary>
        /// <param name="registerType">State to register.</param>
        /// <returns>Returns true if success, otherwise returns false.</returns>
        public bool Register(Type registerType)
        {
            if (registerType == null ||
                !(typeof(FSMState).IsAssignableFrom(registerType)))
                return false;

            if (registeredStates == null)
                registeredStates = new List<FSMState>();

            int idx = registeredStates.FindIndex(s => s.GetType() == registerType);
            if (idx != -1)
                return false;

            FSMState newState =
                registerType.Assembly.CreateInstance(
                    registerType.FullName, false,
                    BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, null, null, null, null)
                    as FSMState;
            var initMethod = registerType.GetMethod("Init", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            if (initMethod == null)
                return false;

            initMethod.Invoke(newState, new object[] { this });
            registeredStates.Add(newState);

            return true;
        }

        /// <summary>
        /// Launch FSM with a initialize state.
        /// </summary>
        /// <typeparam name="T">Type of the initialize state.</typeparam>
        public void Launch<T>()
            where T : FSMState, new()
        {
            if (currentState != null)
                return;

            Type registerType = typeof(T);
            if (registerType == null)
                return;

            FSMState s = GetStateInstance<T>();
            if (s == null)
                return;

            currentState = s;
            s.EnterState(null);
        }

        /// <summary>
        /// Launch FSM with a initialize state.
        /// </summary>
        /// <param name="initialState">Type of the initialize state.</param>
        public void Launch(Type initialState)
        {
            if (currentState != null)
                return;

            if (initialState == null ||
                !(typeof(FSMState).IsAssignableFrom(initialState)))
                return;

            FSMState s = GetStateInstance(initialState);
            if (s == null)
                return;

            currentState = s;
            s.EnterState(null);
        }

        /// <summary>
        /// Pass an event to FSM.
        /// </summary>
        /// <param name="evt">Event.</param>
        public void OnEvent(Enum evt)
        {
            if (currentState == null ||
                !IsLegalEvent(currentState, evt))
                return;

            Type next = currentState.Transition(evt);
            if (next == null)
                return;

            FSMState nextState = GetStateInstance(next);
            if (nextState == null)
                return;

            Type lastStateType = currentState.GetType();
            currentState.ExitState(next);
            currentState.Reset();
            currentState = nextState;
            nextState.EnterState(lastStateType);
        }

        /// <summary>
        /// Construct a new symbol.
        /// </summary>
        /// <typeparam name="OriginalT">Original data type.</typeparam>
        /// <param name="eventCode">Event sent when triggered.</param>
        /// <param name="initialValue">Initial value of the symbol.</param>
        /// <param name="valueJudge">Trigger judgement.</param>
        /// <returns></returns>
        public FSMSymbol<OriginalT> CreateSymbol<OriginalT>(
            Enum eventCode, OriginalT initialValue, Predicate<OriginalT> valueJudge)
        {
            return new FSMSymbol<OriginalT>(this, eventCode, initialValue, valueJudge);
        }

        private FSMState GetStateInstance<T>()
        {
            if (registeredStates == null)
                registeredStates = new List<FSMState>();

            return registeredStates.Find(s => s.GetType() == typeof(T));
        }

        private FSMState GetStateInstance(Type stateType)
        {
            if (registeredStates == null)
                registeredStates = new List<FSMState>();

            return registeredStates.Find(s => s.GetType() == stateType);
        }

        private bool IsLegalEvent(FSMState state, Enum eventCode)
        {
            if (state == null ||
                eventCode == null)
                return false;

            Type t = state.GetType();
            List<Type> nestedTypes = new List<Type>(
                t.GetNestedTypes(
                    BindingFlags.Static |
                    BindingFlags.Public |
                    BindingFlags.NonPublic
                )
            );

            Type eventType = eventCode.GetType();
            int idx = nestedTypes.FindIndex(t => eventType == t);

            return idx != -1;
        }
    }
}