using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    [Header("Positions")]
    [SerializeField] private List<Vector3> platformPositions;

    [Header("Movement Parameters")]
    [SerializeField] private float distanceThreshold;
    [SerializeField] private float smoothingTime;
    private int targetIndex;
    private Vector3 currentVelocity;

    // Start is called before the first frame update
    void Start()
    {
        transform.position = platformPositions[0];
        targetIndex = 1;
    }

    private void FixedUpdate()
    {
        if (Vector3.Distance(transform.position, platformPositions[targetIndex]) >= distanceThreshold) 
            transform.position = Vector3.SmoothDamp(transform.position, platformPositions[targetIndex], ref currentVelocity, smoothingTime);
        else
        {
            if (targetIndex + 1 >= platformPositions.Count)
            {
                platformPositions.Reverse();
                targetIndex = 0;
            }
            else targetIndex++;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        other.transform.parent.parent = transform;
    }

    private void OnTriggerExit(Collider other)
    {
        other.transform.parent.parent = null;
    }

    private void OnDrawGizmos()
    {
        for (int i = 0; i < platformPositions.Count; i++)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawCube(platformPositions[i], transform.lossyScale);
        }
    }
}
