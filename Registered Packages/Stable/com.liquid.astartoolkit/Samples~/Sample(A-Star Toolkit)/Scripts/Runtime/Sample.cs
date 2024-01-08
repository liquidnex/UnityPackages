using System.Collections.Generic;
using UnityEngine;

namespace Liquid.AStarToolkit.Samples.AStarToolkitSample
{
    public class Sample : MonoBehaviour
    {
        private void Awake()
        {
            Init();
        }

        private void Init()
        {
            // Test for AStar
            List<HexAStarNode> map = new List<HexAStarNode>();
            map.Add(new HexAStarNode(0, 2, -2, 1));
            map.Add(new HexAStarNode(1, 1, -2, 1));
            map.Add(new HexAStarNode(2, 0, -2, 1));
            map.Add(new HexAStarNode(-1, 2, -1, 1));
            map.Add(new HexAStarNode(0, 1, -1, 0));
            map.Add(new HexAStarNode(1, 0, -1, 0));
            map.Add(new HexAStarNode(2, -1, -1, 1));
            map.Add(new HexAStarNode(-2, 2, 0, 1));
            map.Add(new HexAStarNode(-1, 1, 0, 1));
            map.Add(new HexAStarNode(0, 0, 0, 1));
            map.Add(new HexAStarNode(1, -1, 0, 0));
            map.Add(new HexAStarNode(2, -2, 0, 1));
            map.Add(new HexAStarNode(-2, 1, 1, 1));
            map.Add(new HexAStarNode(-1, 0, 1, 0));
            map.Add(new HexAStarNode(0, -1, 1, 0));
            map.Add(new HexAStarNode(1, -2, 1, 1));
            map.Add(new HexAStarNode(-2, 0, 2, 1));
            map.Add(new HexAStarNode(-1, -1, 2, 1));
            map.Add(new HexAStarNode(0, -2, 2, 1));

            HexAStarNode startNode = map.Find(n => n.Idx == new Vector3Int(-2, 2, 0));
            HexAStarNode endNode = map.Find(n => n.Idx == new Vector3Int(+2, -2, 0));

            AStarCore core = new AStarCore();
            core.Init(map);
            bool success = core.FindPath(startNode, endNode, out AStarNode[] path);
            if (success)
            {
                foreach (var p in path)
                {
                    HexAStarNode n = p as HexAStarNode;
                    Debug.Log(n.Idx);
                }
            }
        }
    }
}