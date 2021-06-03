using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class PlayerManager : MonoBehaviour
{
    public int id;
    public string username;
    public TMP_Text text;
    void Update()
    {
        text.text = username;
        if(transform.localScale.x == -1)
            text.transform.localScale = new Vector3(-1, 1, 0);
        else
            text.transform.localScale = new Vector3(1, 1, 0);
    }
}
