using System.Collections;
using System.Collections.Generic;
using Leguar.TotalJSON;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable]
public class LineData
{
    public Vector3[] positions;
    public Vector3 OutNodePosition;
    public Vector3 InNodePosition;
}

[System.Serializable]
public class BuildingData
{
    public Vector3 position;
    public bool availableToBuild;
    public bool availableToUse;
}

[System.Serializable]
public class NodeData
{
    public Vector3 position;
    public int item_Count;
    public bool ignore;
}

public class SaveController : MonoBehaviour
{
    public static SaveController instance;
    private void Start()
    {
        instance = this;

        StartCoroutine(LoadlAll());
    }

    private IEnumerator LoadlAll()
    {
        yield return StartCoroutine(LoadBuildingsFromJson());
        yield return StartCoroutine(LoadLinesFromJson());
        yield return StartCoroutine(LoadNodesFromJson());
        yield return StartCoroutine(DOTweenManager.instance.FadeBackground());
        
        StartCoroutine(TutorialController.instance.CheckTutorial());
    }

    public void SaveLinesToJson()
    {
        JSON jsonObject = new JSON();
        jsonObject.Add("Lines count", GameManager.instance.AllLines.Count);
        
        for (int j = 0; j < GameManager.instance.AllLines.Count; j++)
        {
            LineData lineData = new LineData();

            int positionCount = GameManager.instance.AllLines[j].line.positionCount;
            lineData.positions = new Vector3[positionCount];
            for (int i = 0; i < positionCount; i++)
            {
                lineData.positions[i] = GameManager.instance.AllLines[j].line.GetPosition(i);
            }

            lineData.InNodePosition = GameManager.instance.AllLines[j].inNode.transform.position;
            lineData.OutNodePosition = GameManager.instance.AllLines[j].outNode.transform.position;

            jsonObject.Add("Line " + j, JSON.Serialize(lineData));
        }
        
        
        string json = jsonObject.CreatePrettyString();

        string filePath = Application.persistentDataPath + "/lineData.json";
        System.IO.File.WriteAllText(filePath, json);
    }

    private IEnumerator LoadLinesFromJson()
    {
        yield return new WaitForEndOfFrame();
        string filePath = Application.persistentDataPath + "/lineData.json";
        if (System.IO.File.Exists(filePath))
        {
            string jsonAsString = System.IO.File.ReadAllText(filePath);
            JSON jsonObject = JSON.ParseString(jsonAsString);
            
            for (int i = 0; i < jsonObject.GetJNumber("Lines count").AsInt(); i++)
            {
                LineData line = jsonObject.GetJSON("Line " + i).Deserialize<LineData>();
                GameManager.instance.LoadLine(line);
            }
        }
    }

    public void SaveBuildingsToJson()
    {
        JSON jsonObject = new JSON();
        jsonObject.Add("Buildings count", GameManager.instance.AllBuildings.Count);
        
        for (int j = 0; j < GameManager.instance.AllBuildings.Count; j++)
        {
            BuildingData buildingData = new BuildingData();

            buildingData.position = GameManager.instance.AllBuildings[j].transform.position;
            buildingData.availableToBuild = GameManager.instance.AllBuildings[j].AvailableToBuild;
            buildingData.availableToUse = GameManager.instance.AllBuildings[j].AvailableToUse;

            jsonObject.Add("Building " + j, JSON.Serialize(buildingData));
        }
        
        
        string json = jsonObject.CreatePrettyString();

        string filePath = Application.persistentDataPath + "/buildingData.json";
        System.IO.File.WriteAllText(filePath, json);
    }
    
    private IEnumerator LoadBuildingsFromJson()
    {
        yield return new WaitForEndOfFrame();
        string filePath = Application.persistentDataPath + "/buildingData.json";
        if (System.IO.File.Exists(filePath))
        {
            string jsonAsString = System.IO.File.ReadAllText(filePath);
            JSON jsonObject = JSON.ParseString(jsonAsString);
            
            if (jsonObject.GetJNumber("Buildings count") != null)
                for (int i = 0; i < jsonObject.GetJNumber("Buildings count").AsInt(); i++)
                {
                    BuildingData building = jsonObject.GetJSON("Building " + i).Deserialize<BuildingData>();
                    GameManager.instance.LoadBuilding(building);
                }
        }
    }
    
    public void SaveNodesToJson()
    {
        JSON jsonObject = new JSON();
        jsonObject.Add("Nodes count", GameManager.instance.AllNodes.Count);
        
        for (int j = 0; j < GameManager.instance.AllNodes.Count; j++)
        {
            NodeData nodeData = new NodeData();

            nodeData.position = GameManager.instance.AllNodes[j].transform.position;
            nodeData.item_Count = GameManager.instance.AllNodes[j].item_count;
            nodeData.ignore = GameManager.instance.AllNodes[j].ignore;

            jsonObject.Add("Node " + j, JSON.Serialize(nodeData));
        }
        
        
        string json = jsonObject.CreatePrettyString();

        string filePath = Application.persistentDataPath + "/nodeData.json";
        System.IO.File.WriteAllText(filePath, json);
    }
    
    private IEnumerator LoadNodesFromJson()
    {
        yield return new WaitForEndOfFrame();
        string filePath = Application.persistentDataPath + "/nodeData.json";
        if (System.IO.File.Exists(filePath))
        {
            string jsonAsString = System.IO.File.ReadAllText(filePath);
            JSON jsonObject = JSON.ParseString(jsonAsString);
            
            for (int i = 0; i < jsonObject.GetJNumber("Nodes count").AsInt(); i++)
            {
                NodeData node = jsonObject.GetJSON("Node " + i).Deserialize<NodeData>();
                GameManager.instance.LoadNode(node);
            }
        }
    }

    public void DeleteAllSaves()
    {
        StartCoroutine(DeleteAllSavesRoutine());
    }

    
    public IEnumerator DeleteAllSavesRoutine()
    {
        string filePath = Application.persistentDataPath + "/nodeData.json";
        System.IO.File.Delete(filePath);
        filePath = Application.persistentDataPath + "/buildingData.json";
        System.IO.File.Delete(filePath);
        filePath = Application.persistentDataPath + "/lineData.json";
        System.IO.File.Delete(filePath);

        PlayerPrefs.DeleteAll();
        yield return new WaitForEndOfFrame();
        SceneManager.LoadScene(0);
    }
}