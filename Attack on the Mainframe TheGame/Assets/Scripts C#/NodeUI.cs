using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NodeUI : MonoBehaviour
{
    public GameObject ui;

    public TextMeshProUGUI title;

    public TextMeshProUGUI description;
    public TextMeshProUGUI damage;
    public TextMeshProUGUI range;
    public TextMeshProUGUI firerate;
    //public TextMeshProUGUI upgradeDescription;
    public TextMeshProUGUI effect;

    public TextMeshProUGUI levelUpCost;
    public TextMeshProUGUI upgradeCost1;
    public TextMeshProUGUI upgradeCost2;
    public TextMeshProUGUI sellAmount;

    private Node target;
    public Button upgradeButton1;
    public Button upgradeButton2;
    public Button sellButton;

    public GameObject levelUpButton;
    public GameObject UpgradeButtons;

    private void Update()
    {
        sellButton.interactable = BuildManager.instance.GetComponent<WaveSpawner>().BuildMode;
        if (!target.isMaxed)
        {
            int multiplyer = (target.upgradeNr > 0) ? 2 : 1;
            levelUpButton.GetComponent<Button>().interactable = PlayerStats.Money >= target.turretBlueprint.levelUpCost * (target.towerLevel + 1);
            upgradeButton1.interactable = PlayerStats.Money >= target.turretBlueprint.upgradeCost[target.upgradeNr + 1 * multiplyer - 1];
            upgradeButton2.interactable = PlayerStats.Money >= target.turretBlueprint.upgradeCost[target.upgradeNr + 2 * multiplyer - 1];
        }

        //if (BuildManager.instance.GetComponent<WaveSpawner>().BuildMode) 
        //{
        //    //if (!target.isMaxed)
        //    //{
        //    //    int multiplyer = (target.upgradeNr > 0) ? 2 : 1;
        //    //    levelUpButton.GetComponent<Button>().interactable = PlayerStats.Money >= target.turretBlueprint.levelUpCost * target.UpgradeMultiplier;
        //    //    upgradeButton1.interactable = PlayerStats.Money >= target.turretBlueprint.upgradeCost[target.upgradeNr + 1 * multiplyer - 1];
        //    //    upgradeButton2.interactable = PlayerStats.Money >= target.turretBlueprint.upgradeCost[target.upgradeNr + 2 * multiplyer - 1];
        //    //}
        //    sellButton.interactable = true;
        //}
        //else
        //{
        //    //levelUpButton.GetComponent<Button>().interactable = false;
        //    //upgradeButton1.interactable = false;
        //    //upgradeButton2.interactable = false;
        //    //sellAmount.text = "Sell: <color=#FFD500>$" + target.SellAmount + "</color>";
        //    sellButton.interactable = false;
        //}



    }
    public void SetTarget(Node _target)
    {
        target = _target;

        //transform.position = target.GetBuildPosition();

        if (!target.isMaxed)
        {
            int multiplyer = (target.upgradeNr > 0) ? 2 : 1;

           /* upgradeDescription.text = "";*///"Upgrades: <color=#00FF00>" + target.turretBlueprint.upgradeDescription[target.upgradeNr + 1 * multiplyer - 1] + " OR " + target.turretBlueprint.upgradeDescription[target.upgradeNr + 2 * multiplyer - 1] + "</color>";
            upgradeCost1.text = target.turretBlueprint.upgradeNames[target.upgradeNr + 1 * multiplyer - 1] + ": <color=#FFD500>$" + target.turretBlueprint.upgradeCost[target.upgradeNr + 1 * multiplyer - 1] + "</color>";
            upgradeCost2.text = target.turretBlueprint.upgradeNames[target.upgradeNr + 2 * multiplyer - 1] + ": <color=#FFD500>$" + target.turretBlueprint.upgradeCost[target.upgradeNr + 2 * multiplyer - 1] + "</color>";
            upgradeButton1.interactable = PlayerStats.Money >= target.turretBlueprint.upgradeCost[target.upgradeNr + 1 * multiplyer - 1];
            upgradeButton2.interactable = PlayerStats.Money >= target.turretBlueprint.upgradeCost[target.upgradeNr + 2 * multiplyer - 1];

            upgradeButton1.gameObject.SetActive(true);
        }
        else
        {
            //upgradeDescription.text = "<color=#00FF00>UPGRADED</color>";
            upgradeCost1.text = "MAXED";
            upgradeCost2.text = "MAXED";
            levelUpCost.text = "MAXED";
            upgradeButton1.interactable = false;
            upgradeButton2.interactable = false;
            levelUpButton.GetComponent<Button>().interactable = false;

            upgradeButton1.gameObject.SetActive(false);
        }
        string titleToDisplay = (target.upgradeNr > 0) ? target.turretBlueprint.upgradeNames[target.upgradeNr - 1] : target.turretBlueprint.title;
        title.text = (target.UpgradeMultiplier >= 3) ? "<color=#FFD500>" + titleToDisplay + " (" + target.UpgradeMultiplier + ")</color>" : titleToDisplay + " (" + target.UpgradeMultiplier + ")";
        description.text = (target.upgradeNr > 0) ? target.turretBlueprint.upgradeDescription[target.upgradeNr - 1] : target.turretBlueprint.description;
        damage.text = "Damage: " + target.turret.GetComponent<Turret>().bulletDamage; //+ " -> <b><color=#00FF00>" + target.turretBlueprint.upgradedPrefab.GetComponent<Turret>().bulletPrefab.GetComponent<Bullet>().damage + "</color></b>";
        range.text = "Range: " + target.turret.GetComponent<Turret>().range * 100; //+ " -> <b><color=#00FF00>" + target.turretBlueprint.upgradedPrefab.GetComponent<Turret>().range + "</color></b>";
        firerate.text = "Frequency: " + target.turret.GetComponent<Turret>().fireRate; //+ " -> <b><color=#00FF00>" + target.turretBlueprint.upgradedPrefab.GetComponent<Turret>().fireRate + "</color></b>";
        effect.text = (target.upgradeNr > 0) ? target.turretBlueprint.upgradeEffect[target.upgradeNr - 1] : string.Empty;

        int nextLevel = target.towerLevel + 1;
        int nextLevelCost = target.turretBlueprint.levelUpCost * (target.towerLevel + 1);
        levelUpCost.text = (target.isMaxed)? "<color=#FFFFFF>MAXED</color>" : "Upgrade: <color=#FFD500>$" + nextLevelCost + "</color>"; //level " + nextLevel
        sellAmount.text = "Sell: <color=#FFD500>$" + target.SellAmount + "</color>";

        UpdateUiButtons();

        ui.SetActive(true);
    }

    private void UpdateUiButtons()
    {
        bool readyToUpgrade = target.towerLevel == 2 || target.towerLevel == 5;
        levelUpButton.SetActive(!readyToUpgrade);
        UpgradeButtons.SetActive(readyToUpgrade);
    }

    public void ShowUpgradeStats(int upgradeIndex)
    {
        if (target.isMaxed)
            return;

        Turret turretToUpgrade = target.turret.GetComponent<Turret>();

        bool readyToUpgrade = target.towerLevel == 2 || target.towerLevel == 5;

        if (!readyToUpgrade)
        {
            damage.text = "Damage: " + turretToUpgrade.bulletDamage + " <color=#00FF00>-> " + (turretToUpgrade.bulletDamage + turretToUpgrade.upgradeDamage * (target.towerLevel + 1)) + "</color>";
            range.text = "Range: " + turretToUpgrade.range * 100 + " <color=#00FF00>-> " + (turretToUpgrade.range + turretToUpgrade.upgradeRange) * 100 + "</color>";
            firerate.text = "Frequency: " + turretToUpgrade.fireRate + " <color=#00FF00>-> " + (turretToUpgrade.fireRate + turretToUpgrade.upgradeFrenquency) + "</color>";
            return;
        }

        int multiplyer = (target.upgradeNr > 0) ? 2 : 1;

        string currentTitle = (target.upgradeNr > 0) ? target.turretBlueprint.upgradeNames[target.upgradeNr - 1] : target.turretBlueprint.title;
        title.text = currentTitle + " <color=#00FF00>-> " + target.turretBlueprint.upgradeNames[target.upgradeNr + upgradeIndex * multiplyer - 1] + "</color>";
        description.text = "<color=#00FF00>" + target.turretBlueprint.upgradeDescription[target.upgradeNr + upgradeIndex * multiplyer - 1] + "</color>";
        damage.text = "Damage: " + turretToUpgrade.bulletDamage + " <color=#00FF00>-> " + target.turretBlueprint.upgradedPrefab[target.upgradeNr + upgradeIndex * multiplyer - 1].GetComponent<Turret>().bulletDamage + "</color>";
        range.text = "Range: " + turretToUpgrade.range * 100 + " <color=#00FF00>-> " + target.turretBlueprint.upgradedPrefab[target.upgradeNr + upgradeIndex * multiplyer - 1].GetComponent<Turret>().range * 100 + "</color>";
        firerate.text = "Frequency: " + turretToUpgrade.fireRate + " <color=#00FF00>-> " + target.turretBlueprint.upgradedPrefab[target.upgradeNr + upgradeIndex * multiplyer - 1].GetComponent<Turret>().fireRate + "</color>";
        effect.text = "<color=#00FF00>" + target.turretBlueprint.upgradeEffect[target.upgradeNr + upgradeIndex * multiplyer - 1] + "</color>";
    }
    public void HideUpgradeStats()
    {
        bool readyToUpgrade = target.towerLevel == 2 || target.towerLevel == 5;

        if (!readyToUpgrade)
        {
            damage.text = "Damage: " + target.turret.GetComponent<Turret>().bulletDamage;
            range.text = "Range: " + target.turret.GetComponent<Turret>().range * 100;
            firerate.text = "Frequency: " + target.turret.GetComponent<Turret>().fireRate;
            return;
        }

        if (target.isMaxed)
            return;

        string titleToDisplay = (target.upgradeNr > 0) ? target.turretBlueprint.upgradeNames[target.upgradeNr - 1] : target.turretBlueprint.title;
        title.text = (target.UpgradeMultiplier >= 3) ? "<color=#FFD500>" + titleToDisplay + " (" + target.UpgradeMultiplier + ")</color>" : titleToDisplay + " (" + target.UpgradeMultiplier + ")";
        description.text = (target.upgradeNr > 0) ? target.turretBlueprint.upgradeDescription[target.upgradeNr - 1] : target.turretBlueprint.description;
        damage.text = "Damage: " + target.turret.GetComponent<Turret>().bulletDamage;
        range.text = "Range: " + target.turret.GetComponent<Turret>().range * 100;
        firerate.text = "Frequency: " + target.turret.GetComponent<Turret>().fireRate;
        effect.text = (target.upgradeNr > 0) ? target.turretBlueprint.upgradeEffect[target.upgradeNr - 1] : string.Empty;
    }

    public void Hide()
    {
        ui.SetActive(false);
    }
    public void Upgrade(int upgradeIndex)
    {
        target.UpgradeTurret(upgradeIndex);
        SetTarget(target);

        //BuildManager.instance.DeselectNode();

        //upgradeCost1.text = "DONE";
        //upgradeCost2.text = "DONE";
        //upgradeButton1.interactable = false;
        //upgradeButton2.interactable = false;
    }

    public void LevelUp()
    {
        target.levelUpTower();
    }
    public void Sell()
    {
        target.SellTurret();
        BuildManager.instance.DeselectNode();
        target.isMaxed = false;
    }
    //public void InDecline(bool active)
    //{
    //    target.OnHoverSell(active);
    //}
}
