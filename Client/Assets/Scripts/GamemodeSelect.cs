using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamemodeSelect : MonoBehaviour
{
    public static Dictionary <int, string> gamemodes = new Dictionary<int, string>()
    {
        {1, "Regualr"},
        {2, "Soccer"},
    };

    public void Vote(int _gamemode)
    {
        ClientSend.GamemodeSelect(_gamemode);
    }
}
