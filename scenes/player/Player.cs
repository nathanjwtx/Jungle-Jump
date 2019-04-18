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

    public string NewAnim { get => _newAnim; set => _newAnim = value; }
    public string Anim { get => _anim; set => _anim = value; }
    public State CurrentState { get => _currentState; set => _currentState = value; }

    public override void _Ready()
    {
        ChangeState(State.IDLE);
    }

    public override void _PhysicsProcess(float delta)
    {
        base._PhysicsProcess(delta);
        if (NewAnim != Anim)
        {
            Anim = NewAnim;
            GetNode<AnimationPlayer>("AnimationPlayer").Play(Anim);
        }
    }

    public void ChangeState(State newState)
    {
        CurrentState = newState;
        switch (CurrentState)
        {
            case State.IDLE:
                Anim = "idle";
                break;
            case State.RUN:
                Anim = "run";
                break;
            case State.HURT:
                Anim = "hurt";
                break;
            case State.JUMP:
                Anim = "jump_up";
                break;
            case State.DEAD:
                Hide();
                break;
            default:
                break;
        }
    }
}
