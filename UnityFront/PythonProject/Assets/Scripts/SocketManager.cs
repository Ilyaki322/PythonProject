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

        m_socket.OnAny((name, response) =>
        {
            Debug.Log($"Event: {name}, Response: {response}");
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

    async void OnDestroy()
    {
        if (m_socket != null && m_socket.Connected)
        {
            await m_socket.DisconnectAsync();
        }
        m_socket?.Dispose();
    }
}
