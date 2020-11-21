using UnityEngine;
using System.Collections;
using System;

public class CameraShake : MonoBehaviour
{
	Transform camTransform;

	[Tooltip("How long the object should shake for.")]
	public float shakeDuration = 0f;

	[Tooltip("Amplitude of the shake. A larger value shakes the camera harder.")]
	public float shakeAmount = 0.7f;
	public float decreaseFactor = 1.0f;

	public Action overShake;

	Vector3 originalPos;

	void Awake()
	{
		if (camTransform == null)
		{
			camTransform = transform;
		}
	}

	void OnEnable()
	{
		originalPos = camTransform.localPosition;
	}

	void Update()
	{
		if (shakeDuration > 0)
		{
			camTransform.localPosition = originalPos + UnityEngine.Random.insideUnitSphere * shakeAmount;

            if (Time.timeScale > 0)
            {
				shakeDuration -= Time.deltaTime * decreaseFactor;
            }
            else
            {
				shakeDuration = 0;
            }
		}
		else
		{
			shakeDuration = 0f;
			camTransform.localPosition = originalPos;

            if (overShake != null)
            {
				overShake.Invoke();
            }
		}
	}
}