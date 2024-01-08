using UnityEngine;

namespace Liquid.SkillSystem
{
    /// <summary>
    /// Preset game object type buffable field.
    /// </summary>
    public class BuffableFieldGameObject3D : BuffableField<AttributableGameObject3D, GameObject3DAttr>
    {
        /// <summary>
        /// Construct buffable field by a game object value.
        /// </summary>
        /// <param name="initValue">Initial value.</param>
        public BuffableFieldGameObject3D(GameObject initValue)
            : base(initValue) {}

        /// <summary>
        /// Apply a attribute to the value.
        /// </summary>
        /// <param name="v">Attribute value.</param>
        protected override void ApplyAttr(GameObject3DAttr v)
        {
            if (value == null)
                return;

            GameObject valueAsGameObject = value;

            valueAsGameObject.name = v.GameObjectName;
            valueAsGameObject.layer = v.Layer;
            valueAsGameObject.SetActive(v.ActiveSelf);

            valueAsGameObject.transform.name = v.TransformName;
            valueAsGameObject.transform.tag = v.TransformTag;
            valueAsGameObject.transform.SetParent(v.Parent);
            valueAsGameObject.transform.SetSiblingIndex(v.SiblingIndex);
            valueAsGameObject.transform.position = v.Position;
            valueAsGameObject.transform.rotation = v.Rotation;
            valueAsGameObject.transform.localPosition = v.LocalPosition;
            valueAsGameObject.transform.localRotation = v.LocalRotation;
            valueAsGameObject.transform.localScale = v.LocalScale;

            Renderer valueRenderer = valueAsGameObject.GetComponent<Renderer>();
            if (valueRenderer != null)
            {
                valueRenderer.name = v.RendererName;
                valueRenderer.enabled = v.RendererEnabled;
                valueRenderer.materials = v.RendererMaterials;
                valueRenderer.sharedMaterials = v.RendererSharedMaterials;
            }
        }

        public static implicit operator BuffableFieldGameObject3D(GameObject v)
        {
            return new BuffableFieldGameObject3D(v);
        }

        public static implicit operator GameObject(BuffableFieldGameObject3D one)
        {
            if (one == null)
                return null;
            return one.value;
        }
    }
}