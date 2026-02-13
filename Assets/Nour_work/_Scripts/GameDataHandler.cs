using UnityEngine;

public class GameDataHandler : MonoBehaviour
{
    private const string KEY_SCENE = "SavedSceneName";
    private const string KEY_DAY = "DayNumber";
    private const string KEY_HEART = "lightHeartLevel";
    private const string KEY_TRAIT_1 = "Trait1";
    private const string KEY_TRAIT_2 = "Trait2";
    private const string KEY_TRAIT_3 = "Trait3";
    private const string KEY_HAS_SAVE = "HasSaveGame";
    
    public static void SaveProgress(string nextSceneName)
    {
        PlayerPrefs.SetString(KEY_SCENE, nextSceneName);
        PlayerPrefs.SetInt(KEY_HAS_SAVE, 1);
        PlayerPrefs.Save();
    }

    public static void DeleteSave()
    {
        PlayerPrefs.DeleteKey(KEY_SCENE);
        PlayerPrefs.DeleteKey(KEY_HAS_SAVE);
        PlayerPrefs.DeleteKey(KEY_TRAIT_1);
        PlayerPrefs.DeleteKey(KEY_TRAIT_2);
        PlayerPrefs.DeleteKey(KEY_TRAIT_3);
        PlayerPrefs.DeleteKey(KEY_DAY);
        PlayerPrefs.DeleteKey(KEY_HEART);
        PlayerPrefs.Save();
    }

    public static bool HasSave()
    {
        return PlayerPrefs.HasKey(KEY_HAS_SAVE);
    }

    public static string GetSavedScene()
    {
        return PlayerPrefs.GetString(KEY_SCENE, "Shihab_work/Scenes/IndoorsScene");
    }
}