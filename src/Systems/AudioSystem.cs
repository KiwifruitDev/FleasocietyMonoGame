// Fleasociety (MonoGame)
// Copyright (c) 2026 KiwifruitDev
// Licensed under the MIT License
// View LICENSE before reviewing source code

using System;
using FMOD;
using FMODStudio = FMOD.Studio;
using Microsoft.Xna.Framework;

namespace Fleasociety.Systems;

/// <summary>
/// Manages FMOD audio system and provides easy access to audio functionality
/// </summary>
public class AudioSystem : IDisposable
{
    private FMODStudio.System _system;
    private readonly string _bankPath;
    
    /// <summary>
    /// Music bank
    /// </summary>
    private FMODStudio.Bank _musicBank;
    
    /// <summary>
    /// Music event instance
    /// </summary>
    private FMODStudio.EventInstance _musicEvent;
    
    /// <summary>
    /// Master volume (0.0 to 1.0)
    /// </summary>
    public float MasterVolume { get; set; } = 0.5f;
    
    /// <summary>
    /// Whether audio is initialized
    /// </summary>
    public bool IsInitialized => _system.handle != IntPtr.Zero;
    
    /// <summary>
    /// Whether music is playing
    /// </summary>
    public bool IsMusicPlaying => _musicEvent.handle != IntPtr.Zero;
    
    /// <summary>
    /// Whether music is paused
    /// </summary>
    public bool IsMusicPaused { get; private set; }
    
    /// <summary>
    /// Original volume before pause
    /// </summary>
    private float _originalVolume = 1.0f;
    
    /// <summary>
    /// Current target volume for lerping
    /// </summary>
    private float _targetVolume = 1.0f;
    
    /// <summary>
    /// Whether we are currently lerping the volume
    /// </summary>
    private bool _isLerping = false;
    
    /// <summary>
    /// Current lerp progress (0.0 to 1.0)
    /// </summary>
    private float _lerpProgress = 0f;
    
    /// <summary>
    /// Total lerp duration in seconds
    /// </summary>
    private float _lerpDuration = 0.5f;
    
    /// <summary>
    /// Start volume for the current lerp
    /// </summary>
    private float _lerpStartVolume = 1.0f;

    public AudioSystem(string bankPath = "content/audio/Music.bank")
    {
        _bankPath = bankPath;
    }

    /// <summary>
    /// Initialize the FMOD system
    /// </summary>
    public void Initialize()
    {
        if (_system.handle != IntPtr.Zero)
            return;
        
        FMODStudio.System.create(out _system);
        _system.initialize(512, FMODStudio.INITFLAGS.NORMAL, FMOD.INITFLAGS.NORMAL, IntPtr.Zero);
    }

    /// <summary>
    /// Load the music bank
    /// </summary>
    public bool LoadMusicBank()
    {
        if (_system.handle == IntPtr.Zero)
            return false;
        
        try
        {
            // Load strings bank first
            _system.loadBankFile("content/audio/Music.strings.bank", FMODStudio.LOAD_BANK_FLAGS.NORMAL, out _);
            
            // Load music bank
            var result = _system.loadBankFile(_bankPath, FMODStudio.LOAD_BANK_FLAGS.NORMAL, out _musicBank);
            if (result == FMOD.RESULT.OK)
            {
                _musicBank.loadSampleData();
                return true;
            }
        }
        catch
        {
            return false;
        }
        
        return false;
    }

    /// <summary>
    /// Start playing music event
    /// </summary>
    public bool PlayMusic(string eventPath = "event:/EndOfDay")
    {
        if (_system.handle == IntPtr.Zero || _musicBank.handle == IntPtr.Zero)
            return false;
        
        try
        {
            _system.getEvent(eventPath, out FMODStudio.EventDescription eventDescription);
            eventDescription.createInstance(out _musicEvent);
            _musicEvent.start();
            _musicEvent.setVolume(MasterVolume);
            IsMusicPaused = false;
            return true;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Stop music
    /// </summary>
    public void StopMusic()
    {
        if (_musicEvent.handle != IntPtr.Zero)
        {
            _musicEvent.stop(FMODStudio.STOP_MODE.IMMEDIATE);
            _musicEvent.release();
            _musicEvent = default;
        }
    }

    /// <summary>
    /// Pause music with smooth fade to 0.75 using lerp
    /// </summary>
    public void PauseMusic(float fadeDuration = 0.5f)
    {
        if (_musicEvent.handle == IntPtr.Zero)
            return;
        
        IsMusicPaused = true;
        _originalVolume = MasterVolume;
        _lerpDuration = fadeDuration;
        _lerpStartVolume = MasterVolume;
        _targetVolume = 0.25f;
        _lerpProgress = 0f;
        _isLerping = true;
    }

    /// <summary>
    /// Unpause music with smooth fade in using lerp
    /// </summary>
    public void UnpauseMusic(float fadeDuration = 0.5f)
    {
        if (_musicEvent.handle == IntPtr.Zero)
            return;
        
        IsMusicPaused = false;
        _lerpDuration = fadeDuration;
        _lerpStartVolume = MasterVolume;
        _targetVolume = _originalVolume;
        _lerpProgress = 0f;
        _isLerping = true;
    }

    /// <summary>
    /// Set music volume
    /// </summary>
    public void SetMusicVolume(float volume)
    {
        MasterVolume = Math.Clamp(volume, 0.0f, 1.0f);
        if (_musicEvent.handle != IntPtr.Zero)
        {
            _musicEvent.setVolume(MasterVolume);
        }
    }

    /// <summary>
    /// Update the FMOD system and handle volume lerping
    /// </summary>
    public void Update(GameTime gameTime)
    {
        if (_system.handle != IntPtr.Zero)
        {
            _system.update();
        }
        
        // Update volume lerping
        if (_isLerping && _musicEvent.handle != IntPtr.Zero)
        {
            _lerpProgress += (float)(gameTime.ElapsedGameTime.TotalSeconds / _lerpDuration);
            _lerpProgress = MathHelper.Clamp(_lerpProgress, 0f, 1f);
            
            // Use MathHelper.Lerp for smooth interpolation
            MasterVolume = MathHelper.Lerp(_lerpStartVolume, _targetVolume, _lerpProgress);
            _musicEvent.setVolume(MasterVolume);
            
            // Check if lerp is complete
            if (_lerpProgress >= 1f)
            {
                _isLerping = false;
                MasterVolume = _targetVolume;
            }
        }
    }
    
    /// <summary>
    /// Update the FMOD system without game time (for backwards compatibility)
    /// </summary>
    public void Update()
    {
        Update(new GameTime());
    }

    /// <summary>
    /// Toggle pause state
    /// </summary>
    public void TogglePause()
    {
        if (IsMusicPaused)
        {
            UnpauseMusic();
        }
        else
        {
            PauseMusic();
        }
    }

    public void Dispose()
    {
        try
        {
            StopMusic();
            if (_musicBank.handle != IntPtr.Zero)
            {
                _musicBank.unload();
            }
            if (_system.handle != IntPtr.Zero)
            {
                _system.release();
            }
        }
        catch
        {
            // Ignore errors during cleanup
        }
    }
}
