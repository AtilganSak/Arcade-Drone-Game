using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAt : MonoBehaviour
{
    public Transform target;

    Transform c_Transform;

    private void Start()
    {
        c_Transform = transform;
        if (target == null)
            target = Camera.main.gameObject.transform;
    }
    void Update()
    {
        c_Transform.LookAt(target);
    }
}
