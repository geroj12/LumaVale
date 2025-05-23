using TMPro;
using UnityEngine;

public class DamageText : MonoBehaviour
{
    public TextMeshProUGUI text;
    private Camera cam;

    public void ShowDamage(float amount, Color color)
    {
        text.text = Mathf.RoundToInt(amount).ToString();
        text.color = color;
        Destroy(gameObject, 1.2f); // nach 1.2s verschwinden
    }
    void Start()
    {
        cam = Camera.main;
    }

    private void Update()
    {
        transform.LookAt(Camera.main.transform);
        transform.position += Vector3.up * Time.deltaTime * 0.5f; // schweben
    }

    void LateUpdate()
    {
        if (cam != null)
        {
            transform.rotation = Quaternion.LookRotation(transform.position - cam.transform.position);
        }
    }
}
