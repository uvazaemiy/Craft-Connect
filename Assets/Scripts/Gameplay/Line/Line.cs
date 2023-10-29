using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
[RequireComponent(typeof(Rigidbody2D))]

public class Line : MonoBehaviour
{
    public LineRenderer line;
    public NodeController inNode;
    public NodeController outNode;
    public List<ItemController> AllItemsOnLine = new List<ItemController>();

    public void Init(LineRenderer linePrefab, NodeController node)
    {
        line = GetComponent<LineRenderer>();
        
        line.widthCurve = linePrefab.widthCurve;
        line.material = linePrefab.material;
        line.colorGradient = linePrefab.colorGradient;
        line.sortingOrder = linePrefab.sortingOrder;
        line.positionCount = 0;

        if (node.Type == Enums.TypeOfNode.In)
            this.inNode = node;
        else
            this.outNode = node;
        
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        rb.isKinematic = true;
        
        gameObject.layer = 6;
        gameObject.tag = "Line";
    }
    
    public void MakeCollider()
    {
        int pointCount = line.positionCount;

        Vector2[] colliderPoints = new Vector2[pointCount];

        for (int i = 0; i < pointCount; i++)
            colliderPoints[i] = line.GetPosition(i);

        EdgeCollider2D edgeCollider = gameObject.AddComponent<EdgeCollider2D>();
        edgeCollider.points = colliderPoints;
        edgeCollider.isTrigger = true;
    }

    public void RemoveItems()
    {
        int sum = 0;
        foreach (ItemController item in AllItemsOnLine)
        {
            sum += item.itemCount;
            Destroy(item.gameObject);
        }
        
        if (outNode)
            if (outNode.transform.parent.transform.parent.GetComponent<BuildingController>().BuildingData.Type == Enums.TypeOfBuildins.Storage)
                outNode.AddItem(sum);
        
        AllItemsOnLine.Clear();
    }

    public void RemoveLine(bool build = false)
    {
        RemoveItems();
        
        GameManager.instance.AllLines.Remove(this);
        
        if (inNode && !build)
            inNode.AllLines.Remove(this);
        if (outNode)
            outNode.AllLines.Remove(this);
        
        Destroy(gameObject);
    }
}
