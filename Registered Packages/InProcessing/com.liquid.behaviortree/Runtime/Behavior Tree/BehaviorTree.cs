using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Liquid.BehaviorTree
{
    public class BehaviorTree : MonoBehaviour
    {
        private enum TreeState
        {
            NONE,
            WORKING
        }

        public bool SetRoot(Node node)
        {
            if (node == null ||
                node.ParentNode != null ||
                !(node is ControlNode controlNode))
                return false;

            SetNodesDirty();
            rootNode = controlNode;
            return true;
        }

        public void Launch(float tickIntervalSec = 0)
        {
            tickTime = 0;
            SetTickInterval(tickIntervalSec);
            Tick();
        }

        public void Clear()
        {
            tickTime = 0;
            ResetAllNodes();
            rootNode = null;
            tmpNodes.Clear();
            nodesDirtyFlag = false;
            state = TreeState.NONE;
        }

        internal void SetNodesDirty()
        {
            nodesDirtyFlag = true;
        }

        internal void UnregisterRunningNode(ActionNode runningNode)
        {
            if (runningNode == null)
                return;

            if (runningNodes.Contains(runningNode))
                runningNodes.Remove(runningNode);
        }

        private void Tick()
        {
            if (rootNode == null)
                return;

            if (state == TreeState.WORKING)
                return;
            state = TreeState.WORKING;

            if (runningNodes.Count > 0)
                TryInterrupt();

            if (runningNodes.Count == 0)
            {
                ResetAllNodes();
                traversingPath.Push(rootNode);
            }
            Traverse();
        }

        private void Traverse()
        {
            Node curNode;
            while ((curNode = GetNext()) != null)
            {
                if (curNode is ControlNode controlNode)
                {
                    if (!traversingPath.Contains(controlNode))
                        traversingPath.Push(controlNode);
                }
                else if (curNode is ExecutionNode executionNode)
                {
                    executionNode.Tick();

                    NodeResult result = executionNode.Result;
                    if (result == NodeResult.RUNNING)
                    {
                        if (executionNode is ActionNode actionNode)
                        {
                            RegisterRunningNode(actionNode);
                            break;
                        }
                    }

                    SolveTop();
                }
            }

            state = TreeState.NONE;
        }

        private Node GetNext()
        {
            while (traversingPath.Count != 0)
            {
                ControlNode top = traversingPath.Peek();
                if (top == null)
                {
                    traversingPath.Pop();
                    continue;
                }

                Node next = top.GetNext();
                if (next == null)
                {
                    traversingPath.Pop();
                    continue;
                }
                return next;
            }
            return null;
        }

        private void SolveTop()
        {
            if (!traversingPath.TryPeek(out ControlNode controlNode))
                return;

            NodeResult r = controlNode.Result;
            if (r == NodeResult.FAILURE ||
                r == NodeResult.SUCCESS)
            {
                traversingPath.Pop();
                SolveTop();
            }
        }

        private void ResetAllNodes()
        {
            List<ActionNode> nodeList = new List<ActionNode>(runningNodes.ToArray());
            foreach (ActionNode n in nodeList)
            {
                n.Stop();
            }
            runningNodes.Clear();

            List<Node> nodes = Nodes;
            foreach (Node n in nodes)
            {
                n.Reset();
            }

            traversingPath.Clear();
        }

        private void TryInterrupt()
        {

        }

        private void RegisterRunningNode(ActionNode runningNode)
        {
            if (runningNode == null)
                return;

            if (!runningNodes.Contains(runningNode))
                runningNodes.Add(runningNode);
        }

        private void SetTickInterval(float tickIntervalSec)
        {
            if (tickIntervalSec >= minTickInterval)
                tickInterval = tickIntervalSec;
        }

        private void Update()
        {
            if (tickInterval < minTickInterval)
                return;

            if (tickTime < tickInterval)
                tickTime += Time.deltaTime;
            else
            {
                tickTime = 0;
                Tick();
            }
        }

        public List<Node> Nodes
        {
            get
            {
                if (rootNode == null)
                    return new List<Node>();

                if (!nodesDirtyFlag &&
                    tmpNodes != null)
                    return new List<Node>(tmpNodes.ToArray());
                
                tmpNodes = new List<Node>();
                tmpNodes.Add(rootNode);
                tmpNodes.AddRange(rootNode.DescendantNodes);
                tmpNodes = tmpNodes.Distinct().ToList();
                nodesDirtyFlag = false;

                return new List<Node>(tmpNodes.ToArray());
            }
        }

        protected ControlNode rootNode;

        private TreeState state = TreeState.NONE;
        private Stack<ControlNode> traversingPath = new Stack<ControlNode>();
        private List<ActionNode> runningNodes = new List<ActionNode>();

        private bool nodesDirtyFlag = false;
        private List<Node> tmpNodes = new List<Node>();
        private float tickTime = 0f;
        private float tickInterval = 0f;
        private const float minTickInterval = 0.1f;
    }
}