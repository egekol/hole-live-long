namespace Lib.Camera
{
    public interface ICameraView
    {
        CameraViewData CameraViewData { get; }
        void SetPriority(int priority);
        void SetActive(bool isActive);
    }
}