using System;
using System.Collections.Generic;

namespace Liquid.BehaviorTree
{
    public abstract class ControlNode : Node
    {
        public enum InterruptMode
        {
            NONE,
            SELF,
            LOW_PRIORITY,
            BOTH
        }

        public ControlNode(BehaviorTree tree, InterruptMode im)
            : base(tree) 
        {
            Interrupter = im;
        }

        public virtual Node GetNext()
        {
            List<Node> childs = ChildNodes;
            int i = childs.FindIndex(c =>
                c.Result == NodeResult.NONE ||
                c.Result == NodeResult.RUNNING);

            if (i < 0 ||
                i >= childs.Count)
                return null;

            return childs[i];
        }

        protected void StopChilds()
        {
            if (behaviorTree == null)
                return;

            List<Node> childs = ChildNodes;
            List<Node> runningChilds = childs.FindAll(c => c.Result == NodeResult.RUNNING);

            foreach (Node c in runningChilds)
            {
                if (c is ActionNode actionNode)
                    actionNode.Stop();
            }
        }

        protected List<NodeResult> ChildResults
        {
            get
            {
                List<NodeResult> l = new List<NodeResult>();
                foreach (Node n in ChildNodes)
                {
                    if (n == null)
                        continue;

                    l.Add(n.Result);
                }
                return l;
            }
        }

        public readonly InterruptMode Interrupter;
    }
}