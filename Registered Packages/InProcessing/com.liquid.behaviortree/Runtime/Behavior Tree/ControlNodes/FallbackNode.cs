using System.Collections.Generic;

namespace Liquid.BehaviorTree
{
    public class FallbackNode : ControlNode
    {
        public FallbackNode(BehaviorTree tree, InterruptMode im)
            : base(tree, im) {}

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

                if (results.Contains(NodeResult.SUCCESS))
                    tmpResult = NodeResult.SUCCESS;
                else
                    tmpResult = NodeResult.FAILURE;
                return tmpResult;
            }
        }

        protected NodeResult tmpResult = NodeResult.NONE;
    }
}