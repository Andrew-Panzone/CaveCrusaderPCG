using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseLook : MonoBehaviour
{
    public int angleOffset = 90;
    public GameObject bullet;
    GameObject[] bullets = new GameObject[5];
    public AudioClip shootSFX;

    void Update() {
        // derives a direction from the current mouse position
        var dir = Input.mousePosition - Camera.main.WorldToScreenPoint(transform.position);

        // get angle from vector
        var angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg + angleOffset;

        // rotate player
        transform.GetChild(1).transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

        if (Input.GetKeyDown("space")) {
            AudioSource.PlayClipAtPoint(shootSFX, Camera.main.transform.position);
            GameObject newB = Instantiate(bullet);
            dir.Normalize();
            Vector3 dir3 = new Vector3(dir.x, dir.y, 0);
            newB.transform.position = transform.position+dir3/2;
            newB.GetComponent<Rigidbody>().velocity = dir3 * 5;
            Destroy(bullets[4]);
            bullets[4] = bullets[3];
            bullets[3] = bullets[2];
            bullets[2] = bullets[1];
            bullets[1] = bullets[0];
            bullets[0] = newB;
        }
    }
}