using System.Collections.Generic;
using UnityEngine;

namespace One_Tap_UI.UI.Data
{
    [CreateAssetMenu(fileName = "Display Data", menuName = "One Tap UI/UI/Tabs/Display Data")]
    public class DisplayData : ScriptableObject // This class handles all the display settings
    {
        public List<Vector2Int> resolutions = new()
        {
            new Vector2Int(1920, 1080), new Vector2Int(1600, 1000),
            new Vector2Int(1280, 720), new Vector2Int(1280, 800)
        };

        public List<FullScreenMode> fullScreenModes = new()
        {
            FullScreenMode.ExclusiveFullScreen, FullScreenMode.FullScreenWindow, FullScreenMode.Windowed
        };

        public void GetResolutionOptions(List<string> options)
        {
            for (int i = 0, count = resolutions.Count; i < count; i++)
            {
                options.Add($"{resolutions[i].x}x{resolutions[i].y}");
            }
        }


        public void SetResolution(int level)
        {
            var resolution = resolutions[level];
            Screen.SetResolution(resolution.x, resolution.y, Screen.fullScreen);
        }
        
        public void SetFullScreenMode(int level)
        {
            Screen.fullScreenMode = fullScreenModes[level];
        }

        public void SetRefreshRate(int level)
        {
            Application.targetFrameRate = level;
        }
        
        public void SetVSync(int level)
        {
            var i = Mathf.RoundToInt(level);
            QualitySettings.vSyncCount = i; // 1 is on, 0 is off
        }
    }
}