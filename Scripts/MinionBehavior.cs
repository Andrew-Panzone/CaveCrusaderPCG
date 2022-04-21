using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinionBehavior : MonoBehaviour
{
    // public GameObject[] lootItems; (to be implemented once loot items script is finished)
    public int damageAmount = 20;

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            var playerHealth = other.transform.parent.GetComponent<PlayerHealth>();
            playerHealth.TakeDamage(damageAmount);
            DestroyMinion();
        }

        if(other.CompareTag("Sword"))
        {
            DestroyMinion();
        }
    }

    void DestroyMinion()
    {
        Destroy(gameObject, 0.2f);
    }
}
