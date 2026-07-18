namespace Undrone.FSharp

open Godot

[<AllowNullLiteral>]
type GameWorld() =
    inherit Node2D()

    let drone = new Sprite2D(
        Texture = GD.Load<Texture2D>("res://assets/drone.svg"),
        Scale = Vector2(0.5f, 0.5f)
    )

    member this.Initialize() =
        // Dynamically calculate the center of the screen
        drone.Position <- this.GetViewportRect().Size / 2.0f
        this.AddChild(drone)

    member this.Update(delta: double) =
        // Calculate movement direction from arrow keys (standard UI mappings)
        let dx = 
            (if Input.IsActionPressed("ui_right") then 1.0f else 0.0f) - 
            (if Input.IsActionPressed("ui_left") then 1.0f else 0.0f)
        let dy = 
            (if Input.IsActionPressed("ui_down") then 1.0f else 0.0f) - 
            (if Input.IsActionPressed("ui_up") then 1.0f else 0.0f)
        
        let direction = Vector2(dx, dy)
        let velocity = 
            if direction.LengthSquared() > 0.0f then
                direction.Normalized() * 400.0f
            else
                Vector2.Zero

        drone.Position <- drone.Position + velocity * (float32 delta)
