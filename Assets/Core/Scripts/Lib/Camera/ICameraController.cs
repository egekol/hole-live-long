namespace Lib.Camera
{
    public interface ICameraController
    {
        void Init(ICameraManager cameraManager);
        ICameraManager GetManager();
        void SwitchCamera(string cameraName, string blendType, float duration = -1f);
        void SetTransitionMode(string blendType, float duration = -1f);
    }
}