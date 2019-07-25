using UnityEngine;
using UnityEngine.UI;

namespace BallBlaster
{

    public class CoinsCounter : MonoBehaviour
    {
        [SerializeField] private Player _player;

        [Header("- Text -")]
        [SerializeField] private Text _coinsText;

        [Header("- Picked UP coin effect -")]
        [SerializeField] private Transform _parentCanvas;
        [SerializeField] private GameObject _pickedUpCoinTextPrefab;
        [SerializeField] private Vector3 _pickedCoinTextOffset;

        #region MonoBehaviour

        private void OnValidate()
        {
            if (_parentCanvas != null && _parentCanvas.GetComponent<Canvas>() == null)
            {
                Debug.LogError("CoinsCounter: ParentCanvas has to have Canvas component.");
                _parentCanvas = null;
            }

            if (_pickedUpCoinTextPrefab != null && (_pickedUpCoinTextPrefab.GetComponentInChildren<Text>() == null))
            {
                Debug.LogError("CoinsCounter: PickedUpTextPrefab has to have Text component.");
                _pickedUpCoinTextPrefab = null;
            }
        }

        private void Start()
        {
            // Subscribe to player event when it picks up a coin
            _player.CointPickedUpEvent += OnCoinPickedUp;

            UpdateCoinCountText();
        }

        #endregion

        private void OnCoinPickedUp(Coin coin)
        {
            // Updates coin data
            Data.SetCoinsCount(Data.GetCoinsCount() + coin.Value);
            UpdateCoinCountText();

            // Spawn textObject
            GameObject textObject = Instantiate(_pickedUpCoinTextPrefab, coin.transform.position + _pickedCoinTextOffset, _pickedUpCoinTextPrefab.transform.rotation, _parentCanvas);
            textObject.GetComponentInChildren<Text>().text = coin.Value.ToString();
            Destroy(textObject, 2f);
        }

        private void UpdateCoinCountText()
        {
            _coinsText.text = NumberFormatter.ToKMB(Data.GetCoinsCount());
        }

    }
}