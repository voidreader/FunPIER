using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class SkinManager : MonoBehaviour {

    public List<ItemHolder> ItemHoldersList;
    public List<Skin> SkinList;
    [HideInInspector]
    public List<Skin> SaveItemsList = new List<Skin>();

    public Color DefaultColor;
    public Color SelectedColor;

    public PlayerLevelManager LevelManager;

    private void OnEnable()
    {
        LoadData();
        Invoke("UpdateInfo", 0.2f);
    }

    private void UpdateInfo()
    {
        for (int i = 0; i < SkinList.Count; i++)
        {
            ItemHoldersList[i].PriceText.text = "LEVEL " + SkinList[i].NecessaryLevel;
            if (LevelManager.CurrentLevel < SkinList[i].NecessaryLevel)
            {
                ItemHoldersList[i].InfoPanel.SetActive(true);
                ItemHoldersList[i].ItemIcon.color = Color.black;
            }
            else if(LevelManager.CurrentLevel >= SkinList[i].NecessaryLevel)
            {
                ItemHoldersList[i].InfoPanel.SetActive(false);
                ItemHoldersList[i].ItemIcon.color = Color.white;
            }

            if (SkinList[i].Selected)
            {
                ItemHoldersList[i].ButtonFrame.color = SelectedColor;
            }
            else
            {
                ItemHoldersList[i].ButtonFrame.color = DefaultColor;
            }
        }
    }

    public void SelectButton(int id)
    {
        if (LevelManager.CurrentLevel >= SkinList[id].NecessaryLevel)
        {
            foreach (var item in SkinList)
            {
                item.Selected = false;
            }
            SkinList[id].Selected = true;
            Player.ChangeSpriteAction(id);
            PlayerPrefs.SetInt("CurrenSkin", id);
            UpdateInfo();
            SaveData();
        }
    }

    private void SaveData()
    {
        SaveItemsList.Clear();

        for (int i = 0; i < SkinList.Count; i++)
        {
            SaveItemsList.Add(SkinList[i]);
        }

        BinaryFormatter bf = new BinaryFormatter();
        FileStream stream = new FileStream(Application.persistentDataPath + "/" + "skin.data", FileMode.Create);

        bf.Serialize(stream, SaveItemsList);
        stream.Close();
    }

    public void LoadData()
    {
        if (File.Exists(Application.persistentDataPath + "/" + "skin.data"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream stream = new FileStream(Application.persistentDataPath + "/" + "skin.data", FileMode.Open);

            SaveItemsList = (List<Skin>)bf.Deserialize(stream);
            stream.Close();

            for (int i = 0; i < SaveItemsList.Count; i++)
            {
                SkinList[i] = SaveItemsList[i];
            }
        }
    }

}
