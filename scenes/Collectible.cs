using Godot;
using System;
using System.Collections.Generic;

public class Collectible : Area2D
{
    [Signal]
    delegate void Pickup();
    
    private Dictionary<string, string> Textures = new Dictionary<string, string>();

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        Textures.Add("cherry", "res://assets/sprites/cherry.png");
        Textures.Add("gem", "res://assets/sprites/gem.png");
    }

    public void Init(string type, Vector2 pos)
    {
        GetNode<Sprite>("Sprite").Texture = GD.Load<Texture>(Textures[type]);
        Position = pos;
    }
    
    private void _on_Collectible_body_entered(object body)
    {
        EmitSignal("Pickup");
        QueueFree();
    }
//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}



