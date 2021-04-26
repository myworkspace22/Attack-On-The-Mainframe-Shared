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
    public TextMeshProUGUI upgradeDescription;

    public TextMeshProUGUI levelUpCost;
    public TextMeshProUGUI upgradeCost1;
    public TextMeshProUGUI upgradeCost2;
    public TextMeshProUGUI sellAmount;

    private Node target;
    public Button upgradeButton1;
    public Button upgradeButton2;

    public GameObject levelUpButton;
    public GameObject UpgradeButtons;
    public void SetTarget(Node _target)
    {
        target = _target;

        //transform.position = target.GetBuildPosition();

        if (!target.isMaxed)
        {

            int multiplyer = (target.upgradeNr > 0) ? 2 : 1;

            upgradeDescription.text = "";//"Upgrades: <color=#00FF00>" + target.turretBlueprint.upgradeDescription[target.upgradeNr + 1 * multiplyer - 1] + " OR " + target.turretBlueprint.upgradeDescription[target.upgradeNr + 2 * multiplyer - 1] + "</color>";
            upgradeCost1.text = target.turretBlueprint.upgradeNames[target.upgradeNr + 1 * multiplyer - 1] + ": <color=#FFD500>$" + target.turretBlueprint.upgradeCost[target.upgradeNr + 1 * multiplyer - 1] + "</color>";
            upgradeCost2.text = target.turretBlueprint.upgradeNames[target.upgradeNr + 2 * multiplyer - 1] + ": <color=#FFD500>$" + target.turretBlueprint.upgradeCost[target.upgradeNr + 2 * multiplyer - 1] + "</color>";
            upgradeButton1.interactable = true;
            upgradeButton2.interactable = true;

            upgradeButton1.gameObject.SetActive(true);
        }
        else
        {

            upgradeDescription.text = "<color=#00FF00>UPGRADED</color>";
            upgradeCost1.text = "MAXED";
            upgradeCost2.text = "MAXED";
            upgradeButton1.interactable = false;
            upgradeButton2.interactable = false;

            upgradeButton1.gameObject.SetActive(false);
        }

        title.text = target.turretBlueprint.title;
        description.text = target.turretBlueprint.description;
        damage.text = "Damage: " + target.turret.GetComponent<Turret>().bulletDamage; //+ " -> <b><color=#00FF00>" + target.turretBlueprint.upgradedPrefab.GetComponent<Turret>().bulletPrefab.GetComponent<Bullet>().damage + "</color></b>";
        range.text = "Range: " + target.turret.GetComponent<Turret>().range + "00"; //+ " -> <b><color=#00FF00>" + target.turretBlueprint.upgradedPrefab.GetComponent<Turret>().range + "</color></b>";
        firerate.text = "Frequency: " + target.turret.GetComponent<Turret>().fireRate; //+ " -> <b><color=#00FF00>" + target.turretBlueprint.upgradedPrefab.GetComponent<Turret>().fireRate + "</color></b>";

        int nextLevel = target.towerLevel + 1;
        int nextLevelCost = target.turretBlueprint.levelUpCost * target.UpgradeMultiplier;
        levelUpCost.text = "Upgrade: " + "<color=#FFD500>$" + nextLevelCost + "</color>"; //level " + nextLevel
        sellAmount.text = "SELL: <color=#FFD500>$" + target.SellAmount + "</color>";

        UpdateUiButtons();

        ui.SetActive(true);
    }

    private void UpdateUiButtons()
    {
        levelUpButton.SetActive(target.towerLevel < 3);
        UpgradeButtons.SetActive(target.towerLevel >= 3);
    }

    public void ShowUpgradeStats(int upgradeIndex)
    {
        if (target.towerLevel < 3)
        {
            damage.text = "Damage: " + target.turret.GetComponent<Turret>().bulletDamage + " <color=#00FF00>-> " + (target.turret.GetComponent<Turret>().bulletDamage + 5) + "</color>";
            range.text = "Range: " + target.turret.GetComponent<Turret>().range + " <color=#00FF00>-> " + (target.turret.GetComponent<Turret>().range + 0.1f) + "00" + "</color>";
            firerate.text = "Frequency: " + target.turret.GetComponent<Turret>().fireRate + " <color=#00FF00>-> " + (target.turret.GetComponent<Turret>().fireRate + 0.2f) + "</color>";
            return;
        }

        if (target.isMaxed)
            return;

        int multiplyer = (target.upgradeNr > 0) ? 2 : 1;

        upgradeDescription.text = "<color=#00FF00>" + target.turretBlueprint.upgradeDescription[target.upgradeNr + upgradeIndex * multiplyer - 1] + "</color>";

        damage.text = "Damage: " + target.turret.GetComponent<Turret>().bulletDamage + " <color=#00FF00>-> " + target.turretBlueprint.upgradedPrefab[target.upgradeNr + upgradeIndex * multiplyer - 1].GetComponent<Turret>().bulletDamage + "</color>";
        range.text = "Range: " + target.turret.GetComponent<Turret>().range + " <color=#00FF00>-> " + target.turretBlueprint.upgradedPrefab[target.upgradeNr + upgradeIndex * multiplyer - 1].GetComponent<Turret>().range + "00" + "</color>";
        firerate.text = "Frequency: " + target.turret.GetComponent<Turret>().fireRate + " <color=#00FF00>-> " + target.turretBlueprint.upgradedPrefab[target.upgradeNr + upgradeIndex * multiplyer - 1].GetComponent<Turret>().fireRate + "</color>";
    }
    public void HideUpgradeStats()
    {
        if(target.towerLevel < 3)
        {
            damage.text = "Damage: " + target.turret.GetComponent<Turret>().bulletDamage;
            range.text = "Range: " + target.turret.GetComponent<Turret>().range + "00";
            firerate.text = "Frequency: " + target.turret.GetComponent<Turret>().fireRate;
            return;
        }

        if (target.isMaxed)
            return;

        upgradeDescription.text = "";
        damage.text = "Damage: " + target.turret.GetComponent<Turret>().bulletDamage;
        range.text = "Range: " + target.turret.GetComponent<Turret>().range + "00";
        firerate.text = "Frequency: " + target.turret.GetComponent<Turret>().fireRate;
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
}
