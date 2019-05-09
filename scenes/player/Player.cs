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
    [Export] public int Acceleration;
    [Export] public float Invincibility;

    [Signal]
    delegate void LifeChanged();
    [Signal]
    delegate void Dead();
    [Signal]
    delegate void ScoreChanged();


    public enum State
    { IDLE, RUN, JUMP, HURT, DEAD, CROUCH, CLIMB };

    public string NewAnim { get; set; }
    public string Anim { get; set; }
    public State CurrentState { get; set; }
    private Vector2 _velocity;
    public bool KeyRight { get; set; }
    public bool KeyLeft { get; set; }
    public bool KeyJump { get; set; }
    public bool KeyCrouch { get; set; }
    public int Life { get; set; }
    
    private bool Friction { get; set; }

    private AnimationPlayer _animationPlayer;
    private Sprite _sprite;
    private Timer _invincibleTimer;

    public override void _Ready()
    {
        _animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
        _sprite = GetNode<Sprite>("Sprite");
        _invincibleTimer = GetNode<Timer>("Invulnerability");
        _invincibleTimer.WaitTime = Invincibility;
    }

    public void Start(Vector2 startPos)
    {
        Life = 3;
        EmitSignal("LifeChanged", Life);
        EmitSignal("ScoreChanged", 0);
        Position = startPos;
        Show();
        ChangeState(State.IDLE);
    }

    public override void _PhysicsProcess(float delta)
    {
        base._PhysicsProcess(delta);

        if (CurrentState == State.HURT)
        {
            return;
        }
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
        
        GetCollisions();

        if (CurrentState == State.JUMP && IsOnFloor())
        {
            ChangeState(State.IDLE);
        }

    }

    private void GetCollisions()
    {
        if (CurrentState == State.HURT)
        {
            return;
        }
        for (int i = 0; i < GetSlideCount(); i++)
        {
            var collision = GetSlideCollision(i);

            if (collision.Collider is TileMap)
            {
                TileMap t = (TileMap) collision.Collider;
                if (t.Name == "Danger")
                {
                    Hurt();
                }
            }

            if (collision.Collider is Enemy)
            {
                Enemy enemy = (Enemy) collision.Collider;
                RectangleShape2D shape = (RectangleShape2D) enemy.GetNode<CollisionShape2D>("CollisionShape2D").Shape;
                var playerFeet = (Position + shape.Extents).y;
                if (playerFeet < enemy.Position.y)
                {
                    enemy.TakeDamage();
                    _velocity.y = -200;
                }
                else
                {
                    Hurt();
                }
            }
        }
    }

    public void GetInput()
    {
        KeyJump = Input.IsActionJustPressed("ui_jump");
        KeyLeft = Input.IsActionPressed("ui_left");
        KeyRight = Input.IsActionPressed("ui_right");
        KeyCrouch = Input.IsActionPressed("ui_crouch");

        if (!KeyLeft && !KeyRight && !KeyCrouch && IsOnFloor())
        {
            ChangeState(State.IDLE);
            Friction = true;
        }
        else if (KeyLeft)
        {
            _sprite.FlipH = true;
            if (!KeyCrouch)
            {
                ChangeState(State.RUN);
                _velocity.x = Math.Max(_velocity.x - Acceleration, -RunSpeed);    
            }
            else if (KeyCrouch)
            {
                _velocity.x = Math.Max(_velocity.x - Acceleration, -RunSpeed / 2f);
            }
            
        }
        else if (KeyRight)
        {
            _sprite.FlipH = false;
            if (!KeyCrouch)
            {
                ChangeState(State.RUN);
                _velocity.x = Math.Min(_velocity.x + Acceleration, RunSpeed);                
            }
            else if (KeyCrouch)
            {
                _velocity.x = Math.Min(_velocity.x + Acceleration, RunSpeed / 2f);
            }

        }

        if (KeyCrouch)
        {
            ChangeState(State.CROUCH);
        }
        if (IsOnFloor())
        {
            if (KeyJump)
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
            case State.HURT:
                NewAnim = "hurt";
                _velocity.y = -200;
                _velocity.x = -100 * Mathf.Sign(_velocity.x);
                Life -= 1;
                EmitSignal("LifeChanged", Life);
                _invincibleTimer.Start();
                if (Life <= 0)
                {
                    ChangeState(State.DEAD);
                }
                break;
            case State.CROUCH:
                NewAnim = "crouch";
                break;
            case State.DEAD:
                EmitSignal("Dead");
                Hide();
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

