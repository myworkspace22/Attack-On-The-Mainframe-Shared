using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private Transform target;

    public float speed = 70f;

    public int damage = 50;

    public float explosionRadius = 0f;
    public GameObject impactEffect;
    public GameObject impactMissile;

    public void Seek (Transform _target)
    {
        target = _target;
    }
    private void Update()
    {
        if (target == null)
        {
            Destroy(gameObject);
            return;
        }
        Vector2 dir = target.position - transform.position;
        float distanceThisFrame = speed * Time.deltaTime;

        if(dir.magnitude <= distanceThisFrame)
        {
            HitTarget();
            return;
        }
        transform.Translate(dir.normalized * distanceThisFrame, Space.World);
    }
    void HitTarget()
    {
        GameObject effectIns = (GameObject)Instantiate(impactEffect, transform.position, transform.rotation);
        Destroy(effectIns, 2f);

        if (explosionRadius > 0f)
        {
            Explode();
        }
        else
        {
            Damage(target);
        }

        Destroy(gameObject);
    }
    void Damage (Transform enemy)
    {
        Enemy e = enemy.GetComponent<Enemy>();

        if (e != null)
        {
            e.TakeDamage(damage);
        }
    }
    void Explode()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, explosionRadius); //Physics.OverlapSphere(transform.position, explosionRadius);
        foreach (Collider2D collider in colliders)
        {
            if (collider.tag == "Enemy")
            {
                GameObject effectIns = (GameObject)Instantiate(impactMissile, collider.transform.position, collider.transform.rotation);
                Destroy(effectIns, 2f);

                Damage(collider.transform);
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}
