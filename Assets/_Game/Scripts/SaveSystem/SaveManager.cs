using UnityEngine;

/// <summary>
/// SaveManager handles the saving and loading of the game's state,
/// including the current level index. It uses the SaveSystem for the actual
/// persistence logic and ensures that the game follows SOLID principles.
/// </summary>
public static class SaveManager
{
    private static SaveData _currentSaveData;

    /// <summary>
    /// Loads the SaveData from the SaveSystem.
    /// </summary>
    static SaveManager()
    {
        _currentSaveData = SaveSystem.LoadGame();
    }

    /// <summary>
    /// Saves the level index to the SaveData and persists it using SaveSystem.
    /// </summary>
    /// <param name="levelIndex">The new level index to save.</param>
    public static void SaveLevelIndex(int levelIndex)
    {
        _currentSaveData.CurrentLevelIndex = levelIndex;
        SaveSystem.SaveGame(_currentSaveData);
    }

    /// <summary>
    /// Loads the saved level index from SaveData.
    /// </summary>
    /// <returns>The saved current level index.</returns>
    public static int LoadLevelIndex()
    {
        return _currentSaveData.CurrentLevelIndex;
    }
}