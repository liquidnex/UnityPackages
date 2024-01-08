using System.Collections.Generic;

namespace Liquid.SkillSystem
{
    /// <summary>
    /// Storing effects and arrange the relationship between the buffs they belong to.
    /// </summary>
    public class EffectsDock
    {
        /// <summary>
        /// Get the list of activated buffs.
        /// </summary>
        /// <returns>Activated buffs.</returns>
        public List<Effect> GetActiveEffects()
        {
            List<Buff> buffs = GetRelatedBuffs();
            BuffsDAG buffsDAG = new BuffsDAG(buffs);
            List<Buff> activeBuffs = buffsDAG.GetActiveBuffs();

            List<Effect> activeEffects = new List<Effect>();
            foreach (Effect e in Effects)
            {
                if(e == null ||
                   e.MasterBuff == null)
                   continue;

                if (activeBuffs.Contains(e.MasterBuff))
                {
                    activeEffects.Add(e);
                }
            }

            return activeEffects;
        }

        /// <summary>
        /// Get the buff list that all effects depend on (the duplicate has been removed).
        /// </summary>
        /// <returns>Related buffs.</returns>
        public List<Buff> GetRelatedBuffs()
        {
            List<Buff> buffs = new List<Buff>();
            foreach (Effect e in Effects)
            {
                if (e == null ||
                    e.MasterBuff == null)
                    continue;

                if (!buffs.Contains(e.MasterBuff))
                    buffs.Add(e.MasterBuff);
            }

            return buffs;
        }

        public List<Effect> Effects = new List<Effect>();
    }
}