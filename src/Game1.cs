// Fleasociety (MonoGame)
// Copyright (c) 2026 KiwifruitDev
// Licensed under the MIT License
// View LICENSE before reviewing source code

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Fleasociety.Screens;
using Fleasociety.Systems;
using Fleasociety.Graphics;
using MonoGame.Extended.Screens;

namespace Fleasociety;

public class Game1 : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    private readonly ScreenManager _screenManager;
    
    // Systems
    private AudioSystem _audioSystem;
    private InputSystem _inputSystem;
    
    // Target resolution for rendering
    internal const int BaseWidth = 960;
    internal const int BaseHeight = 540;
    private RenderTarget2D _renderTarget;
    
    // Focus tracking
    private bool _wasFocused = true;
    private bool _pausedByUser = false;
    private bool _pausedByFocus = false;
    private bool _pauseScreenCloseRequested;

    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        _screenManager = new ScreenManager();
        Components.Add(_screenManager);
        Content.RootDirectory = "content";
        IsMouseVisible = true;
        Exiting += OnExiting;
    }

    protected override void Initialize()
    {
        Window.AllowUserResizing = true;
        _graphics.PreferredBackBufferWidth = 1280;
        _graphics.PreferredBackBufferHeight = 720;
        _graphics.IsFullScreen = false;
        _graphics.HardwareModeSwitch = false;
        _graphics.ApplyChanges();
        
        // Initialize systems
        _inputSystem = new InputSystem(this);
        _audioSystem = new AudioSystem();
        _audioSystem.Initialize();
        
        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        
        // Initialize global textures
        GlobalTextures.Initialize(GraphicsDevice);
        
        // Create render target at base resolution
        _renderTarget = new RenderTarget2D(
            GraphicsDevice,
            BaseWidth,
            BaseHeight,
            false,
            GraphicsDevice.PresentationParameters.BackBufferFormat,
            DepthFormat.Depth24);
        
        // Load concept texture
        var conceptTexture = Content.Load<Texture2D>("graphics/concept");
        
        // Load bitmap font
        FontManager.Load(Content);
        
        _screenManager.ShowScreen(new ConceptScreen(this, _spriteBatch, conceptTexture));
        
        // Load FMOD music
        _audioSystem.LoadMusicBank();
        _audioSystem.PlayMusic("event:/EndOfDay");
        
        // Handle window focus changes
        Window.TextInput += OnTextInput;
        
        // Set window title
        Window.Title = "Fleasociety";
    }

    private void OnTextInput(object sender, TextInputEventArgs e)
    {
        // Handle text input if needed
    }

    protected override void Update(GameTime gameTime)
    {
        if (_pauseScreenCloseRequested)
        {
            _pauseScreenCloseRequested = false;
            if (_screenManager.ActiveScreen is PauseScreen)
            {
                _screenManager.CloseScreen();
            }
        }

        // Update input system
        _inputSystem.Update();
        
        // Check for fullscreen toggle
        if (_inputSystem.IsAltEnterPressed() || _inputSystem.IsF11Pressed())
        {
            ToggleFullscreen();
        }
        
        // Check for pause toggle (user-initiated)
        if (_inputSystem.IsEscapePressed() || _inputSystem.IsPPressed())
        {
            _pausedByUser = !_pausedByUser;
            if (_pausedByUser)
            {
                PauseGame();
            }
            else
            {
                ResumeGame();
            }
        }
        
        // Check for window focus changes
        bool isFocused = this.IsActive;
        if (isFocused != _wasFocused)
        {
            _wasFocused = isFocused;
            
            if (isFocused)
            {
                // Window regained focus - unpause if pause was due to focus loss
                if (_pausedByFocus)
                {
                    _pausedByFocus = false;
                    ResumeGame();
                }
            }
            else
            {
                // Window lost focus - pause the game
                // Only if not already paused by user
                if (!_pausedByUser)
                {
                    _pausedByFocus = true;
                    PauseGame();
                }
            }
        }
        
        // Update audio system with game time for lerping
        _audioSystem.Update(gameTime);
        
        // Exit on Back button
        if (_inputSystem.IsKeyDown(Keys.Escape) && _inputSystem.IsKeyDown(Keys.Back))
        {
            Exit();
        }
        
        // ScreenManager is a GameComponent and is updated by the base implementation.
        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        // Draw to render target at base resolution (960x540)
        GraphicsDevice.SetRenderTarget(_renderTarget);
        GraphicsDevice.Clear(Color.Black);
        
        // Draw managed screens to the fixed-resolution render target.
        base.Draw(gameTime);
        
        // Draw render target to screen with nearest neighbor scaling
        GraphicsDevice.SetRenderTarget(null);
        GraphicsDevice.Clear(Color.Black);
        
        var backBuffer = GraphicsDevice.Viewport;
        var destination = GetLetterboxedRectangle(backBuffer.Width, backBuffer.Height);
        if (destination.Width > 0 && destination.Height > 0)
        {
            _spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp);
            _spriteBatch.Draw(_renderTarget, destination, Color.White);
            _spriteBatch.End();
        }
    }

    private static Rectangle GetLetterboxedRectangle(int width, int height)
    {
        if (width <= 0 || height <= 0)
        {
            return Rectangle.Empty;
        }

        int scaledWidth;
        int scaledHeight;
        if ((long)width * BaseHeight > (long)height * BaseWidth)
        {
            scaledHeight = height;
            scaledWidth = (int)((long)height * BaseWidth / BaseHeight);
        }
        else
        {
            scaledWidth = width;
            scaledHeight = (int)((long)width * BaseHeight / BaseWidth);
        }

        return new Rectangle(
            (width - scaledWidth) / 2,
            (height - scaledHeight) / 2,
            scaledWidth,
            scaledHeight);
    }

    private void ToggleFullscreen()
    {
        _graphics.IsFullScreen = !_graphics.IsFullScreen;
        if (_graphics.IsFullScreen)
        {
            _graphics.PreferredBackBufferWidth = 1920;
            _graphics.PreferredBackBufferHeight = 1080;
        }
        else
        {
            _graphics.PreferredBackBufferWidth = 1280;
            _graphics.PreferredBackBufferHeight = 720;
        }

        _graphics.ApplyChanges();
    }

    private void PauseGame()
    {
        if (_screenManager.ActiveScreen is PauseScreen existingPauseScreen)
        {
            if (existingPauseScreen.IsFadingOut)
            {
                _pauseScreenCloseRequested = false;
                existingPauseScreen.Show();
                _audioSystem.PauseMusic(0.5f);
            }
            return;
        }

        if (_screenManager.ActiveScreen == null)
        {
            return;
        }

        var pauseScreen = new PauseScreen(this, _spriteBatch);
        pauseScreen.FadeOutCompleted += () => _pauseScreenCloseRequested = true;
        _screenManager.ShowScreen(pauseScreen);
        _audioSystem.PauseMusic(0.5f);
    }

    private void ResumeGame()
    {
        if (_screenManager.ActiveScreen is PauseScreen pauseScreen && !pauseScreen.IsFadingOut)
        {
            pauseScreen.Hide();
            _audioSystem.UnpauseMusic(0.5f);
        }
    }

    private void OnExiting(object sender, EventArgs args)
    {
        // Clean up systems
        _screenManager.ClearScreens();
        GlobalTextures.Dispose();
        _audioSystem?.Dispose();
        _renderTarget?.Dispose();
    }
}
