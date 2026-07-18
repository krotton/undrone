namespace Undrone.FSharp

open Godot
open System.Text.Json

[<CLIMutable>]
type MapPosition = { x: float32; y: float32 }

[<CLIMutable>]
type MapData = { drone_start: MapPosition; trees: MapPosition[] }

[<AllowNullLiteral>]
type GameWorld(mapPath: string) =
    inherit Node2D()

    let drone = new Sprite2D(
        Texture = GD.Load<Texture2D>("res://assets/drone.svg"),
        Scale = Vector2(0.5f, 0.5f)
    )

    let mutable basePosition = Vector2.Zero
    let mutable currentOffset = Vector2.Zero

    member this.Initialize() =
        // Load and parse the JSON map
        let jsonText = FileAccess.GetFileAsString(mapPath)
        let mapData = JsonSerializer.Deserialize<MapData>(jsonText)

        // Set the drone starting position
        basePosition <- Vector2(mapData.drone_start.x, mapData.drone_start.y)
        drone.Position <- basePosition
        
        // Spawn tree sprites from the map data
        let treeTexture = GD.Load<Texture2D>("res://assets/tree.svg")
        for treePos in mapData.trees do
            let tree = new Sprite2D(
                Texture = treeTexture,
                Position = Vector2(treePos.x, treePos.y),
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
