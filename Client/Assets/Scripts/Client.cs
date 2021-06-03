using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Net;
using System.Net.Sockets;
using System;
public class Client : MonoBehaviour
{
    public static Client instance;
    public static int dataBufferSize = 2048;

    public string ip =  "tcp://2.tcp.ngrok.io:19007";//"127.0.0.1"; //"tcp://2.tcp.ngrok.io"; //LOCAL NGROK MAYBE?
    public int port = 2020;
    public int myId = 0;
    public TCP tcp;
    //public UDP udp;

    private bool isConnected = false;

    private delegate void packetHandler(Packet _packet);
    private static Dictionary<int, packetHandler> packetHandlers;

    public List<GameObject> serverObjects = new List<GameObject>();
    public GameObject[] deactivateObjects;
    public Button[] deactivateButtons;
    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }else if(instance != this)
        {
            Debug.Log("Instance Exists YEEET!");
            Destroy(this);
        }
    }

    public void Start()
    {
        tcp = new TCP();
        //udp = new UDP();
    }

    private void OnApplicationQuit()
    {
        Dissconnect();
    }

    public void ConnectToServer()
    {
        initClientData();
        isConnected = true;
        tcp.Connect();
    }
    
    public class TCP
    {
        public TcpClient socket;

        private NetworkStream stream;
        private Packet recievedData;
        private byte[] recieveBuffer;

        public void Connect()
        {
            socket = new TcpClient
            {
                ReceiveBufferSize = dataBufferSize,
                SendBufferSize = dataBufferSize
            };

            recieveBuffer = new byte[dataBufferSize];
            
            socket.BeginConnect(instance.ip, instance.port, ConnectCallBack, socket);
        }

        private void ConnectCallBack(IAsyncResult _result)
        {
            socket.EndConnect(_result);

            if (!socket.Connected)
            {
                return;
            }

            stream = socket.GetStream();

            recievedData = new Packet();

            stream.BeginRead(recieveBuffer, 0, dataBufferSize, RecieveCallback, null);
        }
        
        public void SendData(Packet _packet)
        {
            try
            {
                if(socket != null)
                {
                    stream.BeginWrite(_packet.ToArray(), 0, _packet.Length(), null, null);
                }
            }
            catch(Exception e)
            {
                Debug.Log($"Error: {e}");
            }
        }

        private void RecieveCallback(IAsyncResult _result)
        {
            try
            {
                int _byteLength = stream.EndRead(_result);
                if (_byteLength <= 0)
                {
                    instance.Dissconnect();
                    return;
                }

                byte[] _data = new byte[_byteLength];
                Array.Copy(recieveBuffer, _data, _byteLength);

                recievedData.Reset(HandleData(_data));
                stream.BeginRead(recieveBuffer, 0, dataBufferSize, RecieveCallback, null);
            }
            catch
            {
                Dissconnect();
            }
        }

        private void Dissconnect()
        {
            instance.Dissconnect();

            stream = null;
            recievedData = null;
            recieveBuffer = null;
            socket = null;
        }

        private bool HandleData(byte[] _data)
        {
            int _packetLength = 0;

            recievedData.SetBytes(_data);

            if(recievedData.UnreadLength() >= 4)
            {
                _packetLength = recievedData.ReadInt();
                if (_packetLength <= 0)
                {
                    return true;    
                }
            }

            while(_packetLength > 0 && _packetLength <= recievedData.UnreadLength())
            {
                byte[] _packetBytes = recievedData.ReadBytes(_packetLength);
                ThreadManager.ExecuteOnMainThread(() =>
                {
                    using (Packet _packet = new Packet(_packetBytes))
                    {
                        int _packetId = _packet.ReadInt();
                        packetHandlers[_packetId](_packet);
                    }
                });
                _packetLength = 0;

                if(recievedData.UnreadLength() >= 4)
                {
                    _packetLength = recievedData.ReadInt();
                    if (_packetLength <= 0)
                    {
                        return true;    
                    }
                }
            }
            if(_packetLength <= 1)
            {
                return true;
            }
            return false;
        }
    }
    private void initClientData()
    {

        packetHandlers = new Dictionary<int, packetHandler>()
        {
            {(int) ServerPackets.welcome, ClientHandle.Welcome},
            {(int) ServerPackets.gamemodeSelect, ClientHandle.GamemodeSelect},
            {(int) ServerPackets.spawnPlayer, ClientHandle.SpawnPlayer},
            {(int) ServerPackets.playerPostion, ClientHandle.PlayerPostion},
            {(int) ServerPackets.playerDisconnected, ClientHandle.PlayerDisconnected},
            {(int) ServerPackets.playerAnimation, ClientHandle.PlayerAnimation},
            {(int) ServerPackets.ballLocation, ClientHandle.BallLocation},
            {(int) ServerPackets.spawnBall, ClientHandle.Spawnball},
            {(int) ServerPackets.playerTeam, ClientHandle.PlayerTeam},
            {(int) ServerPackets.score, ClientHandle.Score},
            {(int) ServerPackets.playersConnected, ClientHandle.PlayersConnected},
        };
        Debug.Log("Init Packets");
    }

    public void Dissconnect()
    {
        if(isConnected)
        {
            isConnected = false;
            tcp.socket.Close();
            //udp.socket.Close();
            GameManager.players.Clear();
            GameManager.instance.ball = null;
            ScoreManager.Clear();
            packetHandlers.Clear();
            tcp = new TCP();
            for(int i = 0; i < serverObjects.Count; i++)
            {
                Destroy(serverObjects[i]);
            }
            for(int i = 0; i < deactivateObjects.Length; i++)
            {
                deactivateObjects[i].SetActive(false);
            }
            for(int i = 0; i < deactivateButtons.Length; i++)
            {
                deactivateButtons[i].interactable = true;
            }
            Debug.Log("Player has disconnected from server.");
        }
    }
}
