using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class GunStore : MonoBehaviour {

    public List<Item> GunList;
    public List<ItemHolder> ItemHoldersList;
    [HideInInspector]
    public List<Item> SaveItemsList = new List<Item>();

    public Color DefaultColor;
    public Color UsedColor;

    private void OnEnable()
    {
        LoadData();
        Invoke("UpdateInfo", 0.2f);
    }

    private void UpdateInfo()
    {
        for (int i = 0; i < GunList.Count; i++)
        {
            ItemHoldersList[i].PriceText.text = GunList[i].Price.ToString();
            if (GunList[i].Bought)
            {
                ItemHoldersList[i].InfoPanel.SetActive(false);
                ItemHoldersList[i].ItemIcon.color = Color.white;
            }
            else
            {
                ItemHoldersList[i].ItemIcon.color = Color.black;
            }

            if (GunList[i].Used)
            {
                ItemHoldersList[i].ButtonFrame.color = UsedColor;
            }
            else
            {
                ItemHoldersList[i].ButtonFrame.color = DefaultColor;
            }
        }
    }

    public void BuyButton(int id)
    {
        if (!GunList[id].Bought)
        {
            if (MoneyManager.Instance.EnoughMoney(GunList[id].Price))
            {
                MoneyManager.Instance.MinysMoney(GunList[id].Price);
                GunList[id].Bought = true;
                PlayerPrefs.SetInt("CurrenWeapon", id);
                WeaponManager.InitAction();
                foreach (var item in GunList)
                {
                    item.Used = false;
                }
                GunList[id].Used = true;
                UpdateInfo();
                SaveData();
            }
        }
        else
        {
            foreach (var item in GunList)
            {
                item.Used = false;
            }
            PlayerPrefs.SetInt("CurrenWeapon", id);
            WeaponManager.InitAction();
            GunList[id].Used = true;
            UpdateInfo();
            SaveData();
        }
    }

    private void SaveData()
    {
        SaveItemsList.Clear();

        for (int i = 0; i < GunList.Count; i++)
        {
            SaveItemsList.Add(GunList[i]);
        }

        BinaryFormatter bf = new BinaryFormatter();
        FileStream stream = new FileStream(Application.persistentDataPath + "/" + "gun.data", FileMode.Create);

        bf.Serialize(stream, SaveItemsList);
        stream.Close();
    }

    public void LoadData()
    {
        if (File.Exists(Application.persistentDataPath + "/" + "gun.data"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream stream = new FileStream(Application.persistentDataPath + "/" + "gun.data", FileMode.Open);

            SaveItemsList = (List<Item>)bf.Deserialize(stream);
            stream.Close();

            for (int i = 0; i < SaveItemsList.Count; i++)
            {
                GunList[i] = SaveItemsList[i];
            }
        }
    }
}
