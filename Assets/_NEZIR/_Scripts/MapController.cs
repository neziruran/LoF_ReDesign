using System.Collections.Generic;
using UnityEngine;

public class MapController : MonoBehaviour
{
    [Header("Map References")]
    [SerializeField] private GameObject earthMap;   // Earth map GameObject
    [SerializeField] private GameObject lavaMap;    // Lava map GameObject
    [SerializeField] private GameObject waterMap;   // Water map GameObject
    [SerializeField] private GameObject airMap;     // Air map GameObject

    private Dictionary<Map, GameObject> _mapDictionary;

    private void Awake()
    {
        // Initialize the map dictionary for easier management
        _mapDictionary = new Dictionary<Map, GameObject>
        {
            { Map.Earth, earthMap },
            { Map.Lava, lavaMap },
            { Map.Water, waterMap },
            { Map.Air, airMap }
        };

        ActivateSelectedMap();
    }

    /// <summary>
    /// Activates the selected map based on the LevelSelection data.
    /// </summary>
    private void ActivateSelectedMap()
    {
        if (LevelSelection.Instance == null)
        {
            Debug.LogError("LevelSelection instance is missing! Defaulting to Earth map.");
            SetActiveMap(Map.Earth);
            return;
        }

        // Get the selected map from the LevelSelection singleton
        Map selectedMap = LevelSelection.Instance.SelectedMap;
        SetActiveMap(selectedMap);
    }

    /// <summary>
    /// Activates the selected map and deactivates all others.
    /// </summary>
    /// <param name="mapName">The name of the map to activate (Earth, Lava, Water, Air).</param>
    public void SetActiveMap(Map mapName)
    {
        foreach (var map in _mapDictionary)
        {
            // Activate the selected map and deactivate all others
            map.Value.SetActive(map.Key == mapName);
        }
    }
}