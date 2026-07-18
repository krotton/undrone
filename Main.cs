using Godot;
using Undrone.FSharp;

public partial class Main : Node2D
{
    private GameLoop _gameLoop;

    public override void _Ready()
    {
        GD.Print("C# Host: Initializing F# GameLoop...");
        _gameLoop = new GameLoop(this);
        _gameLoop.Ready();
    }

    public override void _Process(double delta)
    {
        _gameLoop.Process(delta);
    }
}
