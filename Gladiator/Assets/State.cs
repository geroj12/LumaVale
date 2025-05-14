using UnityEngine;

public class State : MonoBehaviour
{
    public bool Strafe;
    public bool blockedAttack;
    public bool equipped = false;
    public bool blocking = false;

    public bool mouseOnLeftSide = false;
    public bool mouseOnRightSide = false;
    public bool mouseOnTopSide = false;
    public bool mouseOnDownSide = false;

    public void ResetMouseDirections()
    {
        mouseOnLeftSide = false;
        mouseOnRightSide = false;
        mouseOnTopSide = false;
        mouseOnDownSide = false;
    }

}
