using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;

public class NetworkManager : MonoBehaviour
{
    public static NetworkManager instance;

    public GameObject playerPrefab;
    public GameObject ball;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Debug.Log("Instance Exists YEEET!");
            Destroy(this);
        }
    }

    Server server;

    public void Start()
    {
        Server.Start(Constants.maxPlayers, 25565);
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 120;
    }

    private void OnApplicationQuit()
    {
        Server.Stop();
    }
    int spawn = 0;
    public Player InstantiatePlayer(int _team)
    {
        if (spawn > 2)
            spawn = 0;
        spawn++;
        Player player = Instantiate(playerPrefab, Constants.spawnPoints[spawn], Quaternion.identity).GetComponent<Player>();
        player.SetTeam(_team);
        return player;
    }
}
