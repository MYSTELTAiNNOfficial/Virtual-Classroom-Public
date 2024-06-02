using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class ChatController : NetworkBehaviour
{
    NetworkVariable<FixedString512Bytes> chats = new NetworkVariable<FixedString512Bytes>();
    NetworkList<FixedString512Bytes> chatsList;
    public string chatString;
    public List<string> chatList;

    private void OnChatValueChanged(FixedString512Bytes previousValue, FixedString512Bytes newValue)
    {
        if (newValue.ToString() != null && newValue.ToString() != "")
        {
            AddChatClientRpc(newValue.ToString());
            RemoveChatsNetVarDataServerRpc();
        }
    }

    private void Awake()
    {
        chatsList = new NetworkList<FixedString512Bytes>();
        chats.OnValueChanged += OnChatValueChanged;
        chatsList.OnListChanged += OnChatsListChanged;
    }

    //private void NetworkManager_OnClientDisconnectCallback(ulong obj)
    //{
    //    chats.OnValueChanged -= OnChatValueChanged;
    //    chatsList.OnListChanged -= OnChatsListChanged;
    //    NetworkManager.OnClientDisconnectCallback -= NetworkManager_OnClientDisconnectCallback;

    //}

    private void OnChatsListChanged(NetworkListEvent<FixedString512Bytes> changeEvent)
    {
        switch (changeEvent.Type)
        {
            case NetworkListEvent<FixedString512Bytes>.EventType.Add:
                {
                    AddChatListClientRpc(changeEvent.Value.ToString());
                    break;
                }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void RemoveChatsNetVarDataServerRpc()
    {
        chats.Value = null;
    }

    [ServerRpc(RequireOwnership = false)]
    public void AddChatServerRpc(string text)
    {
        chats.Value = chats.Value.ToString() + text;
        AddChatClientRpc(text);
    }
    [ClientRpc]
    public void AddChatClientRpc(string text)
    {
        chatString = text;
    }

    public void SendChat(string text)
    {
        if (text == "" || text == null)
        {
            return;
        }

        AddChatListServerRpc(text);
    }

    public string GetChatList()
    {
        return chatString;
    }

    //PLACEHOLDER

    [ServerRpc(RequireOwnership = false)]
    public void RemoveChatListServerRpc()
    {
        chats.Value = null;
    }

    [ServerRpc(RequireOwnership = false)]
    public void AddChatListServerRpc(string text)
    {
        chatsList.Add(new FixedString512Bytes(text));
        AddChatListClientRpc(text);
    }

    [ClientRpc]
    public void AddChatListClientRpc(string text)
    {
        chatList.Add(text);
    }

    public string GetChatsList()
    {
        return chatList[chatList.Count - 1];
    }
}
