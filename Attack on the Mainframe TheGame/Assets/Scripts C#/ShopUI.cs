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

    public void SelectTower(TurretBluePrint towerBP)
    {
        title.text = towerBP.title;
        description.text = towerBP.description;
        cost.text = "Cost: <b><color=#FFD500>$" + towerBP.cost + "</color></b>";
        damage.text = "DAMAGE: " + towerBP.prefab.GetComponent<Turret>().bulletDamage;
        range.text = "RANGE: " + towerBP.prefab.GetComponent<Turret>().range;
        firerate.text = "Firerate: " + towerBP.prefab.GetComponent<Turret>().fireRate;

        ui.SetActive(true);
    }

    public void DeselectTower()
    {
        ui.SetActive(false);
    }
}
