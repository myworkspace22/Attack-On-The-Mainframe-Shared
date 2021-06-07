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
    public bool Tesla;
    //private CircleCollider2D circleCollider;

    private void Start()
    {
        sR = GetComponent<SpriteRenderer>();
        //circleCollider = GetComponent<CircleCollider2D>();
    }
    private void Update()
    {
        if (explosionRange <= 0)
        {
            explosionRange = float.MaxValue;
        }
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

        //if (circleCollider != null)
        //{
        //    circleCollider.radius = size.x / 2;
        //}
        float fadeSice;
        if (Tesla) 
        {
            fadeSice = 1 - transform.localScale.x / (explosionRange * 8);
        }
        else
        {
            fadeSice = 1 - transform.localScale.x / (explosionRange * 2);
        }
        Color color = sR.color;
        color.a = fadeSice;
        sR.color = color;
    }
}
