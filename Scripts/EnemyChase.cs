using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyChase : MonoBehaviour
{
    public Transform player;
    Collider playerCol;

    public int agroRange;

    bool facingRight = true;

    public float speed;

    [SerializeField]
    Vector2 startPos;


    // Start is called before the first frame update
    void Start() {
        startPos = transform.position;
        player = GameObject.FindGameObjectWithTag("Player").transform;
        playerCol = player.gameObject.GetComponent<Collider>();
        Physics.IgnoreCollision(GetComponent<Collider>(),playerCol,true);
        Physics.IgnoreLayerCollision(3,6,true);
    }

    // Update is called once per frame
    void Update() {
        float dist2Player = Vector2.Distance(transform.position, player.position);

        Vector3 move;
        if (dist2Player < agroRange) {
            move = (player.position - transform.position).normalized*speed;
            if(player.position.x > transform.position.x && !facingRight) {
                Flip();
            }
            if(player.position.x < transform.position.x && facingRight) {
                Flip();
            }
        } else {
            move = (transform.position - new Vector3(startPos.x, startPos.y, 0)).normalized*speed;
            if(startPos.x > transform.position.x && !facingRight) {
                Flip();
            }

            if(startPos.x < transform.position.x && facingRight) {
                Flip();
            }
        }
        gameObject.GetComponent<Rigidbody>().velocity = move;
    }
    
    void Flip(){
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
        facingRight = !facingRight;
    }
}
