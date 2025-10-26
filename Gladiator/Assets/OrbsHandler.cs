using System;
using UnityEngine;

public class OrbsHandler : MonoBehaviour
{
    [SerializeField] private int maxOrbs = 5;

    private int currentOrbs;

    public int CurrentOrbs => currentOrbs;
    public int MaxOrbs => maxOrbs;

    public void AddOrb()
    {
        if (currentOrbs >= maxOrbs) return;

        currentOrbs++;
    }

  
}
