using System.Runtime.InteropServices;
using SDL2;

namespace Sandman;

public class InputManager
{
    private readonly byte[] _keyState = new byte[(int)SDL.SDL_Scancode.SDL_NUM_SCANCODES];
    private int _x;
    private int _y;
    private uint _mouseButtonState;

    public MouseState MouseState => new(_x, _y, SDL.SDL_BUTTON(1) == _mouseButtonState,
        SDL.SDL_BUTTON(2) == _mouseButtonState, SDL.SDL_BUTTON(3) == _mouseButtonState);

    public void ProcessEvents()
    {
        Marshal.Copy(SDL.SDL_GetKeyboardState(out var numKeys), _keyState, 0, numKeys);
        _mouseButtonState = SDL.SDL_GetMouseState(out _x, out _y);
    }

    public bool KeyDown(SDL.SDL_Scancode scancode)
    {
        return _keyState[(int)scancode] == 1;
    }
}

public record MouseState(int X, int Y, bool Left, bool Middle, bool Right);