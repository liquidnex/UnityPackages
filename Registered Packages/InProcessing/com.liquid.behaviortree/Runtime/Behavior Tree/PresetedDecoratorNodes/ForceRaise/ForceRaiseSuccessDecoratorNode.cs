namespace Liquid.BehaviorTree
{
    public class ForceSuccessDecoratorNode : ForceRaiseDecoratorNode
    {
        public ForceSuccessDecoratorNode(BehaviorTree tree)
            : base(tree, true) {}
    }
}