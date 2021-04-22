using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopUI : MonoBehaviour
{
    public GameObject ui;

    public TextMeshProUGUI title;

    public TextMeshProUGUI description;
    public TextMeshProUGUI cost;
    public TextMeshProUGUI damage;
    public TextMeshProUGUI range;
    public TextMeshProUGUI firerate;
    public TextMeshProUGUI upgradeDescription; //måske

    public void SelectTower(TurretBluePrint towerBP)
    {
        title.text = towerBP.title;
        description.text = towerBP.description;
        cost.text = "Cost: <b><color=#FFD500>$" + towerBP.cost + "</color></b>";
        damage.text = "DAMAGE: <color=#00FF00>" + towerBP.prefab.GetComponent<Turret>().bulletPrefab.GetComponent<Bullet>().damage + "</color>";
        range.text = "RANGE: <color=#00FF00>" + towerBP.prefab.GetComponent<Turret>().range + "</color>";
        firerate.text = "Firerate: <color=#00FF00>" + towerBP.prefab.GetComponent<Turret>().fireRate + "</color>";
        upgradeDescription.text = "Upgrades: " + towerBP.upgadeDescription1 + " OR " + towerBP.upgadeDescription2;

        ui.SetActive(true);
    }

    public void DeselectTower()
    {
        ui.SetActive(false);
    }
}
