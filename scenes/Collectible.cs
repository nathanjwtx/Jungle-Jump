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
        CreateSpriteDict();
    }

    public void Init(string type, Vector2 pos)
    {
        CreateSpriteDict();
//        GD.Print(GetParent().GetName());
        GetNode<Sprite>("Sprite").Texture = GD.Load<Texture>(Textures[type]);
        Position = pos;
    }
    
    private void _on_Collectible_body_entered(object body)
    {
        EmitSignal("Pickup");
        QueueFree();
    }

    private void CreateSpriteDict()
    {
        if (Textures.Count == 0)
        {
            Textures.Add("cherry", "res://assets/sprites/cherry.png");
            Textures.Add("gem", "res://assets/sprites/gem.png");
        }
    }
}



