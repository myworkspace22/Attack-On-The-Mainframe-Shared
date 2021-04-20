using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Node : MonoBehaviour
{
    private Animator anim;
    public Vector3 positionOffset;

    [Header("Tower Properties")]
    public GameObject towerRange;

    [HideInInspector]
    public GameObject turret;
    [HideInInspector]
    public TurretBluePrint turretBlueprint;
    [HideInInspector]
    public bool isUpgraded = false;

    BuildManager buildManager;

    private void Start()
    {
        anim = GetComponent<Animator>();

        buildManager = BuildManager.instance;

        towerRange.SetActive(false);
    }

    public Vector2 GetBuildPosition()
    {
        return transform.position + positionOffset;
    }

    //MÅSKE
    public void ChangeRange(bool rangeStatus)
    {
        towerRange.SetActive(rangeStatus);
    }
    public void ChangeRange(bool rangeStatus, float rangeRadius)
    {
        towerRange.transform.localScale = new Vector2(rangeRadius * 2, rangeRadius * 2);
        towerRange.SetActive(rangeStatus);
    }
    

    private void OnMouseDown()
    {
        if (EventSystem.current.IsPointerOverGameObject())
            return;

        if (turret != null)
        {
            buildManager.SelectNode(this);
            return;
        }

        if (!buildManager.CanBuild)
            return;

        BuildTurret(buildManager.GetTurretToBuild());
    }
    void BuildTurret(TurretBluePrint blueprint)
    {
        if (PlayerStats.Money < blueprint.cost)
        {
            Debug.Log("Build more Pylons!");
            return;
        }

        // null check måske ikke nødvendigt
        Collider2D[] canPlaceChecks = Physics2D.OverlapCircleAll(transform.position, 0.1f);
        if (canPlaceChecks != null)
        {
            for (int i = 0; i < canPlaceChecks.Length; i++)
            {
                if (canPlaceChecks[i].gameObject.tag == "Tower")
                {
                    Debug.Log("there is no space for a turret");
                    return;
                }              
            }
        }
        
    
        PlayerStats.Money -= blueprint.cost;

        GameObject _turret = (GameObject)Instantiate(blueprint.prefab, GetBuildPosition(), Quaternion.identity);
        turret = _turret;

        turretBlueprint = blueprint;

        Debug.Log("Turret build!");

        //move collider forwards to make it easy to select
        Vector3 pos = transform.position;
        pos.z = -0.1f;
        transform.position = pos;

        //make the collider as big as the tower
        BoxCollider2D collider = GetComponent<BoxCollider2D>();
        collider.size = new Vector2(0.5f, 0.5f);

        //add tag "Tower"
        gameObject.tag = "Tower";
    }
    public void UpgradeTurret()
    {
        if (PlayerStats.Money < turretBlueprint.upgradeCost)
        {
            Debug.Log("Not enough money to Upgrade!");
            return;
        }
        PlayerStats.Money -= turretBlueprint.upgradeCost;

        //Get rid of the old turret
        Destroy(turret);

        //Building a upgraded turret
        GameObject _turret = (GameObject)Instantiate(turretBlueprint.upgradedPrefab, GetBuildPosition(), Quaternion.identity);
        turret = _turret;

        isUpgraded = true;

        ChangeRange(true, turret.GetComponent<Turret>().range);

        Debug.Log("Turret Upgraded!");
    }
    public void SellTurret()
    {
        PlayerStats.Money += turretBlueprint.GetSellAmount();

        //Sell effect for later use!

        //move collider back again
        Vector3 pos = transform.position;
        pos.z = 0.0f;
        transform.position = pos;

        //make the collider as small again
        BoxCollider2D collider = GetComponent<BoxCollider2D>();
        collider.size = new Vector2(0.25f, 0.25f);

        //remove tag "Tower"
        gameObject.tag = "Untagged";

        Destroy(turret);
        turretBlueprint = null;

        ChangeRange(false);
    }
    private void OnMouseEnter()
    {
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }
        if (!buildManager.CanBuild)
        {
            return;
        }
        Collider2D[] canPlaceChecks = Physics2D.OverlapCircleAll(transform.position, 0.1f);
        if (canPlaceChecks != null)
        {
            for (int i = 0; i < canPlaceChecks.Length; i++)
            {
                if (canPlaceChecks[i].gameObject.tag == "Tower")
                {
                    anim.SetBool("Decline", true);
                    return;
                }
            }
        }
        if (buildManager.HasMoney)
        {
            anim.SetBool("Place", true);
            ChangeRange(true, buildManager.GetTurretToBuild().prefab.GetComponent<Turret>().range);
        }
        else
        {
            anim.SetBool("Decline", true);
        }
    }
    private void OnMouseExit()
    {
        if (buildManager.CanBuild)
            ChangeRange(false);
        anim.SetBool("Place", false);
        anim.SetBool("Decline", false);
    }
}
