using System.Runtime.InteropServices;
using SDL2;

namespace Sandman;

public class Engine
{
    private readonly IntPtr _renderer;
    private readonly int _w;
    private readonly int _h;
    private readonly int[,] _world;
    private IntPtr _worldTexture;
    private readonly SandmanPlayer _sandman;

    public Engine(IntPtr renderer, int w, int h)
    {
        _renderer = renderer;
        _w = w;
        _h = h;

        _world = new int[h, w];
        _sandman = new SandmanPlayer(renderer, w / 2, h / 2);
    }

    public void Setup()
    {
        _worldTexture = SDL.SDL_CreateTexture(
            _renderer,
            SDL.SDL_PIXELFORMAT_ARGB8888,
            (int)SDL.SDL_TextureAccess.SDL_TEXTUREACCESS_STREAMING,
            _w,
            _h
        );
    }

    public void Tick()
    {
        _sandman.Tick(_world, _w, _h);

        var random = Random.Shared.Next(2);

        for (int y = _h - 1; y > 1; y--)
        {
            for (int x = 0; x < _w; x++)
            {
                if (_world[y - 1, x] != 0)
                {
                    if (_world[y, x] == 0)
                    {
                        _world[y, x] = _world[y - 1, x];
                        _world[y - 1, x] = 0;
                    }

                    if (random == 0 && x - 1 > 0 && _world[y, x - 1] == 0)
                    {
                        _world[y, x - 1] = _world[y - 1, x];
                        _world[y - 1, x] = 0;
                    }

                    if (random == 1 && x + 1 < _w && _world[y, x + 1] == 0)
                    {
                        _world[y, x + 1] = _world[y - 1, x];
                        _world[y - 1, x] = 0;
                    }
                }
            }
        }
    }

    public void Render()
    {
        SDL.SDL_LockTexture(_worldTexture, IntPtr.Zero, out IntPtr pixels, out int pitch);

        for (int i = 0; i < _w * _h; i++)
        {
            var y = i / _w;
            var x = i % _w;

            Marshal.WriteInt32(pixels, i * 4, _world[y, x]);
        }

        // Unlock the texture.
        SDL.SDL_UnlockTexture(_worldTexture);

        // Render the texture to the screen.
        SDL.SDL_RenderCopy(_renderer, _worldTexture, IntPtr.Zero, IntPtr.Zero);

        _sandman.Render();
    }

    public void HandleMouseClickEvent(int mouseX, int mouseY)
    {
    }

    public void HandleInputEvents(InputManager inputManager)
    {
        _sandman.HandleInputEvents(inputManager);

        var mouseState = inputManager.MouseState;

        if (mouseState.Left)
        {
            PlaceSand(mouseState.X, mouseState.Y);
        }
    }

    private void PlaceSand(int x, int y)
    {
        if (x < 1 || x >= _w - 1 || y < 1 || y >= _h - 1)
            return;

        _world[y, x] = 0xCDAA7F;
        _world[y, x - 1] = 0xCDAA7F;
        _world[y, x + 1] = 0xCDAA7F;

        _world[y - 1, x - 1] = 0xCDAA7F;
        _world[y - 1, x + 1] = 0xCDAA7F;
    }
}