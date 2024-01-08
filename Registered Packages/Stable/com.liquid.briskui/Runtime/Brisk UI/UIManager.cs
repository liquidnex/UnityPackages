using Liquid.CommonUtils;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Liquid.BriskUI
{
    /// <summary>
    /// Manage UI forms and schedule them.
    /// </summary>
    public class UIManager : MonoBehaviour
    {
        private List<UIForm> uiForms = new List<UIForm>();
        private List<UIForm> tmpHiddenUIs = new List<UIForm>();
        private UICatalog uiCatalog = null;
        private List<IUIProvider> uiProviders = new List<IUIProvider>(); 

        private Camera uiCamera;
        private Canvas uiCanvas;
        private GameObject normalUIRootPanel;
        private GameObject popupsUIRootPanel;
        private GameObject uniqueUIRootPanel;

        private static UIManager instance;

        /// <summary>
        /// Original UI forms.
        /// </summary>
        public List<UIForm> UIForms
        {
            get => uiForms;
        }

        /// <summary>
        /// UI camera.
        /// </summary>
        public Camera UICamera
        {
            get => uiCamera;
        }

        /// <summary>
        /// Canvas component of the UI root gameobject.
        /// </summary>
        public Canvas UICanvas
        {
            get => uiCanvas;
        }

        /// <summary>
        /// A root gameobject for normal UIs in UI root.
        /// </summary>
        public GameObject NormalUIRootPanel
        {
            get => normalUIRootPanel;
        }

        /// <summary>
        /// A root gameobject for popup UIs in UI root.
        /// </summary>
        public GameObject PopupsUIRootPanel
        {
            get => popupsUIRootPanel;
        }

        /// <summary>
        /// A root gameobject for unique UIs in UI root.
        /// </summary>
        public GameObject UniqueUIRootPanel
        {
            get => uniqueUIRootPanel;
        }

        /// <summary>
        /// Access interface of singleton design pattern object.
        /// </summary>
        public static UIManager Instance
        {
            get
            {
                if (instance == null)
                {
                    GameObject gameObject = new GameObject("_UIManager");
                    DontDestroyOnLoad(gameObject);
                    instance = gameObject.AddComponent<UIManager>();
                    instance.Init();
                }
                return instance;
            }
        }

        /// <summary>
        /// Create a UI form instance by its type.
        /// </summary>
        /// <typeparam name="T">Type of UI form.</typeparam>
        /// <returns>UI form created.</returns>
        public T CreateUI<T>()
            where T : UIForm
        {
            Type t = typeof(T);
            return CreateUI(t) as T;
        }

        public T CreateUI<T>(GameObject uiObject)
            where T : UIForm
        {
            Type t = typeof(T);
            return CreateUI(t, uiObject) as T;
        }

        /// <summary>
        /// Create a UI form instance by its type.
        /// </summary>
        /// <param name="t">Type of UI form.</param>
        /// <returns>UI form created.</returns>
        public UIForm CreateUI(Type t)
        {
            if (t == null)
                return null;
            if (!typeof(UIForm).IsAssignableFrom(t))
                return null;

            GameObject uiObject = GetUIObject(t);
            if (uiObject == null)
                return null;

            return CreateUI(t, uiObject);
        }

        public UIForm CreateUI(Type t, GameObject uiObject)
        {
            if (t == null || uiObject == null)
                return null;
            if (!typeof(UIForm).IsAssignableFrom(t))
                return null;

            uiObject.name = t.Name;
            if (uiObject.GetComponent<RectTransform>() == null)
            {
                RectTransform rt = uiObject.AddComponent<RectTransform>();
                rt.anchorMin = new Vector2(0f, 0f);
                rt.anchorMax = new Vector2(1f, 1f);
                rt.pivot = new Vector2(0.5f, 0.5f);
                rt.offsetMin = new Vector2(0f, 0f);
                rt.offsetMax = new Vector2(0f, 0f);
                rt.localPosition = Vector2.zero;
                rt.sizeDelta = new Vector2(0, 0);
            }
            if (uiObject.GetComponent<CanvasRenderer>() == null)
            {
                CanvasRenderer canvasRenderer = uiObject.AddComponent<CanvasRenderer>();
                canvasRenderer.cullTransparentMesh = true;
            }
            if (uiObject.GetComponent<CanvasGroup>() == null)
            {
                CanvasGroup canvasGroup = uiObject.AddComponent<CanvasGroup>();
                canvasGroup.alpha = 1.0f;
                canvasGroup.interactable = true;
                canvasGroup.blocksRaycasts = true;
                canvasGroup.ignoreParentGroups = false;
            }
            if (uiObject.GetComponent(t) == null)
                uiObject.AddComponent(t);

            UIForm ui = uiObject.GetComponent<UIForm>();
            if (ui.HasMode(UIMode.NORMAL))
            {
                uiObject.transform.SetParent(normalUIRootPanel.transform, false);
            }
            else if (ui.HasMode(UIMode.POPUP))
            {
                uiObject.transform.SetParent(popupsUIRootPanel.transform, false);
            }
            else if (ui.HasMode(UIMode.UNIQUE))
            {
                uiObject.transform.SetParent(uniqueUIRootPanel.transform, false);
            }

            uiForms.Add(ui);
            ShowUI(ui);
            return ui;
        }

        /// <summary>
        /// Destroy a UI form by its type.
        /// When there are multiple UIs of the same type, the first one found will be destroyed.
        /// </summary>
        /// <typeparam name="T">Type of UI form.</typeparam>
        public void DestroyUI<T>()
            where T : UIForm
        {
            Type t = typeof(T);
            DestroyUI(t);
        }

        /// <summary>
        /// Destroy a UI form by its type.
        /// When there are multiple UIs of the same type, the first one found will be destroyed.
        /// </summary>
        /// <param name="t">Type of UI form.</param>
        public void DestroyUI(Type t)
        {
            if (t == null)
                return;

            UIForm ui = FindUI(t);
            if (ui == null)
                return;

            DestroyUI(ui);
        }

        /// <summary>
        /// Destroy an exist UI form.
        /// </summary>
        /// <param name="ui">UI form that needs to be destroyed.</param>
        public void DestroyUI(UIForm ui)
        {
            if (ui == null)
                return;

            tmpHiddenUIs.Remove(ui);

            if (!uiForms.Contains(ui))
                return;

            HideUI(ui);

            uiForms.Remove(ui);

            if (ui.gameObject != null)
            {
                GameObject.Destroy(ui.gameObject);
            }
        }

        /// <summary>
        /// Show a hidden UI form by type.
        /// When the specified UI form type cannot be found, create it.
        /// </summary>
        /// <typeparam name="T">Type of UI form.</typeparam>
        /// <returns>Displayed UI form.</returns>
        public T ShowOrCreateUI<T>()
            where T : UIForm
        {
            T ui = FindUI<T>();
            if (ui != null)
            {
                ShowUI(ui);
                return ui;
            }
            else
            {
                return CreateUI<T>();
            }
        }

        /// <summary>
        /// Display a UI form by its type.
        /// When there are multiple UIs of the same type, the first one found will be displayed.
        /// </summary>
        /// <typeparam name="T">Type of UI form.</typeparam>
        /// <returns>Displayed UI form.</returns>
        public T ShowUI<T>()
            where T : UIForm
        {
            Type t = typeof(T);
            return ShowUI(t) as T;
        }

        /// <summary>
        /// Display a UI form by its type.
        /// When there are multiple UIs of the same type, the first one found will be displayed.
        /// </summary>
        /// <param name="t">Type of UI form.</param>
        /// <returns>Displayed UI form.</returns>
        public UIForm ShowUI(Type t)
        {
            if (t == null)
                return null;

            UIForm ui = FindUI(t);
            if (ui == null)
                return null;

            ShowUI(ui);
            return ui;
        }

        /// <summary>
        /// Display an exist UI form.
        /// </summary>
        /// <param name="ui">UI form that needs to be displayed.</param>
        public void ShowUI(UIForm ui)
        {
            if (ui == null)
                return;

            if (!uiForms.Contains(ui))
                return;

            if (ui.HasMode(UIMode.NORMAL))
            {
                if (ui.HasMode(UIMode.KEEP_TOP))
                    ui.gameObject.transform.SetAsLastSibling();
                else if (ui.HasMode(UIMode.KEEP_BOTTOM))
                    ui.gameObject.transform.SetAsFirstSibling();

                ShowNormalUI(ui);
            }
            else if (ui.HasMode(UIMode.POPUP))
            {
                ShowPopupUI(ui);
            }
            else if (ui.HasMode(UIMode.UNIQUE))
            {
                ShowUniqueUI(ui);
            }
        }

        /// <summary>
        /// Hide a UI form by its type.
        /// When there are multiple UIs of the same type, the first one found will be hided.
        /// </summary>
        /// <typeparam name="T">Type of UI form.</typeparam>
        public void HideUI<T>()
            where T : UIForm
        {
            Type t = typeof(T);
            HideUI(t);
        }

        /// <summary>
        /// Hide a UI form by its type.
        /// When there are multiple UIs of the same type, the first one found will be hided.
        /// </summary>
        /// <param name="t">Type of UI form.</param>
        public void HideUI(Type t)
        {
            if (t == null)
                return;

            UIForm ui = FindUI(t);
            if (ui == null)
                return;

            HideUI(ui);
        }

        /// <summary>
        /// Hide an exist UI form.
        /// </summary>
        /// <param name="ui">UI form that needs to be hided.</param>
        public void HideUI(UIForm ui)
        {
            if (ui == null)
                return;

            if (!uiForms.Contains(ui))
                return;

            if (ui.HasMode(UIMode.NORMAL))
                HideNormalUI(ui);
            else if (ui.HasMode(UIMode.POPUP))
                HidePopupUI(ui);
            else if (ui.HasMode(UIMode.UNIQUE))
                HideUniqueUI(ui);
        }

        /// <summary>
        /// Find a UI form by its type.
        /// When there are multiple UIs of the same type, the first one found will be found.
        /// </summary>
        /// <typeparam name="T">Type of UI form.</typeparam>
        /// <returns>Search result.</returns>
        public T FindUI<T>()
            where T : UIForm
        {
            Type t = typeof(T);
            return FindUI(t) as T;
        }

        /// <summary>
        /// Find a UI form by its type.
        /// When there are multiple UIs of the same type, the first one found will be found.
        /// </summary>
        /// <param name="t">Type of UI form.</param>
        /// <returns>Search result.</returns>
        public UIForm FindUI(Type t)
        {
            if (uiForms.Count == 0)
                return null;

            List<UIForm> pickedUIs = uiForms.FindAll(
                ui => {
                    if (t.IsAssignableFrom(ui.GetType()))
                        return true;
                    else
                        return false;
                }
            );

            if (pickedUIs.Count == 0)
                return null;

            pickedUIs.Sort();
            return pickedUIs[pickedUIs.Count-1];
        }

        /// <summary>
        /// Find UI forms by their type.
        /// </summary>
        /// <param name="match">Find predicate.</param>
        /// <returns>Search results.</returns>
        public List<T> FindAllUI<T>(Predicate<T> match)
            where T : UIForm
        {
            List<UIForm> pickedUIs = uiForms.FindAll(
                ui => {
                    if (ui is T e)
                    {
                        if (match(e))
                            return true;
                        else
                            return false;
                    }
                    return false;
                }
            );

            return pickedUIs.ConvertAll<T>(n => (T)n);
        }

        /// <summary>
        /// Clear all UI forms.
        /// </summary>
        public void Clear()
        {
            List<UIForm> showingUIs = uiForms.FindAll(ui => ui.IsShowing);
            foreach (UIForm ui in showingUIs)
            {
                ui.Hide();
            }
            tmpHiddenUIs.Clear();
            uiForms.Clear();

            normalUIRootPanel.RemoveAllChilds();
            popupsUIRootPanel.RemoveAllChilds();
            uniqueUIRootPanel.RemoveAllChilds();
        }

        /// <summary>
        /// Register a catalog for UIManager.
        /// </summary>
        /// <param name="typeName">Type name.</param>
        /// <param name="catalogData">Catalog data for UIManager.</param>
        public void RegisterCatalog(string typeName, UICatalogData catalogData)
        {
            if (uiCatalog == null ||
                uiCatalog.Data == null)
                return;

            if (typeName == null ||
                !catalogData.IsLegal)
                return;

            if (uiCatalog.Data.ContainsKey(typeName))
                uiCatalog.Data[typeName] = catalogData;
            else
                uiCatalog.Data.Add(typeName, catalogData);
        }

        public void UnregisterCatalog(string typeName)
        {
            if (uiCatalog == null ||
                uiCatalog.Data == null)
                return;

            uiCatalog.Data.Remove(typeName);
        }

        /// <summary>
        /// Register a provider for UIManager.
        /// </summary>
        /// <param name="provider">Provider for UIManager.</param>
        public void RegisterProvider(IUIProvider provider)
        {
            if (uiProviders == null)
                return;

            if (provider.Labels == null ||
                provider.Labels.Count == 0)
                return;

            if (!uiProviders.Contains(provider))
                uiProviders.Add(provider);
        }

        public void UnregisterProvider(IUIProvider provider)
        {
            if (uiProviders == null)
                return;

            uiProviders.Remove(provider);
        }

        public void RefreshTexts()
        {
            foreach (UIForm ui in uiForms)
            {
                ui.RefreshText();
            }
        }

        private void Init()
        {
            const string configPath = "Data/UI/UI Catalog";
            UICatalog data = Resources.Load<UICatalog>(configPath);
            if (data != null)
                uiCatalog = data;

            if (uiCatalog == null)
            {
                Debug.LogWarning($"No preset UI catalog found. Instead, a default catalog is generated to replace it.\nPlease Check the {configPath} folder to make sure catalog config is there.");
                uiCatalog = ScriptableObject.CreateInstance<UICatalog>();
            }

            GameObject uiRoot = Instantiate(Resources.Load("Prefab/UI/Root UI") as GameObject);
            uiRoot.name = "Root UI";
            uiRoot.transform.SetParent(gameObject.transform);

            uiCamera = uiRoot.FindComponent<Camera>("UI Camera");
            uiCanvas = uiRoot.FindComponent<Canvas>("UI Canvas");
            normalUIRootPanel = uiRoot.FindGameObject("Root UI/UI Canvas/Normal UI Root Panel");
            popupsUIRootPanel = uiRoot.FindGameObject("Root UI/UI Canvas/Popups UI Root Panel");
            uniqueUIRootPanel = uiRoot.FindGameObject("Root UI/UI Canvas/Unique UI Root Panel");
        }

        private GameObject GetUIObject(Type type)
        {
            if (type == null)
                return null;

            string typeName = type.Name;
            if (!uiCatalog.Data.TryGetValue(typeName, out UICatalogData info))
                return null;

            string providerLabel = info.UILabel;
            string uiAddress = info.UIAddress;
            List<IUIProvider> providers = uiProviders.FindAll(p => p != null && p.Labels.Contains(providerLabel));
            foreach (IUIProvider p in providers)
            {
                if (p == null)
                    continue;

                GameObject go = p.Get(uiAddress);
                if (go != null)
                    return go;
            }

            return null;
        }

        private void ShowNormalUI(UIForm ui)
        {
            ui.Show();
        }

        private void ShowPopupUI(UIForm ui)
        {
            List<UIForm> showingUIs = uiForms.FindAll(
                    f => f.HasMode(UIMode.POPUP) && f.IsShowing
                );

            showingUIs.Sort();

            int idx = showingUIs.FindIndex(f => f == ui);
            for (int i = 0; i < showingUIs.Count; ++i)
            {
                UIForm f = showingUIs[i];
                if (i < idx)
                {
                    f.Freeze();
                }
                else if (i == idx)
                {
                    f.Show();
                }
                else if (i > idx)
                {
                    f.Hide();
                }
            }
        }

        private void ShowUniqueUI(UIForm ui)
        {
            tmpHiddenUIs = uiForms.FindAll(f => f.IsShowing);
            foreach (UIForm f in tmpHiddenUIs)
            {
                f.Hide();
            }

            ui.Show();
        }

        private void HideNormalUI(UIForm ui)
        {
            ui.Hide();
        }

        private void HidePopupUI(UIForm ui)
        {
            List<UIForm> showingUIs = uiForms.FindAll(
                f => f.HasMode(UIMode.POPUP) && f.IsShowing
            );

            showingUIs.Sort();

            int idx = showingUIs.FindIndex(f => f == ui);
            for (int i = 0; i < showingUIs.Count; ++i)
            {
                UIForm f = showingUIs[i];

                if (i == idx - 1)
                {
                    f.Show();
                }
                else if (i < idx)
                {
                    f.Freeze();
                }
                else if (i >= idx)
                {
                    f.Hide();
                }
            }
        }

        private void HideUniqueUI(UIForm ui)
        {
            if (ui == null)
                return;

            foreach (UIForm f in tmpHiddenUIs)
            {
                f.Show();
            }
            tmpHiddenUIs.Clear();

            ui.Hide();
        }

        private void Awake()
        {
            if (instance != null &&
                instance != this)
            {
                DestroyImmediate(gameObject);
            }
        }
    }
}