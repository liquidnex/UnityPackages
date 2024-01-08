using System.Collections.Generic;

namespace Liquid.AStarToolkit
{
    /// <summary>
    /// Pathfinding core type.
    /// </summary>
    public class AStarCore
    {
        /// <summary>
        /// Init A* algorithm core with a map.
        /// </summary>
        /// <param name="nodeMap">A node map.</param>
        public void Init<T>(List<T> nodeMap)
            where T: AStarNode
        {
            List<AStarNode> baseNodeMap = 
                nodeMap.ConvertAll<AStarNode>(
                    n => n
                );
            map = baseNodeMap;
        }

        /// <summary>
        /// Do pathfinding.
        /// </summary>
        /// <param name="theStartNode">Start node for pathfinding.</param>
        /// <param name="theEndNode">End node for pathfinding.</param>
        /// <param name="path">Result path.</param>
        /// <returns>Determine if a suitable path has been found.</returns>
        public bool FindPath(
            AStarNode theStartNode, 
            AStarNode theEndNode, 
            out AStarNode[] path
        )
        {
            foreach (var e in map)
            {
                e.Init(this);
            }

            if (theStartNode != null &&
                theEndNode != null &&
                map.Contains(theStartNode) &&
                map.Contains(theEndNode)
            )
            {
                endNode = theEndNode;
                nowNode = null;

                openList.Clear();
                theStartNode.CalculateF();
                openList.Add(theStartNode);
                closeList.Clear();

                do
                {
                    openList.Sort();
                    nowNode = openList[0];
                    MoveNowNodeFromOpen2Close();

                    List<AStarNode> neibos = new List<AStarNode>(nowNode.GetHeibos());
                    foreach (var neibo in neibos)
                    {
                        if (neibo == null)
                            continue;

                        if (neibo.Weight <= 0)
                            continue;
                        if (closeList.FindIndex(n => n == neibo) != -1)
                            continue;

                        if (openList.FindIndex(n => n == neibo) == -1)
                        {
                            openList.Add(neibo);
                            neibo.SetParent(nowNode);
                        }
                        else
                        {
                            neibo.TrySwitchParent(nowNode);
                        }
                    }
                }
                while (GetPathFindingState() == PathFindingStatus.PROCESSING);

                if (GetPathFindingState() == PathFindingStatus.END_WITH_PATH)
                {
                    path = ArrangePath().ToArray();
                    return true;
                }
            }
            path = new List<AStarNode>().ToArray();
            return false;
        }

        private void MoveNowNodeFromOpen2Close()
        {
            int idx = openList.FindIndex(n => n == nowNode);
            if (idx == -1)
                return;
            openList.RemoveAt(idx);
            closeList.Add(nowNode);
        }

        private List<AStarNode> ArrangePath(List<AStarNode> path = null, AStarNode nowNode = null)
        {
            if (path == null)
                path = new List<AStarNode>();
            if (nowNode == null)
                nowNode = endNode;

            path.Add(nowNode);
            if (nowNode.Parent == null)
            {
                path.Reverse();
                return path;
            }
            else
                return ArrangePath(path, nowNode.Parent);
        }

        private enum PathFindingStatus
        {
            PROCESSING,
            END_WITH_PATH,
            END_WITH_NO_PATH
        }

        private PathFindingStatus GetPathFindingState()
        {
            if (closeList.FindIndex(n => n == endNode) != -1)
                return PathFindingStatus.END_WITH_PATH;
            else if (openList.Count == 0)
                return PathFindingStatus.END_WITH_NO_PATH;
            else
                return PathFindingStatus.PROCESSING;
        }

        public AStarNode EndNode
        {
            get => endNode;
        }

        public List<AStarNode> Map
        {
            get => map;
        }

        private AStarNode nowNode;
        private AStarNode endNode;
        private List<AStarNode> openList = new List<AStarNode>();
        private List<AStarNode> closeList = new List<AStarNode>();
        private List<AStarNode> map = new List<AStarNode>();
    }
}