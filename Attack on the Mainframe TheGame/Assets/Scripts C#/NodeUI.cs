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

    public TextMeshProUGUI upgradeCost;
    public TextMeshProUGUI sellAmount;

    private Node target;
    public Button upgradeButton;

    public void SetTarget(Node _target)
    {
        target = _target;

        //transform.position = target.GetBuildPosition();

        if (!target.isUpgraded)
        {

            upgradeDescription.text = "Upgrades: <color=#00FF00>" + target.turretBlueprint.upgadeDescription1 + " OR " + target.turretBlueprint.upgadeDescription2 + "</color>";
            upgradeCost.text = "UPGRADE: <color=#FFD500>$" + target.turretBlueprint.upgradeCost + "</color>";
            upgradeButton.interactable = true;
        }
        else
        {

            upgradeDescription.text = "<color=#00FF00>UPGRADED</color>";
            upgradeCost.text = "MAXED";
            upgradeButton.interactable = false;
        }

        title.text = target.turretBlueprint.title;
        description.text = target.turretBlueprint.description;
        damage.text = "DAMAGE: " + target.turret.GetComponent<Turret>().bulletPrefab.GetComponent<Bullet>().damage + " -> <b><color=#00FF00>" + target.turretBlueprint.upgradedPrefab.GetComponent<Turret>().bulletPrefab.GetComponent<Bullet>().damage + "</color></b>";
        range.text = "RANGE: " + target.turret.GetComponent<Turret>().range + " -> <b><color=#00FF00>" + target.turretBlueprint.upgradedPrefab.GetComponent<Turret>().range + "</color></b>";
        firerate.text = "Firerate: " + target.turret.GetComponent<Turret>().fireRate + " -> <b><color=#00FF00>" + target.turretBlueprint.upgradedPrefab.GetComponent<Turret>().fireRate + "</color></b>";

        sellAmount.text = "SELL: <color=#FFD500>$" + target.turretBlueprint.GetSellAmount() + "</color>";
        

        ui.SetActive(true);
    }
    public void Hide()
    {
        ui.SetActive(false);
    }
    public void Upgrade()
    {
        target.UpgradeTurret();
        //BuildManager.instance.DeselectNode();
        upgradeCost.text = "DONE";
        upgradeButton.interactable = false;
    }
    public void Sell()
    {
        target.SellTurret();
        BuildManager.instance.DeselectNode();
        target.isUpgraded = false;
    }
}
