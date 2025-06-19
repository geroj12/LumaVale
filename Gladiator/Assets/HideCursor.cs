using UnityEngine;

public class HideCursor : MonoBehaviour
{


    void Start()
    {
        Cursor.visible = false;                      // Nur ausblenden
        Cursor.lockState = CursorLockMode.None;
    }


}
