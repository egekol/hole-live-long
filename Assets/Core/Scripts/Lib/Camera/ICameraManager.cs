namespace Lib.Camera
{
    public interface ICameraManager
    {
        void SetTransitionMode(string blendType, float duration);
        void SetTransitionMode(string blendType);
    }
}