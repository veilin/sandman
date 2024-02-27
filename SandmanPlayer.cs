using SDL2;

namespace Sandman;

class SandmanPlayer
{
    private readonly IntPtr _renderer;
    private readonly SpriteSheet _spriteSheet;
    private bool _directionLeft;
    private int _x;
    private int _y;
    private readonly int _walkSpeed = 2;
    private readonly int _fallSpeed = 2;
    private readonly int _w;
    private readonly int _h;
    private int _jumpHeight;
    private const int MaxJumpHeight = 100;
    private bool _onGround;

    public SandmanPlayer(IntPtr renderer, int x, int y)
    {
        _renderer = renderer;
        _x = x;
        _y = y;
        _w = 32;
        _h = 64;

        _spriteSheet = new SpriteSheet(_renderer, "assets/skeletonBase.png", _w, _h, 320, 640,
            new Dictionary<string, AnimationSequence>
            {
                { "Idle", new AnimationSequence(new[] { 0 }, new[] { 100 }) },
                { "Walk", new AnimationSequence(new[] { 1, 2, 3, 4, 5, 6 }, new[] { 100, 100, 100, 100, 100, 100 }) },
                { "Falling", new AnimationSequence(new[] { 51 }, new[] { 100 }) },
                { "Jumping", new AnimationSequence(new[] { 17, 18, 19 }, new[] { 100, 100, 100 }) }
            }
        );
    }

    public void Render()
    {
        _spriteSheet.Render(_x, _y, _directionLeft);
    }

    private void MoveLeft()
    {
        _directionLeft = true;
        _spriteSheet.CurrentAnimation = "Walk";
        _x -= _walkSpeed;
        if (_x < 0) _x = 0;
    }

    private void MoveRight()
    {
        _directionLeft = false;
        _spriteSheet.CurrentAnimation = "Walk";
        _x += _walkSpeed;
    }
    
    private void Jump()
    {
        if (!_onGround) return;
        _jumpHeight = MaxJumpHeight;
        
    }

    public void Tick(int[,] world, int worldWidth, int worldHeight)
    {
        ProcessGravity(world, worldWidth, worldHeight);
    }

    private void ProcessGravity(int[,] world, int worldWidth, int worldHeight)
    {
        if (_jumpHeight >= _fallSpeed)
        {
            for (int i = 1; i <= _fallSpeed; i++)
            {
                if (_y - i > 0 && world[_y - i, _x + (_w / 2)] == 0)
                {
                    _y--;
                    _jumpHeight--;
                    _spriteSheet.CurrentAnimation = "Jumping";
                    _onGround = false;
                }
            }
            return;
        } 
        
        for (int i = 1; i <= _fallSpeed; i++)
        {
            if (_y + _h + i < worldHeight && world[_y + _h + i, _x + (_w / 2)] == 0)
            {
                _y++;
                _spriteSheet.CurrentAnimation = "Falling";
                _onGround = false;
            }
            else
            {
                _onGround = true;
            }
        }
    }

    public void HandleInputEvents(InputManager inputManager)
    {
        if (inputManager.KeyDown(SDL.SDL_Scancode.SDL_SCANCODE_SPACE))
        {
            Jump();
        }
        
        if (inputManager.KeyDown(SDL.SDL_Scancode.SDL_SCANCODE_A))
        {
            MoveLeft();
            return;
        }

        if (inputManager.KeyDown(SDL.SDL_Scancode.SDL_SCANCODE_D))
        {
            MoveRight();
            return;
        }
        
        _spriteSheet.CurrentAnimation = "Idle";
    }
}