using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionRadius : MonoBehaviour
{
    [HideInInspector]
    public float explosionRange;

    public float growSpeed;
    public float multiplier;
    private SpriteRenderer sR;

    private void Start()
    {
        sR = GetComponent<SpriteRenderer>();
    }
    private void Update()
    {
        
        if (transform.localScale.x >= explosionRange * 2)
        {
            Destroy(gameObject);
        }
        Vector2 size = transform.localScale;
        float grow = Time.deltaTime * growSpeed;
        size.x += grow;
        size.y += grow;
        transform.localScale = size;
        growSpeed += multiplier * Time.deltaTime;

        float fadeSice = 1 - transform.localScale.x / (explosionRange * 2);
        Color color = sR.color;
        color.a = fadeSice;
        sR.color = color;
    }
}
