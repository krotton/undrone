namespace Undrone.FSharp

open Godot

type GameLoop(node: Node2D) =
    let confirmDialog = new ConfirmationDialog(Title = "Exit Game", DialogText = "Are you sure you want to exit?")

    member this.OnExitConfirmed() =
        node.GetTree().Quit()

    member this.OnExitRequested() =
        confirmDialog.PopupCentered()

    member this.OnNewGame() =
        GD.Print("New Game pressed")

    member this.Ready() =
        // Setup Exit Confirmation Dialog
        confirmDialog.Connect("confirmed", Callable.From(System.Action(this.OnExitConfirmed))) |> ignore
        node.AddChild(confirmDialog)

        // Create CanvasLayer to host UI and handle screen-scale anchoring
        let canvasLayer = new CanvasLayer()
        node.AddChild(canvasLayer)

        // Instantiate and add MainMenu with delegates to GameLoop methods
        let onNewGame = System.Action(this.OnNewGame)
        let onExit = System.Action(this.OnExitRequested)
        let mainMenu = new MainMenu(onNewGame, onExit)
        mainMenu.SetAnchorsPreset(Control.LayoutPreset.FullRect)
        canvasLayer.AddChild(mainMenu)

    member this.Process(delta: double) =
        // Game loop ticks will go here
        ()

    member this.UnhandledInput(event: InputEvent) =
        if event.IsActionPressed("ui_cancel") then
            this.OnExitRequested()
            node.GetViewport().SetInputAsHandled()
