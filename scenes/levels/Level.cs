using Godot;
using System;

public class Level : Node2D
{
    [Signal]
    delegate void ScoreChanged();
    
    private TileMap Pickups { get; set;  }
    private Player Player { get; set; }
    public int Score { get; set; }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        Player = GetNode<Player>("Player");
        Pickups = GetNode<TileMap>("Pickups");
        Pickups.Hide();
        Position2D startPos = GetNode<Position2D>("PlayerSpawn");
        GetNode<Player>("Player").Start(new Vector2(startPos.Position.x, startPos.Position.y));
        SetCameraLimits();
        SpawnPickups();
    }

    private void SetCameraLimits()
    {
        
        TileMap world = GetNode<TileMap>("World");
        Rect2 mapSize = world.GetUsedRect();
        Vector2 cellSize = world.CellSize;
        Player.GetNode<Camera2D>("Camera2D").LimitLeft = Convert.ToInt32((mapSize.Position.x - 5) * cellSize.x);
        Player.GetNode<Camera2D>("Camera2D").LimitRight = Convert.ToInt32((mapSize.End.x - 5) * cellSize.x);
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
                Collectible c = new Collectible();
//                Vector2 pos = Pickups.MapToWorld(cell);
//                c.Init(type, pos + Pickups.CellSize / 2);
//                AddChild(c);
//                c.Connect("pickup", this, "on_Collectible_Pickup");
            }
        }
    }
    
    private void _on_Collectible_Pickup()
    {
        Score += 1;
        EmitSignal("ScoreChanged", Score);
    }

    private void _on_Player_Dead()
    {
        return;
    }

}
