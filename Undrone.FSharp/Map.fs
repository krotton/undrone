namespace Undrone.FSharp

open Godot
open System.Text.Json

[<CLIMutable>]
type MapPosition = { X: float32; Y: float32 }

[<CLIMutable>]
type MapData = { DroneStart: MapPosition; Trees: MapPosition[] }

module MapLoader =
    let loadMap (mapPath: string) : MapData =
        let jsonText = FileAccess.GetFileAsString(mapPath)
        JsonSerializer.Deserialize<MapData>(jsonText)
