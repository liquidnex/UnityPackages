using System.Collections.Generic;

namespace Liquid.BehaviorTree
{
    public class SequenceNode : ControlNode
    {
        public SequenceNode(BehaviorTree tree, InterruptMode im)
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

                if (results.Contains(NodeResult.FAILURE))
                    tmpResult = NodeResult.FAILURE;
                else
                    tmpResult = NodeResult.SUCCESS;
                return tmpResult;
            }
        }

        protected NodeResult tmpResult = NodeResult.NONE;
    }
}