using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private Transform target;
    private Vector2 targetsLastPos;

    public float speed = 70f;

    public bool flameThrower;
    private Vector2 flameDir;

    public int damage = 50;

    public float explosionRadius = 0f;
    public GameObject impactEffect;
    public GameObject impactMissile;

    [HideInInspector]
    public int poisonDamage;
    [HideInInspector]
    public float poisonTime;

    public void Seek (Transform _target)
    {
        target = _target;
        targetsLastPos = _target.position;
        if (flameThrower) 
        { 
            flameDir = target.position - transform.position;
            GetComponent<ExplosionRadius>().explosionRange = explosionRadius;
        }
    }
    private void Update()
    {
        if (target == null && targetsLastPos == null)
        {
            Debug.LogWarning(gameObject + " had no target");
            Destroy(gameObject);
            return;
        }

        Vector2 dir;
        if(target == null)
        {
            dir = targetsLastPos - (Vector2)transform.position;
        }
        else
        {
            targetsLastPos = target.position;
            dir = target.position - transform.position;
        }

        float distanceThisFrame = speed * Time.deltaTime;
        if (dir.magnitude <= distanceThisFrame && !flameThrower)
        {
            HitTarget();
            return;
        }

        if (flameThrower)
        {
            transform.Translate(flameDir.normalized * distanceThisFrame, Space.World);
        }
        else
        {
            transform.Translate(dir.normalized * distanceThisFrame, Space.World);
        }
    }
    public void HitTarget()
    {
        GameObject effectIns = (GameObject)Instantiate(impactEffect, transform.position, transform.rotation);

        if (explosionRadius > 0f)
        {
            effectIns.GetComponent<ExplosionRadius>().explosionRange = explosionRadius;
            Explode();
        }
        else if (target != null)
        {
            Destroy(effectIns, 2f);
            Damage(target);
        }

        Destroy(effectIns, 2f);
        Destroy(gameObject);
    }

    
    void Damage (Transform enemy)
    {
        Enemy e = enemy.GetComponent<Enemy>();

        if (e != null)
        {
            e.TakeDamage(damage);
            if(poisonDamage > 0)
            {
                e.Poison(poisonDamage, poisonTime);
            }
        }
    }
    void Explode()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, explosionRadius); //Physics.OverlapSphere(transform.position, explosionRadius);
        foreach (Collider2D collider in colliders)
        {
            if (collider.tag == "Enemy")
            {
                //GameObject effectIns = (GameObject)Instantiate(impactMissile, collider.transform.position, collider.transform.rotation);
                //Destroy(effectIns, 2f);
                
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
