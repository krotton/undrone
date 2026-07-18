namespace Undrone.FSharp

open Godot

type MainMenu(onNewGame: System.Action, onExit: System.Action) as this =
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
        let texture = GD.Load<Texture2D>("res://assets/drone.svg")
        let icon = new TextureRect(
            Texture = texture,
            CustomMinimumSize = Vector2(110.0f, 110.0f),
            ExpandMode = TextureRect.ExpandModeEnum.IgnoreSize,
            StretchMode = TextureRect.StretchModeEnum.KeepAspectCentered,
            SizeFlagsVertical = Control.SizeFlags.ShrinkCenter
        )
        
        // Text (RichTextLabel with BBCode styling)
        let label = new RichTextLabel(
            BbcodeEnabled = true,
            Text = "[font_size=80][b]Un[color=#00f3ff]drone[/color][/b][/font_size]",
            CustomMinimumSize = Vector2(400.0f, 110.0f),
            ScrollActive = false,
            SizeFlagsVertical = Control.SizeFlags.ShrinkCenter
        )
        
        logoHBox.AddChild(icon)
        logoHBox.AddChild(label)
        mainLayout.AddChild(logoHBox)

        // Menu VBoxContainer
        let menuVBox = new VBoxContainer(
            SizeFlagsHorizontal = Control.SizeFlags.ShrinkCenter
        )
        menuVBox.AddThemeConstantOverride("separation", 15)
        
        // 1. New Game Button
        let btnNewGame = new Button(
            Text = "New Game",
            CustomMinimumSize = Vector2(250.0f, 50.0f)
        )
        btnNewGame.AddThemeFontSizeOverride("font_size", 24)
        
        // 2. Continue Button (disabled)
        let btnContinue = new Button(
            Text = "Continue",
            Disabled = true,
            CustomMinimumSize = Vector2(250.0f, 50.0f)
        )
        btnContinue.AddThemeFontSizeOverride("font_size", 24)
        
        // 3. Exit Button
        let btnExit = new Button(
            Text = "Exit",
            CustomMinimumSize = Vector2(250.0f, 50.0f)
        )
        btnExit.AddThemeFontSizeOverride("font_size", 24)
        
        // Connect buttons using actions passed from parent
        btnExit.Connect("pressed", Callable.From(onExit)) |> ignore
        btnNewGame.Connect("pressed", Callable.From(onNewGame)) |> ignore
        
        menuVBox.AddChild(btnNewGame)
        menuVBox.AddChild(btnContinue)
        menuVBox.AddChild(btnExit)
        mainLayout.AddChild(menuVBox)
        
        this.AddChild(mainLayout)
