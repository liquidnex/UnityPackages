using Liquid.CommonUtils;

namespace Liquid.BehaviorTree
{
    public sealed class WaitingForTimeActionNode : ActionNode
    {
        public WaitingForTimeActionNode(BehaviorTree tree, float waitSec) :
            base(tree)
        {
            waitSeconds = waitSec;
        }

        protected override NodeResult CheckExecute()
        {
            return tmpResult;
        }

        protected override NodeResult Execute()
        {
            timer = Timer.CreateTimer();
            timer.Launch(waitSeconds, null, FinishWait);

            return NodeResult.RUNNING;
        }

        private void FinishWait()
        {
            tmpResult = NodeResult.SUCCESS; 
        }

        private Timer timer;
        private readonly float waitSeconds;
    }
}