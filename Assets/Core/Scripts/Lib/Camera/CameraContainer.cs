using System.Collections.Generic;
using Lib.Debugger;

namespace Lib.Camera
{
    public class CameraContainer : ICameraContainer
    {
        private readonly Dictionary<string, ICameraView> _viewMap = new();
        private readonly Dictionary<string, CmCameraModel> _modelMap = new();
        
        public void AddCamera(ICameraView cameraView)
        {
            if (cameraView == null || cameraView.CameraViewData == null || string.IsNullOrEmpty(cameraView.CameraViewData.CameraName))
            {
                return;
            }

            var tryAdd = _viewMap.TryAdd(cameraView.CameraViewData.CameraName, cameraView);
            if (!tryAdd)
            {
                LogHelper.LogError($"CameraContainer AddCamera failed: {cameraView.CameraViewData.CameraName} already exists");
                return;
            }
            var cmCameraModel = new CmCameraModel(cameraView.CameraViewData.CameraName);
            var tryAddModel = _modelMap.TryAdd(cameraView.CameraViewData.CameraName, cmCameraModel);
            if (!tryAddModel)
            {
                LogHelper.LogError($"CameraContainer AddCamera model failed: {cameraView.CameraViewData.CameraName} already exists");
            }
        }

        public void RemoveCamera(ICameraView cameraView)
        {
            if (cameraView == null || cameraView.CameraViewData == null || string.IsNullOrEmpty(cameraView.CameraViewData.CameraName))
            {
                return;
            }

            var tryRemove = _viewMap.Remove(cameraView.CameraViewData.CameraName);
            if (!tryRemove)
            {
                LogHelper.LogError($"CameraContainer RemoveCamera failed: {cameraView.CameraViewData.CameraName} not found");
                return;
            }
            
            var tryRemoveModel = _modelMap.Remove(cameraView.CameraViewData.CameraName);
            if (!tryRemoveModel)
            {
                LogHelper.LogError($"CameraContainer RemoveCamera model failed: {cameraView.CameraViewData.CameraName} not found");
            }
        }
        
        public ICameraView GetCameraByName(string cameraName)
        {
            if (string.IsNullOrEmpty(cameraName))
            {
                return null;
            }

            _viewMap.TryGetValue(cameraName, out var cameraView);
            return cameraView;
        }
        
        public CmCameraModel GetCameraModelByName(string cameraName)
        {
            if (string.IsNullOrEmpty(cameraName))
            {
                return null;
            }

            _modelMap.TryGetValue(cameraName, out var cameraModel);
            return cameraModel;
        }

        public IEnumerable<ICameraView> GetAllCameras()
        {
            return _viewMap.Values;
        }
    }
}