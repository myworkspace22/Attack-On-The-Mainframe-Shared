using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTEST : NetworkBehaviour
{

    [SerializeField]
    private Vector2 movement = new Vector2();

    [Client]
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && hasAuthority)
        {
            //transform.Translate(movement);
            CmdMove();
        }
    }

    [Command]
    private void CmdMove()
    {
        // validate 
        RpcMove();
    }

    [ClientRpc]
    private void RpcMove()
    {
        transform.Translate(movement);
    }
}
