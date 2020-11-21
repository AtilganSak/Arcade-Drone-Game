using DG.Tweening;
using UnityEngine;

public class DOMove : DOBase
{
    public Vector3 startValue;
    public Vector3 endValue = Vector3.one;

    public bool local;

    public override void DO()
    {
        if (Application.isPlaying)
        {
            if (!local)
                c_Transform.DOMove(endValue, duration).SetDelay(doDelay).SetEase(ease).OnComplete(() => DOComplete.Invoke());
            else
                c_Transform.DOLocalMove(endValue, duration).SetDelay(doDelay).SetEase(ease).OnComplete(() => DOComplete.Invoke());
        }
        else
        {
            if (!local)
                transform.position = endValue;
            else
                transform.localPosition = endValue;
        }
    }
    public override void DORevert()
    {
        if (Application.isPlaying)
        {
            if (!local)
                c_Transform.DOMove(startValue, duration).SetDelay(revertDelay).SetEase(ease).OnComplete(() => DORevertComplete.Invoke());
            else
                c_Transform.DOLocalMove(startValue, duration).SetDelay(revertDelay).SetEase(ease).OnComplete(() => DORevertComplete.Invoke());
        }
        else
        {
            if (!local)
                transform.position = startValue;
            else
                transform.localPosition = startValue;
        }
    }
    public override void ResetDO()
    {
        transform.DOPause();
        if (!local)
            transform.position = startValue;
        else
            transform.localPosition = startValue;
    }
    public override void DOLoop()
    {
        if (Application.isPlaying)
        {
            if (!local)
                c_Transform.DOMove(endValue, duration).SetDelay(doDelay).SetEase(ease).SetLoops(-1, loopType);
            else
                c_Transform.DOLocalMove(endValue, duration).SetDelay(doDelay).SetEase(ease).SetLoops(-1, loopType);
        }
        else
        {
            if (!local)
                transform.position = endValue;
            else
                transform.localPosition = endValue;
        }
    }
}
