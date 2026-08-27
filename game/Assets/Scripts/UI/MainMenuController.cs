using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace SheNicest.UI
{
    /// <summary>
    /// 主菜单控制器，管理开始游戏、游戏设置、制作人员和退出游戏按钮。
    /// </summary>
    public class MainMenuController : MonoBehaviour
    {
        [Header("Buttons")]
        [SerializeField] private Button startGameButton;
        [SerializeField] private Button settingsButton;
        [SerializeField] private Button creditsButton;
        [SerializeField] private Button exitGameButton;

        [Header("Panel References")]
        [SerializeField] private GameObject settingsPanel;
        [SerializeField] private GameObject creditsPanel;

        [Header("Scene Names")]
        [SerializeField] private string gameSceneName = "GameScene";

        private void Start()
        {
            BindButtons();
            CloseAllPanels();
        }

        private void BindButtons()
        {
            if (startGameButton != null)
                startGameButton.onClick.AddListener(OnStartGame);

            if (settingsButton != null)
                settingsButton.onClick.AddListener(OnSettings);

            if (creditsButton != null)
                creditsButton.onClick.AddListener(OnCredits);

            if (exitGameButton != null)
                exitGameButton.onClick.AddListener(OnExitGame);
        }

        private void OnDestroy()
        {
            if (startGameButton != null)
                startGameButton.onClick.RemoveListener(OnStartGame);

            if (settingsButton != null)
                settingsButton.onClick.RemoveListener(OnSettings);

            if (creditsButton != null)
                creditsButton.onClick.RemoveListener(OnCredits);

            if (exitGameButton != null)
                exitGameButton.onClick.RemoveListener(OnExitGame);
        }

        private void OnStartGame()
        {
            if (!string.IsNullOrEmpty(gameSceneName) && Application.CanStreamedLevelBeLoaded(gameSceneName))
            {
                SceneManager.LoadScene(gameSceneName);
            }
            else
            {
                Debug.LogWarning($"[MainMenu] Game scene '{gameSceneName}' not found or not in Build Settings.");
            }
        }

        private void OnSettings()
        {
            CloseAllPanels();
            if (settingsPanel != null)
                settingsPanel.SetActive(true);
        }

        private void OnCredits()
        {
            CloseAllPanels();
            if (creditsPanel != null)
                creditsPanel.SetActive(true);
        }

        private void OnExitGame()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

        /// <summary>
        /// 关闭所有弹出面板（供面板上的"返回"按钮调用）。
        /// </summary>
        public void CloseAllPanels()
        {
            if (settingsPanel != null)
                settingsPanel.SetActive(false);

            if (creditsPanel != null)
                creditsPanel.SetActive(false);
        }
    }
}
