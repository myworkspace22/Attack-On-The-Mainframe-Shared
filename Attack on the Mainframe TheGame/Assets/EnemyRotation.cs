using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyRotation : MonoBehaviour
{
    public float rotation;
    void Update()
    {
        transform.Rotate(new Vector3(0f, 0f, rotation) * Time.deltaTime);
    }
}
