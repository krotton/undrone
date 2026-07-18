namespace Undrone.FSharp

open Godot

type GameLoop(node: Node2D) =
    member this.Ready() =
        GD.Print("Hello from F# in Godot!")
        let label = new Label()
        label.Text <- "Undrone"
        label.Position <- Vector2(150.0f, 150.0f)
        label.AddThemeFontSizeOverride("font_size", 64)
        node.AddChild(label)

    member this.Process(delta: double) =
        // Game loop ticks will go here
        ()
