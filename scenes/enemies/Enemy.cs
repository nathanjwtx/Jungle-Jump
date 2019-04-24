using Godot;
using System;

public class Enemy : KinematicBody2D
{
    [Export()] public int Speed;
    [Export()] public float Gravity;

    public Vector2 Velocity { get; private set; }
    private int Facing { get; set; }
    private float NewY { get; set; }

    public override void _Ready()
    {
        Facing = 1;
    }

    public override void _PhysicsProcess(float delta)
    {
        base._PhysicsProcess(delta);
        GetNode<Sprite>("Sprite").FlipH = Velocity.x > 0;
        NewY += Velocity.y + Gravity * delta;
        Velocity = new Vector2(Facing * Speed, NewY);

        Velocity = MoveAndSlide(Velocity, new Vector2(0, -1));

        // GD.Print($"enemy: {GetNode<CollisionShape2D>("CollisionShape2D").GlobalPosition.y - 28}");

        for (int i = 0; i < GetSlideCount(); i++)
        {
            var colliderType = GetSlideCollision(i).GetCollider();
//            GD.Print(colliderType.ToString());
            if (colliderType is Player)
            {
                if (colliderType != null)
                {
                    // ReSharper disable once PossibleInvalidCastException
                    Player p = (Player) colliderType;
                    p.Hurt();
                }
            }

            if (GetSlideCollision(i).Normal.x != 0)
            {
                Facing = Mathf.Sign((int) GetSlideCollision(i).Normal.x);
                Velocity = new Vector2(Velocity.x, -100);
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

