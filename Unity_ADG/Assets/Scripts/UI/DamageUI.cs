using UnityEngine;
using UnityEngine.UI;

public class DamageUI : MonoBehaviour
{
    public DamageText damageText;
    public DOFade damageScreenEffect;
    public float damageTextPosRadius = 50;

    Image damageScreenEffectImage;

    private void OnEnable()
    {
        if (damageScreenEffect != null)
        {
            damageScreenEffectImage = damageScreenEffect.GetComponent<Image>();
            damageScreenEffect.DOComplete.RemoveAllListeners();
            damageScreenEffect.DOComplete.AddListener(() => damageScreenEffect.enabled = false);
        }
    }
    public void AppearDamageScreenEffect()
    {
        if (damageScreenEffect != null)
        {
            damageScreenEffect.ResetDO();
            damageScreenEffectImage.enabled = true;
            damageScreenEffect.DO();
        }
    }
    public void HideDamageScreenEffect()
    {
        if (damageScreenEffect != null)
        {
            damageScreenEffect.ResetDO();
            damageScreenEffectImage.enabled = false;
        }
    }
    public void AppearDamageText(float damageValue)
    {
        if (damageText != null)
        {
            damageText.StopAnimation();
            damageText.c_Transform.anchoredPosition = new Vector2(Random.Range(-(damageTextPosRadius * 2), (damageTextPosRadius * 2)), Random.Range(-(damageTextPosRadius * 2), (damageTextPosRadius * 2)));
            damageText.SetValue(damageValue);
            damageText.Appear();
        }
    }
    public void HideDamageText()
    {
        if (damageText != null)
        {
            damageText.StopAnimation();
            damageText.Hide();
        }
    }
}
