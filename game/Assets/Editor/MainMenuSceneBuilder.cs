using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using SheNicest.UI;

namespace SheNicest.EditorTools
{
    /// <summary>
    /// 通过菜单栏一键创建包含完整主菜单 UI 的场景。
    /// 菜单路径: Tools → Create Main Menu Scene
    /// </summary>
    public static class MainMenuSceneBuilder
    {
        private const string ScenePath = "Assets/Scenes/MainMenu.unity";

        [MenuItem("Tools/Create Main Menu Scene")]
        public static void CreateScene()
        {
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

            // --- EventSystem ---
            var eventSystem = new GameObject("EventSystem");
            eventSystem.AddComponent<UnityEngine.EventSystems.EventSystem>();
            eventSystem.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();

            // --- Canvas ---
            var canvasGo = new GameObject("Canvas");
            var canvas = canvasGo.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            var canvasScaler = canvasGo.AddComponent<CanvasScaler>();
            canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            canvasScaler.referenceResolution = new Vector2(1920, 1080);
            canvasScaler.matchWidthOrHeight = 0.5f;
            canvasGo.AddComponent<GraphicRaycaster>();

            // --- Title Text ---
            var titleGo = CreateText(canvasGo.transform, "GameTitle", "She Nicest", new Vector2(0, 250), 72, TextAnchor.MiddleCenter);

            // --- Button Container ---
            var buttonContainerGo = new GameObject("ButtonContainer");
            buttonContainerGo.transform.SetParent(canvasGo.transform, false);
            var containerRect = buttonContainerGo.AddComponent<RectTransform>();
            containerRect.anchorMin = new Vector2(0.5f, 0.4f);
            containerRect.anchorMax = new Vector2(0.5f, 0.4f);
            containerRect.pivot = new Vector2(0.5f, 0.5f);
            containerRect.anchoredPosition = Vector2.zero;
            containerRect.sizeDelta = new Vector2(400, 400);

            var verticalLayout = buttonContainerGo.AddComponent<VerticalLayoutGroup>();
            verticalLayout.spacing = 20f;
            verticalLayout.childAlignment = TextAnchor.MiddleCenter;
            verticalLayout.childControlWidth = true;
            verticalLayout.childControlHeight = true;
            verticalLayout.childForceExpandWidth = true;
            verticalLayout.childForceExpandHeight = false;

            var containerFitter = buttonContainerGo.AddComponent<ContentSizeFitter>();
            containerFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            // --- Buttons ---
            var startBtn = CreateButton(buttonContainerGo.transform, "StartGameButton", "\u5f00\u59cb\u6e38\u620f", 60);
            var settingsBtn = CreateButton(buttonContainerGo.transform, "SettingsButton", "\u6e38\u620f\u8bbe\u7f6e", 60);
            var creditsBtn = CreateButton(buttonContainerGo.transform, "CreditsButton", "\u5236\u4f5c\u4eba\u5458", 60);
            var exitBtn = CreateButton(buttonContainerGo.transform, "ExitGameButton", "\u9000\u51fa\u6e38\u620f", 60);

            // --- Settings Panel ---
            var settingsPanel = CreatePanel(canvasGo.transform, "SettingsPanel", "\u6e38\u620f\u8bbe\u7f6e");
            var creditsPanel = CreatePanel(canvasGo.transform, "CreditsPanel", "\u5236\u4f5c\u4eba\u5458");

            // --- Controller ---
            var controllerGo = new GameObject("MainMenuController");
            var controller = controllerGo.AddComponent<MainMenuController>();

            // Use SerializedObject to set private [SerializeField] fields
            var so = new SerializedObject(controller);
            so.FindProperty("startGameButton").objectReferenceValue = startBtn;
            so.FindProperty("settingsButton").objectReferenceValue = settingsBtn;
            so.FindProperty("creditsButton").objectReferenceValue = creditsBtn;
            so.FindProperty("exitGameButton").objectReferenceValue = exitBtn;
            so.FindProperty("settingsPanel").objectReferenceValue = settingsPanel;
            so.FindProperty("creditsPanel").objectReferenceValue = creditsPanel;
            so.ApplyModifiedPropertiesWithoutUndo();

            // --- Wire close buttons to CloseAllPanels ---
            WireCloseButton(settingsPanel, controller);
            WireCloseButton(creditsPanel, controller);

            // --- Save Scene ---
            EditorSceneManager.SaveScene(scene, ScenePath);
            Debug.Log($"[MainMenuSceneBuilder] Main menu scene created at {ScenePath}");
        }

