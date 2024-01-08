using System;
using Liquid.CommonUtils;

namespace Liquid.BehaviorTree
{
    public abstract class TimeoutActionNode : ActionNode
    {
        public TimeoutActionNode(BehaviorTree tree, float timeoutSec) :
            base(tree)
        {
            timeoutSecond = timeoutSec;
        }

        public override void Tick()
        {
            if (tmpResult == NodeResult.RUNNING)
                tmpResult = CheckExecute();

            if (tmpResult != NodeResult.NONE)
                return;

            tmpResult = Execute();
            if (tmpResult == NodeResult.NONE)
                throw new Exception("The Solve function cannot return a NONE as the result.");
            if (tmpResult == NodeResult.RUNNING)
            {
                timer = Timer.CreateTimer();
                timer.Launch(timeoutSecond, null, Stop);
            }
        }

        private Timer timer;
        private readonly float timeoutSecond;
    }
}