using Lib.SceneLoad;

namespace Core.Scripts.Gameplay.Managers
{
    public class SceneController : SceneControllerBase
    {
        public override void Init(ISceneSettings sceneSettings)
        {
            SceneSettings = sceneSettings;
        }
    }
}