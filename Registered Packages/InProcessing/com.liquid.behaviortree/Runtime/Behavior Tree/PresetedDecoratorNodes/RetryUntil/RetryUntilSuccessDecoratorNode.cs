namespace Liquid.BehaviorTree
{
    public class RetryUntilSuccessDecoratorNode : RetryUntilDecoratorNode
    {
        public RetryUntilSuccessDecoratorNode(BehaviorTree tree)
            : base(tree, true) {}
    }
}