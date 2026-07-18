namespace Undrone.FSharp

open Godot

[<AllowNullLiteral>]
type GameWorld() =
    inherit Node2D()

    let drone = new Sprite2D(
        Texture = GD.Load<Texture2D>("res://assets/drone.svg"),
        Scale = Vector2(0.5f, 0.5f)
    )

    let mutable basePosition = Vector2.Zero

    member this.Initialize() =
        // Dynamically calculate the center of the screen
        basePosition <- this.GetViewportRect().Size / 2.0f
        drone.Position <- basePosition
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
        let isMoving = direction.LengthSquared() > 0.0f

        let velocity = 
            if isMoving then
                direction.Normalized() * 400.0f
            else
                Vector2.Zero

        // Update basePosition
        basePosition <- basePosition + velocity * (float32 delta)

        // Apply a gentle horizontal waving offset when idle
        let waveOffset = 
            if not isMoving then
                let timeSeconds = (float32 (Time.GetTicksMsec())) / 1000.0f
                let amplitude = 12.0f
                let frequency = 3.0f
                Vector2(amplitude * sin(frequency * timeSeconds), 0.0f)
            else
                Vector2.Zero

        drone.Position <- basePosition + waveOffset
