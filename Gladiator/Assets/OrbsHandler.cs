using System;
using UnityEngine;

public class OrbsHandler : MonoBehaviour
{
    [Header("Energy Settings")]
    [SerializeField] private int maxOrbs = 5;
    private int currentOrbs = 0;

    // Event, damit UI oder andere Systeme reagieren können
    public event Action<int, int> OnOrbsChanged;

    public int CurrentOrbs => currentOrbs;
    public int MaxOrbs => maxOrbs;

    public bool IsFull => currentOrbs >= maxOrbs;

    public void AddOrb()
    {
        if (currentOrbs >= maxOrbs) return;

        currentOrbs++;
        OnOrbsChanged?.Invoke(currentOrbs, maxOrbs);
    }

    public void ResetOrbs()
    {
        currentOrbs = 0;
        OnOrbsChanged?.Invoke(currentOrbs, maxOrbs);
    }
}
