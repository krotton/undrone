namespace Undrone.FSharp

open Godot

type GameLoop(node: Node2D) =
    let confirmDialog = new ConfirmationDialog(Title = "Exit Game", DialogText = "Are you sure you want to exit?")

    member this.OnExitConfirmed() =
        node.GetTree().Quit()

    member this.OnExitPressed() =
        confirmDialog.PopupCentered()

    member this.OnNewGamePressed() =
        GD.Print("New Game pressed")

    member this.Ready() =
        // Setup Exit Confirmation Dialog
        confirmDialog.Connect("confirmed", Callable.From(System.Action(this.OnExitConfirmed))) |> ignore
        node.AddChild(confirmDialog)

        // Instantiate and add MainMenu with delegates to GameLoop methods
        let onNewGame = System.Action(this.OnNewGamePressed)
        let onExit = System.Action(this.OnExitPressed)
        let mainMenu = new MainMenu(onNewGame, onExit)
        node.AddChild(mainMenu)

    member this.Process(delta: double) =
        // Game loop ticks will go here
        ()

    member this.UnhandledInput(event: InputEvent) =
        if event.IsActionPressed("ui_cancel") then
            this.OnExitPressed()
            node.GetViewport().SetInputAsHandled()
