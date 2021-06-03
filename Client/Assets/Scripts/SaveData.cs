using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class SaveData : MonoBehaviour
{
    ServerData serverData;

    public static SaveData instance;
    public Client client;
    
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

    void Start()
    {
        Debug.Log(Application.persistentDataPath + "/ServerData.json");
        try{
            string jsonString = System.IO.File.ReadAllText(Application.persistentDataPath + "/ServerData.json");
            serverData = JsonUtility.FromJson<ServerData>(jsonString);
            ipAddress.text = serverData.ip;
            port.text = serverData.port.ToString();
            username.text = serverData.playersName;
        } catch(Exception e)
        {
            Debug.Log($"No such files exist {e}");
        }
    }

    public void SaveServerData(string ip, int port, string _username)
    {
        ServerData _serverData = new ServerData(ip, port, _username);
        Debug.Log(JsonUtility.ToJson(_serverData));
        System.IO.File.WriteAllText(Application.persistentDataPath + "/ServerData.json", JsonUtility.ToJson(_serverData));
    }

    // UI MANAGER
    public GameObject startMenu;
    public InputField username;
    public InputField port;
    public InputField ipAddress;

    public void ConnectToServer()
    {
        startMenu.SetActive(false);
        client.ip = ipAddress.text;
        client.port = int.Parse(port.text);
        SaveServerData(ipAddress.text, int.Parse(port.text), username.text);
        Client.instance.ConnectToServer();
    }
}

class ServerData
{
    public string ip;
    public int port;
    public string playersName;
    public ServerData(string _ip, int _port, string _playersName)
    {
        ip = _ip;
        port = _port;
        playersName = _playersName;
    }
}
