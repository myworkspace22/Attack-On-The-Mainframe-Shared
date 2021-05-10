using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using TMPro;
using System;

public class ChatBehavior : NetworkBehaviour
{
    [SerializeField] private GameObject chatUI = null;
    [SerializeField] private TMP_Text chatText = null;
    [SerializeField] private TMP_InputField inputField = null;

    private static event Action<string> OnMessage;

    public override void OnStartAuthority() // kan bruges til vores UI shop osv.
    {
        chatUI.SetActive(true);

        OnMessage += HandleNewMessage;
    }

    [ClientCallback]
    private void OnDestroy()
    {
        if (hasAuthority)
        {
            OnMessage -= HandleNewMessage;
        }
    }

    private void HandleNewMessage(string message)
    {
        chatText.text += message;
    }

    [Client]
    public void Send()//string message)
    {
        string message = inputField.text;
        if(Input.GetKeyDown(KeyCode.Return) && !string.IsNullOrWhiteSpace(message))
        {
            CmdSendMessage(inputField.text);
            inputField.text = string.Empty;
        }
    }

    [Command]
    private void CmdSendMessage(string message)
    {
        //validate
        RpcHandleMessage($"[{connectionToClient.connectionId}]: {message}");
    }

    [ClientRpc]
    private void RpcHandleMessage(string message)
    {
        OnMessage?.Invoke($"\n{message}");
    }
}
