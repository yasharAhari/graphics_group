using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 10.0f;
    public int timeUntilDestroy = 1;
    // Start is called before the first frame update
    void Start()
    {
        // Destroy the projectile at 2 seconds
        Object.Destroy( gameObject , 2.0f );
    }

    // Update is called once per frame
    void Update()
    {
        //gameObject.GetComponent<Rigidbody>( ).velocity = transform.forward * speed;

        Vector3 startPos = transform.position;
        Quaternion shotAngle = transform.rotation;

        //gameObject.GetComponent<Rigidbody>( ).AddForce( transform.forward * speed , ForceMode.Impulse );

    }
}
