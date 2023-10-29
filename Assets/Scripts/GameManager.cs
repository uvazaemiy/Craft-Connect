using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    
    [Header("General Logic")]
    public ItemController ItemPrefab;
    public float itemSpeed = 1;
    public float cameraSpeed = 16;
    [Space]
    [Header("All Objects")]
    public List<NodeController> AllNodes;
    public List<Line> AllLines;
    public List<BuildingController> AllBuildings;

    private void Start()
    {
        instance = this;
    }

    public void LoadLine(LineData savedLine)
    {
        Line createdLine = new Line();
        int i = 0;

        foreach (NodeController node in AllNodes)
        {
            if (savedLine.InNodePosition == node.transform.position || savedLine.OutNodePosition == node.transform.position)
            {
                if (i == 0)
                {
                    createdLine = node.AddDrawComponents();
                    createdLine.line.positionCount = savedLine.positions.Length;

                    for (int k = 0; k < createdLine.line.positionCount; k++)
                    {
                        createdLine.line.SetPosition(k, savedLine.positions[k]);
                        createdLine.MakeCollider();
                        createdLine.line.colorGradient = DrawPathController.instance.CorrectColor;
                    }
                }

                if (savedLine.InNodePosition == node.transform.position)
                {
                    createdLine.inNode = node;
                    node.AllLines.Add(createdLine);
                }
                else
                {
                    createdLine.outNode = node;
                    node.AllLines.Add(createdLine);
                }
                
                i++;
                if (i == 2)
                    break;
            }
        }
        
        AllLines.Add(createdLine);
    }

    public void LoadBuilding(BuildingData savedBuilding)
    {
        foreach (BuildingController building in AllBuildings)
            if (building.transform.position == savedBuilding.position && savedBuilding.availableToUse)
                StartCoroutine(building.Build(true));
    }
    
    public void LoadNode(NodeData savedNode)
    {
        foreach (NodeController node in AllNodes)
            if (node.transform.position == savedNode.position)
            {
                node.item_count = savedNode.item_Count;
                node.ignore = savedNode.ignore;
                if (node.ItemText)
                    node.ItemText.text = "X" + node.item_count;
            }
    }

    private void OnApplicationPause()
    {
        SaveController.instance.SaveLinesToJson();
        SaveController.instance.SaveBuildingsToJson();
        SaveController.instance.SaveNodesToJson();
    }

    private void OnApplicationQuit()
    {
        SaveController.instance.SaveLinesToJson();
        SaveController.instance.SaveBuildingsToJson();
        SaveController.instance.SaveNodesToJson();
    }
}