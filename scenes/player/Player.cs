using Godot;
using System;
using static Godot.GD;

public class Player : KinematicBody2D
{

    [Export]
    public int RunSpeed;
    [Export]
    public int JumpSpeed;
    [Export]
    public int Gravity;

    [Signal]
    delegate void LifeChanged();
    [Signal]
    delegate void Dead();


    public enum State
    { IDLE, RUN, JUMP, HURT, DEAD };

    public string NewAnim { get; set; }
    public string Anim { get; set; }
    public State CurrentState { get; set; }
    public Vector2 Velocity { get; set; }
    public bool KeyRight { get; set; }
    public bool KeyLeft { get; set; }
    public bool KeyJump { get; set; }
    public int Life { get; set; }

    public override void _Ready()
    {
        ChangeState(State.IDLE);
    }

    public void Start(Vector2 startPos)
    {
        Life = 3;
        EmitSignal("LifeChanged", Life);
        Position = startPos;
        Show();
        ChangeState(State.IDLE);
    }

    public override void _PhysicsProcess(float delta)
    {
        base._PhysicsProcess(delta);

        Velocity = new Vector2(Velocity.x, Velocity.y + Gravity * delta);
        GetInput();
        if (NewAnim != Anim)
        {
            Anim = NewAnim;
            //GD.Print(Anim);
            GetNode<AnimationPlayer>("AnimationPlayer").Play(Anim);
        }
        Velocity = MoveAndSlide(Velocity, new Vector2(0, -1));
        if (CurrentState == State.HURT)
        {
            return;
        }

        for (int i = 0; i < GetSlideCount(); i++)
        {
            var collision = GetSlideCollision(i);
            var colliderType = collision.GetCollider();
            
            if (colliderType.ToString() == "Godot.TileMap")
            {
                TileMap t = (TileMap) colliderType;
                if (t.Name == "Danger")
                {
                    Hurt();
                }
            }
        }

        if (CurrentState == State.JUMP && IsOnFloor())
        {
            ChangeState(State.IDLE);
        }
        if (CurrentState == State.JUMP && Velocity.y > 0)
        {
            NewAnim = "jump_down";
        }
    }

    public async void ChangeState(State newState)
    {
        CurrentState = newState;
        switch (CurrentState)
        {
            case State.IDLE:
                NewAnim = "idle";
                break;
            case State.RUN:
                NewAnim = "run";
                break;
            case State.HURT:
                NewAnim = "hurt";
                Velocity = new Vector2(-100 * Mathf.Sign(Velocity.x), -200);
                Life -= 1;
                EmitSignal("LifeChanged", Life);
                Timer invTimer = GetNode<Timer>("Invulnerability");
                invTimer.Start();
                await ToSignal(invTimer, "timeout");
                ChangeState(State.IDLE);
                if (Life <=0)
                {
                    ChangeState(State.DEAD);
                }
                break;
            case State.JUMP:
                NewAnim = "jump_up";
                break;
            case State.DEAD:
                EmitSignal("Dead");
                Hide();
                break;
            default:
                break;
        }
    }

    public void GetInput()
    {
        if (CurrentState == State.HURT)
        {
            return;
        }

        KeyRight = Input.IsActionPressed("ui_right");
        KeyLeft = Input.IsActionPressed("ui_left");
        KeyJump = Input.IsActionPressed("ui_jump");

        Velocity = new Vector2(0, Velocity.y);

        if (KeyRight)
        {
            float x = Velocity.x + RunSpeed;
            Velocity = new Vector2(x, Velocity.y);
            GetNode<Sprite>("Sprite").FlipH = false;
        }
        if (KeyLeft)
        {
            float x = Velocity.x - RunSpeed;
            Velocity = new Vector2(x, Velocity.y);
            GetNode<Sprite>("Sprite").FlipH = true;
        }
        if (KeyJump && IsOnFloor())
        {
            ChangeState(State.JUMP);
            Velocity = new Vector2(Velocity.x, JumpSpeed);
        }
        if (CurrentState == State.IDLE && Velocity.x != 0)
        {
            ChangeState(State.RUN);
        }
        if (CurrentState == State.RUN && Velocity.x == 0)
        {
            ChangeState(State.IDLE);
        }
        if ((CurrentState == State.RUN || CurrentState == State.IDLE) && !IsOnFloor())
        {
            ChangeState(State.JUMP);
        }
    }

    public void Hurt()
    {
        if (CurrentState != State.HURT)
        {
            ChangeState(State.HURT);
        }
    }

    private void _on_Invulnerability_timeout()
    {
        ChangeState(State.IDLE);
    }
}

