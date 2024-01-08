using System;
using System.Collections.Generic;

namespace Liquid.SkillSystem
{
    /// <summary>
    /// Buff supported fields.
    /// The buff field is realized through the backup eigenvalue system and the effects of continuous superposition calculation.
    /// </summary>
    /// <typeparam name="T">Attributable object.</typeparam>
    /// <typeparam name="Attr">The characteristic type.</typeparam>
    public abstract class BuffableField<T, Attr> : IEffectHandler
        where T : IAttributable<Attr>
    {
        /// <summary>
        /// Construct a buffable field from the initial value.
        /// </summary>
        /// <param name="init">Initial value.</param>
        public BuffableField(T init)
        {
            originalAttr = init.Extract();
            value = init;
        }

        /// <summary>
        /// Judge whether the current handler contains specific effects.
        /// </summary>
        /// <param name="e">The specific effect.</param>
        /// <returns>
        /// Returns true if current handler contains the specific efffects,
        /// otherwise returns false.
        /// </returns>
        public bool Contains(Effect e)
        {
            return effectsDock.Effects.Contains(e);
        }

        /// <summary>
        /// Give an effect to the handler to manage.
        /// </summary>
        /// <param name="e">Effect to add.</param>
        public void AddEffect(Effect e)
        {
            if (e.IsMatched(typeof(T)))
            {
                effectsDock.Effects.Add(e);
                UpdateValueByEffects();
            }
        }

        /// <summary>
        /// Remove an exist effect from handler.
        /// </summary>
        /// <param name="e">Effect to remove.</param>
        public void RemoveEffect(Effect e)
        {
            effectsDock.Effects.Remove(e);
            UpdateValueByEffects();
        }

        /// <summary>
        /// Remove all effects related to the specified buff.
        /// </summary>
        /// <param name="buff">Buff.</param>
        public void RemoveEffects(Buff buff)
        {
            effectsDock.Effects.RemoveAll(e => e.MasterBuff == buff);
            UpdateValueByEffects();
        }

        /// <summary>
        /// Remove all effects.
        /// </summary>
        public void RemoveEffects()
        {
            effectsDock.Effects.Clear();
            UpdateValueByEffects();
        }

        /// <summary>
        /// Get all buffs that effect depends on.
        /// </summary>
        /// <returns>Related buff list.</returns>
        public List<Buff> GetBuffs()
        {
            return effectsDock.GetRelatedBuffs();
        }

        protected void UpdateValueByEffects()
        {
            List<Effect> activeEffects = effectsDock.GetActiveEffects();
            ResetValue();
            foreach (Effect e in activeEffects)
            {
                e.Solve(ref value);
            }

            if (OnValueChangedCallback != null)
                OnValueChangedCallback();
        }

        protected void ResetValue()
        {
            ApplyAttr(originalAttr);
        }

        protected abstract void ApplyAttr(Attr v);

        /// <summary>
        /// Get current value of this buffable field.
        /// </summary>
        public T Value
        {
            get => value;
        }

        /// <summary>
        /// Get original attribute value of this buffable field.
        /// </summary>
        public Attr OriginalAttr
        {
            get => originalAttr;
            set 
            {
                originalAttr = value;
                UpdateValueByEffects();
            }
        }

        /// <summary>
        /// Callback when value changes.
        /// </summary>
        public event Action OnValueChangedCallback;

        protected T value;
        protected Attr originalAttr;
        protected EffectsDock effectsDock = new EffectsDock();
    }
}