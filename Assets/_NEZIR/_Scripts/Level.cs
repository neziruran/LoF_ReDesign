using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private SpriteRenderer levelRenderer; // Reference to the SpriteRenderer for the level
    [SerializeField] private SpriteRenderer gemRenderer;   // Reference to the SpriteRenderer for the gem (manual control)
    [SerializeField] private LevelSelection levelSelection;

    private readonly HashSet<GameObject> _playersInTrigger = new HashSet<GameObject>();

    private Coroutine _levelAlphaCoroutine; // To manage level alpha animations
    private Coroutine _gemAlphaCoroutine;   // To manage gem alpha animations

    private void Start()
    {
        levelSelection = GetComponentInParent<LevelSelection>();
        ResetLevel(); // Initialize the level sprite to the deactivated state
    }

    /// <summary>
    /// Resets the level visuals to the deactivated state.
    /// </summary>
    public void ResetLevel()
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
    /// </summary>
    private void ActivateLevel()
    {
        levelSelection.SetLevel(this);
        StartLerp(levelRenderer, levelSelection.ActivatedAlpha);
    }

    /// <summary>
    /// Deactivates the level sprite, animating its alpha value to the deactivated state.
    /// </summary>
    private void DeActivateLevel()
    {
        levelSelection.SetLevel(null);
        StartLerp(levelRenderer, levelSelection.DeactivateAlpha);
    }

    /// <summary>
    /// Activates the level and gem sprite, animating their alpha values to the activated state.
    /// </summary>
    public void OnLevelAccomplished()
    {
        StartLerp(levelRenderer, levelSelection.ActivatedAlpha);
        StartLerp(gemRenderer, levelSelection.ActivatedAlpha);
    }

    /// <summary>
    /// Smoothly transitions the alpha value of a SpriteRenderer using a coroutine.
    /// Ensures no multiple coroutines interfere with each other.
    /// </summary>
    /// <param name="spriteRenderer">The SpriteRenderer to animate.</param>
    /// <param name="targetAlpha">The desired alpha value (0-1).</param>
    private void StartLerp(SpriteRenderer spriteRenderer, float targetAlpha)
    {
        // Stop any existing coroutine for this SpriteRenderer
        if (spriteRenderer == levelRenderer && _levelAlphaCoroutine != null)
        {
            StopCoroutine(_levelAlphaCoroutine);
        }
        else if (spriteRenderer == gemRenderer && _gemAlphaCoroutine != null)
        {
            StopCoroutine(_gemAlphaCoroutine);
        }

        // Start a new coroutine
        if (spriteRenderer == levelRenderer)
        {
            _levelAlphaCoroutine = StartCoroutine(LerpAlpha(spriteRenderer, targetAlpha, levelSelection.AnimationDuration));
        }
        else if (spriteRenderer == gemRenderer)
        {
            _gemAlphaCoroutine = StartCoroutine(LerpAlpha(spriteRenderer, targetAlpha, levelSelection.AnimationDuration));
        }
    }

    /// <summary>
    /// Coroutine to smoothly transition the alpha value of a SpriteRenderer.
    /// </summary>
    /// <param name="spriteRenderer">The SpriteRenderer to animate.</param>
    /// <param name="targetAlpha">The desired alpha value (0-1).</param>
    /// <param name="duration">The duration of the animation.</param>
    private IEnumerator LerpAlpha(SpriteRenderer spriteRenderer, float targetAlpha, float duration)
    {
        if (spriteRenderer == null) yield break; // Ensure the SpriteRenderer is not null

        float time = 0f;
        Color startColor = spriteRenderer.color; // Capture the current color of the SpriteRenderer
        float startAlpha = startColor.a;         // Get the current alpha value

        while (time < duration)
        {
            // Smoothly interpolate the alpha value over the duration
            float newAlpha = Mathf.Lerp(startAlpha, targetAlpha, time / duration);
            Color newColor = startColor;
            newColor.a = newAlpha; // Update the alpha channel
            spriteRenderer.color = newColor;

            time += Time.deltaTime; // Increment time
            yield return null;      // Wait for the next frame
        }

        // Ensure the final alpha value is set
        Color finalColor = spriteRenderer.color;
        finalColor.a = targetAlpha;
        spriteRenderer.color = finalColor;
    }

    private void OnTriggerEnter(Collider other)
    {
        var isPlayer = other.TryGetComponent(out PlayerMovement _);

        // Check if the object entering the trigger is a player
        if (isPlayer)
        {
            _playersInTrigger.Add(other.gameObject);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        // Check the number of players in the trigger area
        if (_playersInTrigger.Count >= levelSelection.RequiredPlayerToActivation)
        {
            ActivateLevel();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        var isPlayer = other.TryGetComponent(out PlayerMovement _);
        // Remove the player from the set when they exit the trigger

        if (isPlayer)
        {
            _playersInTrigger.Remove(other.gameObject);
            if (_playersInTrigger.Count < levelSelection.RequiredPlayerToActivation)
            {
                DeActivateLevel();
            }
        }
    }
}
