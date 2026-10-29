// Fleasociety (MonoGame)
// Copyright (c) 2026 KiwifruitDev
// Licensed under the MIT License
// View LICENSE before reviewing source code

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Screens;
using MonoGame.Extended.Tweening;
using Fleasociety.Graphics;
using Fleasociety.Systems;

namespace Fleasociety.Screens;

/// <summary>
/// Pause screen displayed over the active game screen.
/// </summary>
public class PauseScreen : GameScreen
{
    private readonly FadeState _fade = new();
    private readonly Tweener _tweener = new();
    private readonly SpriteBatch _spriteBatch;
    private bool _isFadingOut;
    private const float FadeDuration = 0.5f;

    private sealed class FadeState
    {
        public float Alpha { get; set; }
    }

    public bool IsFadingOut => _isFadingOut;
    public event Action FadeOutCompleted;

    public PauseScreen(Game game, SpriteBatch spriteBatch)
        : base(game)
    {
        _spriteBatch = spriteBatch;
    }

    public override void UnloadContent()
    {
        _tweener.Dispose();
        base.UnloadContent();
    }

    public void Show()
    {
        _isFadingOut = false;
        _tweener.TweenTo(_fade, fade => fade.Alpha, 0.75f, FadeDuration);
    }

    public void Hide()
    {
        _isFadingOut = true;
        _tweener.TweenTo(_fade, fade => fade.Alpha, 0f, FadeDuration)
            .OnEnd(_ => FadeOutCompleted?.Invoke());
    }

    public override void OnActivated()
    {
        base.OnActivated();
        Show();
    }

    public override void Update(GameTime gameTime)
    {
        _tweener.Update((float)gameTime.ElapsedGameTime.TotalSeconds);
    }

    public override void Draw(GameTime gameTime)
    {
        if (_fade.Alpha <= 0)
        {
            return;
        }

        _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);

        var overlay = new Rectangle(0, 0, Game1.BaseWidth, Game1.BaseHeight);
        _spriteBatch.Draw(GlobalTextures.WhitePixel, overlay, Color.Black * _fade.Alpha);

        if (FontManager.IsLoaded)
        {
            TextRenderer.DrawCentered(_spriteBatch, FontManager.Munro, "Paused", Color.White, _fade.Alpha);
        }

        _spriteBatch.End();
    }
}
