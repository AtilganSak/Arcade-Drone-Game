using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [Header("Shake Parameters")]
    public float shakeDuration = 0.2F;
    public float shakeStrong = 0.2F;
    public float shakeAbsorptionForce = 1.0F;

    Camera mainCamera;

    CameraShake c_ShakeComponent;

    GameObject cameraObject;

    Damage damageComponent;

    bool haveComponent;

    private void OnEnable()
    {
        mainCamera = Camera.main;
        cameraObject = mainCamera?.gameObject;
    }
    public void DOShake(float duration, float shakeAmount, float decreaseFactor)
    {
        if (!haveComponent)
        {
            c_ShakeComponent = cameraObject.AddComponent<CameraShake>();
            c_ShakeComponent.overShake += ItsOverShake;
            haveComponent = true;
        }
        if (duration > 0)
        {
            if (c_ShakeComponent != null)
            {
                c_ShakeComponent.shakeDuration = duration;
                c_ShakeComponent.shakeAmount = shakeAmount;
                c_ShakeComponent.decreaseFactor = decreaseFactor;
            }
        }
    }
    public void AdjustDamageComponent(Damage damageCom)
    {
        damageComponent = damageCom;
        damageComponent.onDamaged -= OnDamaged;
        damageComponent.onDamaged += OnDamaged;
    }
#if UNITY_EDITOR
    [Button("Shake Sim")]
    void DoShakeOnInspector()
    {
        if (Application.isPlaying)
        {
            DOShake(shakeDuration, shakeStrong, shakeAbsorptionForce);
        }
    }
#endif
    void OnDamaged(float amount)
    {
        DOShake(shakeDuration, shakeStrong, shakeAbsorptionForce);
    }
    void ItsOverShake()
    {
        haveComponent = false;
        if (c_ShakeComponent)
        {
            Destroy(c_ShakeComponent);
        }
    }
}
