using Godot;
using System;
using System.Collections.Generic;

public class Level2 : Level
{
    private float DayLengthModifier;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        AnimationPlayer sunset = GetNode<AnimationPlayer>("ParallaxBackground/ParallaxLayer/AnimationPlayer");
        DayLengthModifier = 1 + (new Random().Next(0, 10) / 10f);
        GD.Print(DayLengthModifier);
        sunset.PlaybackSpeed = DefaultDayLengthRatio * DayLengthModifier;
        sunset.Play("dawn_to_dusk");
        
    }

    private void _on_AnimationPlayer_animation_started(String anim_name)
    {
        GD.Print(anim_name);
    }

    private void _on_AnimationPlayer_animation_finished(String anim_name)
    {
        GD.Print("done");
    }

}
