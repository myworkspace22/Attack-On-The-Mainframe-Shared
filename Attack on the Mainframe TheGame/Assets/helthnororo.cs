using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class helthnororo : MonoBehaviour
{
    private Quaternion beoundTheTrueFart;
    public Transform originalFart;
    // Start is called before the first frame update
    void Start()
    {
        beoundTheTrueFart = transform.rotation;
    }

    // Update is called once per frame
    void Update()
    {
        transform.rotation = beoundTheTrueFart;
        Vector2 tmp = originalFart.position;
        tmp.y += 0.3f;
        transform.position = tmp;
    }
}
