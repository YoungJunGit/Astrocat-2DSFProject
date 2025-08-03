using System;
using static InputHandler;

public class InputDisposer : IDisposable
{
    private InputHandler _inputHandler;
    private InputState _originInputState;
    
    public InputDisposer(InputHandler inputHandler, InputState inputState)
    {
        _inputHandler = inputHandler;
        
        _originInputState = inputHandler.CurrentInputState;
        inputHandler.CurrentInputState = inputState;
    }

    public void Dispose()
    {
        _inputHandler.CurrentInputState = _originInputState;
    }
}