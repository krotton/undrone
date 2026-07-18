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
