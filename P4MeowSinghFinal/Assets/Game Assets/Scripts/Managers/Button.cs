using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Button : MonoBehaviour
{
    private Button button;
    public GameObject titleScreen;

    // Code based off clicky crates project

    void Start()
    {
        button = GetComponent<Button>();
    }

    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            titleScreen.SetActive(false);
        }
    }
}
