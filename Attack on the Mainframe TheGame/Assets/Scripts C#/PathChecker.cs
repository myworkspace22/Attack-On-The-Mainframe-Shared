using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathChecker : MonoBehaviour
{
    public Transform endTarget;

    public bool PathCheck()
    {
        GraphNode node1 = AstarPath.active.GetNearest(transform.position, NNConstraint.Default).node;
        GraphNode node2 = AstarPath.active.GetNearest(endTarget.position, NNConstraint.Default).node;

        return PathUtilities.IsPathPossible(node1, node2);
    }
}
