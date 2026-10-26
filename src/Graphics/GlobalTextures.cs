// Fleasociety (MonoGame)
// Copyright (c) 2026 KiwifruitDev
// Licensed under the MIT License
// View LICENSE before reviewing source code

using Microsoft.Xna.Framework.Graphics;

namespace Fleasociety.Graphics;

/// <summary>
/// Provides globally accessible textures used throughout the game
/// </summary>
public static class GlobalTextures
{
    /// <summary>
    /// A 1x1 white pixel texture for drawing rectangles and overlays
    /// </summary>
    public static Texture2D WhitePixel { get; private set; }
    
    /// <summary>
    /// Initializes the global textures. Must be called after GraphicsDevice is available.
    /// </summary>
    public static void Initialize(GraphicsDevice graphicsDevice)
    {
        WhitePixel = new Texture2D(graphicsDevice, 1, 1);
        WhitePixel.SetData(new[] { Microsoft.Xna.Framework.Color.White });
    }
    
    /// <summary>
    /// Disposes all global textures
    /// </summary>
    public static void Dispose()
    {
        WhitePixel?.Dispose();
        WhitePixel = null;
    }
}
