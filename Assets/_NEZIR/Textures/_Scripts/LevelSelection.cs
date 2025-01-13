using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelSelection : MonoBehaviour
{
    
    [SerializeField] private Level selectedLevel;


    public void SetLevel(Level level)
    {
        selectedLevel = level;
    }
    
}
