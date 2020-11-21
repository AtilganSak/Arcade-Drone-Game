using UnityEngine;
using UnityEngine.UI;

public class HealtUI : MonoBehaviour
{
    public Gradient gradient;

    Image image;

    private void OnEnable()
    {
        image = GetComponent<Image>();
    }
    public void SetValue(float value)
    {
        if (value > 1) value = 1;
        if (value < 0) value = 0;
        image.fillAmount = value;
        image.color = gradient.Evaluate(value);
    }
}
