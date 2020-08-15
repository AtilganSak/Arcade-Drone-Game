using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneCameraRotate : MonoBehaviour
{
    [SerializeField] CinemachineFreeLook cinemachineFree;

    public float smootingY;
    public float smootingX;

    public float returnBackSpeed = 3F;
    public float returnBackTime = 3F;

    Transform myTransform;

    Vector3 firstPosition;
    Vector3 deltaPosition;

    bool moving;
    public bool canReturnBack;

    float targetX;
    float targetY;
    float counter;

    private void OnEnable()
    {
        myTransform = GetComponent<Transform>();
    }
    private void Update()
    {
        if (!moving)
        {
            counter += Time.deltaTime;
            if (counter >= returnBackTime)
            {
                counter = 0;
                canReturnBack = true;
            }
        }
        if (canReturnBack)
        {
            cinemachineFree.m_YAxis.Value = Mathf.Lerp(cinemachineFree.m_YAxis.Value, 0.5f, Time.deltaTime * returnBackSpeed);
            cinemachineFree.m_XAxis.Value = Mathf.Lerp(cinemachineFree.m_XAxis.Value, 0f, Time.deltaTime * returnBackSpeed);

            if (Mathf.Abs(cinemachineFree.m_YAxis.Value - 0.5F) < 0.01f
                && Mathf.Abs(cinemachineFree.m_XAxis.Value) < 0.01f)
                canReturnBack = false;
        }
    }
    private void LateUpdate()
    {
#if UNITY_STANDALONE
        if (Input.GetMouseButtonDown(0))
        {
            firstPosition = Input.mousePosition;
            deltaPosition = Vector3.zero;

            counter = 0;
            moving = true;
            canReturnBack = false;
        }
        if (Input.GetMouseButton(0))
        {
            deltaPosition = firstPosition - Input.mousePosition;

            firstPosition = Input.mousePosition;

            targetY = cinemachineFree.m_YAxis.Value + deltaPosition.normalized.y;
            targetX = cinemachineFree.m_XAxis.Value - deltaPosition.x;

            cinemachineFree.m_YAxis.Value = Mathf.Lerp(cinemachineFree.m_YAxis.Value, targetY, Time.deltaTime * smootingY);

            cinemachineFree.m_XAxis.Value = Mathf.Lerp(cinemachineFree.m_XAxis.Value, targetX, Time.deltaTime * smootingX);
        }
        if (Input.GetMouseButtonUp(0))
        {
            moving = false;
        }
#endif
#if UNITY_ANDROID
        if (Input.touchCount > 0)
        {
            if (Input.GetTouch(0).phase == TouchPhase.Began)
            {
                firstPosition = Input.mousePosition;
                deltaPosition = Vector3.zero;

                moving = true;
            }
            if (Input.GetTouch(0).phase == TouchPhase.Moved)
            {
                deltaPosition = firstPosition - Input.mousePosition;

                firstPosition = Input.mousePosition;

                targetY = cinemachineFree.m_YAxis.Value + deltaPosition.normalized.y;
                targetX = cinemachineFree.m_XAxis.Value - deltaPosition.x;

                cinemachineFree.m_YAxis.Value = Mathf.Lerp(cinemachineFree.m_YAxis.Value, targetY, Time.deltaTime * smootingY);

                cinemachineFree.m_XAxis.Value = Mathf.Lerp(cinemachineFree.m_XAxis.Value, targetX, Time.deltaTime * smootingX);
            }
            if (Input.GetTouch(0).phase == TouchPhase.Ended)
            {
                moving = false;
            }
        }
#endif
    }
}
