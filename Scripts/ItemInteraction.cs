using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemInteraction : MonoBehaviour
{
    // public Text shoeText;
    // public Text bandageText;
    // public Text compassText;
    // public Text flashLightText;
    public int ShoesNo = 0;
    public int FlashlightNo = 0;
    public int BandageNo = 0;
    public int CompassNo = 0;

    public GameObject Shoes;
    public GameObject Flashlight;
    public GameObject Bandage;
    public GameObject Compass;
    public GameObject CompassItem;
    //public int CampfireNo = 0;

    float distance;
    float shortDis;
    float angle;
    Vector3 playerPos;
    Vector3 posNearest;
    Slider healthSlider;

    //public float timer = 1.0f;

    // Start is called before the first frame update
    void Start()
    {
        healthSlider = GameObject.Find("HealthBar").GetComponent<Slider>();
        Shoes = GameObject.Find("shoes");
        Flashlight = GameObject.Find("flashlight");
        Bandage = GameObject.Find("bandage");
        Compass = GameObject.Find("compass");
        CompassItem = GameObject.Find("compassitem");
        Shoes.SetActive(false);
        Flashlight.SetActive(false);
        Bandage.SetActive(false);
        Compass.SetActive(false);
        CompassItem.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        //Using Running Shoes
        if (ShoesNo > 0)
        {
            Shoes.SetActive(true);
            GameObject.FindWithTag("Player").GetComponent<Player>().speed *= 1.5f;
            ShoesNo--;
            // shoeText.text = "Shoes: Acquired";
            
        }


        //Using Bandage
        if (BandageNo > 0)
        {
            Bandage.SetActive(true);
            if (Input.GetKeyDown(KeyCode.R))
            {
                GameObject.FindWithTag("Player").GetComponent<PlayerHealth>().currentPlayerHealth += 50;
                healthSlider.value = GameObject.FindWithTag("Player").GetComponent<PlayerHealth>().currentPlayerHealth;
                BandageNo--;
            }

            // bandageText.text = "Bandage: Acquired (Press R)";
        }


        //Using Flashlight
        if (FlashlightNo > 0)
        {
            Flashlight.SetActive(true);
            GameObject.FindWithTag("MainCamera").GetComponent<Camera>().orthographicSize += 5;
            FlashlightNo--;
            // flashLightText.text = "Flashlight: Acquired";
        }


        //Using Compass
        if (CompassNo > 0)
        {
            Compass.SetActive(true);
            if(CompassItem.active == false)
            {
                CompassItem.SetActive(true);
            }
            else {
                GetDirection();
                CompassItem.transform.rotation = Quaternion.Euler(0.0f, 0.0f, angle);
            }
        }
    }

    void GetDirection()
    {
        playerPos = GameObject.FindWithTag("Player").transform.position;
        GameObject[] bossList = GameObject.FindGameObjectsWithTag("Enemy");
        shortDis = 100000;

        foreach (GameObject boss in bossList)
        {
            Vector3 bossPos = boss.transform.position;
            distance = Vector3.Distance(playerPos, bossPos);
            if(distance < shortDis)
            {
                shortDis = distance;
                posNearest = bossPos;

            }
        }

        angle = AngleCal(transform.forward, (posNearest - playerPos));

        print(angle);
        // compassText.text = "Compass: Acquired " + angle;
    }

    float AngleCal(Vector3 from, Vector3 to)
    {
        float x = to.x - from.x;
        float y = to.y - from.y;

        float hypotenuse = Mathf.Sqrt(Mathf.Pow(x, 2f) + Mathf.Pow(y, 2f));

        float cos = x / hypotenuse;
        float radian = Mathf.Acos(cos);

        float angle = 180 / (Mathf.PI / radian);

        if (y < 0)
        {
            angle = -angle;
        }
        else if ((y == 0) && (x < 0))
        {
            angle = 180;
        }
        
        return angle;
    }
}
