using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBar : MonoBehaviour
{
    private Quaternion newPosition;
    public Transform originalPosition;

    private bool upsideDown;

    void Start()
    {
        Camera[] cameras = Camera.allCameras;

        for (int i = 0; i < cameras.Length; i++)
        {
            if (cameras[i].gameObject.activeSelf)
            {
                newPosition = cameras[i].transform.rotation;

                upsideDown = cameras[i].transform.position.x != -3;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        transform.rotation = newPosition;
        Vector2 tmp = originalPosition.position;
        tmp.y += (upsideDown)? -0.3f: 0.3f;
        transform.position = tmp;
    }
}