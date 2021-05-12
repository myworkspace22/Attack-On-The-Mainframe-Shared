using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using TMPro;
using UnityEngine.UI;
using System;

public class NetworkGamePlayer : NetworkBehaviour
{

    [SyncVar]
    private string displayName = "Loading...";

    //[SyncVar(hook = nameof(HandleReadyStatusChanged))]
    //public bool IsReady = false;

    //private bool isLeader;

    //public bool IsLeader 
    //{
    //    set 
    //    { 
    //        isLeader = value;
    //        startGameButton.gameObject.SetActive(value);
    //    } 
    //}

    private NetworkManagerLobby room;
    private NetworkManagerLobby Room 
    { 
        get 
        { 
            if (room != null) 
            { 
                return room; 
            }
            return room = NetworkManager.singleton as NetworkManagerLobby; 
        } 
    }


    public override void OnStartClient()
    {

        DontDestroyOnLoad(gameObject); // ik nej så

        Room.GamePlayers.Add(this);
    }

    public override void OnStopClient()
    {

        Room.GamePlayers.Remove(this);

    }

    [Server]
    public void SetDisplayName(string displayName)
    {
        this.displayName = displayName;
    }
}

