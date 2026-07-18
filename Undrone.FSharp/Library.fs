namespace Undrone.FSharp

open Godot

type MainMenu(node: Node2D, confirmDialog: ConfirmationDialog) as this =
    inherit CenterContainer()

    do
        this.CustomMinimumSize <- Vector2(1152.0f, 648.0f)
        
        // VBoxContainer to align the logo and the menu vertically
        let mainLayout = new VBoxContainer()
        mainLayout.AddThemeConstantOverride("separation", 40)
        
        // HBoxContainer to align icon and text horizontally
        let logoHBox = new HBoxContainer()
        logoHBox.AddThemeConstantOverride("separation", 20)
        
        // Icon (TextureRect loaded from logo.svg)
        let texture = GD.Load<Texture2D>("res://assets/logo.svg")
        let icon = new TextureRect()
        icon.Texture <- texture
        icon.CustomMinimumSize <- Vector2(110.0f, 110.0f)
        icon.ExpandMode <- TextureRect.ExpandModeEnum.IgnoreSize
        icon.StretchMode <- TextureRect.StretchModeEnum.KeepAspectCentered
        icon.SizeFlagsVertical <- Control.SizeFlags.ShrinkCenter
        
        // Text (RichTextLabel with BBCode styling)
        let label = new RichTextLabel()
        label.BbcodeEnabled <- true
        label.Text <- "[font_size=80][b]Un[color=#00f3ff]drone[/color][/b][/font_size]"
        label.CustomMinimumSize <- Vector2(400.0f, 110.0f)
        label.ScrollActive <- false
        label.SizeFlagsVertical <- Control.SizeFlags.ShrinkCenter
        
        logoHBox.AddChild(icon)
        logoHBox.AddChild(label)
        mainLayout.AddChild(logoHBox)

        // Menu VBoxContainer
        let menuVBox = new VBoxContainer()
        menuVBox.AddThemeConstantOverride("separation", 15)
        menuVBox.SizeFlagsHorizontal <- Control.SizeFlags.ShrinkCenter
        
        // 1. New Game Button
        let btnNewGame = new Button()
        btnNewGame.Text <- "New Game"
        btnNewGame.CustomMinimumSize <- Vector2(250.0f, 50.0f)
        btnNewGame.AddThemeFontSizeOverride("font_size", 24)
        
        // 2. Continue Button (disabled)
        let btnContinue = new Button()
        btnContinue.Text <- "Continue"
        btnContinue.Disabled <- true
        btnContinue.CustomMinimumSize <- Vector2(250.0f, 50.0f)
        btnContinue.AddThemeFontSizeOverride("font_size", 24)
        
        // 3. Exit Button
        let btnExit = new Button()
        btnExit.Text <- "Exit"
        btnExit.CustomMinimumSize <- Vector2(250.0f, 50.0f)
        btnExit.AddThemeFontSizeOverride("font_size", 24)
        
        // Connect buttons
        btnExit.Connect("pressed", Callable.From(System.Action(fun () -> confirmDialog.PopupCentered()))) |> ignore
        btnNewGame.Connect("pressed", Callable.From(System.Action(fun () -> GD.Print("New Game pressed")))) |> ignore
        
        menuVBox.AddChild(btnNewGame)
        menuVBox.AddChild(btnContinue)
        menuVBox.AddChild(btnExit)
        mainLayout.AddChild(menuVBox)
        
        this.AddChild(mainLayout)

type GameLoop(node: Node2D) =
    let confirmDialog = new ConfirmationDialog()

    member this.Ready() =
        // Setup Exit Confirmation Dialog
        confirmDialog.Title <- "Exit Game"
        confirmDialog.DialogText <- "Are you sure you want to exit?"
        confirmDialog.Connect("confirmed", Callable.From(System.Action(fun () -> node.GetTree().Quit()))) |> ignore
        node.AddChild(confirmDialog)

        // Instantiate and add MainMenu
        let mainMenu = new MainMenu(node, confirmDialog)
        node.AddChild(mainMenu)

    member this.Process(delta: double) =
        // Game loop ticks will go here
        ()

    member this.UnhandledInput(event: InputEvent) =
        if event.IsActionPressed("ui_cancel") then
            confirmDialog.PopupCentered()
            node.GetViewport().SetInputAsHandled()
