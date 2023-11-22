using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bolt : MonoBehaviour {

    Rigidbody rbBolt;
    public float speed;
    public float Damage = 4f;

    private void Start() {
        rbBolt = GetComponent<Rigidbody>();
        rbBolt.velocity = transform.forward * speed;
    }

    private void OnTriggerEnter(Collider collision) {
        if (collision.tag == "Asteroid") {
            collision.GetComponent<AsteroidHealth>().Damage(Damage);
            Destroy(gameObject);
        }
    }
}
