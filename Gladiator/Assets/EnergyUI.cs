using UnityEngine;
using UnityEngine.UI;

public class EnergyUI : MonoBehaviour
{
    [SerializeField] private Slider energySlider;
    [SerializeField] private State playerState;
    
    
    [System.Obsolete]
    private void Start()
    {
        if (playerState == null)
            playerState = FindObjectOfType<State>();

        energySlider.maxValue = playerState.maxEnergy;
    }

    private void Update()
    {
        energySlider.value = playerState.currentEnergy;
    }


   
}
