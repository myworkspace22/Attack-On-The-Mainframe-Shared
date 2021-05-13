using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Node : MonoBehaviour
{
    private Animator anim;
    public PathChecker pathChecker;
    public Vector3 positionOffset;

    [Header("Tower Properties")]
    public GameObject towerRange;
    public GameObject[] towerStars;
    
    [Header("Animation Ref.")]
    public SpriteRenderer spriteToChange;

    [HideInInspector]
    public GameObject turret;
    [HideInInspector]
    public TurretBluePrint turretBlueprint;
    [HideInInspector]
    public bool isMaxed = false;
    [HideInInspector]
    public int upgradeNr;
    [HideInInspector]
    public int UpgradeMultiplier 
    { 
        get 
        {
            if (upgradeNr == 0)
            {
                return 1;
            }
            else if (upgradeNr > 0 && upgradeNr < 3)
            {
                return 2;
            }
            else if (upgradeNr >= 3)
            {
                return 3;
            }

            Debug.LogWarning("Wrong Upgarde Nr:" + upgradeNr);
            return 0;
        }
    }
    [HideInInspector]
    public int towerLevel;
    [HideInInspector]
    public int SellAmount
    {
        get
        {
            return priceLocked / 2;
        }
    }

    [HideInInspector]
    public int priceLocked;
    [HideInInspector]
    public int priceUnlocked;

    BuildManager buildManager;

    private void Start()
    {
        upgradeNr = 0;
        towerLevel = 0;
        

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
        {
            buildManager.DeselectNode();
            return;
        }
           
        BuildTurret(buildManager.GetTurretToBuild());
    }
    void BuildTurret(TurretBluePrint blueprint)
    {
        if (!buildManager.GetComponent<WaveSpawner>().BuildMode)
            return;

        if (PlayerStats.Money < blueprint.cost)
        {
            Debug.Log("Not enough money to Build!");
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
        
        //path check
        //if (!CheckPathWPC())
            //return;

        PlayerStats.Money -= blueprint.cost;
        priceLocked += blueprint.cost; //skal ændres

        GameObject _turret = (GameObject)Instantiate(blueprint.prefab, GetBuildPosition(), Quaternion.identity);
        turret = _turret;

        turretBlueprint = blueprint;

        //Debug.Log("Turret build!");
        anim.SetBool("Place", false);
        if (!Input.GetButton("KeepBuilding"))
        {
            buildManager.SelectNode(this);
        }
        
        //buildManager.DeselectTower();
        //ChangeRange(false);

        //move collider forwards to make it easy to select
        Vector3 pos = transform.position;
        pos.z = -0.1f;
        transform.position = pos;

        //make the collider as big as the tower
        BoxCollider2D collider = GetComponent<BoxCollider2D>();
        collider.size = new Vector2(0.5f, 0.5f);

        //add tag "Tower"
        gameObject.tag = "Tower";

        //scan a* path
        AstarPath.active.Scan();
        if (!pathChecker.PathCheck())
        {
            BuildManager.instance.DeselectNode();
            SellTurret();
        }
    }

    public void levelUpTower()
    {
        if (!buildManager.GetComponent<WaveSpawner>().BuildMode)
            return;

        if (PlayerStats.Money < turretBlueprint.levelUpCost * UpgradeMultiplier)
        {
            Debug.Log("Not enough money to level up!");
            return;
        }
        PlayerStats.Money -= turretBlueprint.levelUpCost * UpgradeMultiplier;
        priceLocked += turretBlueprint.levelUpCost * UpgradeMultiplier;

        if (towerLevel >= 3)
        {
            Debug.LogWarning("trying to level beyound level 3");
            return;
        }

        towerLevel++;
        for (int i = 0; i < 3; i++)
        {
            towerStars[i].SetActive(towerLevel > i);
        }

        turret.GetComponent<Turret>().bulletDamage += 5;
        turret.GetComponent<Turret>().fireRate += 0.2f;
        turret.GetComponent<Turret>().range += 0.1f;

        ChangeRange(true, turret.GetComponent<Turret>().range);
        buildManager.nodeUI.SetTarget(this);
        buildManager.nodeUI.ShowUpgradeStats(2);
    }

    public void UpgradeTurret(int index)
    {
        if (!buildManager.GetComponent<WaveSpawner>().BuildMode)
            return;

        int upgradeindex = (upgradeNr > 0) ?  upgradeNr + index * 2: upgradeNr + index;

        if (PlayerStats.Money < turretBlueprint.upgradeCost[upgradeindex - 1])
        {
            Debug.Log("Not enough money to Upgrade!");
            return;
        }
        PlayerStats.Money -= turretBlueprint.upgradeCost[upgradeindex - 1];
        priceLocked += turretBlueprint.upgradeCost[upgradeindex - 1];

        //Get rid of the old turret
        Destroy(turret);

        //Building a upgraded turret
        GameObject _turret = (GameObject)Instantiate(turretBlueprint.upgradedPrefab[upgradeindex - 1], GetBuildPosition(), Quaternion.identity);
        turret = _turret;
        isMaxed = (upgradeindex > 2);

        ChangeRange(true, turret.GetComponent<Turret>().range);

        buildManager.nodeUI.SetTarget(this);

        upgradeNr = upgradeindex;
        towerLevel = 0;
        foreach (GameObject star in towerStars)
        {
            star.SetActive(false);
        }

        Debug.Log("Turret Upgraded!");
    }
    public void SellTurret()
    {
        if (!buildManager.GetComponent<WaveSpawner>().BuildMode)
            return;

        PlayerStats.Money += SellAmount;
        priceLocked = 0;

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
        upgradeNr = 0;
        towerLevel = 0;
        foreach (GameObject star in towerStars)
        {
            star.SetActive(false);
        }

        ChangeRange(false);

        AstarPath.active.Scan();
    }

    private void OnMouseEnter()
    {
        if (!buildManager.GetComponent<WaveSpawner>().BuildMode)
            return;

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
            spriteToChange.sprite = buildManager.GetTurretToBuild().prefab.GetComponent<SpriteRenderer>().sprite;
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

    public void LockPrice()
    {
        priceLocked += priceUnlocked;
        priceUnlocked = 0;
    }


    private bool CheckPathWPC()
    {
        //Create fake block

        //move collider forwards to make it easy to select
        //Vector3 pos = transform.position;
        //pos.z = -0.1f;
        //transform.position = pos;

        //make the collider as big as the tower
        BoxCollider2D collider = GetComponent<BoxCollider2D>();
        collider.size = new Vector2(0.5f, 0.5f);

        //add tag "Tower"
        gameObject.tag = "Tower";

        //scan a* path
        AstarPath.active.Scan();
        Debug.Log("PathCheck 1101 = " + pathChecker.PathCheck());





        //Chek for path
        bool path = pathChecker.PathCheck();



        //Clean UP

        //move collider back again
        //pos.z = 0.0f;
        //transform.position = pos;

        //make the collider as small again
        collider.size = new Vector2(0.25f, 0.25f);

        //remove tag "Tower"
        gameObject.tag = "Untagged";

        //scan a* path
        AstarPath.active.Scan();
        Debug.Log("PathCheck 2202 = " + pathChecker.PathCheck());

        return path;
    }
}
