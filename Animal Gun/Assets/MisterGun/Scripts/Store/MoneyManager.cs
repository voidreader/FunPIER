using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MoneyManager : MonoBehaviour {

	public static MoneyManager Instance { get; set; }

    public Text MoneyText;

    private int _moneyValue;

    private void Awake () {
        Instance = this;
    }

    private void Start()
    {
        _moneyValue = PlayerPrefs.GetInt("Money");
        MoneyText.text = _moneyValue.ToString();
    }

    public bool EnoughMoney(int value)
    {
        if (_moneyValue >= value)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void AddMoney(int value)
    {
        _moneyValue += value;
        UpdateUI();
    }

    public void MinysMoney(int value)
    {
        _moneyValue -= value;
        UpdateUI();
    }

    private void UpdateUI()
    {
        MoneyText.text = _moneyValue.ToString();
        PlayerPrefs.SetInt("Money", _moneyValue);
    }
}
