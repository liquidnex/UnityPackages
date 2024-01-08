using Liquid.CommonUtils;
using System;
using UnityEngine;

namespace Liquid.Samples.FSMSample
{
    public class FSMStateT0 : FSMState
    {
        public enum FSMStateT0_Events
        {
            EVENT_0
        }

        public override void EnterState(Type fromState)
        {
            base.EnterState(fromState);
            iT0 = 1;
            Debug.Log("Enter State T0");
        }

        public override void ExitState(Type nextState)
        {
            Debug.Log("Exit State T0");
        }

        public override void Reset()
        {
            base.Reset();
            iT0 = 0;
        }

        public override Type Transition(Enum eventCode)
        {
            switch (eventCode)
            {
                case FSMStateT0_Events.EVENT_0:
                    return typeof(FSMStateT1);
            }
            return null;
        }

        private int iT0 = 0;
    }

    public class FSMStateT1 : FSMState
    {
        public override void EnterState(Type fromState)
        {
            base.EnterState(fromState);
            iT1 = 1;
            Debug.Log("Enter State T1");
        }

        public override void ExitState(Type nextState)
        {
            Debug.Log("Exit State T1");
        }

        public override void Reset()
        {
            base.Reset();
            iT1 = 0;
        }

        public override Type Transition(Enum eventCode)
        {
            return null;
        }

        private int iT1 = 0;
    }

    public class Sample : MonoBehaviour
    {
        private void Awake()
        {
            Init();
        }

        private void Init()
        {
            FSM fsmTest = new FSM();
            fsmTest.Register<FSMStateT0>();
            fsmTest.Register<FSMStateT1>();
            fsmTest.Launch<FSMStateT0>();

            var symbol = fsmTest.CreateSymbol(FSMStateT0.FSMStateT0_Events.EVENT_0, 0, v => v == 1);
            Debug.Log(fsmTest.CurrentState);
            symbol.Set(1);
            Debug.Log(fsmTest.CurrentState);
        }
    }
}