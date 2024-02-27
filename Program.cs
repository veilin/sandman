using Sandman;
using SDL2;

IntPtr window;
IntPtr renderer;
bool running = true;
int w = 1280;
int h = 768;

Engine engine;
InputManager inputManager = new InputManager();

Setup();

while (running)
{
    PollEvents();
    engine.HandleInputEvents(inputManager);
    engine.Tick();
    Render();
}

CleanUp();

void Setup() 
{
    // Initilizes SDL.
    if (SDL.SDL_Init(SDL.SDL_INIT_VIDEO) < 0)
    {
        Console.WriteLine($"There was an issue initializing SDL. {SDL.SDL_GetError()}");
    }

    // Create a new window given a title, size, and passes it a flag indicating it should be shown.
    window = SDL.SDL_CreateWindow(
        "Sandman",
        SDL.SDL_WINDOWPOS_UNDEFINED, 
        SDL.SDL_WINDOWPOS_UNDEFINED, 
        w, 
        h, 
        SDL.SDL_WindowFlags.SDL_WINDOW_SHOWN);

    if (window == IntPtr.Zero)
    {
        Console.WriteLine($"There was an issue creating the window. {SDL.SDL_GetError()}");
    }

    // Creates a new SDL hardware renderer using the default graphics device with VSYNC enabled.
    renderer = SDL.SDL_CreateRenderer(
        window,
        -1,
        SDL.SDL_RendererFlags.SDL_RENDERER_ACCELERATED);

    if (renderer == IntPtr.Zero)
    {
        Console.WriteLine($"There was an issue creating the renderer. {SDL.SDL_GetError()}");
    }
    
    var ttfInitError = SDL_ttf.TTF_Init();
    if (ttfInitError != IntPtr.Zero)
    {
        Console.WriteLine($"There was an issue initializing the ttf library. {SDL.SDL_GetError()}");
    }

    engine = new Engine(renderer, w, h);
    engine.Setup(); 
}

void PollEvents()
{
    // Check to see if there are any events and continue to do so until the queue is empty.
    while (SDL.SDL_PollEvent(out SDL.SDL_Event e) == 1)
    {
        switch (e.type)
        {
            case SDL.SDL_EventType.SDL_KEYUP:
                if (e.key.keysym.scancode == SDL.SDL_Scancode.SDL_SCANCODE_ESCAPE)
                {
                    running = false;
                }
                break;
            case SDL.SDL_EventType.SDL_QUIT:
                running = false;
                break;
            case SDL.SDL_EventType.SDL_MOUSEMOTION:
                if (e.motion.state == SDL.SDL_BUTTON_LMASK)
                {
                    engine.HandleMouseClickEvent(e.motion.x, e.motion.y);
                }
                break;
            case SDL.SDL_EventType.SDL_MOUSEBUTTONDOWN:
                engine.HandleMouseClickEvent(e.motion.x, e.motion.y);
                break;
        }
    }

    inputManager.ProcessEvents();
}

void Render()
{
    // Sets the color that the screen will be cleared with.
    SDL.SDL_SetRenderDrawColor(renderer, 5, 5, 5, 255);
    SDL.SDL_SetRenderDrawBlendMode(renderer, SDL.SDL_BlendMode.SDL_BLENDMODE_BLEND);
    
    // Clears the current render surface.
    SDL.SDL_RenderClear(renderer);

    engine.Render();
    
    // Switches out the currently presented render surface with the one we just did work on.
    SDL.SDL_RenderPresent(renderer);
}

void CleanUp()
{
    SDL.SDL_DestroyRenderer(renderer);
    SDL.SDL_DestroyWindow(window);
    SDL.SDL_Quit();
}