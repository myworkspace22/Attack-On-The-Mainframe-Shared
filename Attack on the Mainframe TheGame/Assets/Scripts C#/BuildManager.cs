using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildManager : MonoBehaviour
{
    public static BuildManager instance;
    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("More than one BuildManager in Scene!");
            return;
        }
        instance = this;
    }

    private TurretBluePrint turretToBuild;
    private Node selectedNode;

    public NodeUI nodeUI;
    public ShopUI shopUI;

    public PlayerStats playerstats;

    public bool CanBuild { get { return turretToBuild != null; } }
    public bool HasMoney { get { return playerstats.Money >= turretToBuild.cost; } }

    public void SelectNode (Node node)
    {
        if (selectedNode == node)
        {
            DeselectNode();
            return;
        }

        if (selectedNode != null)
            selectedNode.ChangeRange(false);

        if (node.turret != null)
            node.ChangeRange(true, node.turret.GetComponent<Turret>().range);

        selectedNode = node;
        shopUI.DeselectTower();
        turretToBuild = null;

        nodeUI.SetTarget(node);
    }
    public void DeselectNode()
    {
        if (selectedNode != null)
            selectedNode.ChangeRange(false);
        selectedNode = null;
        nodeUI.Hide();
    }
    public void SelectTurretToBuild (TurretBluePrint turret)
    {
        turretToBuild = turret;
        shopUI.SelectTower(turret);
        //selectedNode = null;

        DeselectNode();
    }
    public TurretBluePrint GetTurretToBuild()
    {
        return turretToBuild;
    }
    //public void DeselectTower()
    //{
    //    turretToBuild = null;
    //    shopUI.DeselectTower();
    //}
}
