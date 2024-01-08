using System;

namespace Liquid.AStarToolkit
{
    /// <summary>
    /// Inherit the AStarNode class to define your custom node type for A* algorithm.
    /// </summary>
    public abstract class AStarNode: IComparable
    {
        public abstract AStarNode[] GetHeibos();

        protected abstract double Distance(AStarNode n);

        public AStarNode(
            string theNodeID,
            double initWeight
        )
        {
            nodeID = theNodeID;
            weight = initWeight;
        }

        public void Init(AStarCore core)
        {
            f = 0;
            g = 0;
            h = 0;
            parent = null;
            astarCore = core;
        }

        public void SetParent(AStarNode n)
        {
            if (n == null)
                return;

            parent = n;
            CalculateF();
        }

        public void TrySwitchParent(AStarNode newParent)
        {
            double newG = CalculateG(weight, newParent);
            if (newG < g)
            {
                SetParent(newParent);
            }
        }

        public double CalculateF()
        {
            if (astarCore.EndNode == null)
                return 0;

            g = CalculateG(0, this);
            h = Distance(astarCore.EndNode);
            f = g + h;
            return f;
        }

        public int CompareTo(object obj)
        {
            if (obj is AStarNode other)
            {
                if (f < other.f)
                    return -1;
                else if (f == other.f)
                    return 0;
                else if (f > other.f)
                    return 1;
            }
            return 0;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(obj, null)) return false;

            if (ReferenceEquals(obj, this)) return true;

            if (obj.GetType() != this.GetType()) return false;

            AStarNode c = (AStarNode)obj;
            return Equals(c);
        }

        public bool Equals(AStarNode other)
        {
            if (ReferenceEquals(other, null)) return false;
            if (ReferenceEquals(other, this)) return true;
            if (GetHashCode() != other.GetHashCode()) return false;
            if (nodeID != other.nodeID) return false;
            return true;
        }

        public override int GetHashCode()
        {
            return nodeID.GetHashCode();
        }

        public static bool operator ==(AStarNode lhs, AStarNode rhs)
        {
            if ((object)lhs == null && (object)rhs == null)
                return true;
            else if ((object)lhs == null || (object)rhs == null)
                return false;
            else
                return lhs.Equals(rhs);
        }

        public static bool operator !=(AStarNode lhs, AStarNode rhs)
        {
            return !(lhs == rhs);
        }

        private double CalculateG(double g, AStarNode node)
        {
            if (node.parent == null)
                return g;
            else
            {
                g += node.weight;
                return CalculateG(g, node.parent);
            }
        }

        public double Weight
        {
            get => weight;
        }

        public AStarNode Parent
        {
            get => parent;
        }

        protected readonly string nodeID;

        private double f = 0;
        private double g = 0;
        private double h = 0;
        private readonly double weight = 0;

        protected AStarCore astarCore;
        private AStarNode parent;
    }
}