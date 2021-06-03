using System.Collections.Generic;
using UnityEngine;
public class GamemodeSelect : MonoBehaviour
{

    public static Dictionary<int, string> clientsData = new Dictionary<int, string>();
    public static Dictionary<int, bool> hasVoted = new Dictionary<int, bool>();
    public static Dictionary<int, bool> hasSpawned = new Dictionary<int, bool>();
    static Dictionary<Player, int> playerData = new Dictionary<Player, int>();

    public static bool gamemodeConfirmed;
    public static Dictionary<int, int> gamemodeVotes = new Dictionary<int, int>()
    {
        {1, 0},
        {2, 0},
    };
    public static void VoteGameMode(int _gamemode, int _player)
    {
        if (gamemodeConfirmed)
        {
            if (hasVoted[_player] == false)
                hasVoted[_player] = true;
            SelectGamemode();
        }
        if (gamemodeVotes.ContainsKey(_gamemode))
        {
            gamemodeVotes[_gamemode] = gamemodeVotes[_gamemode] + 1;
        }
        if (hasVoted.ContainsKey(_player))
        {
            if (hasVoted[_player] == false)
                hasVoted[_player] = true;
        }
        int votes = 0;
        foreach (KeyValuePair<int, int> client in gamemodeVotes)
        {
            votes = client.Value + votes;
        }
        if (Server.clients.Count >= votes)
            SelectGamemode();
    }

    static int gamemode = 0;
    public static void SelectGamemode()
    {
        int highest = 0;
        if (!gamemodeConfirmed)
        {
            foreach (KeyValuePair<int, int> client in gamemodeVotes)
            {
                if (client.Value > highest)
                {
                    highest = client.Value;
                    gamemode = client.Key;
                }
            }
            foreach (KeyValuePair<int, bool> client in hasVoted)
            {
                if (hasVoted[client.Key] == false)
                    return;
            }
            gamemodeConfirmed = true;
            if (highest == 0)
            {
                gamemode = 2;
            }
        }
        switch (gamemode)
        {
            case 1:
                int noTeam = 0;
                foreach (KeyValuePair<int, string> client in clientsData)
                {
                    if (hasVoted[client.Key] == false)
                        return;
                    if (hasVoted.ContainsKey(client.Key) && hasVoted[client.Key] == true)
                    {
                        if (hasSpawned.ContainsKey(client.Key) && hasSpawned[client.Key] == false)
                        {
                            Server.clients[client.Key].SendIntoGame(client.Value, noTeam);
                            ServerSend.SendGameMode(1);
                            hasSpawned[client.Key] = true;
                            ScoreManager.AddPlayer(Server.clients[client.Key].player.id);
                            noTeam++;
                        }
                    }
                }

                break;
        }
    }
    static int connectedPlayers = 0;
    public static void AddPlayer(int _id, string _name)
    {
        clientsData.Add(_id, _name);
        hasVoted.Add(_id, false);
        hasSpawned.Add(_id, false);
        ServerSend.PlayersConnected(connectedPlayers++);
    }

    public static void RemovePlayer(int _playerId)
    {
        ServerSend.PlayersConnected(connectedPlayers--);
        clientsData.Remove(_playerId);
        hasVoted.Remove(_playerId);
        hasSpawned.Remove(_playerId);
    }
}
