using System;
using UnityEngine;

/// <summary>
/// SaveData holds the data that will be saved and loaded by the SaveSystem.
/// For now, this includes the CurrentLevelIndex, but can be expanded as the game grows.
/// </summary>
[Serializable]
public class SaveData
{
    [Header("Game Progress")]
    [Tooltip("The index of the current level the player is on.")]
    [SerializeField]
    public int CurrentLevelIndex = 0;
}