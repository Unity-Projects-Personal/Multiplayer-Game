using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;

public class ServerSend
{
    private static void SendTCPData(int _toClient, Packet _packet)
    {
        _packet.WriteLength();
        Server.clients[_toClient].tcp.SendData(_packet);
    }
    
    private static void SendTCPDataToAll(Packet _packet)
    {
        _packet.WriteLength();
        for (int i = 1; i <= Server.maxPlayers; i++)
        {
            Server.clients[i].tcp.SendData(_packet);
        }
    }
    private static void SendTCPDataToAll(int _exceptClient, Packet _packet)
    {
        _packet.WriteLength();
        for (int i = 1; i <= Server.maxPlayers; i++)
        {
            if(i != _exceptClient)
                Server.clients[i].tcp.SendData(_packet);
        }
    }

    #region packets
    public static void Welcome(int _toClient, string _msg)
    {
        using (Packet _packet = new Packet((int)ServerPackets.welcome))
        {
            _packet.Write(_msg);
            _packet.Write(_toClient);

            SendTCPData(_toClient, _packet);
        }
    }

    public static void SpawnPlayer(int _toClent, Player _player)
    {
        using(Packet _packet = new Packet((int)ServerPackets.spawnPlayer))
        {
            _packet.Write(_player.id);
            _packet.Write(_player.username);
            _packet.Write(_player.transform.position);

            SendTCPData(_toClent, _packet);
        }
    }

    public static void PlayerPosition(Player _player)
    {
        using(Packet _packet = new Packet((int)ServerPackets.playerPostion))
        {
            _packet.Write(_player.id);
            _packet.Write(_player.username);
            _packet.Write(_player.transform.position);
            _packet.Write(_player.transform.localScale);
            _packet.Write(_player.time);
            SendTCPDataToAll(_packet);
        }
    }
    public static void PlayerDisconnected(int _playerId)
    {
        using(Packet _packet = new Packet((int)ServerPackets.playerDisconnected))
        {
            _packet.Write(_playerId);
            GamemodeSelect.RemovePlayer(_playerId);
            ScoreManager.RemovePlayer(_playerId);
            SendTCPDataToAll(_packet);
        }
    }
    public static void PlayerAnimation(Player _playerId, int _animId)
    {
        using(Packet _packet = new Packet((int) ServerPackets.playerAnimation))
        {
            _packet.Write(_playerId.id);
            _packet.Write(_animId);
            SendTCPDataToAll(_packet);
        }
    }
    public static void SendGameMode(int _gamemode)
    {
        using(Packet _packet = new Packet((int) ServerPackets.gamemodeSelect))
        {
            _packet.Write(_gamemode);
            SendTCPDataToAll(_packet);
        }
    }

    public static void Ball(Vector2 location)
    {
        using(Packet _packet = new Packet((int) ServerPackets.ballLocation))
        {
            _packet.Write(location);
            SendTCPDataToAll(_packet);
        }
    }
    public static void SpawnBall(Vector2 location)
    {
        using(Packet _packet = new Packet((int) ServerPackets.ballLocation))
        {
            _packet.Write(location);
            SendTCPDataToAll(_packet);
        }
    }
    public static void PlayerTeam(Player _team)
    {
        using(Packet _packet = new Packet((int) ServerPackets.playerTeam))
        {
            _packet.Write(_team.id);
            _packet.Write(_team);
            SendTCPDataToAll(_packet);
        }
    }
    public static void Score(int _id, string _name, int _team, int _score)
    {
        using(Packet _packet = new Packet((int) ServerPackets.score))
        {
            _packet.Write(_id);
            _packet.Write(_name);
            _packet.Write(_team);
            _packet.Write(_score);

            SendTCPDataToAll(_packet);
        }
    }

    public static void PlayersConnected(int _playersConnected)
    {
        using(Packet _packet = new Packet((int) ServerPackets.playersConnected))
        {
            _packet.Write(_playersConnected);
            SendTCPDataToAll(_packet);
        }
    }
    #endregion
}
