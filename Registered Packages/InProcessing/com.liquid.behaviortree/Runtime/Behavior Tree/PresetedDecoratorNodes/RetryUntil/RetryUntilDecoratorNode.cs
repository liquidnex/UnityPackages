using System.Collections.Generic;

namespace Liquid.BehaviorTree
{
    public abstract class RetryUntilDecoratorNode : DecoratorNode
    {
        public RetryUntilDecoratorNode(BehaviorTree tree, bool untilSuccess)
            : base(tree)
        {
            retryUntilSuccess = untilSuccess;
        }

        public override void Reset()
        {
            tmpResult = NodeResult.NONE;
        }

        public override Node GetNext()
        {
            List<Node> childs = ChildNodes;
            if (childs.Count == 0)
                return null;

            if ((retryUntilSuccess && tmpResult == NodeResult.SUCCESS) ||
                (!retryUntilSuccess && tmpResult == NodeResult.FAILURE) ||
                tmpResult == NodeResult.RUNNING)
                return null;

            Node child = childs[0];
            return child;
        }

        public override NodeResult Result
        {
            get
            {
                if ((retryUntilSuccess && tmpResult == NodeResult.SUCCESS) ||
                    (!retryUntilSuccess && tmpResult == NodeResult.FAILURE))
                    return tmpResult;

                List<NodeResult> childResults = ChildResults;
                if (childResults.Count == 0)
                    tmpResult = NodeResult.FAILURE;
                else
                    tmpResult = childResults[0];

                if ((retryUntilSuccess && tmpResult == NodeResult.FAILURE) ||
                    (!retryUntilSuccess && tmpResult == NodeResult.SUCCESS))
                    tmpResult = NodeResult.NONE;

                return tmpResult;
            }
        }

        protected readonly bool retryUntilSuccess = false;
        protected NodeResult tmpResult = NodeResult.NONE;
    }
}