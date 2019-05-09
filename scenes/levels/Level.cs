using Godot;
using System;
using GlobalGameState;

public class Level : Node2D
{
    [Signal]
    delegate void ScoreChanged();
    [Signal]
    delegate void LifeChanged();

    private TileMap Pickups { get; set;  }
    private TileMap Blocks { get; set; }
    private Player Player { get; set; }
    private HUD Hud { get; set; }
    private PackedScene Collectible { get; set; }
    public int Score { get; set; }

    public float DefaultDayLengthRatio { get; } = 1.0f;

    private GameState _gameState;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        _gameState = (GameState)GetNode("/root/GameState");
        Collectible = (PackedScene) ResourceLoader.Load("res://scenes/Collectible.tscn");
        Player = GetNode<Player>("Player");
        Pickups = GetNode<TileMap>("Pickups");
        Blocks = GetNode<TileMap>("Blocks");
        Hud = GetNode<HUD>("CanvasLayer/HUD");
        Pickups.Hide();
        Blocks.Hide();
        Score = 0;

        Position2D startPos = GetNode<Position2D>("PlayerSpawn");
        
        /* signal ScoreChanged is emitted from 2 places so needs to be connected in both places. Need
         the Player.Connect(...) when connecting from outside Player*/
        Connect("ScoreChanged", Hud, "_on_ScoreChanged");
        Player.Connect("ScoreChanged", Hud, "_on_ScoreChanged");
        Connect("LifeChanged", Hud, "_on_Player_LifeChanged");
        Player.Connect("LifeChanged", Hud, "_on_Player_LifeChanged");
        
        GetNode<Player>("Player").Start(new Vector2(startPos.Position.x, startPos.Position.y));

        SetCameraLimits();
        SpawnPickups();
    }

    private void SetCameraLimits()
    {
        
        TileMap world = GetNode<TileMap>("World");
        Rect2 mapSize = world.GetUsedRect();
        Vector2 cellSize = world.CellSize;
        Player.GetNode<Camera2D>("Camera2D").LimitLeft = Convert.ToInt32((mapSize.Position.x - 15) * cellSize.x);
        Player.GetNode<Camera2D>("Camera2D").LimitRight = Convert.ToInt32((mapSize.End.x + 5) * cellSize.x);
    }

    private void SpawnPickups()
    {
        foreach (Vector2 cell in Pickups.GetUsedCells())
        {
            int id = Pickups.GetCellv(cell);
            string type = Pickups.TileSet.TileGetName(id);
            if (type.ToLower() == "gem" || type.ToLower() == "cherry")
            {
                GD.Print(type);
                Collectible c = (Collectible) Collectible.Instance();
                Vector2 pos = Pickups.MapToWorld(cell);
                c.Init(type, pos + Pickups.CellSize / 2);
                AddChild(c);
                c.Connect("Pickup", this, "_on_Collectible_Pickup");
            }
        }
    }
    
    private void _on_Collectible_Pickup(string type)
    {
        if (type == "cherry") 
        {
            Score += 1;
            EmitSignal("ScoreChanged", Score);
        }
        else if (type == "gem")
        {
            Player p = GetNode<Player>("Player");
            if (p.Life < 5)
            {
                EmitSignal("LifeChanged", p.Life += 1);
            }
        }
    }

    private void _on_Player_Dead()
    {
        _gameState.Restart();
    }

    private void _on_Door_body_entered(object body)
    {
        _gameState.NextLevel();
    }

}
