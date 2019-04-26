using Godot;
using System;

public class Level : Node2D
{
    [Signal]
    delegate void ScoreChanged();
    [Signal]
    delegate void LifeChanged();
    
    private TileMap Pickups { get; set;  }
    private Player Player { get; set; }
    private PackedScene Collectible { get; set; }
    public int Score { get; set; }
    

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        Collectible = (PackedScene) ResourceLoader.Load("res://scenes/Collectible.tscn");
        Player = GetNode<Player>("Player");
        Pickups = GetNode<TileMap>("Pickups");
        Pickups.Hide();
        Score = 0;
        // EmitSignal("ScoreChanged", Score);
        Position2D startPos = GetNode<Position2D>("PlayerSpawn");
        GetNode<Player>("Player").Start(new Vector2(startPos.Position.x, startPos.Position.y));

        Connect("ScoreChanged", GetNode<HUD>("CanvasLayer/HUD"), "_on_ScoreChanged");
        Connect("LifeChanged", GetNode<HUD>("CanvasLayer/HUD"), "_on_Player_LifeChanged");

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
                int newLives = p.Life += 1;
                EmitSignal("LifeChanged", p.Life);
            }
        }
    }

    private void _on_Player_Dead()
    {
        return;
    }

}
