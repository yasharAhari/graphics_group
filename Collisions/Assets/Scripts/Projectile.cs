using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 10.0f;
    
    // Start is called before the first frame update
    void Start()
    {
        // Destroy the projectile at 2 seconds
        //Object.Destroy( gameObject , 3f );
        gameObject.GetComponent<Rigidbody>().velocity = transform.forward * speed;
    }

    private void OnCollisionEnter(Collision collision)
    {
        Object.Destroy(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        //gameObject.GetComponent<Rigidbody>( ).velocity = transform.forward * speed;

        //Vector3 startPos = transform.position;
        //Quaternion shotAngle = transform.rotation;

        //gameObject.GetComponent<Rigidbody>( ).AddForce( transform.forward * speed , ForceMode.Impulse );

    }
}
