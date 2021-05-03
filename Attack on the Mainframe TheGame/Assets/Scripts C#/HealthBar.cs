using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBar : MonoBehaviour
{
    private Quaternion newPosition;
    public Transform originalPosition;
    // Start is called before the first frame update
    void Start()
    {
        newPosition = transform.rotation;
    }

    // Update is called once per frame
    void Update()
    {
        transform.rotation = newPosition;
        Vector2 tmp = originalPosition.position;
        tmp.y += 0.3f;
        transform.position = tmp;
    }
}
