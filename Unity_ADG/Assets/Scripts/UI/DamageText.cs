using UnityEngine;
using TMPro;

public class DamageText : MonoBehaviour
{
    public bool autoPlay = true;

    public RectTransform c_Transform { get; private set; }

    DOFade DOFade;
    TMP_Text tmp_Text;

    float showedValue;

    private void OnEnable()
    {
        DOFade = GetComponent<DOFade>();
        if (DOFade == null) enabled = false;
        tmp_Text = GetComponent<TMP_Text>();
        if (tmp_Text == null) enabled = false;
        c_Transform = GetComponent<RectTransform>();

        DOFade.DOComplete.RemoveAllListeners();
        DOFade.DOComplete.AddListener(() => tmp_Text.enabled = false);
    }
    private void Start()
    {
        if (autoPlay)
            DOFade.DO();
    }
    public void Appear()
    {
        tmp_Text.enabled = true;

        DOFade.ResetDO();
        DOFade.DO();
    }
    public void Hide()
    {
        tmp_Text.enabled = false;
    }
    public void SetValue(float value)
    {
        if (value.Equals(showedValue)) return;

        tmp_Text.text = "-" + value.ToString();
        showedValue = value;
    }
    public void StopAnimation()
    {
        DOFade.ResetDO();
    }
}
