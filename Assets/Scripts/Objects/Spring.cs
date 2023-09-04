using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spring : MonoBehaviour
{
    [Header("Spring Force")]
    [SerializeField] private float force;

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<Rigidbody>(out Rigidbody objRB))
        {
            objRB.velocity = new Vector3(objRB.velocity.x, 0, objRB.velocity.z);
            objRB.AddForce(Vector3.up * force, ForceMode.Impulse);
        }
    }
}
