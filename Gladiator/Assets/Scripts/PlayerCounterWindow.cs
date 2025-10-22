using System;
using System.Collections;
using UnityEngine;

public class PlayerCounterWindow : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField, Tooltip("Wie lange der Counter aktiv bleibt.")]
    private float activeDuration = 1.2f;

    [SerializeField, Tooltip("Cooldown nach Ablauf des Counters.")]
    private float cooldown = 1.0f;

    public bool IsActive { get; private set; }
    public bool IsOnCooldown { get; private set; }

    public event Action OnCounterWindowOpened;
    public event Action OnCounterWindowClosed;
    public event Action OnCounterTriggered;

    private Coroutine counterRoutine;
    public void TryActivate()
    {
        if (IsOnCooldown || IsActive)
            return;

        if (counterRoutine != null)
            StopCoroutine(counterRoutine);

        counterRoutine = StartCoroutine(CounterRoutine());
    }

    public void TriggerCounter()
    {
        if (!IsActive)
            return;

        IsActive = false;
        OnCounterTriggered?.Invoke();
        OnCounterWindowClosed?.Invoke();

        // Sofort Cooldown starten
        if (counterRoutine != null)
            StopCoroutine(counterRoutine);

        counterRoutine = StartCoroutine(CooldownRoutine());
    }
    private IEnumerator CounterRoutine()
    {
        // Counter-Fenster aktivieren
        IsActive = true;
        OnCounterWindowOpened?.Invoke();

        yield return new WaitForSeconds(activeDuration);

        // Fenster schließen
        IsActive = false;
        OnCounterWindowClosed?.Invoke();

        // Cooldown starten
        yield return StartCoroutine(CooldownRoutine());
    }

    private IEnumerator CooldownRoutine()
    {
        IsOnCooldown = true;
        yield return new WaitForSeconds(cooldown);
        IsOnCooldown = false;
    }

}
