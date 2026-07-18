namespace Undrone.FSharp

open Godot

type GameLoop(node: Node2D) =
    member this.Ready() =
        let texture = GD.Load<Texture2D>("res://assets/logo.svg")
        let sprite = new Sprite2D()
        sprite.Texture <- texture
        sprite.Position <- Vector2(576.0f, 324.0f)
        sprite.Scale <- Vector2(0.5f, 0.5f)
        node.AddChild(sprite)

    member this.Process(delta: double) =
        // Game loop ticks will go here
        ()
