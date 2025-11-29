using Core.Scripts.Gameplay.Levels;
using Core.Scripts.Lib.Utility;

namespace Core.Scripts.Gameplay.Managers
{
    public enum MovementState
    {
        None,
        Swiping,
        Idle,
    }

    public class MovementManager : Singleton<MovementManager>
    {
        private LevelModel _levelModel;
        private ILevelGenerator _levelGenerator;

        public MovementState MovementState { get; set; } = MovementState.Idle;

        public void Initialize(LevelModel levelModel, ILevelGenerator levelGenerator)
        {
            _levelModel = levelModel;
            _levelGenerator = levelGenerator;
        }

        public void Move()
        {
            
        }
    }
}