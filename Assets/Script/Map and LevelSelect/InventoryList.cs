using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryList : MonoBehaviour
{
    public List<int> inventorySlots;
    public List<GameObject> allPrefab;
    private List<GameObject> allRobot;
    public float slotOffset = 5f;
    public float slotSpacing = 10f;
    public Vector2 slotSize = new Vector2(32f, 32f);

    public LevelSelectManager levelSelectManager;


    private void Awake()
    {
        levelSelectManager = GameObject.Find("LevelSelectManager").GetComponent<LevelSelectManager>();
        allRobot = new List<GameObject>();
    }
    // Update is called once per frame

    private void SpawnAllUnit(List<int> inventorySlots)
    {
        foreach (int i in inventorySlots)
        {
            GameObject gameObject;
            switch (i)
            {
                case 0:
                    gameObject = Instantiate(allPrefab[0], this.transform);
                    allRobot.Add(gameObject);
                    OrganizeUnit(allRobot);
                    break;
                case 1:
                    gameObject = Instantiate(allPrefab[1], this.transform);
                    allRobot.Add(gameObject);
                    OrganizeUnit(allRobot);
                    break;
                case 2:
                    gameObject = Instantiate(allPrefab[2], this.transform);
                    allRobot.Add(gameObject);
                    OrganizeUnit(allRobot);
                    break;
                case 3:
                    gameObject = Instantiate(allPrefab[3], this.transform);
                    allRobot.Add(gameObject);
                    OrganizeUnit(allRobot);
                    break;
                case 4:
                    gameObject = Instantiate(allPrefab[4], this.transform);
                    allRobot.Add(gameObject);
                    OrganizeUnit(allRobot);
                    break;
                default:
                    break;

            }
        }
    }

    private void OrganizeUnit(List<GameObject> allRobot)
    {
        for (int i = 0; i < allRobot.Count; i++)
        {
            RectTransform rectTransform = allRobot[i].GetComponent<RectTransform>();

            rectTransform.anchorMin = new Vector2(0, 0.5f);
            rectTransform.anchorMax = new Vector2(0, 0.5f);
            rectTransform.pivot = new Vector2(0, 0.5f);

            rectTransform.anchoredPosition = new Vector2(i * (slotSize.x + slotSpacing) + slotOffset, 0);

        }
    }

    public void Load(string SaveName)
    {
        SaveObject saveObject = SaveSystem.LoadObject<SaveObject>(SaveName);
        if (saveObject == null)
        {
            Debug.Log("No Save File Found");
        }
        else
        {
            foreach (GameObject robot in allRobot)
            {
                Destroy(robot);
            }
            allRobot.Clear();
            inventorySlots.Clear();
            foreach (int robotID in saveObject.unit)
            {
                inventorySlots.Add(robotID);
            }
        }

        SpawnAllUnit(inventorySlots);

    }

    public void ClearUnit()
    {
        foreach (GameObject robot in allRobot)
        {
            Destroy(robot);
        }
        allRobot.Clear();
        inventorySlots.Clear();
    }

    [Serializable]
    public class SaveObject
    {
        public int level;
        public int[] unit;
    }
}
