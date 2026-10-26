// Fleasociety (MonoGame)
// Copyright (c) 2026 KiwifruitDev
// Licensed under the MIT License
// View LICENSE before reviewing source code

using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.BitmapFonts;

namespace Fleasociety.Systems;

/// <summary>
/// Manages bitmap fonts for the game
/// </summary>
public static class FontManager
{
    private static BitmapFont _munroFont;
    
    /// <summary>
    /// The Munro bitmap font
    /// </summary>
    public static BitmapFont MunroFont => _munroFont;
    
    /// <summary>
    /// Whether the fonts are loaded
    /// </summary>
    public static bool IsLoaded => _munroFont != null;
    
    /// <summary>
    /// Load all fonts
    /// </summary>
    public static void Load(ContentManager content)
    {
        //_munroFont = content.Load<BitmapFont>("fonts/munro");
    }
    
    /// <summary>
    /// Unload all fonts
    /// </summary>
    public static void Unload()
    {
        _munroFont = null;
    }
}
