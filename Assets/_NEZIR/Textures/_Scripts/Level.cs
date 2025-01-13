using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level : MonoBehaviour
{
    [SerializeField] private SpriteRenderer levelRenderer; // Reference to the SpriteRenderer for the level
    [SerializeField] private SpriteRenderer gemRenderer;   // Reference to the SpriteRenderer for the gem (manual control)
    [SerializeField] private float deactivateAlpha = 0.75f; // Alpha value for deactivated state (normalized 0-1)
    [SerializeField] private float activatedAlpha = 1.0f;  // Alpha value for activated state (normalized 0-1)
    [SerializeField] private float animationDuration = 0.5f; // Duration of the transition animation
    [SerializeField] private LevelSelection levelSelection;
    [SerializeField] private int requiredPlayerToActivation = 4;

    private HashSet<GameObject> playersInTrigger = new HashSet<GameObject>();

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
            Color color = levelRenderer.color;
            color.a = deactivateAlpha; // Set alpha to the deactivated value
            levelRenderer.color = color;
        }
        
        if (gemRenderer != null)
        {
            Color color = gemRenderer.color;
            color.a = deactivateAlpha; // Set alpha to the deactivated value
            gemRenderer.color = color;
        }
    }

    /// <summary>
    /// Activates the level sprite, animating its alpha value to the activated state.
    /// </summary>
    private void ActivateLevel()
    {
        levelSelection.SetLevel(this);
        StartCoroutine(LerpAlpha(levelRenderer, activatedAlpha, animationDuration));
    }
    
    private void DeActivateLevel()
    {
        levelSelection.SetLevel(null);
        StartCoroutine(LerpAlpha(levelRenderer, deactivateAlpha, animationDuration));
    }

    /// <summary>
    /// Activates the level and gem sprite, animating its alpha value to the activated state.
    /// </summary>
    public void OnLevelAccomplished()
    {
        StartCoroutine(LerpAlpha(levelRenderer, activatedAlpha, animationDuration));
        StartCoroutine(LerpAlpha(gemRenderer, activatedAlpha, animationDuration));
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

        float time = 0;
        Color startColor = spriteRenderer.color; // Capture the starting color
        float startAlpha = startColor.a; // Extract the starting alpha value

        while (time < duration)
        {
            float newAlpha = Mathf.Lerp(startAlpha, targetAlpha, time / duration); // Calculate the new alpha
            Color newColor = startColor;
            newColor.a = newAlpha; // Update the alpha of the color
            spriteRenderer.color = newColor; // Apply the new color to the SpriteRenderer

            time += Time.deltaTime; // Increment time
            yield return null; // Wait for the next frame
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
            playersInTrigger.Add(other.gameObject);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        // Check the number of players in the trigger area
        if (playersInTrigger.Count >= requiredPlayerToActivation)
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
            playersInTrigger.Remove(other.gameObject);
            if (playersInTrigger.Count < requiredPlayerToActivation)
            {
                DeActivateLevel();
            }
        }
    }
}
