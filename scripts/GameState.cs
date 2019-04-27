using Godot;

namespace GlobalGameState
{
public class GameState : Node
{
    public int NumberOfLevels { get; set; }
    public int CurrentLevel { get; set; }

    public string GameScene { get; } = "res://scenes/Main.tscn";

    private string TitleScreen = "res://scenes/ui/Start.tscn";

    public override void _Ready()
    {
        base._Ready();
        NumberOfLevels = 2;
    }

    public void Restart()
    {
        GetTree().ChangeScene("TitleScreen");
    }
    
    private void NextLevel()
    {
        CurrentLevel += 1;
        // Add a game over screen?
        if (CurrentLevel <= NumberOfLevels)
        {
            GetTree().ReloadCurrentScene();
        }
    }
}
}