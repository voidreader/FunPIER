using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IAP : MonoBehaviour{

	public static IAP _instance;

	private void Awake(){
		_instance = this;
	}

	
	// Use this for initialization
	void Start () {
		if (PlayerPrefs.GetInt("DOUBLE_BOOST") == 1) DoubleBoostPurchase(); //Checking if DOUBLE BOOST is purchased
	}
	
	// Update is called once per frame
	void Update (){
        //Updating money amount text for "Get money for money"
		purchaseAmount = Statistics._instance.DMax(5000, decimal.Round(30 * Statistics._instance.ballDamage * (int) Statistics._instance.fireRate
			              * LevelBar._instance.level  * LevelBar._instance.level * (int)Statistics._instance.fireRate / 2));
		purchaseMoneyText.text = Statistics._instance.GetSuffix(purchaseAmount);
	}

    /// <summary>
    /// On Purchase Successful method for "Max firerate boost"
    /// </summary>
	public void MaxFireRatePurchase(){
		Statistics._instance.MaxFireRate();
	}

    /// <summary>
    /// On Purchase Successful method for "Reoving ads"
    /// </summary>
    public void RemoveAds(){
		PlayerPrefs.SetInt("ADS", 1);
	}
	
	
	public bool doublePurchased = false;
	public GameObject doubleText;

    /// <summary>
    /// On Purchase Successful method for "Double boost"
    /// </summary>
    public void DoubleBoostPurchase(){
		PlayerPrefs.SetInt("DOUBLE_BOOST", 1);
		doublePurchased = true;
		doubleText.SetActive(true);
		
	}

    /// <summary>
    /// On Purchase Successful method for "Get money for money"
    /// </summary>
    public Text purchaseMoneyText;
	private decimal purchaseAmount;

	public void PurchaseMoney(){
		Statistics._instance.AddMoney(purchaseAmount);
        Toast.instance.ShowMessage("Money Purchased");
	}
}
