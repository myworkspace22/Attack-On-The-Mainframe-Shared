using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Node : MonoBehaviour
{
    public PathChecker pathChecker;
    public Vector3 positionOffset;

    [Header("Tower Properties")]
    public GameObject towerRange;
    public GameObject[] towerStars;
    
    [Header("Animation Ref.")]
    public SpriteRenderer spriteToChange;
    public SpriteRenderer rangeSprite;
    private SpriteRenderer sR;
    public Sprite hoverBackground;
    private Sprite baseSprite;
    private Color baseColor;

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
            return priceUnlocked + (priceLocked / 2);
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

        buildManager = BuildManager.instance;

        buildManager.GetComponent<WaveSpawner>().OnWavePriceLocked += LockPrice;

        towerRange.SetActive(false);

        sR = GetComponent<SpriteRenderer>();

        baseColor = sR.color;

        baseSprite = sR.sprite;
    }

    //private void OnDestroy()
    //{
    //    buildManager.GetComponent<WaveSpawner>().OnWavePriceLocked -= LockPrice;
    //}

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


        GameObject _turret = (GameObject)Instantiate(blueprint.prefab, GetBuildPosition(), Quaternion.identity);
        turret = _turret;

        turretBlueprint = blueprint;

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
            return;
        }

        PlayerStats.Money -= blueprint.cost;
        priceUnlocked += blueprint.cost; //skal ændres

        //Debug.Log("Turret build!");
        EndHover();
        //if (!Input.GetButton("KeepBuilding"))
        //{
        //    buildManager.SelectNode(this);
        //}

        buildManager.newTowers.Push(this);
    }

    public void levelUpTower()
    {
        //if (!buildManager.GetComponent<WaveSpawner>().BuildMode)
        //    return;



        //if (PlayerStats.Money < turretBlueprint.levelUpCost * UpgradeMultiplier)
        //{
        //    Debug.Log("Not enough money to level up!");
        //    return;
        //}
        //PlayerStats.Money -= turretBlueprint.levelUpCost * UpgradeMultiplier;

        //if (buildManager.GetComponent<WaveSpawner>().BuildMode)
        //{
        //    priceUnlocked += turretBlueprint.levelUpCost * UpgradeMultiplier;
        //}
        //else
        //{
        //    priceLocked += turretBlueprint.levelUpCost * UpgradeMultiplier;
        //}

        if (PlayerStats.Money < turretBlueprint.levelUpCost * (towerLevel + 1))
        {
            Debug.Log("Not enough money to level up!");
            return;
        }
        PlayerStats.Money -= turretBlueprint.levelUpCost * (towerLevel + 1);

        if (buildManager.GetComponent<WaveSpawner>().BuildMode)
        {
            priceUnlocked += turretBlueprint.levelUpCost * (towerLevel + 1);
        }
        else
        {
            priceLocked += turretBlueprint.levelUpCost * (towerLevel + 1);
        }


        if (towerLevel >= 6)
        {
            Debug.LogWarning("trying to level beyound level 6");
            return;
        }

        Turret turretToUpgrade = turret.GetComponent<Turret>();

        turretToUpgrade.bulletDamage += turretToUpgrade.upgradeDamage * (towerLevel + 1);
        turretToUpgrade.fireRate += turretToUpgrade.upgradeFrenquency;
        turretToUpgrade.range += turretToUpgrade.upgradeRange;
        turretToUpgrade.damageOverTime += turretToUpgrade.upgradeLaserDoT * (towerLevel + 1);

        towerLevel++;
        StarUI();

        ChangeRange(true, turret.GetComponent<Turret>().range);
        buildManager.nodeUI.SetTarget(this);
        buildManager.nodeUI.ShowUpgradeStats(2);
    }

    private void StarUI()
    {
        for (int i = 0; i < 6; i++)
        {
            towerStars[i].SetActive(towerLevel > i);
        }
    }

    public void UpgradeTurret(int index)
    {
        //if (!buildManager.GetComponent<WaveSpawner>().BuildMode)
        //    return;

        int upgradeindex = (upgradeNr > 0) ?  upgradeNr + index * 2: upgradeNr + index;

        if (PlayerStats.Money < turretBlueprint.upgradeCost[upgradeindex - 1])
        {
            Debug.Log("Not enough money to Upgrade!");
            return;
        }

        PlayerStats.Money -= turretBlueprint.upgradeCost[upgradeindex - 1];

        if (buildManager.GetComponent<WaveSpawner>().BuildMode)
        {
            priceUnlocked += turretBlueprint.upgradeCost[upgradeindex - 1];
        }
        else
        {
            priceLocked += turretBlueprint.upgradeCost[upgradeindex - 1];
        }

        //Get rid of the old turret
        Destroy(turret);

        //Building a upgraded turret
        GameObject _turret = (GameObject)Instantiate(turretBlueprint.upgradedPrefab[upgradeindex - 1], GetBuildPosition(), Quaternion.identity);
        turret = _turret;
        isMaxed = (upgradeindex > 2);

        ChangeRange(true, turret.GetComponent<Turret>().range);

        buildManager.nodeUI.SetTarget(this);

        //turret.GetComponent<Turret>().towerPlatform.color = Color.white;
        turret.GetComponent<Animator>().SetBool("selected", true);

        upgradeNr = upgradeindex;
        towerLevel ++;
        StarUI();

        Debug.Log("Turret Upgraded!");
    }
    public void SellTurret()
    {
        if (!buildManager.GetComponent<WaveSpawner>().BuildMode)
            return;

        PlayerStats.Money += SellAmount;
        priceLocked = 0;
        priceUnlocked = 0;

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
        buildManager.hoverNode = this;
        Collider2D[] canPlaceChecks = Physics2D.OverlapCircleAll(transform.position, 0.1f);
        if (canPlaceChecks != null)
        {
            for (int i = 0; i < canPlaceChecks.Length; i++)
            {
                if (canPlaceChecks[i].gameObject.tag == "Tower")
                {
                    InDecline();
                    return;
                }
            }
        }
        if (buildManager.HasMoney)
        {
            spriteToChange.sprite = buildManager.GetTurretToBuild().prefab.GetComponent<SpriteRenderer>().sprite;
            ChangeRange(true, buildManager.GetTurretToBuild().prefab.GetComponent<Turret>().range);
            spriteToChange.gameObject.SetActive(true);
            spriteToChange.color = new Color(0, 0.5372466f, 0.5849056f);
            sR.color = new Color(0.5849056f, 0.5849056f, 0.5849056f);
            sR.sortingOrder = 2;
            rangeSprite.color = new Color(1, 1, 1, 0.78f);
            sR.sprite = hoverBackground;
        }
        else
        {
            InDecline();
        }

    }

    private void OnMouseExit()
    {
        if (buildManager.CanBuild)
            ChangeRange(false);
        spriteToChange.gameObject.SetActive(false);
        spriteToChange.sortingOrder = 3;
        sR.color = baseColor;
        sR.sortingOrder = -1;
        rangeSprite.color = new Color(1, 1, 1, 1);
        sR.sprite = baseSprite;
    }
    public void EndHover()
    {
        ChangeRange(false);
        spriteToChange.gameObject.SetActive(false);
        spriteToChange.sortingOrder = 3;
        sR.color = baseColor;
        sR.sortingOrder = -1;
        rangeSprite.color = new Color(1, 1, 1, 1);
        sR.sprite = baseSprite;
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

        //Check for path
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
    public void InDecline()
    {
        spriteToChange.sprite = buildManager.GetTurretToBuild().prefab.GetComponent<SpriteRenderer>().sprite;
        ChangeRange(true, buildManager.GetTurretToBuild().prefab.GetComponent<Turret>().range);
        spriteToChange.gameObject.SetActive(true);
        spriteToChange.color = new Color(0.5843138f, 0, 0);
        sR.color = new Color(0.5843138f, 0, 0);
        sR.sortingOrder = 0;
        spriteToChange.sortingOrder = 1;
        rangeSprite.color = new Color(1, 0, 0, 0.78f);
        sR.sprite = hoverBackground;
    }
    public void OnHoverSell(bool active)
    {
        if (active)
        {
            spriteToChange.sprite = buildManager.GetTurretToBuild().prefab.GetComponent<SpriteRenderer>().sprite;
            ChangeRange(true, buildManager.GetTurretToBuild().prefab.GetComponent<Turret>().range);
            spriteToChange.gameObject.SetActive(true);
            spriteToChange.color = new Color(0.5843138f, 0, 0);
            sR.color = new Color(0.5843138f, 0, 0);
            sR.sortingOrder = 1;
            spriteToChange.sortingOrder = 2;
            rangeSprite.color = new Color(1, 0, 0, 0.78f);
            sR.sprite = hoverBackground;
        }
    }
}
