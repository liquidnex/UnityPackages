using UnityEngine;

namespace Liquid.SkillSystem
{
    /// <summary>
    /// Attributable type for unity 3D game object.
    /// </summary>
    public class AttributableGameObject3D : IAttributable<GameObject3DAttr>
    {
        /// <summary>
        /// Default conversion constructor.
        /// </summary>
        public AttributableGameObject3D(GameObject v)
        {
            value = v;
        }

        /// <summary>
        /// Access the attribute value.
        /// </summary>
        public GameObject3DAttr Extract()
        {
            return new GameObject3DAttr(value);
        }

        public static implicit operator AttributableGameObject3D(GameObject v)
        {
            return new AttributableGameObject3D(v);
        }

        public static implicit operator GameObject(AttributableGameObject3D one)
        {
            return one.value;
        }

        /// <summary>
        /// Access the original value.
        /// </summary>
        public GameObject GameObject3D
        {
            get => value;
        }

        protected GameObject value;
    }
}