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

    [Export] public int Accenleration;

    [Signal]
    delegate void LifeChanged();
    [Signal]
    delegate void Dead();


    public enum State
    { IDLE, RUN, JUMP, HURT, DEAD, CROUCH, CLIMB };

    public string NewAnim { get; set; }
    public string Anim { get; set; }
    public State CurrentState { get; set; }
    private Vector2 _velocity = new Vector2();
    public bool KeyRight { get; set; }
    public bool KeyLeft { get; set; }
    public bool KeyJump { get; set; }
    public bool KeyCrouch { get; set; }
    public int Life { get; set; }

    private bool OnPlatform { get; set; }
    private bool Friction { get; set; }

    private AnimationPlayer _animationPlayer;
    private Sprite _sprite;

    public override void _Ready()
    {
        _animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
        _sprite = GetNode<Sprite>("Sprite");
//        Print("loaded");
//        ChangeState(State.IDLE);
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
        Vector2 snap;

        _velocity.y += Gravity * delta;

        GetInput();

        if (CurrentState == State.JUMP && IsOnFloor())
        {
            snap = new Vector2(0, 0);
        }
        else
        {
            snap = new Vector2(0, 40);
        }

        if (_velocity.y != 0 && CurrentState == State.JUMP)
        {
            ChangeState(State.JUMP);
        }

        _velocity = MoveAndSlideWithSnap(_velocity, snap, Vector2.Up);

        if (CurrentState == State.JUMP && IsOnFloor())
        {
            ChangeState(State.IDLE);
        }

    }

    public void GetInput()
    {
        bool keyJump = Input.IsActionJustPressed("ui_jump");
        bool keyLeft = Input.IsActionPressed("ui_left");
        bool keyRight = Input.IsActionPressed("ui_right");

        if (!keyLeft && !keyRight && IsOnFloor())
        {
            ChangeState(State.IDLE);
            Friction = true;
        }
        else if (keyLeft)
        {
            ChangeState(State.RUN);
            _sprite.FlipH = true;
            _velocity.x = Math.Max(_velocity.x - Accenleration, -RunSpeed);
        }
        else if (keyRight)
        {
            ChangeState(State.RUN);
            _sprite.FlipH = false;
            _velocity.x = Math.Min(_velocity.x + Accenleration, RunSpeed);
        }
        
        if (IsOnFloor())
        {
            if (keyJump)
            {
                ChangeState(State.JUMP);
                _velocity.y = JumpSpeed;    
            }

            if (Friction)
            {
                _velocity.x = Mathf.Lerp(_velocity.x, 0, 0.2f);
            }
        }
        else
        {
            ChangeState(State.JUMP);
            if (Friction)
            {
                _velocity.x = Mathf.Lerp(_velocity.x, 0, 0.05f);
            }
        }


    }

    public void ChangeState(State state)
    {
        CurrentState = state;
        switch (state)
        {
            case State.IDLE:
                NewAnim = "idle";
                break;
            case State.JUMP:
                if (_velocity.y < 0)
                {
                    NewAnim = "jump_up";    
                }
                else if (_velocity.y > 0)
                {
                    NewAnim = "jump_down";
                }
                break;
            case State.RUN:
                NewAnim = "run";
                break;
            
        }
        _animationPlayer.Play(NewAnim);
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

