using System.Collections.Generic;

namespace Liquid.SkillSystem
{
    /// <summary>
    /// Provide operation related to effect management.
    /// </summary>
    public interface IEffectHandler
    {
        /// <summary>
        /// Judge whether the current handler contains specific effects.
        /// </summary>
        /// <param name="e">The specific effect.</param>
        /// <returns>
        /// Returns true if current handler contains the specific efffects,
        /// otherwise returns false.
        /// </returns>
        public bool Contains(Effect e);

        /// <summary>
        /// Give an effect to the handler to manage.
        /// </summary>
        /// <param name="e">Effect to add.</param>
        public void AddEffect(Effect e);

        /// <summary>
        /// Remove an exist effect from handler.
        /// </summary>
        /// <param name="e">Effect to remove.</param>
        public void RemoveEffect(Effect e);

        /// <summary>
        /// Remove all effects related to the specified buff.
        /// </summary>
        /// <param name="buff">Buff.</param>
        public void RemoveEffects(Buff buff);

        /// <summary>
        /// Remove all effects.
        /// </summary>
        public void RemoveEffects();

        /// <summary>
        /// Get all buffs that effect depends on.
        /// </summary>
        /// <returns>Related buff list.</returns>
        public List<Buff> GetBuffs();
    }
}