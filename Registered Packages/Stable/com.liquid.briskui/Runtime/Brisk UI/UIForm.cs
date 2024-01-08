using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Liquid.BriskUI
{
    /// <summary>
    /// A UI form is a single UI page.
    /// </summary>
    public abstract class UIForm :
        MonoBehaviour, IComparable<UIForm>
    {
        private Dictionary<Text, TextContent> variableTexts = new Dictionary<Text, TextContent>();
        private Dictionary<TMP_Text, TextContent> variableTMPTexts = new Dictionary<TMP_Text, TextContent>();

        /// <summary>
        /// Determine whether this UI form is visible.
        /// </summary>
        public bool IsShowing
        {
            get
            {
                bool active = gameObject.activeInHierarchy;
                bool visible = GetCanvasGroup().alpha > 0;

                if (active && visible)
                    return true;
                else
                    return false;
            }
        }

        /// <summary>
        /// Display the current UI directly. 
        /// If you want to display it intelligently according to UIMode, please use UIManager.Instance.ShowUI instead.
        /// </summary>
        public virtual void Show()
        {
            CanvasGroup g = GetCanvasGroup();
            g.alpha = 1;
            g.blocksRaycasts = true;

            Active();
        }

        /// <summary>
        /// Hide the current UI directly. 
        /// If you want to display it intelligently according to UIMode, please use UIManager.Instance.HideUI instead.
        /// </summary>
        public virtual void Hide()
        {
            CanvasGroup g = GetCanvasGroup();
            g.alpha = 0;
            g.blocksRaycasts = false;

            Freeze();
        }

        public bool HasMode(UIMode flags)
        {
            return GetMode().HasFlag(flags);
        }

        /// <summary>
        /// Set the content of a UI text.
        /// </summary>
        /// <param name="t">UI text component.</param>
        /// <param name="textID">Text id.</param>
        public void SetText(Text t, string textID)
        {
            TextContent tc = new TextContent(textID);
            if (variableTexts.ContainsKey(t))
            {
                variableTexts[t] = tc;
            }
            else
            {
                variableTexts.Add(t, tc);
            }

            t.text = variableTexts[t];
        }

        /// <summary>
        /// Set the content of a UI text.
        /// </summary>
        /// <param name="t">UI text component.</param>
        /// <param name="content">Text content.</param>
        public void SetText(Text t, TextContent content)
        {
            if (content == null)
                return;

            if (variableTexts.ContainsKey(t))
            {
                variableTexts[t] = content;
            }
            else
            {
                variableTexts.Add(t, content);
            }

            t.text = variableTexts[t];
        }

        /// <summary>
        /// Set the content of a UI text.
        /// </summary>
        /// <param name="t">UI text component.</param>
        /// <param name="textID">Text id.</param>
        /// <param name="textParam">Text parameter.</param>
        public void SetText(Text t, string textID, params string[] textParam)
        {
            TextContent tc = new TextContent(textID, textParam);
            if (variableTexts.ContainsKey(t))
            {
                variableTexts[t] = tc;
            }
            else
            {
                variableTexts.Add(t, tc);
            }

            t.text = variableTexts[t];
        }

        /// <summary>
        /// Set the content of a UI text.
        /// </summary>
        /// <param name="t">UI text component.</param>
        /// <param name="textID">Text id.</param>
        /// <param name="textParam">Text parameter.</param>
        public void SetText(Text t, string textID, params TextContent[] textParam)
        {
            TextContent tc = new TextContent(textID, textParam);
            if (variableTexts.ContainsKey(t))
            {
                variableTexts[t] = tc;
            }
            else
            {
                variableTexts.Add(t, tc);
            }

            t.text = variableTexts[t];
        }

        /// <summary>
        /// Set the content of a UI text.
        /// </summary>
        /// <param name="t">UI text component.</param>
        /// <param name="textID">Text id.</param>
        /// <param name="textParam">Text parameter.</param>
        public void SetText(Text t, string textID, Dictionary<string, string> textParam)
        {
            TextContent tc = new TextContent(textID, textParam);
            if (variableTexts.ContainsKey(t))
            {
                variableTexts[t] = tc;
            }
            else
            {
                variableTexts.Add(t, tc);
            }

            t.text = variableTexts[t];
        }

        /// <summary>
        /// Set the content of a UI text.
        /// </summary>
        /// <param name="t">UI text component.</param>
        /// <param name="textID">Text id.</param>
        /// <param name="textParam">Text parameter.</param>
        public void SetText(Text t, string textID, Dictionary<string, TextContent> textParam)
        {
            TextContent tc = new TextContent(textID, textParam);
            if (variableTexts.ContainsKey(t))
            {
                variableTexts[t] = tc;
            }
            else
            {
                variableTexts.Add(t, tc);
            }

            t.text = variableTexts[t];
        }

        /// <summary>
        /// Set the content of a UI text.
        /// </summary>
        /// <param name="t">UI text component.</param>
        /// <param name="textID">Text id.</param>
        public void SetText(TMP_Text t, string textID)
        {
            TextContent tc = new TextContent(textID);
            if (variableTMPTexts.ContainsKey(t))
            {
                variableTMPTexts[t] = tc;
            }
            else
            {
                variableTMPTexts.Add(t, tc);
            }

            t.text = variableTMPTexts[t];
        }

        /// <summary>
        /// Set the content of a UI text.
        /// </summary>
        /// <param name="t">UI text component.</param>
        /// <param name="content">Text content.</param>
        public void SetText(TMP_Text t, TextContent content)
        {
            if (content == null)
                return;

            if (variableTMPTexts.ContainsKey(t))
            {
                variableTMPTexts[t] = content;
            }
            else
            {
                variableTMPTexts.Add(t, content);
            }

            t.text = variableTMPTexts[t];
        }

        /// <summary>
        /// Set the content of a UI text.
        /// </summary>
        /// <param name="t">UI text component.</param>
        /// <param name="textID">Text id.</param>
        /// <param name="textParam">Text parameter.</param>
        public void SetText(TMP_Text t, string textID, params string[] textParam)
        {
            TextContent tc = new TextContent(textID, textParam);
            if (variableTMPTexts.ContainsKey(t))
            {
                variableTMPTexts[t] = tc;
            }
            else
            {
                variableTMPTexts.Add(t, tc);
            }

            t.text = variableTMPTexts[t];
        }

        /// <summary>
        /// Set the content of a UI text.
        /// </summary>
        /// <param name="t">UI text component.</param>
        /// <param name="textID">Text id.</param>
        /// <param name="textParam">Text parameter.</param>
        public void SetText(TMP_Text t, string textID, params TextContent[] textParam)
        {
            TextContent tc = new TextContent(textID, textParam);
            if (variableTMPTexts.ContainsKey(t))
            {
                variableTMPTexts[t] = tc;
            }
            else
            {
                variableTMPTexts.Add(t, tc);
            }

            t.text = variableTMPTexts[t];
        }

        /// <summary>
        /// Set the content of a UI text.
        /// </summary>
        /// <param name="t">UI text component.</param>
        /// <param name="textID">Text id.</param>
        /// <param name="textParam">Text parameter.</param>
        public void SetText(TMP_Text t, string textID, Dictionary<string, string> textParam)
        {
            TextContent tc = new TextContent(textID, textParam);
            if (variableTMPTexts.ContainsKey(t))
            {
                variableTMPTexts[t] = tc;
            }
            else
            {
                variableTMPTexts.Add(t, tc);
            }

            t.text = variableTMPTexts[t];
        }

        /// <summary>
        /// Set the content of a UI text.
        /// </summary>
        /// <param name="t">UI text component.</param>
        /// <param name="textID">Text id.</param>
        /// <param name="textParam">Text parameter.</param>
        public void SetText(TMP_Text t, string textID, Dictionary<string, TextContent> textParam)
        {
            TextContent tc = new TextContent(textID, textParam);
            if (variableTMPTexts.ContainsKey(t))
            {
                variableTMPTexts[t] = tc;
            }
            else
            {
                variableTMPTexts.Add(t, tc);
            }

            t.text = variableTMPTexts[t];
        }

        public void RefreshText()
        {
            foreach (var v in variableTexts)
            {
                if (v.Key != null && v.Value != null)
                    v.Key.text = v.Value;
            }

            foreach (var v in variableTMPTexts)
            {
                if (v.Key != null && v.Value != null)
                    v.Key.text = v.Value;
            }
        }

        public int CompareTo(UIForm other)
        {
            int modeWeightA = GetWeight(this);
            int modeWeightB = GetWeight(other);

            if (modeWeightA - modeWeightB != 0)
                return modeWeightA - modeWeightB;

            int siblingIndexA = 0;
            GameObject rootUINodeA = GetRootUINode();
            if (rootUINodeA != null)
            {
                siblingIndexA = rootUINodeA.transform.GetSiblingIndex();
            }

            int siblingIndexB = 0;
            GameObject rootUINodeB = other.GetRootUINode();
            if (other.gameObject != null)
            {
                siblingIndexB = rootUINodeB.transform.GetSiblingIndex();
            }

            return siblingIndexA - siblingIndexB;
        }

        public void Active()
        {
            SetEnabled(true);
        }

        /// <summary>
        /// Freeze the responses of all interactive controls on this UI form.
        /// </summary>
        public void Freeze()
        {
            SetEnabled(false);
        }

        protected virtual UIMode GetMode()
        {
            return UIMode.NORMAL | UIMode.KEEP_TOP;
        }

        private GameObject GetRootUINode()
        {
            if (gameObject == null)
                return null;

            Transform trans = gameObject.transform;
            while (trans.parent != null)
            {
                if (trans.parent == UIManager.Instance.NormalUIRootPanel ||
                    trans.parent == UIManager.Instance.PopupsUIRootPanel ||
                    trans.parent == UIManager.Instance.UniqueUIRootPanel)
                {
                    return trans.gameObject;
                }

                trans = trans.parent;
            }
            return null;
        }

        private void SetEnabled(bool enabled = true)
        {
            AudioSource source = GetComponent<AudioSource>();
            if (source != null)
            {
                if (enabled)
                    source.Play();
                else
                    source.Stop();
            }

            GetCanvasGroup().interactable = enabled;
        }

        private CanvasGroup GetCanvasGroup()
        {
            CanvasGroup g = GetComponent<CanvasGroup>();
            if (g == null)
                g = gameObject.AddComponent<CanvasGroup>();

            return g;
        }

        private static int GetWeight(UIForm ui)
        {
            if (ui == null)
                return -1;
            if (ui.HasMode(UIMode.UNIQUE))
                return 3;
            else if (ui.HasMode(UIMode.POPUP))
                return 2;
            else if (ui.HasMode(UIMode.NORMAL))
                return 1;
            return 0;
        }
    }
}