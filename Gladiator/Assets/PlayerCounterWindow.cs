using System;
using System.Collections;
using UnityEngine;

public class PlayerCounterWindow : MonoBehaviour
{
    [Header("Counter Window Settings")]
    [SerializeField] private float activeDuration = 1.2f;
    [SerializeField] private float cooldown = 1.0f;

    public bool IsActive { get; private set; } = false;
    public bool IsOnCooldown { get; private set; } = false;

    public event Action OnCounterWindowOpened;
    public event Action OnCounterWindowClosed;

    private Coroutine activeRoutine;

    public void TryActivate()
    {
        if (IsOnCooldown) return;

        if (activeRoutine != null)
            StopCoroutine(activeRoutine);

        activeRoutine = StartCoroutine(CounterRoutine());
    }

    private IEnumerator CounterRoutine()
    {
        IsActive = true;
        OnCounterWindowOpened?.Invoke();

        yield return new WaitForSeconds(activeDuration);

        IsActive = false;
        OnCounterWindowClosed?.Invoke();

        // Cooldown starten
        IsOnCooldown = true;
        yield return new WaitForSeconds(cooldown);
        IsOnCooldown = false;
    }
}
