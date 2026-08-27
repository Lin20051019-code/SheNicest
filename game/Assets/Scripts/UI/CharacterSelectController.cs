using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace SheNicest.UI
{
    /// <summary>
    /// 角色选择场景控制器，左右切换角色，选择后进入游戏场景。
    /// </summary>
    public class CharacterSelectController : MonoBehaviour
    {
        [Header("Character Data")]
        [SerializeField] private List<string> characterNames = new List<string> { "角色1", "角色2", "角色3", "角色4", "角色5" };
        [SerializeField] private List<string> characterDescriptions = new List<string>
        {
            "角色1介绍占位文字",
            "角色2介绍占位文字",
            "角色3介绍占位文字",
            "角色4介绍占位文字",
            "角色5介绍占位文字"
        };

        [Header("UI References")]
        [SerializeField] private Text characterNameText;
        [SerializeField] private Text characterDescriptionText;
        [SerializeField] private Button leftArrowButton;
        [SerializeField] private Button rightArrowButton;
        [SerializeField] private Button selectCharacterButton;

        [Header("Scene Names")]
        [SerializeField] private string gameSceneName = "GameScene";

        private int currentIndex = 0;

        private void Start()
        {
            if (leftArrowButton != null)
                leftArrowButton.onClick.AddListener(PreviousCharacter);

            if (rightArrowButton != null)
                rightArrowButton.onClick.AddListener(NextCharacter);

            if (selectCharacterButton != null)
                selectCharacterButton.onClick.AddListener(OnSelectCharacter);

            UpdateDisplay();
        }

        private void OnDestroy()
        {
            if (leftArrowButton != null)
                leftArrowButton.onClick.RemoveListener(PreviousCharacter);

            if (rightArrowButton != null)
                rightArrowButton.onClick.RemoveListener(NextCharacter);

            if (selectCharacterButton != null)
                selectCharacterButton.onClick.RemoveListener(OnSelectCharacter);
        }

        private void PreviousCharacter()
        {
            currentIndex--;
            if (currentIndex < 0)
                currentIndex = characterNames.Count - 1;
            UpdateDisplay();
        }

        private void NextCharacter()
        {
            currentIndex++;
            if (currentIndex >= characterNames.Count)
                currentIndex = 0;
            UpdateDisplay();
        }

        private void UpdateDisplay()
        {
            if (characterNameText != null && currentIndex >= 0 && currentIndex < characterNames.Count)
                characterNameText.text = characterNames[currentIndex];

            if (characterDescriptionText != null && currentIndex >= 0 && currentIndex < characterDescriptions.Count)
                characterDescriptionText.text = characterDescriptions[currentIndex];
        }

        private void OnSelectCharacter()
        {
            PlayerPrefs.SetInt("SelectedCharacter", currentIndex);
            PlayerPrefs.Save();

            if (!string.IsNullOrEmpty(gameSceneName) && Application.CanStreamedLevelBeLoaded(gameSceneName))
            {
                SceneManager.LoadScene(gameSceneName);
            }
            else
            {
                Debug.LogWarning($"[CharacterSelect] Game scene '{gameSceneName}' not found or not in Build Settings.");
            }
        }
    }
}
