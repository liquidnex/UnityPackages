using System.Collections.Generic;

namespace Liquid.SkillSystem
{
    /// <summary>
    /// A directed acyclic graph describing the coverage relationship between buffs.
    /// </summary>
    public class BuffsDAG
    {
        private class DAGBuffElem
        {
            public DAGBuffElem(Buff b)
            {
                InDegreeNames = b.InputPortNames;
                OutDegreeNames = b.OutputPortNames;
                Buff = b;
            }

            public bool IsIsolated
            {
                get
                {
                    if (InDegreeNames.Count == 0 &&
                        OutDegreeNames.Count == 0)
                        return true;
                    else
                        return false;
                }
            }

            public List<string> InDegreeNames;
            public List<string> OutDegreeNames;
            public Buff Buff;
        }

        /// <summary>
        /// Construct directed acyclic graph from buff list.
        /// </summary>
        /// <param name="input">Initial buffs.</param>
        public BuffsDAG(List<Buff> input)
        {
            foreach (Buff b in input)
            {
                if (b != null)
                    buffs.Add(new DAGBuffElem(b));
            }

            for (int i = 0; i < buffs.Count; ++i)
            {
                DAGBuffElem e = buffs[i];

                e.InDegreeNames.RemoveAll(s => CountBuffByName(s) == 0);
                e.OutDegreeNames.RemoveAll(s => CountBuffByName(s) == 0);
            }
        }

        /// <summary>
        /// Get all activate buffs in current directed acyclic graph.
        /// </summary>
        /// <returns>Activated buffs.</returns>
        public List<Buff> GetActiveBuffs()
        {
            List<Buff> activedBuffList = new List<Buff>();
            Dictionary<string, List<DAGBuffElem>> DAGGroups = new Dictionary<string, List<DAGBuffElem>>();

            foreach (DAGBuffElem e in buffs)
            {
                if (e.IsIsolated)
                {
                    activedBuffList.Add(e.Buff);
                }
                else
                {
                    string groupName = e.Buff.DAGGroupName;
                    if (groupName == null ||
                        (groupName != null &&
                         !DAGGroups.ContainsKey(groupName)))
                    {
                        DAGGroups.Add(groupName, new List<DAGBuffElem> { e });
                    }
                    else if (groupName != null &&
                            DAGGroups.ContainsKey(groupName))
                    {
                        DAGGroups[groupName].Add(e);
                    }
                }
            }

            foreach (string key in DAGGroups.Keys)
            {
                List<DAGBuffElem> elemList = DAGGroups[key];
                DAGGroups[key] = TopologicalSorting(elemList);
            }

            foreach (List<DAGBuffElem> value in DAGGroups.Values)
            {
                if (value != null && value.Count > 0)
                    activedBuffList.Add(value[0].Buff);
            }

            return activedBuffList;
        }

        private List<DAGBuffElem> TopologicalSorting(List<DAGBuffElem> elems)
        {
            List<DAGBuffElem> elemPool = elems;
            List<DAGBuffElem> sortedElems = new List<DAGBuffElem>();

            while (elemPool.Count > 0)
            {
                DAGBuffElem e = elemPool.Find(i => i.InDegreeNames.Count == 0);
                if (e == null)
                    break;

                foreach (string n in e.OutDegreeNames)
                {
                    List<DAGBuffElem> childs = elemPool.FindAll(i => i.Buff.Name == n);
                    for (int i = 0; i < childs.Count; ++i)
                    {
                        childs[i].InDegreeNames.Remove(e.Buff.Name);
                    }
                }
                e.OutDegreeNames.Clear();

                elemPool.Remove(e);
                if (!sortedElems.Contains(e))
                    sortedElems.Add(e);
            }

            return sortedElems;
        }

        private int CountBuffByName(string buffName)
        {
            int count = 0;
            foreach (DAGBuffElem e in buffs)
            {
                if (e.Buff.Name == buffName)
                {
                    ++count;
                }
            }

            return count;
        }

        private List<DAGBuffElem> buffs = new List<DAGBuffElem>();
    }
}