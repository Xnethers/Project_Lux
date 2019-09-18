using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawLine : MonoBehaviour
{

    private LineRenderer lineRenderer;
    private float counter;
    private float dist;
    public Transform origin;
    public Transform destination;
    public float lineDrawSpeed = 0.1f;

    // Use this for initialization
    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();

        lineRenderer.startWidth = 0.45f;
        lineRenderer.endWidth = 0.45f;
        lineRenderer.enabled=false;
        origin = transform;

    }

    // Update is called once per frame
    void Update()
    {
        if (lineRenderer.enabled)
        {
            lineRenderer.SetPosition(0, origin.position);
            dist = Vector3.Distance(origin.position, destination.position);

            // if (counter < dist)
            // {
            //     The head is in the pointAlongLine.
                counter += .1f / lineDrawSpeed;

                float x = Mathf.Lerp(0, dist, counter);

                Vector3 pointA = origin.position;
                Vector3 pointB = destination.position;

                Vector3 pointAlongLine = x * Vector3.Normalize(pointB - pointA) + pointA;

                lineRenderer.SetPosition(1, pointAlongLine);

            //     Debug.Log(counter);
            // }
        }

    }
    public void SetLineEnabled(bool enabled){
        lineRenderer.enabled = enabled;
    }
}
