namespace Liquid.BehaviorTree
{
    public class ForceRaiseFailureDecoratorNode : ForceRaiseDecoratorNode
    {
        public ForceRaiseFailureDecoratorNode(BehaviorTree tree)
            : base(tree, false) {}
    }
}