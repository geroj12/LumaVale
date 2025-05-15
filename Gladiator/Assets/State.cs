using UnityEngine;

public class State : MonoBehaviour
{
    public bool Strafe;
    public bool blockedAttack;
    public bool equipped = false;
    public bool blocking = false;

    public bool mouseOnLeftSide = false;
    public bool mouseOnRightSide = false;
    public bool attackUp = false;
    public bool attackThrust = false;

    public void ResetMouseDirections()
    {
        mouseOnLeftSide = false;
        mouseOnRightSide = false;
        attackThrust = false;
        attackUp = false;

    }
}
