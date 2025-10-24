using UnityEngine;

public class CombatInputHandler : MonoBehaviour
{
    public bool MouseLeftDown { get; private set; }
    public bool MouseLeftUp { get; private set; }
    public bool MouseLeftHeld { get; private set; }

    public bool BlockPressed { get; private set; }
    public bool BlockHeld { get; private set; }
    public bool BlockReleased { get; private set; }

    public bool ScrollUp { get; private set; }
    public bool ScrollDown { get; private set; }

    private float lastScroll;

    public void UpdateInput()
    {
        MouseLeftDown = Input.GetMouseButtonDown(0);
        MouseLeftUp = Input.GetMouseButtonUp(0);
        MouseLeftHeld = Input.GetMouseButton(0);

        BlockPressed = Input.GetMouseButtonDown(1);
        BlockHeld = Input.GetMouseButton(1);
        BlockReleased = Input.GetMouseButtonUp(1);

        float scroll = Input.GetAxis("Mouse ScrollWheel");
        ScrollUp = scroll > 0f && lastScroll == 0f;
        ScrollDown = scroll < 0f && lastScroll == 0f;
        lastScroll = scroll;
    }
}