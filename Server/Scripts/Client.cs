using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;
using System.Net;
using System.Net.Sockets;

public class Client
{
    public static int dataBufferSize = 2048;

    public int id;
    public Player player;
    public TCP tcp;
    //public UDP udp;
    public Client(int _clientId)
    {
        id = _clientId;
        tcp = new TCP(id);
        //udp = new UDP(id);
    }

    public class TCP
    {
        public TcpClient socket;

        private readonly int id;
        private NetworkStream stream;
        private Packet recievedData;
        private byte[] recieveBuffer;

        public TCP(int _id)
        {
            id = _id;
        }

        public void Connect(TcpClient _socket)
        {
            socket = _socket;
            socket.ReceiveBufferSize = dataBufferSize; // 4 mb MAYBE LESS?
            socket.SendBufferSize = dataBufferSize;

            stream = socket.GetStream();

            recievedData = new Packet();

            recieveBuffer = new byte[dataBufferSize];

            stream.BeginRead(recieveBuffer, 0, dataBufferSize, RecieveCallBack, null);

            ServerSend.Welcome(id, "Welcome To Server");
        }

        public void SendData(Packet _packet)
        {
            try
            {
                if (socket != null)
                {
                    stream.BeginWrite(_packet.ToArray(), 0, _packet.Length(), null, null);
                }
            }
            catch (Exception e)
            {
                Debug.Log($"Error {e}");
            }
        }

        private void RecieveCallBack(IAsyncResult _result)
        {
            try
            {
                int _byteLength = stream.EndRead(_result);
                if (_byteLength <= 0)
                {
                    Server.clients[id].Discconect();
                    return;
                }
                byte[] _data = new byte[_byteLength];
                Array.Copy(recieveBuffer, _data, _byteLength);

                recievedData.Reset(HandleData(_data));

                stream.BeginRead(recieveBuffer, 0, dataBufferSize, RecieveCallBack, null);
            }
            catch (Exception e)
            {
                Debug.Log($"Error recieveing TCP data: {e}");
                Server.clients[id].Discconect();
            }
        }
        private bool HandleData(byte[] _data)
        {
            int _packetLength = 0;

            recievedData.SetBytes(_data);

            if (recievedData.UnreadLength() >= 4)
            {
                _packetLength = recievedData.ReadInt();
                if (_packetLength <= 0)
                {
                    return true;
                }
            }

            while (_packetLength > 0 && _packetLength <= recievedData.UnreadLength())
            {
                byte[] _packetBytes = recievedData.ReadBytes(_packetLength);
                ThreadManager.ExecuteOnMainThread(() =>
                {
                    using (Packet _packet = new Packet(_packetBytes))
                    {
                        int _packetId = _packet.ReadInt();
                        Server.PacketHandlers[_packetId](id, _packet);
                    }
                });
                _packetLength = 0;

                if (recievedData.UnreadLength() >= 4)
                {
                    _packetLength = recievedData.ReadInt();
                    if (_packetLength <= 0)
                    {
                        return true;
                    }
                }
            }
            if (_packetLength <= 1)
            {
                return true;
            }
            return false;
        }

        public void Discconect()
        {
            socket.Close();
            stream = null;
            recievedData = null;
            recieveBuffer = null;
            socket = null;
        }

    }

    #region teams
    public int team;
    #endregion

    public void SendIntoGame(string _playerName, int _team)
    {
        if (GamemodeSelect.gamemodeConfirmed)
        {
            player = NetworkManager.instance.InstantiatePlayer(_team);
            player.Initialize(id, _playerName);

            foreach (Client _client in Server.clients.Values)
            {
                if (_client.player != null)
                {
                    if (_client.id != id)
                    {
                        ServerSend.SpawnPlayer(id, _client.player);
                        team = _team;
                    }
                }
            }

            foreach (Client _client in Server.clients.Values)
            {
                if (_client.player != null)
                {
                    ServerSend.SpawnPlayer(_client.id, player);
                    team = _team;
                }
            }
        }
    }

    private void Discconect()
    {
        Debug.Log($"{tcp.socket.Client.RemoteEndPoint} has dissconnected");

        ThreadManager.ExecuteOnMainThread(() =>
        {
            UnityEngine.Object.Destroy(player.gameObject);
            player = null;
        });
        tcp.Discconect();

        ServerSend.PlayerDisconnected(id);
        //udp.Discconect();
    }



}
