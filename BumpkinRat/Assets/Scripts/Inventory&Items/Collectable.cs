using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Collectable : MonoBehaviour
{
    public string itemName;
    public delegate void Collected(string s, int amt = 1);
    public static event Collected OnCollected;

    private void FixedUpdate()
    {
        float y = MathfX.PulseSineFloat(0.005f, 0.25f, 0, 1);
        Vector3 curr = transform.position + new Vector3(0, y, 0);
        transform.position = curr;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            if(OnCollected != null)
            {
                OnCollected(itemName);
                Destroy(gameObject);
            }
        }
    }
}
