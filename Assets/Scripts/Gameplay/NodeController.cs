using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class NodeController : MonoBehaviour
{
    [Header("General settings")]
    public bool tutorial;
    public Enums.TypeOfNode Type;
    public bool ignore;
    public int lineID = 0;
    public List<Line> AllLines;
    [Space]
    [Header("Items settings")]
    public Item ItemData;
    public int item_count;
    public int max_items;
    public Text ItemText;
    [Space]
    [Header("Sprites")]
    public SpriteRenderer Back;
    public SpriteRenderer Icon;
    [Space]
    [Header("Storage Settings")]
    public NodeController inNode;
    
    private bool isLineSelected;

    private void Start()
    {
        GameManager.instance.AllNodes.Add(this);
    }

    private void OnMouseDown()
    {
        AllLines.Add(AddDrawComponents());
        GetComponent<Collider2D>().enabled = false;
        StartCoroutine(DrawPathController.instance.DrawPath(AllLines[AllLines.Count - 1], this));
    }

    public Line AddDrawComponents()
    {
        GameObject newGameObject = new GameObject();
        newGameObject.transform.SetParent(transform);
        newGameObject.name = "Line";
        
        Line newLine = newGameObject.AddComponent<Line>();
        newLine.Init(DrawPathController.instance.LinePrefab, this);

        return newLine;
    }

    private void OnMouseUp()
    {
        GetComponent<Collider2D>().enabled = true;
    }

    public void AddItem(int count)
    {
        BuildingController building = transform.parent.transform.parent.GetComponent<BuildingController>();
        count = building.AvailableToUse ? count : -count;
        
        item_count += count;
        if (ItemText)
        {
            item_count = item_count < 0 ? 0 : item_count;
            ItemText.text = "X" + item_count;
        }
        
        if (building.BuildingData.Type == Enums.TypeOfBuildins.Storage)
            if (inNode)
                inNode.AddItem(count);

        ignore = (building.AvailableToUse && item_count >= max_items && building.BuildingData.Type != Enums.TypeOfBuildins.Storage) 
                  || (!building.AvailableToUse && item_count == 0) ? true : false;
        
        building.CheckResourcesForBuild();
    }

    public IEnumerator Fade(float value, bool save)
    {
        gameObject.SetActive(true);

        if (!save)
            Back.DOFade(value, DOTweenManager.instance.time);
        else
            Back.color = new Color(1, 1, 1, value);
        
        if (ItemText)
        {
            ItemText.gameObject.SetActive(true);
            if (!save)
                ItemText.GetComponentInChildren<SpriteRenderer>().DOFade(value, DOTweenManager.instance.time);
            else
                ItemText.GetComponentInChildren<SpriteRenderer>().color = new Color(1, 1, 1, value);
            ItemText.DOFade(value, DOTweenManager.instance.time);
        }
        if (!save)
            yield return Icon.DOFade(value, DOTweenManager.instance.time).WaitForCompletion();
        else
            Icon.color = new Color(1, 1, 1, value);
        if (value == 0)
        {
            if (ItemText)
                ItemText.gameObject.SetActive(false);
            gameObject.SetActive(false);
        }

        yield return null;
    }
}
