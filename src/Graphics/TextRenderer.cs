// Fleasociety (MonoGame)
// Copyright (c) 2026 KiwifruitDev
// Licensed under the MIT License
// View LICENSE before reviewing source code

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Fleasociety.Graphics;

/// <summary>
/// Helper methods for drawing text
/// </summary>
public static class TextRenderer
{
    private static readonly Vector2 ShadowOffset = new(2, 2);

    public static void Draw(SpriteBatch spriteBatch, SpriteFont font, string text, Color color, Vector2 position, float alpha = 1f)
    {
        spriteBatch.DrawString(font, text, position, color * alpha);
    }

    public static void DrawCentered(SpriteBatch spriteBatch, SpriteFont font, string text, Color color, Vector2 position, float alpha = 1f)
    {
        Vector2 textSize = font.MeasureString(text);
        Vector2 centeredPosition = position - textSize / 2;
        spriteBatch.DrawString(font, text, centeredPosition, color * alpha);
    }

    public static void DrawShadowed(SpriteBatch spriteBatch, SpriteFont font, string text, Color color, Vector2 position, float alpha = 1f)
    {
        spriteBatch.DrawString(font, text, position + ShadowOffset, Color.Black * alpha);
        spriteBatch.DrawString(font, text, position, color * alpha);
    }

    public static void DrawShadowedCentered(SpriteBatch spriteBatch, SpriteFont font, string text, Color color, Vector2 position, float alpha = 1f)
    {
        Vector2 textSize = font.MeasureString(text);
        Vector2 centeredPosition = position - textSize / 2;
        spriteBatch.DrawString(font, text, centeredPosition + ShadowOffset, Color.Black * alpha);
        spriteBatch.DrawString(font, text, centeredPosition, color * alpha);
    }
}
