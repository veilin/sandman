using SDL2;

namespace Sandman;

public record AnimationSequence(int[] Frames, int[] Timings);

public class SpriteSheet
{
    private IntPtr _texture;
    private readonly IntPtr _renderer;
    private readonly string _filePath;
    private readonly int _w;
    private readonly int _h;
    private readonly int _imageWidth;
    private readonly int _imageHeight;
    private readonly Dictionary<string, AnimationSequence> _animations;
    private DateTime _frameTimer = DateTime.UtcNow;
    private int _currentFrameIndex;
    private string _currentAnimation;

    public string CurrentAnimation
    {
        get => _currentAnimation;
        set
        {
            if (_currentAnimation == value)
                return;
            _frameTimer = DateTime.UtcNow;
            _currentFrameIndex = 0;
            _currentAnimation = value;
        }
    }

    public SpriteSheet(IntPtr renderer, string filePath, int w, int h, int imageWidth, int imageHeight,
        Dictionary<string, AnimationSequence> animations)
    {
        _renderer = renderer;
        _filePath = filePath;
        _w = w;
        _h = h;
        _imageWidth = imageWidth;
        _imageHeight = imageHeight;
        _animations = animations;
        _currentAnimation = _animations.Keys.First();
        LoadSpriteSheet();
    }

    private void LoadSpriteSheet()
    {
        IntPtr surface = SDL_image.IMG_Load(_filePath);

        if (surface == IntPtr.Zero)
        {
            // Handle error.
            Console.WriteLine("Unable to load spritesheet: " + SDL.SDL_GetError());
            return;
        }

        // Create a texture from the surface.
        _texture = SDL.SDL_CreateTextureFromSurface(_renderer, surface);

        // Free the surface as it's no longer needed.
        SDL.SDL_FreeSurface(surface);
    }

    public void Render(int screenX, int screenY, bool flip)
    {
        _animations.TryGetValue(CurrentAnimation, out var animationSequence);

        if (animationSequence == null) return;

        var now = DateTime.UtcNow;
        if (_frameTimer.AddMilliseconds(animationSequence.Timings[_currentFrameIndex]) <= now )
        {
            _frameTimer = now;
            _currentFrameIndex++;
            if (_currentFrameIndex >= animationSequence.Frames.Length)
                _currentFrameIndex = 0;
        }

        int spriteIndex = animationSequence.Frames[_currentFrameIndex];

        // Define the source rectangle (the sprite in the spritesheet).
        var srcRect = new SDL.SDL_Rect()
        {
            x = (spriteIndex * _w) % _imageWidth,
            y = ((spriteIndex * _w) / _imageWidth) * _h, 
            w = _w, 
            h = _h
        };

        // Define the destination rectangle (where on the screen to draw the sprite).
        var destRect = new SDL.SDL_Rect() { x = screenX, y = screenY, w = _w, h = _h };

        var center = new SDL.SDL_Point() { x = _w / 2, y = 0};

        SDL.SDL_RenderCopyEx(_renderer, _texture,
            ref srcRect,
            ref destRect,
            0,
            ref center,
            flip ? SDL.SDL_RendererFlip.SDL_FLIP_HORIZONTAL : SDL.SDL_RendererFlip.SDL_FLIP_NONE);
    }
}

