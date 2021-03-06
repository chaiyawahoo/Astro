﻿using UnityEngine;

public class Missile : MonoBehaviour {

    public float Speed = 200f;
    private float lifespan = 10f;
    private float t = 0f;

    private void OnCollisionEnter(Collision collision) {
        if (collision.gameObject.CompareTag("Obstacle")) {
            Destroy(collision.gameObject);
            Destroy(this);
        }
    }

    private void Start() {
        GetComponent<Rigidbody>().velocity = transform.forward * Speed;
    }

    private void Update() {
        t += Time.deltaTime;
        if (t >= lifespan) {
            Destroy(this);
        }
    }
}
