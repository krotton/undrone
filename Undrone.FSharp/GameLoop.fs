namespace Undrone.FSharp

open Godot

type GameLoop(node: Node2D) =
    let confirmDialog = new ConfirmationDialog(Title = "Exit Game", DialogText = "Are you sure you want to exit?")

    member this.OnExitConfirmed() =
        node.GetTree().Quit()

    member this.OnExitRequested() =
        confirmDialog.PopupCentered()

    member this.Ready() =
        // Setup Exit Confirmation Dialog
        confirmDialog.Connect("confirmed", Callable.From(System.Action(this.OnExitConfirmed))) |> ignore
        node.AddChild(confirmDialog)

        // Create CanvasLayer to host UI and handle screen-scale anchoring
        let canvasLayer = new CanvasLayer()
        node.AddChild(canvasLayer)

        // Instantiate and add MainMenu with delegates.
        // We use a closure here to capture canvasLayer, allowing us to free the menu
        // and transition to GameWorld without maintaining mutable fields in GameLoop.
        let onNewGame = System.Action(fun () ->
            canvasLayer.QueueFree()
            let mapData = MapLoader.loadMap "res://maps/map_sample.json"
            let world = new GameWorld(mapData, Name = "GameWorld")
            node.AddChild(world)
            world.Initialize()
        )
        let onExit = System.Action(this.OnExitRequested)
        let menu = new MainMenu(onNewGame, onExit)
        menu.SetAnchorsPreset(Control.LayoutPreset.FullRect)
        canvasLayer.AddChild(menu)

    member this.Process(delta: double) =
        // Retrieve and update the active GameWorld node dynamically
        let world = node.GetNodeOrNull<GameWorld>("GameWorld")
        if not (isNull world) then
            world.Update(delta)

    member this.UnhandledInput(event: InputEvent) =
        if event.IsActionPressed("ui_cancel") then
            this.OnExitRequested()
            node.GetViewport().SetInputAsHandled()
