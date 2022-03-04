using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUp : MonoBehaviour
{
    public GameObject pickupPower;
    public float multiplier = 2.0f;
    public float duration = 4.0f;
    void OnTriggerEnter (Collider other)
    {
        if (other.CompareTag("Player"))
        {
            StartCoroutine(Obtain(other));
        }
    }

    IEnumerator Obtain(Collider player)
    {
        Debug.Log("PowerUp picked up");

        Instantiate(pickupPower, transform.position, transform.rotation);

        yield return new WaitForSeconds(duration);

        Destroy(gameObject);
    }
}
