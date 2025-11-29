using Core.Scripts.Gameplay.Managers;
using UnityEngine;

namespace Core.Scripts.Gameplay.Injection
{
    public class GameContext : MonoBehaviour
    {
        private void Start()
        {
            GameSettings.Instance.InitializeGameBindings();
        }
    }
}