using System.Collections.Generic;

namespace Liquid.BehaviorTree
{
    public abstract class ForceRaiseDecoratorNode : DecoratorNode
    {
        public ForceRaiseDecoratorNode(BehaviorTree tree, bool forceSuccess)
            : base(tree) 
        {
            forceRaiseSuccess = forceSuccess;
        }

        public override void Reset()
        {
            tmpResult = NodeResult.NONE;
        }

        public override NodeResult Result
        {
            get
            {
                if (tmpResult != NodeResult.FAILURE &&
                    tmpResult != NodeResult.SUCCESS)
                {
                    List<NodeResult> childResults = ChildResults;
                    if (childResults.Count == 0)
                        tmpResult = NodeResult.FAILURE;
                    else
                        tmpResult = childResults[0];
                }

                if (forceRaiseSuccess)
                    tmpResult = NodeResult.SUCCESS;
                else
                    tmpResult = NodeResult.FAILURE;
                return tmpResult;
            }
        }

        protected readonly bool forceRaiseSuccess = false;
        protected NodeResult tmpResult = NodeResult.NONE;
    }
}