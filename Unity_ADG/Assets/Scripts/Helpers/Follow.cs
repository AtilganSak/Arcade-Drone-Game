using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follow : MonoBehaviour
{
    public Transform target;

    public float speed;

    public Vector3 offset;

    Transform c_Transform;

    private void OnValidate()
    {
        if(target)
            transform.position = target.TransformPoint(offset);
    }
    private void OnEnable()
    {
        c_Transform = transform;
    }
    private void FixedUpdate()
    {
        c_Transform.position = Vector3.Lerp(c_Transform.position, target.TransformPoint(offset), Time.smoothDeltaTime * speed);
    }
}
