using DG.Tweening;
using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class DOAnchorPos : DOBase
{
    public Vector2 startValue;
    public Vector2 endValue = Vector2.one;

    RectTransform  c_TransformRect;

    internal override void VirtualEnable()
    {
        c_TransformRect = GetComponent<RectTransform>();
    }
    public override void DO()
    {
        if (Application.isPlaying)
        {
            c_TransformRect.DOAnchorPos(endValue, duration).SetDelay(doDelay).SetEase(ease).OnComplete(() => DOComplete.Invoke());
        }
        else
        {
            GetComponent<RectTransform>().anchoredPosition = endValue;
        }
    }
    public override void DORevert()
    {
        if (Application.isPlaying)
        {
            c_TransformRect.DOAnchorPos(startValue, duration).SetDelay(revertDelay).SetEase(ease).OnComplete(() => DORevertComplete.Invoke());
        }
        else
        {
            GetComponent<RectTransform>().anchoredPosition = startValue;
        }
    }
    public override void ResetDO()
    {
#if UNITY_EDITOR
        Undo.RecordObject(gameObject, name + "Changed transform");
#endif
        c_TransformRect.DOPause();
        c_TransformRect.anchoredPosition = startValue;
#if UNITY_EDITOR
        EditorUtility.SetDirty(this);
#endif
    }
    public override void DOLoop()
    {
        if (Application.isPlaying)
        {
            c_TransformRect.DOAnchorPos(endValue, duration).SetDelay(doDelay).SetEase(ease).SetLoops(-1, loopType);
        }
        else
        {
            GetComponent<RectTransform>().anchoredPosition = endValue;
        }
    }
}
