using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
#region Soccer Gamemode
    public static int[] soccerScores;

    public static void InitSoccer()
    {
        soccerScores = new int[2];
    }

    public static int UpdateSoccerScore(int _team)
    {
        soccerScores[_team - 1] = soccerScores[_team - 1] + 1;
        ServerSend.Score(-1, "", _team, soccerScores[_team - 1]);
        return soccerScores[_team - 1];
    }

    public static int GetsoccerScoresoccer(int _team)
    {
        return soccerScores[_team - 1];
    }
#endregion

#region Regualr Gamemode
    public static Dictionary<int, int> regularScores = new Dictionary<int, int>();

    public static void AddPlayer(int _playerId)
    {
        regularScores.Add(_playerId, 0);
    }
    public static void RemovePlayer(int _playerId)
    {
        regularScores.Remove(_playerId);
    }

    public static void UpdateRegualrScore(int _playerId)
    {
        if(regularScores.ContainsKey(_playerId))
        {
            regularScores[_playerId] = regularScores[_playerId] + 1;
            ServerSend.Score(_playerId, Server.clients[_playerId].player.username, 0, regularScores[_playerId]);
        }
        else
        {
            AddPlayer(_playerId);
            regularScores[_playerId] = regularScores[_playerId] + 1;
            ServerSend.Score(_playerId, Server.clients[_playerId].player.username, 0, regularScores[_playerId]);
        }
    }
    public static int ReturnRegularScores(int _playerId)
    {
        return regularScores[_playerId];
    }
#endregion
}
