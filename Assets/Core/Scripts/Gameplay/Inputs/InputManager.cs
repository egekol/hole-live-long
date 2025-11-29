using System;
using Core.Scripts.Lib.Utility;
using Lib.Debugger;
using Unity.Mathematics;

namespace Core.Scripts.Gameplay.Inputs
{
    public class InputManager : Singleton<InputManager>
    {

        public InputState InputState { get; private set; } = InputState.Scrolling;
        public InputDirection InputDirection { get; set; }
        
        public void OnScrollUpdated(float2 scrollPosition)
        {
            if (InputState == InputState.Disabled)
            {
                InputDirection = InputDirection.None;
                return;
            }
            
            InputState = InputState.Scrolling;
            if (math.abs(scrollPosition.x) > math.abs(scrollPosition.y))
            {
                InputDirection = scrollPosition.x > 0 ? InputDirection.Right : InputDirection.Left;
            }
            else if (math.abs(scrollPosition.y) > math.abs(scrollPosition.x))
            {
                InputDirection = scrollPosition.y > 0 ? InputDirection.Up : InputDirection.Down;
            }
            else
            {
                InputDirection = InputDirection.None;
            }
            LogHelper.Log($"Input Direction: {InputDirection} - Scroll Position: {scrollPosition}");
        }
    }
    
    
    public enum InputDirection
    {
        None,
        Up,
        Down,
        Left,
        Right
    }
    
    public enum InputState
    {
        None,
        Scrolling,
        Disabled,
    }
}