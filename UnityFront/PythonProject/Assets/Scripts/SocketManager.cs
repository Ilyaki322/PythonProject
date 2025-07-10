using System;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;

public class SocketManager : MonoBehaviour
{
    SocketIOUnity m_socket;
    [SerializeField] GameController m_gameController;

    string m_token;

    public void SetToken(string token) => m_token = token;

    public void InitSocket(CharacterCombatController enemyController)
    {
        m_socket.On("Attack", (res) =>
        {
            MainThreadDispatcher.Instance.Enqueue(() =>
            {
                enemyController.OnAttack();
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

        m_socket.On("UseItem", (res) =>
        {
            MainThreadDispatcher.Instance.Enqueue(() =>
            {
                enemyController.OnItemUse();
                m_gameController.NextTurn();
            });
        });

        m_socket.On("FourthButton", (res) =>
        {
            MainThreadDispatcher.Instance.Enqueue(() =>
            {
                enemyController.OnFourthButton();
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

    public void OnEnterQueue(int charID)
    {
        print("Clicked EnterQueue");
        m_socket.Emit("EnterQueue", new {charID = charID});
    }

    public void OnLeaveQueue()
    {
        print("Clicked LeaveQueue");
        m_socket.Emit("LeaveQueue", "unity: left queue");
    }

    public void OnAttack() => m_socket.Emit("Attack");
    public void OnDefend() => m_socket.Emit("Defend");
    public void OnItemUse() => m_socket.Emit("UseItem");
    public void OnFourthButton() => m_socket.Emit("FourthButton");

    async void OnDestroy()
    {
        if (m_socket != null && m_socket.Connected)
        {
            await m_socket.DisconnectAsync();
        }
        m_socket?.Dispose();
    }
}
