namespace Undrone.FSharp

open Godot

type GameLoop(node: Node2D) =
    member this.Ready() =
        GD.Print("Hello from F# in Godot!")

    member this.Process(delta: double) =
        // Game loop ticks will go here
        ()
