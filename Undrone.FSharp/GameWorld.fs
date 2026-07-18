namespace Undrone.FSharp

open Godot

type DroneInstance(texture: Texture2D, startPosition: Vector2) =
    let sprite = new Sprite2D(
        Texture = texture,
        Position = startPosition,
        Scale = Vector2(0.5f, 0.5f)
    )
    member this.Sprite = sprite
    member val BasePosition = startPosition with get, set
    member val CurrentOffset = Vector2.Zero with get, set

[<AllowNullLiteral>]
type GameWorld(mapData: MapData) =
    inherit Node2D()

    let mutable drones : DroneInstance[] = [||]

    member this.Initialize() =
        // Spawn tree sprites from the map data
        let treeTexture = GD.Load<Texture2D>("res://assets/tree.svg")
        for treePos in mapData.Trees do
            let tree = new Sprite2D(
                Texture = treeTexture,
                Position = Vector2(treePos.X, treePos.Y),
                Scale = Vector2(1.0f, 1.0f)
            )
            this.AddChild(tree)
            
        // Spawn drones from the map data
        let droneTexture = GD.Load<Texture2D>("res://assets/drone.svg")
        drones <- 
            mapData.Drones 
            |> Array.map (fun pos -> 
                let d = new DroneInstance(droneTexture, Vector2(pos.X, pos.Y))
                this.AddChild(d.Sprite)
                d
            )

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

        // Update each drone position and waving offset
        let timeSeconds = (float32 (Time.GetTicksMsec())) / 1000.0f

        for i in 0 .. drones.Length - 1 do
            let d = drones.[i]
            
            // Update basePosition
            d.BasePosition <- d.BasePosition + velocity * (float32 delta)

            // Calculate the target waving offset with a phase shift per drone
            let targetOffset = 
                if not isMoving then
                    let amplitude = 12.0f
                    let frequency = 3.0f
                    let phase = (float32 i) * 1.5f
                    Vector2(amplitude * sin(frequency * timeSeconds + phase), 0.0f)
                else
                    Vector2.Zero

            // Smoothly interpolate currentOffset towards targetOffset
            d.CurrentOffset <- d.CurrentOffset.Lerp(targetOffset, float32 (10.0 * delta))

            d.Sprite.Position <- d.BasePosition + d.CurrentOffset
