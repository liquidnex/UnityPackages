using System;

namespace Liquid.BehaviorTree
{
    public abstract class ActionNode : ExecutionNode
    {
        public ActionNode(BehaviorTree tree)
            : base(tree) {}

        public override void Tick()
        {
            if (tmpResult == NodeResult.RUNNING)
                tmpResult = CheckExecute();
            
            if (tmpResult == NodeResult.NONE)
            {
                tmpResult = Execute();
                if (tmpResult == NodeResult.NONE)
                    throw new Exception("The Solve function cannot return a NONE as the result.");
            }
        }

        public virtual void Stop()
        {
            tmpResult = NodeResult.FAILURE;
            behaviorTree.UnregisterRunningNode(this);
        }

        public override void Reset()
        {
            tmpResult = NodeResult.NONE;
        }

        protected abstract NodeResult Execute();

        protected abstract NodeResult CheckExecute();
    }
}