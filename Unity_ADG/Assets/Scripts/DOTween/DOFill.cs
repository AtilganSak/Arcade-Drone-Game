using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class DOFill : DOBase
{
    public float startValue;
    public float endValue;

    Image sourceImage;

    internal override void VirtualEnable()
    {
        sourceImage = GetComponent<Image>();
    }
    public override void DO()
    {
        if (Application.isPlaying)
        {
            sourceImage.DOFillAmount(endValue, duration).SetDelay(doDelay).SetEase(ease).OnComplete(() => DOComplete.Invoke());
        }
        else
        {
            GetComponent<Image>().fillAmount = endValue;
        }
    }
    public override void DORevert()
    {
        if (Application.isPlaying)
        {
            sourceImage.DOFillAmount(startValue, duration).SetDelay(revertDelay).SetEase(ease).OnComplete(() => DORevertComplete.Invoke());
        }
        else
        {
            GetComponent<Image>().fillAmount = startValue;
        }
    }
    public override void ResetDO()
    {
        sourceImage.DOPause();
        sourceImage.fillAmount = startValue;
    }
    public override void DOLoop()
    {
        if (Application.isPlaying)
        {
            sourceImage.DOFillAmount(endValue, duration).SetDelay(doDelay).SetEase(ease).SetLoops(-1, loopType);
        }
        else
        {
            GetComponent<Image>().fillAmount = endValue;
        }
    }
}
