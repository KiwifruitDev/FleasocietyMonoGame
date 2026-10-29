// Fleasociety (MonoGame)
// Copyright (c) 2026 KiwifruitDev
// Licensed under the MIT License
// View LICENSE before reviewing source code

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Screens;
using Fleasociety.Graphics;
using Fleasociety.Systems;

namespace Fleasociety.Screens;

/// <summary>
/// The main screen that displays the concept art.
/// </summary>
public class ConceptScreen : GameScreen
{
    private readonly Texture2D _conceptTexture;
    private readonly SpriteBatch _spriteBatch;

    public ConceptScreen(Game game, SpriteBatch spriteBatch, Texture2D conceptTexture)
        : base(game)
    {
        _conceptTexture = conceptTexture;
        _spriteBatch = spriteBatch;
        DrawWhenInactive = true;
    }

    public override void Update(GameTime gameTime)
    {
    }

    public override void Draw(GameTime gameTime)
    {
        _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp);
        _spriteBatch.Draw(_conceptTexture, Vector2.Zero, Color.White);

        if (FontManager.IsLoaded)
        {
            TextRenderer.DrawShadowed(_spriteBatch, FontManager.MunroSmall, "Concept", Color.White, new Vector2(10, 10));
        }

        _spriteBatch.End();
    }
}
