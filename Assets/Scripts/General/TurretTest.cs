using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretTest : MonoBehaviour
{
    public GameObject model;
    private Color normalColor;
    private Color redColor = Color.red;
    private bool isShooting = false;

    void Start()
    {
        normalColor = model.GetComponent<Renderer>().material.color;
        InvokeRepeating("ToggleRedColor", 0.0f, 5.0f);
    }

    void ToggleRedColor() 
    {
        if (isShooting)
        {
            model.GetComponent<Renderer>().material.color = normalColor;
            isShooting = false;
        }
        else
        {
            model.GetComponent<Renderer>().material.color = redColor;
            isShooting = true;
            Invoke("ToggleNormalColor", 2.0f);
        }
    }

    void ToggleNormalColor()
    {
        model.GetComponent<Renderer>().material.color = normalColor;
        isShooting = false;
    }
}

