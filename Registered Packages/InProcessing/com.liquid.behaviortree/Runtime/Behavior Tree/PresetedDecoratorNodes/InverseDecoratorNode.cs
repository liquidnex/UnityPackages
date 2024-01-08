using System.Collections.Generic;

namespace Liquid.BehaviorTree
{
    public class InverseDecoratorNode : DecoratorNode
    {
        public InverseDecoratorNode(BehaviorTree tree)
            : base(tree) {}

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

                List<NodeResult> childResults = ChildResults;
                if (childResults.Count == 0)
                    tmpResult = NodeResult.FAILURE;
                else
                {
                    tmpResult = childResults[0];
                    if (tmpResult == NodeResult.SUCCESS)
                        tmpResult = NodeResult.FAILURE;
                    else if (tmpResult == NodeResult.FAILURE)
                        tmpResult = NodeResult.SUCCESS;
                }

                return tmpResult;
            }
        }

        protected NodeResult tmpResult = NodeResult.NONE;
    }
}
