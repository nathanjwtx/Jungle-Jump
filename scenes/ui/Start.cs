using Godot;
using System;
using GlobalGameState;

public class Start : Control
{
    private GameState _gameState;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        _gameState = (GameState)GetNode("/root/GameState");
    }

    public override void _Input(InputEvent @event)
    {
        base._Input(@event);
        if (@event.IsActionPressed("ui_select"))
        {
            GetTree().ChangeScene(_gameState.GameScene);
        }
    }
}
