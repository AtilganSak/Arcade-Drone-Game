using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

public class DOBase : MonoBehaviour
{
    public float duration = 1;
    public float doDelay;
    public float revertDelay;
    public Ease ease;
    public LoopType loopType;
    public bool resetOnEnable = true;
    public bool doOnStart = false;

    public UnityEvent DOComplete;
    public UnityEvent DORevertComplete;

    [HideInInspector] public bool connect = true;
    [HideInInspector] public int orderIndex;

    internal Transform c_Transform;

    private void OnEnable()
    {
        c_Transform = transform;

        VirtualEnable();

        if (resetOnEnable)
            ResetDO();
    }
    private void Start()
    {
        if (doOnStart)
            DO();

        VirtualStart();
    }
    internal virtual void VirtualStart() { }
    internal virtual void VirtualEnable() { }

    public bool IsTweening()
    {
        return DOTween.IsTweening(c_Transform);
    }
    [Button]
    public virtual void DO() { }
    [Button]
    public virtual void DOLoop() { }
    [Button]
    public virtual void DORevert() { }
    [Button("Reset")]
    public virtual void ResetDO() { }
}
