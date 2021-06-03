using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;

public class ClientHandle : MonoBehaviour
{
    public static void Welcome(Packet _packet)
    {
        string _msg = _packet.ReadString();
        int _myId = _packet.ReadInt();

        Debug.Log($"Message From Server: {_msg}");
        Client.instance.myId = _myId;

        ClientSend.WelcomeRecieved();

        //Client.instance.udp.Connect(((IPEndPoint)Client.instance.tcp.socket.Client.LocalEndPoint).Port);
        //Client.instance.tcp.Connect(((IPEndPoint)Client.instance.tcp.socket.Client.LocalEndPoint).Port);
    }
    public static void SpawnPlayer(Packet _packet)
    {
        int _id = _packet.ReadInt();
        string _username = _packet.ReadString();
        Vector2 _position = _packet.ReadVector2();
        GameManager.instance.SpawnPlayer(_id, _username, _position); 
    }

    public static void PlayerPostion(Packet _packet)
    {
        int _id = _packet.ReadInt();
        string _name = _packet.ReadString();
        Vector2 _position = _packet.ReadVector2();
        Vector2 _scale = _packet.ReadVector2();
        float _time = _packet.ReadFloat();

        if(!GameManager.players.ContainsKey(_id))
        {
            GameManager.instance.SpawnPlayer(_id, _name, _position);
        }
        //Debug.Log("Ping: " + (int)((Time.time - _time) * 1000));

        GameManager.players[_id].transform.position = _position;
        GameManager.players[_id].transform.localScale = new Vector3(_scale.x, 1, 0);
    }
    
    public static void PlayerDisconnected(Packet _packet)
    {
        int _id = _packet.ReadInt();

        Destroy(GameManager.players[_id].gameObject);
        GameManager.players.Remove(_id);
    }

    public static void PlayerAnimation(Packet _packet)
    {
        int _id = _packet.ReadInt();
        int _animId = _packet.ReadInt();
        print($"Animation recieved {_animId}");
        GameManager.players[_id].gameObject.GetComponent<PlayerAnimation>().SetAnimation(_animId);
    }
    static bool isSpawnedBall = false;
    public static void GamemodeSelect(Packet _packet)
    {
        int _gamemode = _packet.ReadInt();
        Vector2 spawn = new Vector2(0, 3);
        if(_gamemode == 2)
        {
            isSpawnedBall = true;
            GameManager.instance.SpawnBall(spawn);
        }
        GameManager.instance.gamemodeSelect.SetActive(false);
        ScoreManager.gamemode = _gamemode;
        Debug.Log("Spawning Ball");
    }
    public static void BallLocation(Packet _packet)
    {
        Vector2 _ballLocation = _packet.ReadVector2();
        if(!isSpawnedBall)
        {
            isSpawnedBall = true;
            GameManager.instance.SpawnBall(_ballLocation);
        }
        GameManager.instance.BallLocation(_ballLocation);
    }
    public static void Spawnball(Packet _packet)
    {
        Vector2 _spawnLocation = _packet.ReadVector2();
        GameManager.instance.SpawnBall(_spawnLocation);
    }
    public static void PlayerTeam(Packet _packet)
    {
        int _id = _packet.ReadInt();
        int _team = _packet.ReadInt();
        switch(_team)
        {
            case 1:
                GameManager.players[_id].GetComponent<SpriteRenderer>().color = Constatnts.enemyColor;
                break;
            case 2:
                GameManager.players[_id].GetComponent<SpriteRenderer>().color = Constatnts.friendlyColor;
                break;
        }
    }
    public static void Score(Packet _packet)
    {
        int _id = _packet.ReadInt();
        string _name = _packet.ReadString();
        int _team = _packet.ReadInt();
        int _score = _packet.ReadInt();
        if(_id != -1)
            ScoreManager.UpdateScore(_score, _id, _name);
        else
        {
            ScoreManager.UpdateSoccerScore(_score, _team);
        }
    }
    public static void PlayersConnected(Packet _packet)
    {
        int _playerConnected = _packet.ReadInt();
        GameManager.instance.ConnectedPlayers(_playerConnected);
    }
    // Dont really want extra processing time so yea
}
