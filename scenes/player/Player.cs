using Godot;
using System;

public class Player : KinematicBody2D
{
    private string _state;
    private string _anim;
    private string _newAnim;

    public string NewAnim { get => _newAnim; set => _newAnim = value; }
    public string Anim { get => _anim; set => _anim = value; }
    public string State { get => _state; set => _state = value; }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        ChangeState("IDLE");
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }

    public void ChangeState(string state)
    {

    }
}
