using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;
using System.Net;
using System.Net.Sockets;

public class Server
{
    public static int maxPlayers { get; private set; }
    public static int Port { get; private set; }
    public static Dictionary<int, Client> clients = new Dictionary<int, Client>();
    public delegate void PacketHandler(int _fromClient, Packet _packet);
    public static Dictionary<int, PacketHandler> PacketHandlers;
    private static TcpListener tcpListener;

    //private static UdpClient udpListener;

    public static void Start(int _maxPlayers, int _Port)
    {
        Port = _Port;
        Int32 port = 25565;
        maxPlayers = _maxPlayers;
        Console.WriteLine($"Starting... on {IPAddress.Any.ToString()}");
        InitServerData();
        //tcpListener = new TcpListener(ip, Port);
        tcpListener = new TcpListener(IPAddress.Any, Port);

        tcpListener.Start();
        tcpListener.BeginAcceptTcpClient(new AsyncCallback(TCPConnectCallback), null);

        //udpListener = new UdpClient(Port);
        //udpListener.BeginReceive(UDPRecieveCallback, null);

        Console.WriteLine($"Server Started, port: {Port}.");
    }
    private static void TCPConnectCallback(IAsyncResult _result)
    {
        TcpClient _client = tcpListener.EndAcceptTcpClient(_result);
        tcpListener.BeginAcceptTcpClient(new AsyncCallback(TCPConnectCallback), null);
        Console.WriteLine($"Connnection From {_client.Client.RemoteEndPoint}");


        for (int i = 1; i <= maxPlayers; i++)
        {
            if (clients[i].tcp.socket == null)
            {
                clients[i].tcp.Connect(_client);
                return;
            }
        }

        Console.WriteLine($"{_client.Client.RemoteEndPoint} failed to connect: server full!!");
    }

    // private static void UDPRecieveCallback(IAsyncResult _result)
    // {
    //     try
    //     {
    //         IPEndPoint _clientEndPoint = new IPEndPoint(IPAddress.Any, 0);
    //         byte[] _data = udpListener.EndReceive(_result,ref _clientEndPoint);
    //         udpListener.BeginReceive(UDPRecieveCallback, null);

    //         if(_data.Length <4)
    //         {
    //             return;
    //         }
    //         using(Packet _packet = new Packet(_data))
    //         {
    //             int _clientId = _packet.ReadInt();

    //             if(_clientId == 0)
    //                 return;
    //             if(clients[_clientId].udp.endPoint == null)
    //             {
    //                 clients[_clientId].udp.Connect(_clientEndPoint);
    //                 return;
    //             }
    //             if(clients[_clientId].udp.endPoint.ToString() == _clientEndPoint.ToString())
    //                 clients[_clientId].udp.HandleData(_packet);
    //         }
    //     }
    //     catch(Exception e)
    //     {
    //         Console.WriteLine($"Error UPD DATA: {e}");
    //     }
    // }

    // public static void SendUDPData(IPEndPoint _clientEndPoint, Packet _packet)
    // {
    //     try
    //     {
    //         if(_clientEndPoint != null)
    //         {
    //             udpListener.BeginSend(_packet.ToArray(), _packet.Length(), _clientEndPoint, null, null);
    //         }
    //     }
    //     catch(Exception e)
    //     {
    //         Console.WriteLine($"Error Sendind UDP Data to {_clientEndPoint}: {e}");
    //     }
    // }

    private static void InitServerData()
    {
        for (int i = 1; i <= maxPlayers; i++)
        {
            clients.Add(i, new Client(i));
        }
        PacketHandlers = new Dictionary<int, PacketHandler>()
        {
            {(int) ClientPackets.welcomeReceived, ServerHandle.WelcomeRecieved},
            {(int) ClientPackets.playerMovement, ServerHandle.PlayerMovement},
            {(int) ClientPackets.gamemodeSelect, ServerHandle.GamemodeSelectPlayer}
        };
    }

    public static void Stop()
    {
        tcpListener.Stop();
        //udpListener.Close();
    }
}
