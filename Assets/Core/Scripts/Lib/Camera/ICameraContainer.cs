using System.Collections.Generic;

namespace Lib.Camera
{
    public interface ICameraContainer
    {
        void AddCamera(ICameraView cameraView);
        void RemoveCamera(ICameraView cameraView);
        ICameraView GetCameraByName(string cameraName);
        CmCameraModel GetCameraModelByName(string cameraName);
        IEnumerable<ICameraView> GetAllCameras();
    }
}