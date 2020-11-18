using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TrailRecorder : MonoBehaviour
{
    public bool record = true;

    public float frequency = 0.5F;
    public int trailLimit = 5;
    public float minDistance = 0.1F;

    public Color pointColor = Color.white;

    Queue<Vector3> trail;

    float timer;

    Transform c_Transform;

    Vector3 lastPos;

    private void OnEnable()
    {
        trail = new Queue<Vector3>(trailLimit);
        c_Transform = transform;
    }
    void Update()
    {
        if (!record) return;

        if ((lastPos - c_Transform.position).magnitude > 0.2F)
        {
            lastPos = c_Transform.position;

            timer += Time.deltaTime;
            if (timer > frequency)
            {
                timer = 0;

                if (trail.Count > 0)
                {
                    if ((c_Transform.position - trail.ElementAt(0)).sqrMagnitude < minDistance)
                        return;
                }
                if (trail.Count + 1 > trailLimit)
                {
                    trail.Dequeue();
                }
                trail.Enqueue(c_Transform.position);
            }
        }
    }
    private void OnDrawGizmos()
    {
        if (trail != null)
        {
            Gizmos.color = pointColor;
            foreach (Vector3 point in trail)
            {
                Gizmos.DrawSphere(point, 0.3F);
            }
        }
    }

    public int GetTrailLength() => trail.Count;
    public Vector3 GetNearPoint(Vector3 realPos)
    {
        float nearDistance = (trail.ElementAt(0) - realPos).sqrMagnitude;
        int index = 0;
        for (int i = 1; i < trail.Count; i++)
        {
            if ((trail.ElementAt(i) - realPos).sqrMagnitude < nearDistance)
            {
                nearDistance = (trail.ElementAt(i) - realPos).sqrMagnitude;
                index = i;
            }
        }
        return trail.ElementAt(index);
    }
}
