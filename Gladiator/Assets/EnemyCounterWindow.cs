using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyCounterWindow : MonoBehaviour
{
    [Header("Counter Settings")]
    [SerializeField] private float activeDuration = 1.5f; // wie lange das Konterfenster offen ist
    [SerializeField] private float cooldown = 1.0f;       // Cooldown, bevor wieder konterbar

    public bool IsActive { get; private set; }
    public bool IsOnCooldown { get; private set; }

    public event Action OnCounterWindowOpened;
    public event Action OnCounterWindowClosed;
    public event Action OnCounterTriggered;

    private Coroutine counterRoutine;

    /// <summary>
    /// Wird aufgerufen, wenn der Spieler blockt (z.B. RMB gedrückt).
    /// </summary>
    public void TryActivate()
    {
        if (IsOnCooldown || IsActive)
            return;

        if (counterRoutine != null)
            StopCoroutine(counterRoutine);

        counterRoutine = StartCoroutine(CounterRoutine());
    }

    private IEnumerator CounterRoutine()
    {
        // Fenster öffnen
        IsActive = true;
        OnCounterWindowOpened?.Invoke();

        // Aktive Phase
        yield return new WaitForSeconds(activeDuration);

        // Fenster schließt sich automatisch
        IsActive = false;
        OnCounterWindowClosed?.Invoke();

        // Cooldown starten
        IsOnCooldown = true;
        yield return new WaitForSeconds(cooldown);
        IsOnCooldown = false;
    }

    /// <summary>
    /// Wird vom EnemyWeapon aufgerufen, wenn der Spieler erfolgreich kontert.
    /// </summary>
    public void TriggerCounter()
    {
        if (!IsActive) return;

        IsActive = false;
        OnCounterTriggered?.Invoke();
        OnCounterWindowClosed?.Invoke();

        // Direkt Cooldown starten
        if (counterRoutine != null)
            StopCoroutine(counterRoutine);
        StartCoroutine(CooldownRoutine());
    }

    private IEnumerator CooldownRoutine()
    {
        IsOnCooldown = true;
        yield return new WaitForSeconds(cooldown);
        IsOnCooldown = false;
    }
}

