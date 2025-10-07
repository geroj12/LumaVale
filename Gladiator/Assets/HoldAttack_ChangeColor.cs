using UnityEngine;
using UnityEngine.UI;

public class HoldAttack_ChangeColor : MonoBehaviour
{
    [SerializeField] Image holdImage;
    [SerializeField] State playerState;
    [Header("Hold Attack Color Feedback")]
    [SerializeField] public Color startColor = Color.cyan;
    [SerializeField] private Color midColor = Color.yellow;
    [SerializeField] private Color endColor = Color.red;

    void Update()
    {
        if (!playerState.holdingAttack)
        {
            holdImage.color = startColor;
        }
    }

    public void UpdateColor(float t)
    {

        if (holdImage != null)
        {
            Color targetColor = (t < 0.5f)
                ? Color.Lerp(startColor, midColor, t * 2f)
                : Color.Lerp(midColor, endColor, (t - 0.5f) * 2f);
            holdImage.color = targetColor;
        }


    }

}
