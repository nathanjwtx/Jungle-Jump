using Godot;
using System;

public class Enemy : KinematicBody2D
{
    [Export()] public int Speed;
    [Export()] public float Gravity;

    public Vector2 Velocity { get; private set; }
    private int Facing { get; set; }

    public override void _Ready()
    {
        
    }

    public override void _PhysicsProcess(float delta)
    {
        base._PhysicsProcess(delta);
        GetNode<Sprite>("Sprite").FlipH = Velocity.x > 0;
        float newY = Velocity.y + Gravity * delta;
        Velocity = new Vector2(Facing * Speed, newY);

        Velocity = MoveAndSlide(Velocity, new Vector2(0, -1));
        for (int i = 0; i < GetSlideCount(); i++)
        {
            var collision = GetSlideCollision(i);
            var colliderType = collision.GetCollider();
            TileMap t = (TileMap) colliderType;
            if (t.Name == "Player")
            {
                if (colliderType != null)
                {
                    // ReSharper disable once PossibleInvalidCastException
                    Player p = (Player) colliderType;
                    p.Hurt();
                }
            }

            if (collision.Normal.x != 0)
            {
                Facing = Mathf.Sign((int) collision.Normal.x);
                Velocity = new Vector2(Velocity.x, -100);
            }
        }

        if (Position.y > 1000)
        {
            QueueFree();
        }
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
