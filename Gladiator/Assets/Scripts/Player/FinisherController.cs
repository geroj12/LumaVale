using System.Collections;
using UnityEngine;
using Unity.Cinemachine;
using static FinisherData;
using System.Linq;


public class FinisherController : MonoBehaviour
{
    [SerializeField] private WeaponFinishers currentWeaponFinishers;
    [SerializeField] private FinisherDetector detector;
    [SerializeField] private FinisherExecutor executor;
    [SerializeField] private WeaponHolster weaponHolster;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            if (detector.TryGetFinisherTarget(out Enemy enemy))
            {
                var dynamicAnchor = enemy.GetComponent<DynamicFinisherAnchor>();
                if (dynamicAnchor != null)
                {
                    dynamicAnchor.UpdateAnchorToPlayer(transform); // "transform" = Spieler
                }
                
                bool isFatal = enemy.GetComponent<EnemyHealth>().HealthPercent <= enemy.GetComponent<EnemyHealth>().fatalFinisherThreshold;
                FinisherData finisher = GetValidFinisher(isFatal);
                if (finisher != null)
                    executor.ExecuteFinisher(enemy, finisher);
                else
                    Debug.LogWarning("Kein gültiger Finisher gefunden für aktuellen Waffentyp.");
            }
        }
    }
    private FinisherData GetValidFinisher(bool isFatal)
    {
        if (currentWeaponFinishers == null || currentWeaponFinishers.finishers.Count == 0)
            return null;

        WeaponType currentType = (WeaponType)weaponHolster.currentWeaponType;

        var matches = currentWeaponFinishers.finishers
            .Where(f => f.isFatal == isFatal && f.weaponType == currentType)
            .ToList();

        return matches.Count > 0 ? matches[Random.Range(0, matches.Count)] : null;
    }
}
