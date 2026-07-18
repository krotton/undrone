namespace Undrone.FSharp

open Godot

type GameLoop(node: Node2D) =
    let confirmDialog = new ConfirmationDialog(Title = "Exit Game", DialogText = "Are you sure you want to exit?")
    let mutable mainMenu : MainMenu option = None
    let mutable gameWorld : GameWorld option = None

    member this.OnExitConfirmed() =
        node.GetTree().Quit()

    member this.OnExitRequested() =
        confirmDialog.PopupCentered()

    member this.OnNewGame() =
        // Remove Main Menu
        match mainMenu with
        | Some menu ->
            menu.QueueFree()
            mainMenu <- None
        | None -> ()

        // Instantiate and add GameWorld scene
        let world = new GameWorld()
        gameWorld <- Some world
        node.AddChild(world)
        world.Initialize()

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
        let menu = new MainMenu(onNewGame, onExit)
        menu.SetAnchorsPreset(Control.LayoutPreset.FullRect)
        canvasLayer.AddChild(menu)
        mainMenu <- Some menu

    member this.Process(delta: double) =
        // Game loop ticks will go here
        ()

    member this.UnhandledInput(event: InputEvent) =
        if event.IsActionPressed("ui_cancel") then
            this.OnExitRequested()
            node.GetViewport().SetInputAsHandled()
