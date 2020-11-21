using DG.Tweening;
using UnityEngine;

public class DORotate : DOBase
{
    public Vector3 startValue;
    public Vector3 endValue = Vector3.one;

    public bool local;

    public override void DO()
    {
        if (Application.isPlaying)
        {
            if (!local)
                c_Transform.DORotate(endValue, duration).SetDelay(doDelay).SetEase(ease).OnComplete(() => DOComplete.Invoke());
            else
                c_Transform.DOLocalRotate(endValue, duration).SetDelay(doDelay).SetEase(ease).OnComplete(() => DOComplete.Invoke());
        }
        else
        {
            if (!local)
                transform.rotation = Quaternion.Euler(endValue);
            else
                transform.localRotation = Quaternion.Euler(endValue);
        }
    }
    public override void DORevert()
    {
        if (Application.isPlaying)
        {
            if (!local)
                c_Transform.DORotate(startValue, duration).SetDelay(revertDelay).SetEase(ease).OnComplete(() => DORevertComplete.Invoke());
            else
                c_Transform.DOLocalRotate(startValue, duration).SetDelay(revertDelay).SetEase(ease).OnComplete(() => DORevertComplete.Invoke());
        }
        else
        {
            if (!local)
                transform.rotation = Quaternion.Euler(startValue);
            else
                transform.localRotation = Quaternion.Euler(startValue);
        }
    }
    public override void ResetDO()
    {
        transform.DOPause();
        if (!local)
            transform.rotation =Quaternion.Euler(startValue);
        else
            transform.localRotation = Quaternion.Euler(startValue);
    }
    public override void DOLoop()
    {
        if (Application.isPlaying)
        {
            if (!local)
                c_Transform.DORotate(endValue, duration).SetDelay(doDelay).SetEase(ease).SetLoops(-1, loopType);
            else
                c_Transform.DOLocalRotate(endValue, duration).SetDelay(doDelay).SetEase(ease).SetLoops(-1, loopType);
        }
        else
        {
            if (!local)
                transform.rotation = Quaternion.Euler(endValue);
            else
                transform.localRotation = Quaternion.Euler(endValue);
        }
    }
}
