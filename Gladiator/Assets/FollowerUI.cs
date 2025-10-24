using TMPro;
using UnityEngine;

public class FollowerUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI orbText;

    [Header("References")]
    [SerializeField] private OrbsHandler orbsHandler;

    void Start()
    {
        if (orbsHandler != null)
        {
            orbsHandler.OnOrbsChanged += UpdateOrbDisplay;
            UpdateOrbDisplay(orbsHandler.CurrentOrbs, orbsHandler.MaxOrbs);
        }
    }

    private void OnDestroy()
    {
        if (orbsHandler != null)
            orbsHandler.OnOrbsChanged -= UpdateOrbDisplay;
    }

    private void UpdateOrbDisplay(int current, int max)
    {
        orbText.text = $"Collected Orbs: {current} / {max}";
    }
}