        private static Button CreateButton(Transform parent, string name, string label, float height)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);

            var rect = go.AddComponent<RectTransform>();
            rect.sizeDelta = new Vector2(0, height);

            var layoutElement = go.AddComponent<LayoutElement>();
            layoutElement.preferredHeight = height;

            var image = go.AddComponent<Image>();
            image.color = new Color(0.2f, 0.3f, 0.5f, 1f);

            var button = go.AddComponent<Button>();
            var colors = button.colors;
            colors.normalColor = new Color(0.2f, 0.3f, 0.5f, 1f);
            colors.highlightedColor = new Color(0.3f, 0.4f, 0.6f, 1f);
            colors.pressedColor = new Color(0.15f, 0.25f, 0.45f, 1f);
            colors.selectedColor = new Color(0.2f, 0.3f, 0.5f, 1f);
            button.colors = colors;

            // Label
            var labelGo = new GameObject("Label");
            labelGo.transform.SetParent(go.transform, false);
            var labelRect = labelGo.AddComponent<RectTransform>();
            labelRect.anchorMin = Vector2.zero;
            labelRect.anchorMax = Vector2.one;
            labelRect.offsetMin = Vector2.zero;
            labelRect.offsetMax = Vector2.zero;
            var labelText = labelGo.AddComponent<Text>();
            labelText.text = label;
            labelText.alignment = TextAnchor.MiddleCenter;
            labelText.fontSize = 32;
            labelText.color = Color.white;
            labelText.raycastTarget = false;
            labelText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

            return button;
        }

        private static void WireCloseButton(GameObject panel, MainMenuController controller)
        {
            var closeBtn = panel.transform.Find("CloseButton")?.GetComponent<Button>();
            if (closeBtn != null)
            {
                closeBtn.onClick.AddListener(controller.CloseAllPanels);
            }
        }

        private static Text CreateText(Transform parent, string name, string content, Vector2 position, int fontSize, TextAnchor anchor)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);
            var rect = go.AddComponent<RectTransform>();
            rect.anchoredPosition = position;
            rect.sizeDelta = new Vector2(600, 100);
            var text = go.AddComponent<Text>();
            text.text = content;
            text.fontSize = fontSize;
            text.alignment = anchor;
            text.color = Color.white;
            text.raycastTarget = false;
            text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            return text;
        }

        private static GameObject CreatePanel(Transform parent, string name, string title)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);
            go.SetActive(false);

            var rect = go.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.sizeDelta = new Vector2(800, 500);

            var bg = go.AddComponent<Image>();
            bg.color = new Color(0.1f, 0.1f, 0.15f, 0.95f);

            // Title
            var titleGo = new GameObject("Title");
            titleGo.transform.SetParent(go.transform, false);
            var titleRect = titleGo.AddComponent<RectTransform>();
            titleRect.anchoredPosition = new Vector2(0, 200);
            titleRect.sizeDelta = new Vector2(700, 80);
            var titleText = titleGo.AddComponent<Text>();
            titleText.text = title;
            titleText.fontSize = 48;
            titleText.alignment = TextAnchor.MiddleCenter;
            titleText.color = Color.white;
            titleText.raycastTarget = false;
            titleText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

            // Close Button
            var closeBtn = CreateButton(go.transform, "CloseButton", "\u8fd4\u56de", 50);
            var closeBtnRect = closeBtn.GetComponent<RectTransform>();
            closeBtnRect.anchorMin = new Vector2(0.5f, 0f);
            closeBtnRect.anchorMax = new Vector2(0.5f, 0f);
            closeBtnRect.pivot = new Vector2(0.5f, 0.5f);
            closeBtnRect.anchoredPosition = new Vector2(0, -220);
            closeBtnRect.sizeDelta = new Vector2(300, 50);

            // Wire close button to the panel's parent Canvas → MainMenuController.CloseAllPanels()
            // We'll connect it after the controller is created in the calling method via UnityEvent
            return go;
        }
    }
}
