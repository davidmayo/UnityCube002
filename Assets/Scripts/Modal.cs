using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Modal : MonoBehaviour
{
    public GameObject canvas;
    public void ButtonClicked()
    {
        Debug.Log($"Canvas = {canvas}");
        Debug.Log($"this = {this}");

        Destroy(this.gameObject);

    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
