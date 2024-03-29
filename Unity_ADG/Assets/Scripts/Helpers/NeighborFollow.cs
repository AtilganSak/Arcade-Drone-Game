﻿using UnityEngine;

public class NeighborFollow: MonoBehaviour
{
    public Transform target;

    public float positionFollowSpeed;
    public float rotationFollowSpeed;

    Transform c_Transform;

    private void Start()
    {
        c_Transform = transform;

    }
    private void FixedUpdate()
    {
        c_Transform.position = Vector3.Lerp(c_Transform.position, target.position, Time.smoothDeltaTime * positionFollowSpeed);

        c_Transform.rotation = Quaternion.Slerp(c_Transform.rotation, target.rotation, Time.smoothDeltaTime * rotationFollowSpeed);
    }
}
