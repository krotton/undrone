namespace Undrone.FSharp

open Godot

[<AllowNullLiteral>]
type Drone(startPosition: Vector2) as this =
    inherit Sprite2D()

    let mutable basePosition = startPosition
    let mutable currentOffset = Vector2.Zero

    do
        this.Texture <- GD.Load<Texture2D>("res://assets/drone.svg")
        this.Scale <- Vector2(0.5f, 0.5f)
        this.Position <- startPosition

    member this.Update(delta: double, velocity: Vector2, isMoving: bool, index: int) =
        // Update basePosition
        basePosition <- basePosition + velocity * (float32 delta)

        // Calculate the target waving offset with a phase shift per drone
        let targetOffset = 
            if not isMoving then
                let timeSeconds = (float32 (Time.GetTicksMsec())) / 1000.0f
                let amplitude = 12.0f
                let frequency = 3.0f
                let phase = (float32 index) * 1.5f
                Vector2(amplitude * sin(frequency * timeSeconds + phase), 0.0f)
            else
                Vector2.Zero

        // Smoothly interpolate currentOffset towards targetOffset
        currentOffset <- currentOffset.Lerp(targetOffset, float32 (10.0 * delta))

        this.Position <- basePosition + currentOffset

[<AllowNullLiteral>]
type Tree(startPosition: Vector2) as this =
    inherit Sprite2D()

    do
        this.Texture <- GD.Load<Texture2D>("res://assets/tree.svg")
        this.Scale <- Vector2(1.0f, 1.0f)
        this.Position <- startPosition

[<AllowNullLiteral>]
type GameWorld(mapData: MapData) =
    inherit Node2D()

    let mutable drones : Drone[] = [||]

    member this.Initialize() =
        // Spawn tree sprites from the map data
        for treePos in mapData.Trees do
            let tree = new Tree(Vector2(treePos.X, treePos.Y))
            this.AddChild(tree)
            
        // Spawn drones from the map data
        drones <- 
            mapData.Drones 
            |> Array.map (fun pos -> 
                let d = new Drone(Vector2(pos.X, pos.Y))
                this.AddChild(d)
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
        for i in 0 .. drones.Length - 1 do
            drones.[i].Update(delta, velocity, isMoving, i)
