using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class scores : MonoBehaviour
{
    public Text[] SoccerText;
    public Text[] regularScores;
    public Text playerScoresTextPrefab;
    public Dictionary<int, Text> playerScoresText = new Dictionary<int, Text>();
    public GameObject playersScoreParent;
    int index = 0;
    public Text SpawnTextElement(int _id)
    {
        Text text = regularScores[index];
        regularScores[index].gameObject.SetActive(true);
        playerScoresText.Add(_id, text);
        index += 1;
        return text;
    }
    public void removeTextElemet(int _id)
    {
        regularScores[index].gameObject.SetActive(true);
        playerScoresText.Remove(_id);
        index -= 1;
    }
}
