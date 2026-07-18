namespace Undrone.FSharp

open Godot

type GameLoop(node: Node2D) =
    let mutable confirmDialog : ConfirmationDialog = null

    member this.Ready() =
        // Create a CenterContainer that spans the whole screen
        let center = new CenterContainer()
        center.CustomMinimumSize <- Vector2(1152.0f, 648.0f)
        
        // HBoxContainer inside it to align icon and text horizontally
        let hbox = new HBoxContainer()
        hbox.AddThemeConstantOverride("separation", 20)
        
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
        
        hbox.AddChild(icon)
        hbox.AddChild(label)
        center.AddChild(hbox)
        node.AddChild(center)

        // Setup Exit Confirmation Dialog
        confirmDialog <- new ConfirmationDialog()
        confirmDialog.Title <- "Exit Game"
        confirmDialog.DialogText <- "Are you sure you want to exit?"
        confirmDialog.Connect("confirmed", Callable.From(System.Action(fun () -> node.GetTree().Quit()))) |> ignore
        node.AddChild(confirmDialog)

    member this.Process(delta: double) =
        // Game loop ticks will go here
        ()

    member this.UnhandledInput(event: InputEvent) =
        if event.IsActionPressed("ui_cancel") then
            confirmDialog.PopupCentered()
            node.GetViewport().SetInputAsHandled()
