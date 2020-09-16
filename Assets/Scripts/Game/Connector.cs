using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using UnityEngine;
#if UNITY_IOS
using System.Runtime.InteropServices;
#endif

public class Connector : MonoBehaviour {

    /// <summary>
    /// Callback for receive data from socket
    /// </summary>
    public Action<string> _receiveCallback = null;

    /// <summary>
    /// Socket instance
    /// </summary>
    private Socket _socket = null;

    /// <summary>
    /// Buffer for data from socket
    /// </summary>
    private byte[] _receiveBuffer = new byte[512];

    /// <summary>
    /// Buffer for common data
    /// </summary>
    private byte[] _allData = new byte[0];

    /// <summary>
    /// Buffer for temporary data
    /// </summary>
    private byte[] _receiveData = new byte[512];

    /// <summary>
    /// Commnads separator
    /// </summary>
    private byte[] _separator = null;

    /// <summary>
    /// Locker object
    /// </summary>
    private object _lockObject = new object ();

    /// <summary>
    /// Queue for sync
    /// </summary>
    private readonly Queue<Action> _actions = new Queue<Action> (64);

#if UNITY_IOS
    /// <summary>
    /// Convert IPV6 for iOS
    /// </summary>
    /// <param name="host">Host or IP</param>
    [DllImport ("__Internal")]
    private static extern string ConnectorCheckIPV6 (string host);
#endif    

    void Awake () {
        _separator = Encoding.UTF8.GetBytes (new char[] { '\n' });
    }

    /// <summary>
    /// Connect to server
    /// </summary>
    /// <param name="receiveCallback">Callback to receive data</param>
    public void Connect (Action connectCallback, Action failCallback, Action<string> receiveCallback) {
        _receiveCallback = receiveCallback;
        AddressFamily family = AddressFamily.InterNetwork;
        string host = Config.ServerHost;
        try {
#if UNITY_EDITOR
            // Android and others works correct with IPV6
#elif UNITY_IOS
            string ipv6 = ConnectorCheckIPV6 (host);
            if (!string.IsNullOrEmpty (ipv6)) {
                family = AddressFamily.InterNetworkV6;
                host = ipv6;
            }
#endif			
        } catch (Exception) {
            Sync (failCallback);
            return;
        }
        _socket = new Socket (family, SocketType.Stream, ProtocolType.Tcp);
        _socket.SetSocketOption (SocketOptionLevel.Socket, SocketOptionName.KeepAlive, true);
        _socket.BeginConnect (host, Config.ServerPort, new AsyncCallback ((result) => {
            try {
                _socket.EndConnect (result);
                _socket.NoDelay = true;
                BeginReceive ();
                Sync (connectCallback);
            } catch (Exception) {
                Sync (failCallback);
            }
        }), _socket);
    }

    /// <summary>
    /// Begin receive data
    /// </summary>
    private void BeginReceive () {
        _socket.BeginReceive (_receiveBuffer, 0, _receiveBuffer.Length, SocketFlags.None, new AsyncCallback (ReceiveCallback), null);
    }

    /// <summary>
    /// Callback on data receive
    /// Send to process or disconnect
    /// </summary>
    private void ReceiveCallback (IAsyncResult result) {
        try {
            int length = _socket.EndReceive (result);
            if (length > 0) {
                string raw = Encoding.UTF8.GetString (_receiveBuffer, 0, length);
                string[] commands = raw.Split ('\n');
                foreach (string cmd in commands) {
                    if (!string.IsNullOrEmpty (cmd)) {
                        Sync (() => {
                            _receiveCallback.Invoke (cmd);
                        });
                    }
                }
                BeginReceive ();
            }
        } catch (Exception) { }
    }

    /// <summary>
    /// Send data to socket
    /// </summary>
    /// <param name="buffer">Bytes array</param>
    private void SendData (byte[] buffer) {
        _socket.BeginSend (buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback (SendCallback), null);
    }

    /// <summary>
    /// Callback data sent
    /// </summary>
    private void SendCallback (IAsyncResult result) {
        _socket.EndSend (result);
    }

    /// <summary>
    /// Disconnect
    /// </summary>
    public void Disconnect () {
        _socket.Disconnect (false);
    }

    /// <summary>
    /// Send data to server
    /// </summary>
    /// <param name="data">JSON string</param>
    public void Send (string data) {
        data += "\n";
        byte[] bytes = Encoding.UTF8.GetBytes (data);
        SendData (bytes);
    }

    /// <summary>
    /// Check actions to sync
    /// </summary>
    private void Update () {
        lock (_lockObject) {
            while (_actions.Count > 0) {
                _actions.Dequeue () ();
            }
        }
    }

    /// <summary>
    /// Sync to main thread
    /// </summary>
    /// <param name="action">Action</param>
    private void Sync (Action action) {
        lock (_lockObject) {
            _actions.Enqueue (action);
        }
    }

}