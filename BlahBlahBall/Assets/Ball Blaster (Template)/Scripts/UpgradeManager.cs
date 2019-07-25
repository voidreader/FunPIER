using UnityEngine;
using UnityEngine.UI;

namespace BallBlaster
{

    public class UpgradeManager : MonoBehaviour
    {

        [Header("- Tabs -")]
        [SerializeField] private RectTransform _speedTabObject;
        [SerializeField] private RectTransform _powerTabObject;

        [Header("- Texts -")]
        [SerializeField] private Text _coinsCountText;
        [SerializeField] private Text _tabNameText;
        [SerializeField] private Text _currentUpgradeValue;
        [SerializeField] private Text _upgradePriceText;

        [Header("- Prices -")]
        [SerializeField] private int _shootSpeedPrice = 50;
        [SerializeField] private int _shootPowerPrice = 100;

        [Header("- Increase -")]
        [SerializeField] private float _increaseSpeedValue = 0.0005f;
        [SerializeField] private int _increasePowerValue = 1;

        [Header("- Sound -")]
        [SerializeField] private AudioSource _audioSource;
        [SerializeField] private AudioClip _upgradeClip;
        [SerializeField] private AudioClip _chooseTabClip;

        private bool _isSpeedTabActive;
        private bool _isPowerTabActive;

        private void Start()
        {
            ActiveSpeedTab();
        }

        public void UdgradeAbility()
        {
            if (_isSpeedTabActive && Data.GetCoinsCount() >= _shootSpeedPrice)
            {
                UpgradeSpeedAbility();
            }
            else if (_isPowerTabActive && Data.GetCoinsCount() >= _shootPowerPrice)
            {
                UpgradePowerAbility();
            }
        }

        private void UpgradeSpeedAbility()
        {
            PlaySound(_upgradeClip);

            Data.SetShootSpeed(Data.GetShootSpeed() - _increaseSpeedValue);
            Data.SetCoinsCount(Data.GetCoinsCount() - _shootSpeedPrice);

            UpdateSpeedText();
            UpdateCoinsText();
        }

        private void UpgradePowerAbility()
        {
            PlaySound(_upgradeClip);

            Data.SetShootPower(Data.GetShootPower() + _increasePowerValue);
            Data.SetCoinsCount(Data.GetCoinsCount() - _shootPowerPrice);

            UpdatePowerText();
            UpdateCoinsText();
        }

        public void ActiveSpeedTab()
        {
            PlaySound(_chooseTabClip);

            _powerTabObject.SetAsFirstSibling();

            _isSpeedTabActive = true;
            _isPowerTabActive = false;

            _tabNameText.text = "Fire speed";
            _upgradePriceText.text = _shootSpeedPrice.ToString();

            UpdateSpeedText();
            UpdateCoinsText();
        }

        public void ActivePowerTab()
        {
            PlaySound(_chooseTabClip);

            _speedTabObject.SetAsFirstSibling();

            _isPowerTabActive = true;
            _isSpeedTabActive = false;

            _tabNameText.text = "Fire power";
            _upgradePriceText.text = _shootPowerPrice.ToString();

            UpdatePowerText();
            UpdateCoinsText();
        }

        private void UpdateCoinsText()
        {
            _coinsCountText.text = NumberFormatter.ToKMB(Data.GetCoinsCount());
        }

        private void UpdateSpeedText()
        {
            _currentUpgradeValue.text = string.Format("{0:0.0000}s", Data.GetShootSpeed());
        }

        private void UpdatePowerText()
        {
            _currentUpgradeValue.text = Data.GetShootPower() + "p";

        }

        private void PlaySound(AudioClip clip)
        {
            if (_audioSource)
            {
                _audioSource.clip = clip;
                _audioSource.Play();
            }
        }

    }

}