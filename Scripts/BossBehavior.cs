using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossBehavior : MonoBehaviour
{
    public int startingHealth = 100;
    public Slider healthSlider;
    public int damageAmount = 20;
    public int currentHealth;

    public static int itemRemain = 4;
    public static int[] itemList = new int[4] { 1, 2, 3, 4 };
    public int currentItem = 0;
    int currentNo;

    void Awake()
    {
        healthSlider = GetComponentInChildren<Slider>();
    }

    // Start is called before the first frame update
    void Start()
    {
        currentHealth = startingHealth;

    }

    public void TakeDamage(int damageAmount)
    {
        if(currentHealth > 0)
        {
            currentHealth -= damageAmount;
            healthSlider.value = currentHealth;
        }
        
        if(currentHealth <= 0)
        {
            LevelManager.score--;

            DestroyBoss();

            // ***** DROP A RANDOM LOOT ITEM *****

            currentItem = 0;
            while (currentItem == 0 && itemRemain > 0)
            {
                currentNo = Random.Range(0, 4);
                currentItem = itemList[currentNo];
            }
            itemList[currentNo] = 0;
            itemRemain--;

            if (currentItem == 1)
            {
                GameObject.FindWithTag("Player").GetComponent<ItemInteraction>().ShoesNo += 1;
            }

            if (currentItem == 2)
            {
                GameObject.FindWithTag("Player").GetComponent<ItemInteraction>().FlashlightNo += 1;
            }

            if (currentItem == 3)
            {
                GameObject.FindWithTag("Player").GetComponent<ItemInteraction>().BandageNo += 1;
            }

            if (currentItem == 4)
            {
                GameObject.FindWithTag("Player").GetComponent<ItemInteraction>().CompassNo += 1;
            }
            

        }
    }

    void DestroyBoss()
    {
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            var playerHealth = other.transform.parent.GetComponent<PlayerHealth>();
            playerHealth.TakeDamage(damageAmount);
        }

        if(other.CompareTag("Sword"))
        {
            TakeDamage(25);
        }
    }
}
