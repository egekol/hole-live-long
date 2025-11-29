namespace Lib.Camera
{
    public class CmCameraModel
    {
        public CmCameraModel(string cameraName)
        {
            CameraName = cameraName;
            Priority = 0;
            FieldOfView = 60f;
        }

        public int Priority { get; set; }
        public float FieldOfView { get; set; }
        public string CameraName { get; set; }
    }
}