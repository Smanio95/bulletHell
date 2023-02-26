using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum Layers
{
    Default = 0,
    Player = 6,
    Bullet = 7,
    EnemyBullet = 8,
    Enemy = 9,
    Untouchable = 12
}

public static class EnemyConstants
{
    public static Vector3[] UPWARD_MOVES = { Vector3.up, Vector3.left + Vector3.up, Vector3.left };
    public static Vector3[] DOWNWARD_MOVES = { Vector3.down, Vector3.left + Vector3.down, Vector3.left };
    public static Vector3[] RIGHT_SIDE_UPWARD_MOVES = { Vector3.right + Vector3.up, Vector3.right };
    public static Vector3[] RIGHT_SIDE_DOWNWARD_MOVES = { Vector3.right + Vector3.down, Vector3.right };
    public static Vector3 UPWARD_ROTATED = new Vector3(0, 0, -45);
    public static Vector3 DOWNWARD_ROTATED = new Vector3(0, 0, 45);
}

public static class AnimationBools
{
    public const string charge = "Charging";
    public const string appear = "appear";
    public const string disappear = "disappear";
}

public static class AnimationClips
{
    public const string appear = "Appear";
    public const string disappear = "Disappear";
}

public enum Levels
{
    Standard = 0,
    Tank = 1,
    Fast = 2,
    Boss = 3
}

public enum MenuIndex
{
    StartMenu = 0,
    Pause = 1,
    End = 2
}


