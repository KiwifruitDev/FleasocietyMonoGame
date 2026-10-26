// Fleasociety (MonoGame)
// Copyright (c) 2026 KiwifruitDev
// Licensed under the MIT License
// View LICENSE before reviewing source code

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Fleasociety.Systems;

/// <summary>
/// Manages input state and provides easy access to input checks
/// </summary>
public class InputSystem
{
    private readonly Game _game;
    
    /// <summary>
    /// Current keyboard state
    /// </summary>
    public KeyboardState CurrentKeyboardState { get; private set; }
    
    /// <summary>
    /// Previous keyboard state
    /// </summary>
    public KeyboardState PreviousKeyboardState { get; private set; }
    
    /// <summary>
    /// Current mouse state
    /// </summary>
    public MouseState CurrentMouseState { get; private set; }
    
    /// <summary>
    /// Previous mouse state
    /// </summary>
    public MouseState PreviousMouseState { get; private set; }

    public InputSystem(Game game)
    {
        _game = game;
    }

    /// <summary>
    /// Update input states
    /// </summary>
    public void Update()
    {
        PreviousKeyboardState = CurrentKeyboardState;
        PreviousMouseState = CurrentMouseState;
        
        CurrentKeyboardState = Keyboard.GetState();
        CurrentMouseState = Mouse.GetState();
    }

    /// <summary>
    /// Check if a key was just pressed (not held)
    /// </summary>
    public bool IsKeyPressed(Keys key)
    {
        return CurrentKeyboardState.IsKeyDown(key) && PreviousKeyboardState.IsKeyUp(key);
    }

    /// <summary>
    /// Check if a key is currently down
    /// </summary>
    public bool IsKeyDown(Keys key)
    {
        return CurrentKeyboardState.IsKeyDown(key);
    }

    /// <summary>
    /// Check if a key was just released
    /// </summary>
    public bool IsKeyReleased(Keys key)
    {
        return CurrentKeyboardState.IsKeyUp(key) && PreviousKeyboardState.IsKeyDown(key);
    }

    /// <summary>
    /// Check if Alt+Enter was pressed
    /// </summary>
    public bool IsAltEnterPressed()
    {
        return IsKeyPressed(Keys.Enter) && (IsKeyDown(Keys.LeftAlt) || IsKeyDown(Keys.RightAlt));
    }

    /// <summary>
    /// Check if F11 was pressed
    /// </summary>
    public bool IsF11Pressed()
    {
        return IsKeyPressed(Keys.F11);
    }

    /// <summary>
    /// Check if Escape was pressed
    /// </summary>
    public bool IsEscapePressed()
    {
        return IsKeyPressed(Keys.Escape);
    }

    /// <summary>
    /// Check if P was pressed (for pause)
    /// </summary>
    public bool IsPPressed()
    {
        return IsKeyPressed(Keys.P);
    }
}
