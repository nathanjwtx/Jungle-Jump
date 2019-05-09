using Godot;
using System;

public class Enemy : KinematicBody2D
{
    [Export] public int Speed;
    [Export] public float Gravity;

    public Vector2 Velocity { get; private set; }
    private int Facing { get; set; }
    private float NewY { get; set; }

    private RayCast2D _rayCast2DLeft;
    private RayCast2D _rayCast2DRight;

    public override void _Ready()
    {
        Facing = -1;
        _rayCast2DLeft = GetNode<RayCast2D>("RayCast_Left");
        _rayCast2DRight = GetNode<RayCast2D>("RayCast_Right");
    }

    public override void _PhysicsProcess(float delta)
    {
        base._PhysicsProcess(delta);
        GetNode<Sprite>("Sprite").FlipH = Facing == 1;
//        GetNode<Sprite>("Sprite").FlipH = Velocity.x > 0;
        NewY += Velocity.y + Gravity * delta;
        Velocity = new Vector2(Facing * Speed, NewY);

        Velocity = MoveAndSlide(Velocity, new Vector2(0, -1));
        
        
//        GD.Print(_rayCast2DLeft.IsColliding());
//        if (_rayCast2DLeft.IsColliding() && Facing == -1)
//        {
//            Facing = 1;
//            _rayCast2DLeft.Enabled = false;
//            _rayCast2DRight.Enabled = true;
//        }
//        else if (_rayCast2DRight.IsColliding() && Facing == 1)
//        {
//            Facing = -1;
//            _rayCast2DLeft.Enabled = true;
//            _rayCast2DRight.Enabled = false;
//        }
        
        for (int i = 0; i < GetSlideCount(); i++)
        {
            var colliderType = GetSlideCollision(i).GetCollider();

            if (colliderType is Player p)
            {
                p.Hurt();
            }

            if (colliderType is TileMap t)
            {
                if (t.Name == "Blocks")
                {
                    GD.Print(t.Name);
                    Facing *= -1;
                }
            }
        }
        
        

        if (Position.y > 1000)
        {
            QueueFree();
        }
    }

    public void TakeDamage()
    {
        GetNode<AnimationPlayer>("AnimationPlayer").Play("death");
        GetNode<CollisionShape2D>("CollisionShape2D").Disabled = true;
        SetPhysicsProcess(false);
    }

    private void _on_AnimationPlayer_animation_finished(string animName)
    {
        if (animName == "death")
        {
            QueueFree();
        }
    }
}

