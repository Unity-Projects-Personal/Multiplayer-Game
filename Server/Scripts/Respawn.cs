using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Respawn : MonoBehaviour
{
    PlayerController player;
    void Start()
    {
        player = GetComponent<PlayerController>();
    }
    void FixedUpdate()
    {
        if(transform.position.x >= Constants.deathBounds.x)
        {
            RespawnPlayer();
        }
        else if(transform.position.x <= Constants.deathBounds.y)
        {
            RespawnPlayer();
        }

        if(transform.position.y <= Constants.deathBounds.w)
        {
            RespawnPlayer();
        }
        else if(transform.position.y >= Constants.deathBounds.z)
            RespawnPlayer();
    }

    void RespawnPlayer()
    {
        if(player.GetHitBy() != -1)
            ScoreManager.UpdateRegualrScore(player.GetHitBy());
        int Rand = Random.Range(0, 3);
        transform.position = Constants.spawnPoints[Rand];
    }
}