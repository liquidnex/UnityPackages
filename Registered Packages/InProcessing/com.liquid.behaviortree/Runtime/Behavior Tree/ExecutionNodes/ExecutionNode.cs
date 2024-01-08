namespace Liquid.BehaviorTree
{
    public abstract class ExecutionNode : Node
    {
        public ExecutionNode(BehaviorTree tree)
            : base(tree) {}

        public abstract void Tick();

        public override bool AddChildNode(Node node)
        {
            return false;
        }

        public override NodeResult Result
        {
            get => tmpResult;
        }

        protected NodeResult tmpResult = NodeResult.NONE;
    }
}