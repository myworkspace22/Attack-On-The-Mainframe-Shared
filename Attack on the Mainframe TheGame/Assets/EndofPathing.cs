using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndofPathing : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Enemy")
        {
            GameObject.Destroy(other.gameObject);
        }
    }
}
