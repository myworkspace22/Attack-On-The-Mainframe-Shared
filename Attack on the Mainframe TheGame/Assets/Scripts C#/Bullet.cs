using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private Transform target;
    private Vector2 targetsLastPos;

    public float speed = 70f;

    public bool flameThrower;
    public bool nuke;
    public bool timbersaw;
    public float stunTime;
    public bool rotateMissile;

    private Vector2 returnPos;
    //private bool returned;


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
        if (timbersaw)
        {
            returnPos = transform.position;
            enemiesDeltDamage = new List<GameObject>();
        }

    }
    private void Update()
    {
        if (nuke && target != null)
        {
            target = null;
        }

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
            if (rotateMissile)
            {
                LookAtTarget();
            }
        }

        float distanceThisFrame = speed * Time.deltaTime;
        if (dir.magnitude <= distanceThisFrame && !flameThrower)
        {
            if (timbersaw)
            {
                if (targetsLastPos != returnPos)
                {
                    flameDir = returnPos - (Vector2)transform.position;
                    targetsLastPos = returnPos;
                    target = null;
                }
                else
                {
                    Destroy(gameObject);
                }
                return;
            }
            HitTarget();
            return;
        }
        if (timbersaw)
        {
            DamageNearbyEnemies();
        }

        if (flameThrower)
        {
            transform.Translate(flameDir.normalized * distanceThisFrame, Space.World);
            DamageNearbyEnemies();
        }
        else
        {
            transform.Translate(dir.normalized * distanceThisFrame, Space.World);
        }
    }

    private void LookAtTarget()
    {
        Vector3 dir = target.position - transform.position;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle - 90, Vector3.forward);
    }

    private void DamageNearbyEnemies()
    {
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
            if (stunTime > 0)
            {
                e.Stun(stunTime);
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
