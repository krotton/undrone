namespace Undrone.FSharp

open Godot

[<AllowNullLiteral>]
type TreeSprite(startPosition: Vector2) as this =
    inherit Sprite2D()

    do
        this.Texture <- GD.Load<Texture2D>("res://assets/tree.svg")
        this.Scale <- Vector2(1.0f, 1.0f)
        this.Position <- startPosition
