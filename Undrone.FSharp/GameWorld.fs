namespace Undrone.FSharp

open Godot

type GameWorld() =
    inherit Node2D()

    override this._Ready() =
        let texture = GD.Load<Texture2D>("res://assets/drone.svg")
        
        // Dynamically calculate the center of the screen
        let viewCenter = this.GetViewportRect().Size / 2.0f
        
        let drone = new Sprite2D(
            Texture = texture,
            Position = viewCenter,
            Scale = Vector2(0.5f, 0.5f)
        )
        this.AddChild(drone)
