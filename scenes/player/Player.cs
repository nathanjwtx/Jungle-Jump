using Godot;
using System;

public class Player : KinematicBody2D
{

    [Export]
    public int RunSpeed;
    [Export]
    public int JumpSpeed;
    [Export]
    public int Gravity;

    public enum State
    { IDLE, RUN, JUMP, HURT, DEAD };

    private State _currentState;
    private string _anim;
    private string _newAnim;
    private Vector2 _velocity;
    private bool _keyRight;
    private bool _keyLeft;
    private bool _keyJump;

    public string NewAnim { get => _newAnim; set => _newAnim = value; }
    public string Anim { get => _anim; set => _anim = value; }
    public State CurrentState { get => _currentState; set => _currentState = value; }
    public Vector2 Velocity { get => _velocity; set => _velocity = value; }
    public bool KeyRight { get => _keyRight; set => _keyRight = value; }
    public bool KeyLeft { get => _keyLeft; set => _keyLeft = value; }
    public bool KeyJump { get => _keyJump; set => _keyJump = value; }

    public override void _Ready()
    {
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
        if (CurrentState == State.JUMP && IsOnFloor())
        {
            ChangeState(State.IDLE);
        }
        if (CurrentState == State.JUMP && Velocity.y > 0)
        {
            NewAnim = "jump_down";
        }
    }

    public void ChangeState(State newState)
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
                break;
            case State.JUMP:
                NewAnim = "jump_up";
                break;
            case State.DEAD:
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
}
