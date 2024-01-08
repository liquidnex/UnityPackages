using UnityEditor;
using UnityEngine;

namespace Liquid.CommonUtils.Editor
{
    public class GUIStyleViewer : EditorWindow
    {
        private Vector2 scrollPosition = new Vector2(0, 0);
        private string search = string.Empty;
        private GUIStyle textStyle;

        [MenuItem("Window/GUI Style Viewer", false, 100)]
        private static void OpenStyleViewer()
        {
            GetWindow<GUIStyleViewer>(false, "View Built-in GUI Style");
        }

        private void OnGUI()
        {
            if (textStyle == null)
            {
                textStyle = new GUIStyle("HeaderLabel");
                textStyle.fontSize = 20;
            }

            GUILayout.BeginHorizontal("HelpBox");
            GUILayout.Label("Click the example to copy its name.", textStyle);
            GUILayout.FlexibleSpace();
            GUILayout.Label("Search:");
            search = EditorGUILayout.TextField(search);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal("PopupCurveSwatchBackground");
            GUILayout.Label("Sample", textStyle, GUILayout.Width(300));
            GUILayout.Label("Name", textStyle, GUILayout.Width(300));
            GUILayout.EndHorizontal();

            scrollPosition = GUILayout.BeginScrollView(scrollPosition);

            foreach (var style in GUI.skin.customStyles)
            {
                if (style.name.ToLower().Contains(search.ToLower()))
                {
                    GUILayout.Space(15);
                    GUILayout.BeginHorizontal("PopupCurveSwatchBackground");
                    if (GUILayout.Button(style.name, style, GUILayout.Width(300)))
                    {
                        EditorGUIUtility.systemCopyBuffer = style.name;
                        Debug.Log(style.name);
                    }
                    EditorGUILayout.SelectableLabel(style.name, GUILayout.Width(300));
                    GUILayout.EndHorizontal();
                }
            }
            GUILayout.EndScrollView();
        }
    }
}