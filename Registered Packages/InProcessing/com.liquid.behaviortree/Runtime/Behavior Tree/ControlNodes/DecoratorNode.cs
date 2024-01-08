namespace Liquid.BehaviorTree
{
    public abstract class DecoratorNode : ControlNode
    {
        public DecoratorNode(BehaviorTree tree)
            : base(tree, InterruptMode.NONE) {}

        public override bool AddChildNode(Node node)
        {
            if (ChildNodes.Count == 0)
                return base.AddChildNode(node);
            else
                return false;
        }
    }
}