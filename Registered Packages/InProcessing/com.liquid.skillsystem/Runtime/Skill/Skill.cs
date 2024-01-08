using Liquid.CommonUtils;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Liquid.SkillSystem
{
    /// <summary>
    /// Manage buffs and adjust data status.
    /// </summary>
    public abstract class Skill : SkillSystemItem
    {
        /// <summary>
        /// Create a skill in factory.
        /// </summary>
        /// <param name="typeName">
        /// Describes the type of skill to create.
        /// The parameter "typeName" can be the type name qualified by its namespace,
        /// or an assembly-qualified name that includes an assembly name specification(AssemblyQualifiedName).
        /// </param>
        /// <param name="args">Construction parameters of skill to create.</param>
        /// <returns>Created skill.</returns>
        public static Skill Create(string typeName, params object[] args)
        {
            return SkillSystemManager.Instance.Create(typeName, args) as Skill;
        }

        /// <summary>
        /// Create a skill in factory.
        /// </summary>
        /// <param name="asm">The assembly for the lookup of type name.</param>
        /// <param name="typeName">
        /// Describes the type of skill to create.
        /// The parameter "typeName" should be the type name qualified by the "asm".
        /// </param>
        /// <param name="args">Construction parameters of skill to create.</param>
        /// <returns>Created skill.</returns>
        public static Skill Create(Assembly asm, string typeName, params object[] args)
        {
            return SkillSystemManager.Instance.Create(asm, typeName, args) as Skill;
        }

        /// <summary>
        /// Create a skill in factory.
        /// </summary>
        /// <param name="args">Parameters for constructor.</param>
        /// <returns>Created skill.</returns>
        public static T Create<T>(params object[] args)
            where T : Skill
        {
            return SkillSystemManager.Instance.Create<T>(args);
        }

        /// <summary>
        /// Create a skill in factory.
        /// </summary>
        /// <param name="type">The type of skill to create.</param>
        /// <param name="args">Construction parameters of skill to create.</param>
        /// <returns>Created skill.</returns>
        public static Skill Create(Type type, params object[] args)
        {
            return SkillSystemManager.Instance.Create(type, args) as Skill;
        }

        /// <summary>
        /// Destroy current skill from factory then remove all buffs managed by this buff.
        /// </summary>
        public void Destroy()
        {
            SkillSystemManager.Instance.Remove(this);
        }

        /// <summary>
        /// Determine whether the skill can be released manually.
        /// </summary>
        /// <returns>
        /// Returns true if the skill can be release manually, otherwise returns false.
        /// </returns>
        public virtual bool IsManualReleaseEnabled()
        { 
            return GetCD() >= 1f;
        }

        /// <summary>
        /// Release the skill, do something and generate buffs here.
        /// </summary>
        public abstract void Release();

        /// <summary>
        /// Get cooldown progress value of the skill.
        /// The value range form 0 to 1.
        /// </summary>
        /// <returns>Cooldown progress value.</returns>
        public abstract float GetCD();

        /// <summary>
        /// Reset cooldown progress.
        /// </summary>
        public virtual void ResetCD() {}

        protected Skill() {}

        /// <summary>
        /// Buff field members of the skill will be destroyed automatically.
        /// </summary>
        protected virtual void OnProductDestroy()
        {
            List<Buff> buffs = TypeUtil.GetFields<Skill, Buff>(this, true, true);
            foreach (Buff b in buffs)
            {
                if(b == null)
                   continue;

                b.Destroy();
            }
        }
    }
}