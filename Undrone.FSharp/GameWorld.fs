namespace Undrone.FSharp

open Godot

[<AllowNullLiteral>]
type GameWorld(mapData: MapData) =
    inherit Node2D()

    let mutable drones : Drone[] = [||]

    member this.Initialize() =
        // Spawn tree sprites from the map data
        for treePos in mapData.Trees do
            let tree = new TreeSprite(Vector2(treePos.X, treePos.Y))
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
