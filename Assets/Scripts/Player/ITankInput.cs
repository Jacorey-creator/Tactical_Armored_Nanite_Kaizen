using UnityEngine;

public interface ITankInput
{
    Vector2 MoveInput { get; }
    Vector2 LookVector { get; }
    bool FirePressed { get; }
}