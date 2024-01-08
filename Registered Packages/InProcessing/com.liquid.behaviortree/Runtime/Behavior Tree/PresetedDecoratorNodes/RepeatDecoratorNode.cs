using System.Collections.Generic;

namespace Liquid.BehaviorTree
{
    public class RepeatDecoratorNode : DecoratorNode
    {
        public RepeatDecoratorNode(BehaviorTree tree, int repeatTimes = 0)
            : base(tree) 
        {
            if (targetRepeatTimes <= 1)
                return;

            targetRepeatTimes = repeatTimes;
        }

        public override void Reset()
        {
            repeatTimes = 0;
            tmpResult = NodeResult.NONE;
        }

        public override Node GetNext()
        {
            List<Node> childs = ChildNodes;
            if (childs.Count == 0)
                return null;

            Node child = childs[0];
            if (repeatTimes < targetRepeatTimes)
            {
                ++repeatTimes;
                return child;
            }
            return null;
        }

        public override NodeResult Result
        {
            get
            {
                if (repeatTimes < targetRepeatTimes)
                    return NodeResult.NONE;
                else
                {
                    if (tmpResult == NodeResult.FAILURE ||
                        tmpResult == NodeResult.SUCCESS)
                        return tmpResult;

                    List<NodeResult> childResults = ChildResults;
                    if (childResults.Count == 0)
                        tmpResult = NodeResult.FAILURE;
                    else
                        tmpResult = childResults[0];
                    return tmpResult;
                }
            }
        }

        protected int repeatTimes = 0;
        protected readonly int targetRepeatTimes = 1;
        protected NodeResult tmpResult = NodeResult.NONE;
    }
}