using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    public GameObject startMenu;
    public InputField serverName;
    public InputField port;
    public InputField ipAddress;
    public Client client;
    public SaveData saveData;
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

    public void ConnectToServer()
    {
        startMenu.SetActive(false);
        client.ip = ipAddress.text;
        client.port = int.Parse(port.text);
        SaveData.instance.SaveServerData(ipAddress.text, int.Parse(port.text), serverName.text);
        Client.instance.ConnectToServer();
    }
}
