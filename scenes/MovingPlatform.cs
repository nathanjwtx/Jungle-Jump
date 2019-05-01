using Godot;
using System;
using static Godot.GD;

public class MovingPlatform : KinematicBody2D
{
    [Export()] public int Speed;

    public override void _Ready()
    {

    }
}
