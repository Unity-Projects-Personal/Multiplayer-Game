using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ScoreManager : MonoBehaviour
{
    static Dictionary<int, string> playerData = new Dictionary<int, string>();
    static Dictionary<int, int> playerScore = new Dictionary<int, int>();

    static Dictionary<int, int> teamScore = new Dictionary<int, int>();
    static scores score;
    public static int gamemode;
    void Start()
    {
        score = GetComponent<scores>();
    }

    public static void UpdateScore(int _score, int _id, string _name)
    {
        if(gamemode == 2)
            return;
        if(playerScore.ContainsKey(_id))
        {
            playerScore[_id] = _score;
            score.playerScoresText[_id].text = $"{_name}\n{_score}";
        }
        else
        {
            playerScore.Add(_id, _score);
            score.SpawnTextElement(_id);
            score.playerScoresText[_id].text = $"{_name}\n{_score}";
            playerData.Add(_id, _name);
            playerScore[_id] = _score;
        }
    }

    public static void UpdateSoccerScore(int _score, int _team)
    {
        if(gamemode == 1)
            return;
        for(int i = 0; i< score.SoccerText.Length; i++)
            score.SoccerText[i].gameObject.SetActive(true);
        if(teamScore.ContainsKey(_team))
        {
            teamScore[_team] = _score;
            score.SoccerText[_team - 1].text = $"TEAM {_team}\n{_score}";
        }
        else
        {
            teamScore.Add(_team, _score);
            score.SoccerText[_team - 1].text = $"TEAM {_team}\n{_score}";   
            teamScore[_team] = _score;
        }
    }
    public static void Clear()
    {
        teamScore.Clear();
        playerScore.Clear();
        playerData.Clear();
        score.playerScoresText.Clear();
    }
}
