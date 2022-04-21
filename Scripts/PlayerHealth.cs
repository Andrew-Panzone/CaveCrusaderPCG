using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// comments to be implemented upon further development of other scripts
public class PlayerHealth : MonoBehaviour
{
    public int startingHealth = 100;
    // public AudioClip deadSFX; // audio clip to be played when player dies (SFX)
    Slider healthSlider; // healthbar that is part of UI (UI)
    public int currentPlayerHealth;


    // Start is called before the first frame update
    void Start()
    {
        healthSlider = GameObject.Find("HealthBar").GetComponent<Slider>();
        currentPlayerHealth = startingHealth;
        healthSlider.value = currentPlayerHealth;
    }

    public void TakeDamage(int damageAmount)
    {
        if(currentPlayerHealth > 0)
        {
            currentPlayerHealth -= damageAmount;
            healthSlider.value = currentPlayerHealth;
        }
        
        if(currentPlayerHealth <= 0)
        {
            
        }
    }
}
