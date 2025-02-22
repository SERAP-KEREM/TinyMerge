using UnityEngine;

namespace _Main._SaveSystem
{
    /// <summary>
    /// SaveSystem provides methods for saving and loading game data via JSON serialization.
    /// It abstracts persistence logic away from the rest of the game to adhere to SOLID principles.
    /// </summary>
    public static class SaveSystem
    {
        #region Constants

        private const string SaveKey = "SaveData";

        #endregion

        #region Public Methods

        /// <summary>
        /// Saves the provided game data into PlayerPrefs as a JSON string.
        /// </summary>
        /// <param name="gameData">An instance of SaveData containing the data to be saved.</param>
        public static void SaveGame(SaveData gameData)
        {
            // Convert the save data to a JSON string
            string jsonData = JsonUtility.ToJson(gameData);

            // Store the JSON string in PlayerPrefs
            PlayerPrefs.SetString(SaveKey, jsonData);

            // Optionally save the PlayerPrefs immediately
            PlayerPrefs.Save();

            Debug.Log("Save data saved successfully.");
        }

        /// <summary>
        /// Loads the game data from PlayerPrefs.
        /// If no data exists, it returns a default instance of SaveData.
        /// </summary>
        /// <returns>An instance of SaveData representing the loaded data.</returns>
        public static SaveData LoadGame()
        {
            if (!PlayerPrefs.HasKey(SaveKey))
            {
                Debug.LogWarning("No save data found. Returning default SaveData.");
                return new SaveData(); // Return default data if no save exists
            }

            // Retrieve the JSON string from PlayerPrefs
            string jsonData = PlayerPrefs.GetString(SaveKey);

            // Convert the JSON string back to a SaveData object
            SaveData saveData = JsonUtility.FromJson<SaveData>(jsonData);

            Debug.Log("Game data loaded successfully.");
            return saveData;
        }

        #endregion
    }
}