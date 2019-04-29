using Godot;
using System;

public class MovingPlatform : KinematicBody2D
{
    [Export()]
    public Vector2 Velocity;

    public override void _Ready()
    {
        
    }

    public override void _PhysicsProcess(float delta)
    {
        using (KinematicCollision2D collision = MoveAndCollide(Velocity * delta))
        {
            if (collision != null)
            {
                Velocity = new Vector2(Velocity.x *= -1, Velocity.y);
            }
        }
    }
//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
