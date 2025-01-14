using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelSelection : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private Level selectedLevel;
    [SerializeField] private float deactivateAlpha = 0.75f; // Alpha value for deactivated state (normalized 0-1)
    [SerializeField] private float activatedAlpha = 1.0f;  // Alpha value for activated state (normalized 0-1)
    [SerializeField] private float animationDuration = 0.5f; // Duration of the transition animation
    [SerializeField] private int requiredPlayerToActivation = 4;

    public float DeactivateAlpha => deactivateAlpha;
    public float ActivatedAlpha => activatedAlpha;
    public float AnimationDuration => animationDuration;
    public int RequiredPlayerToActivation => requiredPlayerToActivation;

    public void SetLevel(Level level)
    {
        selectedLevel = level;
    }
    
}
