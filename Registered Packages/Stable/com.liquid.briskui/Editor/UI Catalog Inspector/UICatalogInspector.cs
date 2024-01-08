using UnityEngine;
using UnityEditor;

namespace Liquid.BriskUI.Editor
{
    [CustomEditor(typeof(UICatalog))]
    public class UICatalogInspector : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            if (target == null)
                return;
            UICatalog catalog = target as UICatalog;
            if (catalog == null)
                return;
            if (serializedObject == null)
                return;

            serializedObject.Update();

            SerializedProperty propertyKeyUITypeNames = serializedObject.FindProperty("UITypeNames");
            SerializedProperty propertyValueUILabels = serializedObject.FindProperty("UILabels");
            SerializedProperty propertyValueUIAddresses = serializedObject.FindProperty("UIAddresses");

            if (propertyKeyUITypeNames == null ||
                propertyValueUILabels == null ||
                propertyValueUIAddresses == null)
                return;

            if (propertyKeyUITypeNames.arraySize != propertyValueUILabels.arraySize ||
                propertyValueUILabels.arraySize != propertyValueUIAddresses.arraySize)
            {
                propertyKeyUITypeNames.ClearArray();
                propertyValueUILabels.ClearArray();
                propertyValueUIAddresses.ClearArray();
            }

            EditorGUILayout.LabelField("UI Catalog:");

            string needRemoveKey = null;
            for (int i = 0; i < propertyKeyUITypeNames.arraySize; ++i)
            {
                EditorGUILayout.BeginVertical("Badge");

                SerializedProperty keyUITypeName = propertyKeyUITypeNames.GetArrayElementAtIndex(i);
                SerializedProperty valueUILabel = propertyValueUILabels.GetArrayElementAtIndex(i);
                SerializedProperty valueUIAddress = propertyValueUIAddresses.GetArrayElementAtIndex(i);
                
                keyUITypeName.stringValue = EditorGUILayout.TextField("UI Type Name", keyUITypeName.stringValue);
                valueUILabel.stringValue = EditorGUILayout.TextField("UI Label", valueUILabel.stringValue);
                valueUIAddress.stringValue = EditorGUILayout.TextField("UI Address", valueUIAddress.stringValue);
                
                Color bc = GUI.backgroundColor;
                GUI.backgroundColor = Color.red;
                if (GUILayout.Button("Remove"))
                    needRemoveKey = keyUITypeName.stringValue;

                GUI.backgroundColor = bc;

                EditorGUILayout.EndVertical();
            }

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Add"))
            {
                catalog.Data.Add(GUID.Generate().ToString(), new UICatalogData("", ""));
            }
            EditorGUILayout.EndHorizontal();

            if(needRemoveKey != null)
                catalog.Data.Remove(needRemoveKey);

            if (GUI.changed)
            {
                serializedObject.ApplyModifiedProperties();
                EditorUtility.SetDirty(target);
            }
        }
    }
}