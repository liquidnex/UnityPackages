using System.Collections.Generic;
using UnityEngine;

namespace Liquid.AStarToolkit
{
    /// <summary>
    /// Pre implementation of a hexagonal basic tile.
    /// </summary>
    public class HexAStarNode : AStarNode
    {
        public HexAStarNode(
            int x,
            int y,
            int z,
            double weight
        ):base(
            x+","+y+","+z, 
            weight
        )
        {
            idx.x = x;
            idx.y = y;
            idx.z = z;
        }

        public override AStarNode[] GetHeibos()
        {
            Vector3Int[] neiboIdxOffsets = new Vector3Int[]{
                new Vector3Int(+1, -1, 0),
                new Vector3Int(+1, 0, -1),
                new Vector3Int(0, +1, -1),
                new Vector3Int(-1, +1, 0),
                new Vector3Int(-1, 0, +1),
                new Vector3Int(0, -1, +1)
            };

            List<Vector3Int> neiboIdxs = new List<Vector3Int>();
            foreach (var offset in neiboIdxOffsets)
            {
                Vector3Int neibo = 
                    new Vector3Int(
                        idx.x + offset.x,
                        idx.y + offset.y,
                        idx.z + offset.z
                    );
                neiboIdxs.Add(neibo);
            }

            return astarCore.Map.FindAll(
                n =>
                {
                    if (n is HexAStarNode hexNode)
                    {
                        if (neiboIdxs.Contains(hexNode.idx))
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
            ).ToArray();
        }

        protected override double Distance(AStarNode n)
        {
            if (n is HexAStarNode hexNode)
            {
                int[] deltas =
                {
                    Mathf.Abs(idx.x - hexNode.idx.x),
                    Mathf.Abs(idx.y - hexNode.idx.y),
                    Mathf.Abs(idx.z - hexNode.idx.z)
                };
                return Mathf.Max(deltas);
            }

            return 0;
        }

        public Vector3Int Idx
        {
            get => idx;
        }

        private Vector3Int idx = Vector3Int.zero; 
    }
}