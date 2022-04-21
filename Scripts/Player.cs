using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

    public float speed = 5;
    public Rigidbody rb;
    public float hp = 100;

    void Start(){
        rb = gameObject.GetComponent<Rigidbody>();
    }

    void FixedUpdate() {
        float moveX = Input.GetAxis("Horizontal");
        float moveY = Input.GetAxis("Vertical");
        Vector3 move = new Vector3(moveX, moveY, 0);
        if (move.magnitude > 1) {
            move.Normalize();
        }
        rb.velocity = (speed * move);
    }
}
