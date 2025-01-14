using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private SpriteRenderer levelRenderer; // Reference to the SpriteRenderer for the level
    [SerializeField] private SpriteRenderer gemRenderer;   // Reference to the SpriteRenderer for the gem (manual control)
    [SerializeField] private LevelSelection levelSelection;
    [SerializeField] private Map map;

    private readonly HashSet<GameObject> _playersInTrigger = new HashSet<GameObject>();

    private Coroutine _activationCoroutine; // Coroutine to handle the map loading timer

    private void Start()
    {
        levelSelection = FindObjectOfType<LevelSelection>();
        ResetLevel(); // Initialize the level sprite to the deactivated state
    }

    /// <summary>
    /// Resets the level visuals to the deactivated state.
    /// </summary>
    private void ResetLevel()
    {
        if (levelRenderer != null)
        {
            StartLerp(levelRenderer, levelSelection.DeactivateAlpha);
        }

        if (gemRenderer != null)
        {
            StartLerp(gemRenderer, levelSelection.DeactivateAlpha);
        }
    }

    /// <summary>
    /// Activates the level sprite, animating its alpha value to the activated state.
    /// Starts a countdown to load the main scene if players remain within the trigger area.
    /// </summary>
    private void ActivateLevel()
    {
        if (_activationCoroutine == null)
        {
            _activationCoroutine = StartCoroutine(StartActivationCountdown());
        }
    }

    /// <summary>
    /// Deactivates the level sprite, animating its alpha value to the deactivated state.
    /// Cancels the map loading countdown if players leave the trigger area.
    /// </summary>
    private void DeActivateLevel()
    {
        if (_activationCoroutine != null)
        {
            StopCoroutine(_activationCoroutine);
            _activationCoroutine = null;
        }
        StartLerp(levelRenderer, levelSelection.DeactivateAlpha);
    }

    /// <summary>
    /// Coroutine to handle the activation countdown.
    /// Waits for a specified duration and loads the map if players don't exit.
    /// </summary>
    private IEnumerator StartActivationCountdown()
    {
        StartLerp(levelRenderer, levelSelection.ActivatedAlpha);

        yield return new WaitForSeconds(levelSelection.AnimationDuration); // Optional fade-in duration

        yield return new WaitForSeconds(3f); // Wait for the full activation time

        // Ensure the players are still in the trigger area
        if (_playersInTrigger.Count >= levelSelection.RequiredPlayerToActivation)
        {
            levelSelection.SetSelectedMap(map);
        }

        _activationCoroutine = null; // Clear the coroutine reference
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out PlayerMovement _))
        {
            _playersInTrigger.Add(other.gameObject);

            if (_playersInTrigger.Count >= levelSelection.RequiredPlayerToActivation)
            {
                ActivateLevel();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out PlayerMovement _))
        {
            _playersInTrigger.Remove(other.gameObject);

            if (_playersInTrigger.Count < levelSelection.RequiredPlayerToActivation)
            {
                DeActivateLevel();
            }
        }
    }

    private void StartLerp(SpriteRenderer spriteRenderer, float targetAlpha)
    {
        if (spriteRenderer == null) return;

        IEnumerator LerpAlpha(float duration)
        {
            float time = 0f;
            float startAlpha = spriteRenderer.color.a;
            while (time < duration)
            {
                float newAlpha = Mathf.Lerp(startAlpha, targetAlpha, time / duration);
                var newColor = spriteRenderer.color;
                newColor.a = newAlpha;
                spriteRenderer.color = newColor;

                time += Time.deltaTime;
                yield return null;
            }
        }

        StartCoroutine(LerpAlpha(levelSelection.AnimationDuration));
    }
}
