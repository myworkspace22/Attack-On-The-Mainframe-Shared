using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NodeUI : MonoBehaviour
{
    public GameObject ui;

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
            upgradeCost.text = "UPGRADE: $" + target.turretBlueprint.upgradeCost;
            upgradeButton.interactable = true;
        }
        else
        {
            upgradeCost.text = "MAXED";
            upgradeButton.interactable = false;
        }

        sellAmount.text = "SELL: $" + target.turretBlueprint.GetSellAmount();

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
