using System;
using UnityEngine;

namespace Liquid.SkillSystem.Samples.SkillSystemSample
{
    public class EffectAddHealth : Effect
    {
        protected EffectAddHealth(double value, Buff buff, IEffectHandler handler)
            : base(buff, handler)
        {
            addHealthValue = value;
        }

        public override bool IsMatched(Type t)
        {
            if (t == typeof(AttributableStruct<double>))
                return true;
            return false;
        }

        public override void Solve<T>(ref T value)
        {
            if (value is AttributableStruct<double> v)
            {
                AttributableStruct<double> d = v + addHealthValue;
                value = (T)(object)d;
            }
        }

        private double addHealthValue;
    }

    public class BuffRemedy : Buff
    {
        protected BuffRemedy(Player p, double d)
            : base("BuffRemedy", null, null, null)
        {
            if (p != null)
            {
                e = Effect.Create<EffectAddHealth>(d, this, p.Health);
            }
        }

        private EffectAddHealth e;
    }

    public class SkillHealSelf : Skill
    {
        public SkillHealSelf(Player p, double v)
        {
            player = p;
            healValue = v;
        }

        public override float GetCD()
        {
            return 1f;
        }

        public override void Release()
        {
            if (player != null)
            {
                b = Buff.Create<BuffRemedy>(player, healValue);
            }
        }

        private Player player;
        private double healValue;
        private BuffRemedy b;
    }

    public class Player
    {
        public BuffableFieldStruct<double> Health = 3;
    }

    public class Sample : MonoBehaviour
    {
        private void Awake()
        {
            Init();
        }

        private void Init()
        {
            Player p = new Player();
            p.Health.OnValueChangedCallback += ()=>Debug.Log("On Health Value Changed.");

            Skill s = Skill.Create<SkillHealSelf>(p, 2);
            if(s.IsManualReleaseEnabled())
                s.Release();
            Debug.Log((double)p.Health);

            s.Destroy();
            Debug.Log((double)p.Health);
        }
    }
}