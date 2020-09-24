using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;

public class LookAt : MonoBehaviour
{
    public Transform target;

    public bool lockY;
    public bool lockX;
    public bool lockZ;

    Transform c_Transform;

    Vector3 relativePosition;

    private void Start()
    {
        c_Transform = transform;
    }
    void Update()
    {
        if (target == null) return;

        relativePosition = target.position - c_Transform.position;

        if(lockY)
            relativePosition.y = 0;
        if (lockZ)
            relativePosition.z = 0;
        if (lockX)
            relativePosition.x = 0;

        c_Transform.rotation = Quaternion.LookRotation(relativePosition);
    }
}
