using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelSelection : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float deactivateAlpha = 0.75f; // Alpha value for deactivated state (normalized 0-1)
    [SerializeField] private float activatedAlpha = 1.0f;  // Alpha value for activated state (normalized 0-1)
    [SerializeField] private float animationDuration = 0.5f; // Duration of the transition animation
    [SerializeField] private int requiredPlayerToActivation = 4;

    public static LevelSelection Instance { get; private set; } // Singleton instance

    public float DeactivateAlpha => deactivateAlpha;
    public float ActivatedAlpha => activatedAlpha;
    public float AnimationDuration => animationDuration;
    public int RequiredPlayerToActivation => requiredPlayerToActivation;

    [Header("Scene Management")]
    [SerializeField] private string mainSceneName = "MainScene"; // Name of the main scene
    private Map selectedMap = Map.Earth;        // Default selected map

    private bool isSceneLoading = false; // Prevent multiple scene loads

    public Map SelectedMap => selectedMap; // Expose selectedMap for other scripts

    private void Awake()
    {
        // Singleton pattern implementation
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // Ensure there's only one instance
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject); // Make the object persistent across scenes
    }

    /// <summary>
    /// Loads the main scene and ensures the selected map is passed to the MapController.
    /// </summary>
    private void LoadMainScene()
    {
        if (isSceneLoading) return; // Ensure the scene isn't loaded repeatedly
        isSceneLoading = true;

        SceneManager.LoadScene(mainSceneName);
        SceneManager.sceneLoaded += OnMainSceneLoaded;
    }

    private void OnMainSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Ensure this logic only runs in the main scene
        if (scene.name != mainSceneName) return;

        // Find the MapController in the loaded scene
        MapController mapController = FindObjectOfType<MapController>();

        if (mapController != null)
        {
            // Pass the selected map to the MapController
            mapController.SetActiveMap(selectedMap);
        }

        // Unsubscribe from the event to avoid repeated calls
        SceneManager.sceneLoaded -= OnMainSceneLoaded;
    }

    /// <summary>
    /// Sets the selected map and loads the main scene after a delay.
    /// </summary>
    /// <param name="map">The map to set as selected.</param>
    public void SetSelectedMap(Map map)
    {
        if (isSceneLoading) return; // Prevent repeated calls
        selectedMap = map; // Update the selected map
        Invoke(nameof(LoadMainScene), 1f); // Load the main scene after 3 seconds
    }
}
