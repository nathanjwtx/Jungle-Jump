using Godot;
using System;
using GlobalGameState;

public class Main : Node
{
    public string LevelNumber { get; set; } 
    public string ScenePath { get; set; }

    private GameState _gameState;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        _gameState = (GameState) GetNode("/root/GameState");

        LevelNumber = _gameState.CurrentLevel.ToString();
        ScenePath = $"res://scenes/levels/Level{LevelNumber}.tscn";
        var map = (PackedScene) GD.Load(ScenePath);
        GetTree().GetRoot().AddChild(map.Instance());
    }

}
