using Godot;
using System;
using System.Collections.Generic;

public class HUD : MarginContainer
{
    private List<string> LifeCounter = new List<string>
    {
        "HBoxContainer/LifeCounter/L1",
        "HBoxContainer/LifeCounter/L2",
        "HBoxContainer/LifeCounter/L3",
        "HBoxContainer/LifeCounter/L4",
        "HBoxContainer/LifeCounter/L5"
    };


    public override void _Ready()
    {
        
    }

    public void _on_Player_LifeChanged(int value)
    {
        for (int i = 0; i < LifeCounter.Count; i++)
        {
            TextureRect t = (TextureRect) GetNode(LifeCounter[i]);
            t.Visible = value > i;
        }
    }

    public void _on_Score_Changed(string value)
    {
        GetNode<Label>("HBoxContainer/ScoreLabel").Text = value;
    }
}
