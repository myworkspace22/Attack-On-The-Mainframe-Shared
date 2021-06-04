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
    public GameObject cbTarget;
    [HideInInspector]
    public int poisonDamage;
    [HideInInspector]
    public float poisonTime;

    private List<GameObject> enemiesDeltDamage;
    public void Seek (Transform _target)
    {
        target = _target;
        targetsLastPos = _target.position;
        if (flameThrower) 
        { 
            flameDir = target.position - transform.position;
            GetComponent<ExplosionRadius>().explosionRange = explosionRadius;
            enemiesDeltDamage = new List<GameObject>();
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

            GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

            foreach (GameObject enemy in enemies)
            {
                float distanceToEnemy = Vector2.Distance(transform.position, enemy.transform.position);
                if (distanceToEnemy <= explosionRadius && !enemiesDeltDamage.Contains(enemy))
                {

                    Damage(enemy.transform);
                    enemiesDeltDamage.Add(enemy);
                }
            }
        }
        else
        {
            transform.Translate(dir.normalized * distanceThisFrame, Space.World);
        }
    }
    public void HitTarget()
    {
        GameObject effectIns = (GameObject)Instantiate(impactEffect, transform.position, transform.rotation);
        
        if (cbTarget != null) { Destroy(cbTarget); }

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

    //private void OnTriggerEnter2D(Collider2D collision)
    //{
    //    Debug.Log("enter trigger");
    //    if (!flameThrower) { return; }
    //    Enemy enemy = collision.GetComponent<Enemy>();

    //    if (enemy != null)
    //    {
    //        GameObject effectIns = (GameObject)Instantiate(impactEffect, enemy.transform.position, enemy.transform.rotation);
    //        Destroy(effectIns, 2f);
    //        Damage(enemy.transform);
    //    }
    //}

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}
