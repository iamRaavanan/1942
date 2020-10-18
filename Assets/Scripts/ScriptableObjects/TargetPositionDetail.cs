using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TargetPosition", menuName = "Target")]
public class TargetPositionDetail : ScriptableObject
{
    [Header("Target Positions")]
    public List<Vector2> _straightPos;
    public List<Vector2> _leftUpPos;
    public List<Vector2> _rightUpPos;
    public List<Vector2> _leftDownPos;
    public List<Vector2> _righttDownPos;
    public List<Vector2> _upPos;
    public Vector2 _bossPos;


    [Header("TopSpawn Movement Position")]
    public List<MovementPos> _topSpawnMovementPos;

    [Header("Top-right Spawn Movement Position")]
    public List<MovementPos> _toprightSpawnMovementPos;

    [Header("Top-left Spawn Movement Position")]
    public List<MovementPos> _topleftSpawnMovementPos;

    [Header("LeftSpawn Movement Position")]
    public List<MovementPos> _leftSpawnMovementPos;

    [Header("RightSpawn Movement Position")]
    public List<MovementPos> _rightSpawnMovementPos;

    [Header("BottomSpawn Movement Position")]
    public List<MovementPos> _bottomSpawnMovementPos;
}

[System.Serializable]
public class MovementPos
{
    public List<Vector2> _path;
}
