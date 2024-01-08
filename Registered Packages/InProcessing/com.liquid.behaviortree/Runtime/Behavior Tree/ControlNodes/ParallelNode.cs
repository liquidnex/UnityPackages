using System.Collections.Generic;

namespace Liquid.BehaviorTree
{
    public class ParallelNode : ControlNode
    {
        public ParallelNode(BehaviorTree tree, InterruptMode im, int successCount = 1)
            : base(tree, im)
        {
            if (successCount < 1)
                successCount = 1;

            targetSuccessCount = successCount;
        }

        public override void Reset()
        {
            tmpResult = NodeResult.NONE;
        }

        public override NodeResult Result
        {
            get
            {
                if (tmpResult == NodeResult.FAILURE ||
                    tmpResult == NodeResult.SUCCESS)
                    return tmpResult;

                List<NodeResult> results = ChildResults;

                if (results.Contains(NodeResult.RUNNING))
                    return NodeResult.RUNNING;

                if (results.Contains(NodeResult.NONE))
                    return NodeResult.NONE;

                int successCount = results.FindAll(r => r == NodeResult.SUCCESS).Count;
                if (successCount >= targetSuccessCount)
                    tmpResult = NodeResult.SUCCESS;
                else
                    tmpResult = NodeResult.FAILURE;
                return tmpResult;
            }
        }

        protected NodeResult tmpResult = NodeResult.NONE;
        private readonly int targetSuccessCount = 1;
    }
}