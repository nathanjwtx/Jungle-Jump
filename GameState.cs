using Godot;

namespace Jungle_Jump
{
    public class GameState : Node
    {
        public int NumberOfLevels { get; set; }
        public int CurrentLevel { get; set; }

        private string GameScene = "res://scenes/levels/Level.tscn";
        
    }
}