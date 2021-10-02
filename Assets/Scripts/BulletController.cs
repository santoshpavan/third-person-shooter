using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour {
    private float speed = 100.0f;

    private Rigidbody bulletRigidBody;

    private void Awake() {
        bulletRigidBody = GetComponent<Rigidbody>();
    }

    private void Start() {
        bulletRigidBody.velocity = transform.forward * speed;
    }

    private void OnTriggerEnter(Collider other) {
        Destroy(gameObject);
    }
}
