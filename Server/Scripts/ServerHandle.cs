using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;

public class ServerHandle 
{
    public static void WelcomeRecieved(int _fromClient, Packet _packet)
    {
        int ClientIdCheck = _packet.ReadInt();
        string _username = _packet.ReadString();

        Console.WriteLine($"{Server.clients[_fromClient].tcp.socket.Client.RemoteEndPoint} connected and now player {_fromClient}"); 
        if(_fromClient != ClientIdCheck)
        {
            Console.WriteLine($"Player \"{_username}\" (ID: {_fromClient}) has assumed the wrong client");
        }
        GamemodeSelect.AddPlayer(_fromClient, _username);
        Console.WriteLine(_username);
    }

    public static void PlayerMovement(int _fromClient, Packet _packet)
    {
        if(!Server.clients.ContainsKey(_fromClient))
            return;
        bool[] _inputs = new bool[_packet.ReadInt()];

        for (int i = 0; i < _inputs.Length; i++)
        {
            _inputs[i] = _packet.ReadBool();
        }
        float _time = _packet.ReadFloat();
        Server.clients[_fromClient].player.SetInput(_inputs, _time);
    }
    public static void GamemodeSelectPlayer(int _fromClient, Packet _packet)
    {
        int _gamemode = _packet.ReadInt();
        GamemodeSelect.VoteGameMode(_gamemode, _fromClient);
        Console.WriteLine($"Player has voted for gamemode {_gamemode}");
    }
}
