using Godot;
using System;

public class MovingPlatform_v2 : MovingPlatform
{

    private PathFollow2D _pathFollow;
    private Path2D _path2D;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        SetPath();
    }

    public override void _PhysicsProcess(float delta)
    {
        base._PhysicsProcess(delta);
        _pathFollow.SetOffset(_pathFollow.GetOffset() + Speed * delta);
    }

    public void SetPath()
    {
        _pathFollow = (PathFollow2D) GetParent();
        _path2D = (Path2D) _pathFollow.GetParent();
    }
}
