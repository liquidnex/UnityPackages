using System;
using System.Reflection;

namespace Liquid.SkillSystem
{
    /// <summary>
    /// An effect represents a change to the attributable object.
    /// </summary>
    public abstract class Effect : SkillSystemItem
    {
        /// <summary>
        /// Create a effect in factory.
        /// </summary>
        /// <param name="typeName">
        /// Describes the type of effect to create.
        /// The parameter "typeName" can be the type name qualified by its namespace,
        /// or an assembly-qualified name that includes an assembly name specification(AssemblyQualifiedName).
        /// </param>
        /// <param name="args">Construction parameters of effect to create.</param>
        /// <returns>Created effect.</returns>
        public static Effect Create(string typeName, params object[] args)
        {
            return SkillSystemManager.Instance.Create(typeName, args) as Effect;
        }

        /// <summary>
        /// Create a effect in factory.
        /// </summary>
        /// <param name="asm">The assembly for the lookup of type name.</param>
        /// <param name="typeName">
        /// Describes the type of effect to create.
        /// The parameter "typeName" should be the type name qualified by the "asm".
        /// </param>
        /// <param name="args">Construction parameters of effect to create.</param>
        /// <returns>Created effect.</returns>
        public static Effect Create(Assembly asm, string typeName, params object[] args)
        {
            return SkillSystemManager.Instance.Create(asm, typeName, args) as Effect;
        }

        /// <summary>
        /// Create an effect in factory.
        /// </summary>
        /// <param name="objects">Parameters for constructor.</param>
        /// <returns>Created effect.</returns>
        public static T Create<T>(params object[] objects)
            where T : Effect
        {
            return SkillSystemManager.Instance.Create<T>(objects);
        }

        /// <summary>
        /// Create a effect in factory.
        /// </summary>
        /// <param name="type">The type of effect to create.</param>
        /// <param name="args">Construction parameters of effect to create.</param>
        /// <returns>Created effect.</returns>
        public static Effect Create(Type type, params object[] args)
        {
            return SkillSystemManager.Instance.Create(type, args) as Effect;
        }

        /// <summary>
        /// Destroy current effect from factory then remove it from related effect handler.
        /// </summary>
        public void Destroy()
        {
            SkillSystemManager.Instance.Remove(this);
        }

        /// <summary>
        /// Determine whether the given type is the target type of this effect.
        /// </summary>
        /// <param name="t">Attributable type.</param>
        /// <returns>
        /// Returns true if the types match, otherwise false.
        /// </returns>
        public abstract bool IsMatched(Type t);

        /// <summary>
        /// Apply effect to solve the value by change it.
        /// </summary>
        /// <typeparam name="T">Attributable type.</typeparam>
        /// <param name="value">Attributable object to change.</param>
        public abstract void Solve<T>(ref T value);

        /// <summary>
        /// Create an effect.
        /// </summary>
        /// <param name="buff">Owner buff of current effect.</param>
        /// <param name="handler">Owner effect handler of current effect.</param>
        protected Effect(Buff buff, IEffectHandler handler)
        {
            masterBuff = buff;
            masterHandler = handler;
        }

        protected virtual void OnProductCreate()
        {
            if (masterHandler != null)
                masterHandler.AddEffect(this);
        }

        protected virtual void OnProductDestroy()
        {
            if (masterHandler != null)
                masterHandler.RemoveEffect(this);
        }

        /// <summary>
        /// Owner buff.
        /// </summary>
        public Buff MasterBuff
        {
            get => masterBuff;
        }

        protected Buff masterBuff;
        private IEffectHandler masterHandler;
    }
}