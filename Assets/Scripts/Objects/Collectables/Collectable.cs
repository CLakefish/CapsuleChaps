using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectable : MonoBehaviour
{
    [SerializeField] private int unlockedItem;

    private bool isActive = false;
    private MovementController mC;
    private Vector3 currentVelocity;

    private void Start()
    {
        if (PlayerPrefs.GetInt("hatIndex" + unlockedItem.ToString()) == 1) Destroy(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(Vector3.up * 150 * Time.deltaTime);
        
        if (isActive)
        {
            transform.position = Vector3.SmoothDamp(transform.position, mC.transform.position + new Vector3(0, 2, 0), ref currentVelocity, 0.2f);

            if (mC.Grounded)
            {
                return;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<MovementController>(out MovementController m))
        {
            isActive = true;
            mC = m;

            PlayerPrefs.SetInt("hatIndex" + unlockedItem.ToString(), 1);
            Destroy(gameObject);
        }
    }
}
