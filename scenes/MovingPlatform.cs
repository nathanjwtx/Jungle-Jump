using Godot;
using System;
using static Godot.GD;

public class MovingPlatform : KinematicBody2D
{
    [Export()] public int Speed;

    private PathFollow2D _pathFollow;

    public override void _Ready()
    {
        SetPath();
        Print(GetParent().Name);
    }

    public override void _PhysicsProcess(float delta)
    {
        base._PhysicsProcess(delta);
//        int speed = 10;
        
        _pathFollow.SetOffset(_pathFollow.GetOffset() + Speed * delta);
        // using (KinematicCollision2D collision = MoveAndCollide(Velocity * delta))
        // {
        //     if (collision != null)
        //     {
        //         Velocity = new Vector2(Velocity.x *= -1, Velocity.y);
        //     }
        // }

    }
//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
    public void SetPath()
    {
        _pathFollow = (PathFollow2D) GetParent();
    }
}
