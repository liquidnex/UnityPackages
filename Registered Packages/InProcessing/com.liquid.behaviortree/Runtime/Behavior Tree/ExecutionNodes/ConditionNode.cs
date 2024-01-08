namespace Liquid.BehaviorTree
{
    public abstract class ConditionNode : ExecutionNode
    {
        public ConditionNode(BehaviorTree tree)
            : base(tree) {}

        public override void Tick()
        {
            if (Execute())
                tmpResult = NodeResult.SUCCESS;
            else
                tmpResult = NodeResult.FAILURE;
        }

        protected abstract bool Execute();
    }
}