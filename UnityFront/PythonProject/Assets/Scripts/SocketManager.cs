using System;
using System.Collections.Generic;
using UnityEngine;

public class SocketManager : MonoBehaviour
{
    SocketIOUnity m_socket;
    [SerializeField] GameController m_gameController;

    string m_token;

    public void SetToken(string token) => m_token = token;

    public void Emit(string eventName, object payload)
    {
        m_socket.Emit(eventName, payload);
    }

    public void Logout()
    {
        m_token = null;
        m_socket.Disconnect();
    }

    public void InitSocket(CharacterCombatController enemyController)
    {
        m_socket.On("Attack", (res) =>
        {
            MainThreadDispatcher.Instance.Enqueue(() =>
            {
                enemyController.OnAttack(res.GetValue<int>());
                m_gameController.NextTurn();
            });
        });

        m_socket.On("Defend", (res) =>
        {
            MainThreadDispatcher.Instance.Enqueue(() =>
            {
                enemyController.OnDefend();
                m_gameController.NextTurn();
            });
        });

        m_socket.On("UseItem", res =>
        {
            MainThreadDispatcher.Instance.Enqueue(() =>
            {
                var guid = res.GetValue<string>();
                enemyController.OnItemUse(guid);

            });
        });


        m_socket.On("Win", (res) =>
        {
            MainThreadDispatcher.Instance.Enqueue(() =>
            {
                enemyController.OnFourthButton();
                m_gameController.OnWin();
            });
        });

        m_socket.On("Lose", (res) =>
        {
            MainThreadDispatcher.Instance.Enqueue(() =>
            {
                m_gameController.OnLose();
            });
        });

        m_socket.On("SkipTurn", (res) =>
        {
            MainThreadDispatcher.Instance.Enqueue(() =>
            {
                m_gameController.NextTurn();
            });
        });
    }

    public void Connect(Action onSuccess)
    {
        Debug.Log("Connecting");
        var uri = new Uri("http://localhost:5000");
        m_socket = new SocketIOUnity(uri, new SocketIOClient.SocketIOOptions
        {
            Query = new Dictionary<string, string>
            {
                {"token", m_token }
            },
            Transport = SocketIOClient.Transport.TransportProtocol.WebSocket
        });

        m_socket.On("connected", (response) =>
        {
            Debug.Log("Connected: " + response);
            onSuccess();
        });

        m_socket.OnDisconnected += (sender, e) =>
        {
            MainThreadDispatcher.Instance.Enqueue(() =>
            {
                m_gameController.HandleDisconnect();
            });
        };

        m_socket.On("MatchFound", (response) =>
        {
            print("IN Event: MatchFound");

            string jsonArray = response.ToString();
            string jsonObject = jsonArray.Substring(1, jsonArray.Length - 2);

            MatchDTO match = JsonUtility.FromJson<MatchDTO>(jsonObject);
            MainThreadDispatcher.Instance.Enqueue(() =>
            {
                m_gameController.OnMatchFound(match.player_character, match.enemy_character, match.is_starting);
            });
        });

        m_socket.Connect();
    }

    public void OnEnterQueue(int charID) => m_socket.Emit("EnterQueue", new {charID = charID});

    public void OnItemUse(int slotIndex, SerializableGuid itemId) => m_socket.Emit("UseItem", new { index = slotIndex, itemId = itemId.ToHexString()});
    public void OnLeaveQueue() => m_socket.Emit("LeaveQueue");
    public void OnAttack(int damage) => m_socket.Emit("Attack", new { damageDealt = damage});
    public void OnDefend() => m_socket.Emit("Defend");
    public void OnItemUse() => m_socket.Emit("UseItem");
    public void OnFourthButton() => m_socket.Emit("EndGame");
    public void OnSkipTurn() => m_socket.Emit("SkipTurn");

    async void OnDestroy()
    {
        if (m_socket != null && m_socket.Connected)
        {
            await m_socket.DisconnectAsync();
        }
        m_socket?.Dispose();
    }
}
