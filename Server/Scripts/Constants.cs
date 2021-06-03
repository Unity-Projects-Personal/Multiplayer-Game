using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Constants
{
    public static Vector2[] spawnPoints = { new Vector2(-11, 2), new Vector2(9, 2), new Vector2(-5, 2), new Vector2(2, 2) };

    public static Vector2 ballSpawn = new Vector2(0, 3);
    public const int TICKS_PER_SEC = 90;
    public const int maxPlayers = 4;
    public const int MS_PER_TICK = 1000 / TICKS_PER_SEC;
    public static Vector4 deathBounds = new Vector4(15, -19, 15.5f, -5);
}
