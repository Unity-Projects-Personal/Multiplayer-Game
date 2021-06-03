using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClientSend : MonoBehaviour
{
    private static void SendTCPData(Packet _packet)
    {
        _packet.WriteLength();
        Client.instance.tcp.SendData(_packet);

    }
    #region Packets
    public static void WelcomeRecieved()
    {
        using(Packet _packet = new Packet((int)ClientPackets.welcomeReceived))
        {
            _packet.Write(Client.instance.myId);
            _packet.Write(SaveData.instance.username.text);

            SendTCPData(_packet);
        }
    }
    public static void PlayerMovement(bool[] _inputs)
    {
        using(Packet _packet = new Packet((int)ClientPackets.playerMovement))
        {
            _packet.Write(_inputs.Length);
            foreach (bool _input in _inputs)
            {
                _packet.Write(_input);
            }
            _packet.Write(Time.time);
            SendTCPData(_packet);
        }
    }
    public static void GamemodeSelect(int _gamemode)
    {
        using(Packet _packet = new Packet((int)ClientPackets.gamemodeSelect))
        {
            _packet.Write(_gamemode);
            SendTCPData(_packet);
        }
    }
    #endregion
}
