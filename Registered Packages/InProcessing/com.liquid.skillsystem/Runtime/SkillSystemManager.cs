using Liquid.CommonUtils;

namespace Liquid.SkillSystem
{
    /// <summary>
    /// Container for buffs and skills.
    /// </summary>
    public class SkillSystemManager : Factory<SkillSystemManager, SkillSystemItem>
    {
        private SkillSystemManager() {}

        /// <summary>
        /// Access interface of singleton mode object.
        /// </summary>
        public static SkillSystemManager Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syslock)
                    {
                        if (instance == null)
                        {
                            instance = new SkillSystemManager();
                        }
                    }
                }
                return instance;
            }
        }

        private static SkillSystemManager instance;
        private static readonly object syslock = new object();
    }
}