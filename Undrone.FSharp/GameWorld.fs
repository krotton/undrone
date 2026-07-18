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
    let mutable currentOffset = Vector2.Zero

    member this.Initialize() =
        // Dynamically calculate the center of the screen
        basePosition <- this.GetViewportRect().Size / 2.0f
        drone.Position <- basePosition
        
        // Add a static tree sprite to the world
        let treeTexture = GD.Load<Texture2D>("res://assets/tree.svg")
        let tree = new Sprite2D(
            Texture = treeTexture,
            Position = Vector2(basePosition.X - 250.0f, basePosition.Y + 50.0f),
            Scale = Vector2(1.0f, 1.0f)
        )
        this.AddChild(tree)
        
        // Add the drone last so it renders on top
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

        // Calculate the target waving offset
        let targetOffset = 
            if not isMoving then
                let timeSeconds = (float32 (Time.GetTicksMsec())) / 1000.0f
                let amplitude = 30.0f
                let frequency = 3.0f
                Vector2(amplitude * sin(frequency * timeSeconds), 0.0f)
            else
                Vector2.Zero

        // Smoothly interpolate currentOffset towards targetOffset
        currentOffset <- currentOffset.Lerp(targetOffset, float32 (3.0 * delta))

        drone.Position <- basePosition + currentOffset
