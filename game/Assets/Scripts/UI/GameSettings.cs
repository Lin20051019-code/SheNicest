using UnityEngine;

namespace SheNicest.UI
{
    /// <summary>
    /// 游戏设置管理器，使用 PlayerPrefs 持久化保存音量和分辨率设置。
    /// </summary>
    public static class GameSettings
    {
        private const string KeyBGMVolume = "BGMVolume";
        private const string KeySFXVolume = "SFXVolume";
        private const string KeyResolutionIndex = "ResolutionIndex";

        private const float DefaultBGMVolume = 1f;
        private const float DefaultSFXVolume = 1f;
        private const int DefaultResolutionIndex = -1; // -1 表示使用默认（最后一个）

        /// <summary>背景音乐音量 (0~1)。</summary>
        public static float BGMVolume
        {
            get => PlayerPrefs.GetFloat(KeyBGMVolume, DefaultBGMVolume);
            set => PlayerPrefs.SetFloat(KeyBGMVolume, Mathf.Clamp01(value));
        }

        /// <summary>游戏音效音量 (0~1)。</summary>
        public static float SFXVolume
        {
            get => PlayerPrefs.GetFloat(KeySFXVolume, DefaultSFXVolume);
            set => PlayerPrefs.SetFloat(KeySFXVolume, Mathf.Clamp01(value));
        }

        /// <summary>分辨率索引（对应 Screen.resolutions 数组）。</summary>
        public static int ResolutionIndex
        {
            get => PlayerPrefs.GetInt(KeyResolutionIndex, DefaultResolutionIndex);
            set => PlayerPrefs.SetInt(KeyResolutionIndex, value);
        }

        /// <summary>保存所有设置到 PlayerPrefs。</summary>
        public static void Save()
        {
            PlayerPrefs.Save();
        }

        /// <summary>应用分辨率设置。</summary>
        public static void ApplyResolution(int index)
        {
            var resolutions = Screen.resolutions;
            if (resolutions == null || resolutions.Length == 0) return;

            if (index < 0 || index >= resolutions.Length)
                index = resolutions.Length - 1;

            var res = resolutions[index];
            Screen.SetResolution(res.width, res.height, Screen.fullScreen);
            ResolutionIndex = index;
        }

        /// <summary>应用所有设置（在游戏启动时调用）。</summary>
        public static void ApplyAll()
        {
            var resolutions = Screen.resolutions;
            if (resolutions != null && resolutions.Length > 0)
            {
                int idx = ResolutionIndex;
                if (idx < 0 || idx >= resolutions.Length)
                    idx = resolutions.Length - 1;
                var res = resolutions[idx];
                Screen.SetResolution(res.width, res.height, Screen.fullScreen);
            }
        }
    }
}
