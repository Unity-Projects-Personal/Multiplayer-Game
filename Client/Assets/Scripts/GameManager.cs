using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public static Dictionary<int, PlayerManager> players = new Dictionary<int, PlayerManager>();

    public GameObject localPlayerPrefab;
    public GameObject playerPrefab;
    public GameObject ballPrefab;
    public GameObject gamemodeSelect;
    public GameObject ball;
    private void Awake()
    {
        for (int i = 0; i < connectedPlayers.Length; i++)
        {
            connectedPlayers[i].SetActive(false);
        }
        Application.targetFrameRate = 60;
        if(instance == null)
        {
            instance = this;
        }else if(instance != this)
        {
            Debug.Log("Instance Exists YEEET!");
            Destroy(this);
        }
    }

    public void SpawnPlayer(int _id, string _username, Vector2 _position)
    {
        GameObject _player;
        if(_id == Client.instance.myId)
        {
            GameManager.instance.gamemodeSelect.SetActive(false);
            _player = Instantiate(localPlayerPrefab, _position, Quaternion.identity);
        }
        else
        {
            _player = Instantiate(playerPrefab, _position, Quaternion.identity);
        }

        _player.GetComponent<PlayerManager>().id = _id;
        _player.GetComponent<PlayerManager>().username = _username;
        players.Add(_id, _player.GetComponent<PlayerManager>());
        Client.instance.serverObjects.Add(_player);
    }
    public void SpawnBall(Vector2 _location)
    {
        if(ball != null)
            return;
        ball = Instantiate(ballPrefab, _location, Quaternion.identity);
        Client.instance.serverObjects.Add(ball);
    }
    public void BallLocation(Vector2 _location)
    {
        if(ball != null)
            ball.transform.position = _location;
        else{
            SpawnBall(_location);
        }
    }
    public GameObject[] connectedPlayers;
    public void ConnectedPlayers(int _players)
    {
        for(int i = 0; i <= _players; i++)
        {
            connectedPlayers[i].SetActive(true);
        }
    }
}
