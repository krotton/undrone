namespace Undrone.FSharp

open Godot
open System.Text.Json

[<CLIMutable>]
type MapPosition = { x: float32; y: float32 }

[<CLIMutable>]
type MapData = { drone_start: MapPosition; trees: MapPosition[] }

module MapLoader =
    let loadMap (mapPath: string) : MapData =
        let jsonText = FileAccess.GetFileAsString(mapPath)
        JsonSerializer.Deserialize<MapData>(jsonText)
