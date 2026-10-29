// Fleasociety (MonoGame)
// Copyright (c) 2026 KiwifruitDev
// Licensed under the MIT License
// View LICENSE before reviewing source code

using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Fleasociety.Systems;

/// <summary>
/// Manages the fonts used by the game
/// </summary>
public static class FontManager
{
    /// <summary>
    /// The Munro font, used for headings and prominent text
    /// </summary>
    public static SpriteFont Munro { get; private set; }

    /// <summary>
    /// The Munro Small font, used for smaller text
    /// </summary>
    public static SpriteFont MunroSmall { get; private set; }

    /// <summary>
    /// Whether the fonts are loaded
    /// </summary>
    public static bool IsLoaded => Munro != null && MunroSmall != null;

    /// <summary>
    /// Load all fonts
    /// </summary>
    public static void Load(ContentManager content)
    {
        Munro = content.Load<SpriteFont>("fonts/munro");
        MunroSmall = content.Load<SpriteFont>("fonts/munro-small");
    }

    /// <summary>
    /// Unload all fonts
    /// </summary>
    public static void Unload()
    {
        Munro = null;
        MunroSmall = null;
    }
}
