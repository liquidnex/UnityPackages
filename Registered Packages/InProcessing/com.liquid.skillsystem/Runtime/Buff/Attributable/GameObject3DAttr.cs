using UnityEngine;

namespace Liquid.SkillSystem
{
    /// <summary>
    /// Preset attribute dataset prepared for unity 3D game object.
    /// </summary>
    public struct GameObject3DAttr
    {
        public GameObject3DAttr(GameObject obj)
        {
            GameObjectName = obj.name;
            Layer = obj.layer;
            ActiveSelf = obj.activeSelf;

            TransformName = obj.transform.name;
            TransformTag = obj.transform.tag;
            Parent = obj.transform.parent;
            SiblingIndex = obj.transform.GetSiblingIndex();
            Position = obj.transform.position;
            Rotation = obj.transform.rotation;
            LocalPosition = obj.transform.localPosition;
            LocalRotation = obj.transform.localRotation;
            LocalScale = obj.transform.localScale;

            Renderer renderer = obj.GetComponent<Renderer>();
            if (renderer != null)
            {
                RendererName = renderer.name;
                RendererEnabled = renderer.enabled;
                RendererMaterials = renderer.materials;
                RendererSharedMaterials = renderer.sharedMaterials;
            }
            else
            {
                RendererName = null;
                RendererEnabled = true;
                RendererMaterials = null;
                RendererSharedMaterials = null;
            }
        }

        public string GameObjectName;
        public int Layer;
        public bool ActiveSelf;

        public string TransformName;
        public string TransformTag;
        public Transform Parent;
        public int SiblingIndex;
        public Vector3 Position;
        public Quaternion Rotation;
        public Vector3 LocalPosition;
        public Quaternion LocalRotation;
        public Vector3 LocalScale;
        public string RendererName;
        public bool RendererEnabled;
        public Material[] RendererMaterials;
        public Material[] RendererSharedMaterials;
    }
}