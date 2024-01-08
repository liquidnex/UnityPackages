using Liquid.CommonUtils;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Liquid.SkillSystem
{
    /// <summary>
    /// A buff represents the manager of a series of temporary effects that can be dynamically added or removed for the target.
    /// </summary>
    public abstract class Buff : SkillSystemItem
    {
        /// <summary>
        /// Create a buff in factory.
        /// </summary>
        /// <param name="typeName">
        /// Describes the type of buff to create.
        /// The parameter "typeName" can be the type name qualified by its namespace,
        /// or an assembly-qualified name that includes an assembly name specification(AssemblyQualifiedName).
        /// </param>
        /// <param name="args">Construction parameters of buff to create.</param>
        /// <returns>Created buff.</returns>
        public static Buff Create(string typeName, params object[] args)
        {
            return SkillSystemManager.Instance.Create(typeName, args) as Buff;
        }

        /// <summary>
        /// Create a buff in factory.
        /// </summary>
        /// <param name="asm">The assembly for the lookup of type name.</param>
        /// <param name="typeName">
        /// Describes the type of buff to create.
        /// The parameter "typeName" should be the type name qualified by the "asm".
        /// </param>
        /// <param name="args">Construction parameters of buff to create.</param>
        /// <returns>Created buff.</returns>
        public static Buff Create(Assembly asm, string typeName, params object[] args)
        {
            return SkillSystemManager.Instance.Create(asm, typeName, args) as Buff;
        }

        /// <summary>
        /// Create a buff in factory.
        /// </summary>
        /// <param name="args">Parameters for constructor.</param>
        /// <returns>Created buff.</returns>
        public static T Create<T>(params object[] args)
            where T : Buff
        {
            return SkillSystemManager.Instance.Create<T>(args);
        }

        /// <summary>
        /// Create a buff in factory.
        /// </summary>
        /// <param name="type">The type of buff to create.</param>
        /// <param name="args">Construction parameters of buff to create.</param>
        /// <returns>Created buff.</returns>
        public static Buff Create(Type type, params object[] args)
        {
            return SkillSystemManager.Instance.Create(type, args) as Buff;
        }

        /// <summary>
        /// Destroy current effect from factory then remove all effects managed by this buff.
        /// </summary>
        public void Destroy()
        {
            SkillSystemManager.Instance.Remove(this);
        }

        /// <summary>
        /// Create a buff.
        /// </summary>
        /// <param name="buffName">Buff name.</param>
        /// <param name="groupName">Group name of the buff.</param>
        /// <param name="inputBuffNames">Input buff names.</param>
        /// <param name="outputBuffNames">Output buff names.</param>
        protected Buff(
            string buffName,
            string groupName,
            List<string> inputBuffNames,
            List<string> outputBuffNames)
        {
            Name = buffName;
            DAGGroupName = groupName;
            inputPortNames = inputBuffNames;
            if (inputPortNames == null)
                inputPortNames = new List<string>();
            outputPortNames = outputBuffNames;
            if (outputPortNames == null)
                outputPortNames = new List<string>();
        }

        /// <summary>
        /// Effect field members of the buff will be destroyed automatically.
        /// </summary>
        protected virtual void OnProductDestroy()
        {
            List<Effect> effects = TypeUtil.GetFields<Buff, Effect>(this, true, true);
            foreach (Effect e in effects)
            {
                if(e == null)
                   continue;

                e.Destroy();
            }
        }

        public List<string> InputPortNames
        {
            get => inputPortNames;
        }

        public List<string> OutputPortNames
        {
            get => outputPortNames;
        }

        public readonly string Name;
        public readonly string DAGGroupName;

        protected readonly List<string> inputPortNames = new List<string>();
        protected readonly List<string> outputPortNames = new List<string>();
    }
}