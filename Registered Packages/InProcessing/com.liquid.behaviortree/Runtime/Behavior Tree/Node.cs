using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;

namespace Liquid.BehaviorTree
{
    public enum NodeResult
    {
        NONE,
        SUCCESS,
        FAILURE,
        RUNNING
    }

    public abstract class Node
    {
        public Node(BehaviorTree tree)
        {
            if (behaviorTree == null)
                throw new ArgumentNullException(nameof(tree));

            behaviorTree = tree;
        }

        public abstract void Reset();

        public virtual bool AddChildNode(Node node)
        {
            if (node == null)
                return false;

            if (behaviorTree == null)
                return false;

            if (!IsLegalChildInPath(node))
                return false;

            if (!behaviorTree.Nodes.Contains(node))
            {
                behaviorTree.SetNodesDirty();
                childNodes.Add(node);
                nodesDirtyFlag = true;
            }

            node.parentNode = this;
            return true;
        }

        private List<Node> GetDescendantNodes(Node node)
        {
            if (node == null)
                return new List<Node>();

            if (!nodesDirtyFlag &&
                tmpDescendantNodes != null)
                return tmpDescendantNodes;

            tmpDescendantNodes = new List<Node>();
            foreach (Node n in node.ChildNodes)
            {
                if (n == null)
                    continue;

                tmpDescendantNodes.Add(n);
                tmpDescendantNodes.AddRange(GetDescendantNodes(n));
                tmpDescendantNodes = tmpDescendantNodes.Distinct().ToList();
            }
            return tmpDescendantNodes;
        }

        private bool IsLegalChildInPath(Node child)
        {
            if (child == null)
                return false;

            List<Node> path = Path;
            path.Add(child);

            string pathStr = "";
            foreach (Node n in path)
            {
                if (n == null)
                    continue;

                if (n is DecoratorNode)
                {
                    pathStr.Append('D');
                }
                else if (n is ControlNode controlNode)
                {
                    if (controlNode.Interrupter == ControlNode.InterruptMode.NONE)
                        pathStr.Append('N');
                    else
                        pathStr.Append('B');
                }
                else
                {
                    pathStr.Append('N');
                }
            }

            if (!pathStr.Contains('B'))
                return true;

            Regex rgx = new Regex(@"BD*[DN]$");
            Match match = rgx.Match(pathStr);
            if (match.Success)
                return true;
            return false;
        }

        public virtual NodeResult Result
        {
            get;
        }

        public int NodeDepth
        {
            get
            {
                int depth = 0;

                Node tmp = this;
                while (tmp.ParentNode != null)
                {
                    tmp = tmp.ParentNode;
                    ++depth;
                }

                return depth;
            }
        }

        public Node ParentNode
        {
            get => parentNode;
        }

        public List<Node> ChildNodes
        {
            get => new List<Node>(childNodes.ToArray());
        }

        public List<Node> DescendantNodes
        {
            get
            {
                if (!nodesDirtyFlag &&
                    tmpDescendantNodes != null)
                    return tmpDescendantNodes;

                tmpDescendantNodes = GetDescendantNodes(this);
                nodesDirtyFlag = false;

                return new List<Node>(tmpDescendantNodes);
            }
        }

        public List<Node> Path
        {
            get
            {
                List<Node> path = new List<Node>();

                Node tmp = this;
                do
                {
                    path.Add(tmp);
                    tmp = tmp.ParentNode;
                }
                while (tmp.ParentNode != null);

                path.Reverse();
                return path;
            }
        }

        protected BehaviorTree behaviorTree;

        private Node parentNode;
        private List<Node> childNodes = new List<Node>();

        private bool nodesDirtyFlag = false;
        private List<Node> tmpDescendantNodes = new List<Node>();
    }
}