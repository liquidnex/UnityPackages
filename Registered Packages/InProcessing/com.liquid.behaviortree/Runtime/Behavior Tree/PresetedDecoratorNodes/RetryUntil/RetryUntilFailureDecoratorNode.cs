namespace Liquid.BehaviorTree
{
    public class RetryUntilFailureDecoratorNode : RetryUntilDecoratorNode
    {
        public RetryUntilFailureDecoratorNode(BehaviorTree tree)
            : base(tree, false) {}
    }
}