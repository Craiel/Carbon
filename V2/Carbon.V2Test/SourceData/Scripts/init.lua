Log:Debug("Setting up key bindings", nil)

binding = RegisterKeyBinding("worldmap_controls")
binding:BindEx("UpArrow", "MoveForward", "Always", "And", nil)
binding:BindEx("DownArrow", "MoveBackward", "Always", "And", nil)
binding:BindEx("LeftArrow", "MoveLeft", "Always", "And", nil)
binding:BindEx("RightArrow", "MoveRight", "Always", "And", nil)

binding:BindEx("W", "MoveForward", "Always", "And", nil)
binding:BindEx("S", "MoveBackward", "Always", "And", nil)
binding:BindEx("A", "MoveLeft", "Always", "And", nil)
binding:BindEx("D", "MoveRight", "Always", "And", nil)

binding:Bind("Z", "Confirm")
binding:Bind("Enter", "Confirm")
binding:Bind("X", "Cancel")
binding:Bind("Escape", "Cancel")

binding:BindEx("Mouse1", "ToggleRotation", "PressAndRelease", "And", nil)
