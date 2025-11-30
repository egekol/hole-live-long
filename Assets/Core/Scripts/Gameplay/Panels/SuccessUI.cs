using Core.Scripts.Gameplay.Levels;
using Core.Scripts.Gameplay.Managers;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Core.Scripts.Gameplay.Panels
{
    public class SuccessUI : MonoBehaviour
    {
        [SerializeField] private Button nextLevelButton;
        [SerializeField] private Button _replyButton;
        [SerializeField] private TextMeshProUGUI _energyCountTMP;
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private Transform _starOne;
        [SerializeField] private Transform _starTwo;
        [SerializeField] private Transform _starThree;
        
        
        private void OnEnable()
        {
            nextLevelButton.onClick.AddListener(OnNextLevelClicked);
            _replyButton.onClick.AddListener(Reply);
        }

        private void Reply()
        {
            GameSettings.Instance.RestartCompletedLevel().Forget();
        }

        private void OnDisable()
        {
            nextLevelButton.onClick.RemoveListener(OnNextLevelClicked);
            _replyButton.onClick.RemoveListener(Reply);
        }

        private void OnNextLevelClicked()
        {
            GameSettings.Instance.LoadNextLevel().Forget();
        }

        public void Show()
        {
            UpdateStars();
            
            _canvasGroup.alpha = 0;
            gameObject.SetActive(true);
            _canvasGroup.DOFade(1f, 0.5f).SetLink(gameObject);
        }

        private void UpdateStars()
        {
            var levelModel = LevelManager.Instance.LevelModel;
            var levelData = levelModel.LevelData;
            var remainingMoves = levelModel.RemainingMoveCount;

            _starOne.gameObject.SetActive(remainingMoves >= levelData.OneStarMinMoves);
            _starTwo.gameObject.SetActive(remainingMoves >= levelData.TwoStarMinMoves);

            // 3. yıldız: Eğer level'da collectable varsa, tümü toplanmadan 3 yıldız verilmeyecek
            var collectedCollectableCount = LevelManager.Instance.CollectedCollectableCount;
            int totalCollectableCount = 0;
            if (levelData.Tiles != null)
            {
                for (int i = 0; i < levelData.Tiles.Length; i++)
                {
                    if (levelData.Tiles[i].Type == TileType.Collectable)
                    {
                        totalCollectableCount++;
                    }
                }
            }

            bool hasCollectables = totalCollectableCount > 0;
            bool hasEnoughMovesForThreeStars = remainingMoves >= levelData.ThreeStarMinMoves;
            bool collectedAllCollectables = !hasCollectables || collectedCollectableCount >= totalCollectableCount;

            _starThree.gameObject.SetActive(hasEnoughMovesForThreeStars && collectedAllCollectables);
            
            // Enerji (collectable) sayısını güncelle
            _energyCountTMP.text = $"{collectedCollectableCount}";
        }
    }
}