using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MapManager : MonoBehaviour
{
    public static MapManager Instance { get; private set; }

    public List<GameObject> listMap = new List<GameObject>();

    public GameObject mapCanvas;
    [SerializeField] private Transform mapParent;
    [SerializeField] private GameObject mapPrefab;

    private GameObject currentMap;

    public delegate void OnClick(GameObject c, GameObject n);
    public OnClick onClick = null;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        RenderMap();
        currentMap = listMap[0];
        mapCanvas.SetActive(false);
    }

    void RenderMap()
    {
        for(int i = 0; i< mapParent.childCount; i++)
        {
            Destroy(mapParent.GetChild(i).gameObject);
        }

        foreach (GameObject map in listMap)
        {
            GameObject mapInstance = Instantiate(mapPrefab, mapParent);
            mapInstance.GetComponentInChildren<TextMeshProUGUI>().text = "Diem buyt:\n" + map.name;
            //if(onClick != null)
            {
                Button mapButton = mapInstance.GetComponent<Button>() ?? mapInstance.AddComponent<Button>();
                mapButton.onClick.AddListener(() => 
                {
                    if (currentMap != map)
                    {
                        onClick(currentMap, map);
                        currentMap = map;
                    }
                    else
                    {
                        mapCanvas.gameObject.SetActive(false);
                    }
                });
            }
        }
    }
}
