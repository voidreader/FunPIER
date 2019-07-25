using UnityEngine;

namespace BallBlaster
{

    public static class Data
    {

        private const string _bestScoreKey = "BestScore";
        private const string _coinsCountKey = "CoinsCount";

        private const string _shootPowerKey = "Power";
        private const string _shootSpeedKey = "Speed";

        public static int GetBestScore() { return PlayerPrefs.GetInt(_bestScoreKey, 0); }

        public static void SetBestScore(int newBestScore) { PlayerPrefs.SetInt(_bestScoreKey, newBestScore); }

        public static int GetCoinsCount() { return PlayerPrefs.GetInt(_coinsCountKey, 1000); }

        public static void SetCoinsCount(int newCoinsCount) { PlayerPrefs.SetInt(_coinsCountKey, newCoinsCount); }

        public static int GetShootPower() { return PlayerPrefs.GetInt(_shootPowerKey, 1); }

        public static void SetShootPower(int value) { PlayerPrefs.SetInt(_shootPowerKey, value); }

        public static float GetShootSpeed() { return PlayerPrefs.GetFloat(_shootSpeedKey, 0.07f); }

        public static void SetShootSpeed(float value) { PlayerPrefs.SetFloat(_shootSpeedKey, value); }
    }
}