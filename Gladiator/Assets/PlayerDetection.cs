using UnityEngine;

public class PlayerDetection : MonoBehaviour
{
    public FMODMusic music;
    private bool isInCombatMusic = false;
    public bool isAttacking;

    void Update()
    {
        if (isAttacking)
        {
            music.SetMusicState("Combat");
            isInCombatMusic = true;
        }
        else if (!isAttacking)
        {
            music.SetMusicState("Main");
            isInCombatMusic = false;
        }
    }

    bool IsAttackingPlayer()
    {
        return isAttacking == true;
    }
}
