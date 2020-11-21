using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class DOColor : DOBase
{
    public Color startColor = Color.white;
    public Color endColor = Color.white;

    Image sourceImage;

    internal override void VirtualEnable()
    {
        sourceImage = GetComponent<Image>();
    }
    public override void DO()
    {
        if (Application.isPlaying)
        {
            sourceImage.DOColor(endColor, duration).SetDelay(doDelay).SetEase(ease).OnComplete(() => DOComplete.Invoke());
        }
        else
        {
            GetComponent<Image>().color = endColor;
        }
    }
    public override void DORevert()
    {
        if (Application.isPlaying)
        {
            sourceImage.DOColor(startColor, duration).SetDelay(revertDelay).SetEase(ease).OnComplete(() => DORevertComplete.Invoke());
        }
        else
        {
            GetComponent<Image>().color = startColor;
        }
    }
    public override void ResetDO()
    {
        sourceImage.DOPause();
        sourceImage.color = startColor;
    }
    public override void DOLoop()
    {
        if (Application.isPlaying)
        {
            sourceImage.DOColor(startColor, duration).SetDelay(revertDelay).SetEase(ease).SetLoops(-1,loopType);
        }
        else
        {
            GetComponent<Image>().color = startColor;
        }
    }
}
