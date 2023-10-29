using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawPathController : MonoBehaviour
{
    public static DrawPathController instance;

    [Header("General settings")]
    public float scale;
    public LineRenderer LinePrefab;
    public DrawBezier Beizer;
    public Gradient CorrectColor;
    public Gradient DragColor;
    public Gradient IncorrectColor;
    [Space]
    public float clickTime;
    
    private Line currentLine;
    private NodeController currentNode;
    private Vector3 startPoint;
    private Vector3 endPoint;
    private Camera camera;

    private NodeController connectedNode;
    private bool misstake;

    void Start()
    {
        instance = this;
        
        camera = Camera.main;
    }

    private void Update()
    {
        if (Input.GetMouseButtonUp(0))
            startPoint = Vector3.zero;
    }

    public IEnumerator DrawPath(Line line, NodeController node)
    {
        currentLine = line;
        currentNode = node;
        LineRenderer lineRenderer = line.line;
        
        startPoint = camera.ScreenToWorldPoint(Input.mousePosition);
        float time = 0.15f;
        
        do
        {
            if (startPoint != Vector3.zero)
            {
                startPoint.z = 0;
            
                if (Input.GetMouseButton(0))
                {
                    CameraMover.instance.allowDrag = false;
                    
                    endPoint = camera.ScreenToWorldPoint(Input.mousePosition);
                    endPoint.z = 0;

                    time -= Time.deltaTime;
                    if (time <= 0)
                    {
                        lineRenderer.positionCount++;
                        time = 0.1f;
                    }

                    if (lineRenderer.positionCount == 0)
                    {
                        lineRenderer.positionCount++;
                        endPoint = node.transform.position;
                        endPoint.z = 0;

                        lineRenderer.SetPosition(0, endPoint);
                        lineRenderer.positionCount++;
                    }

                    lineRenderer.SetPosition(lineRenderer.positionCount - 1, endPoint);
                    
                    lineRenderer.colorGradient = CheckLineCollision(lineRenderer);
                }
            }
        
            yield return new WaitForEndOfFrame();
        } while (startPoint != Vector3.zero && lineRenderer.colorGradient.colorKeys[0].color != CorrectColor.colorKeys[0].color);
        
        misstake = false;

        if (lineRenderer.colorGradient.colorKeys[0].color == DragColor.colorKeys[0].color || lineRenderer.colorGradient.colorKeys[0].color == IncorrectColor.colorKeys[0].color)
            line.RemoveLine();
        else
        {
            if (lineRenderer.positionCount > 5)
            {
                List<Vector3> positions = new List<Vector3>();
                for (int i = 0; i < lineRenderer.positionCount; i += lineRenderer.positionCount / 5)
                    positions.Add(lineRenderer.GetPosition(i));
                Beizer.lineRenderer = lineRenderer;
                Beizer.BezierInterpolate(positions);
            }
            
            lineRenderer.positionCount++;
            lineRenderer.SetPosition(lineRenderer.positionCount - 1, connectedNode.transform.position);
            line.MakeCollider();

            if (line.inNode.tutorial && TutorialController.instance.currentStep != 5 && TutorialController.instance.currentStep != 7)
                TutorialController.instance.stepPassed = true;
            
            GameManager.instance.AllLines.Add(line);
        }
    }

    private Gradient CheckLineCollision(LineRenderer lineRenderer)
    {
        Vector3[] positions = new Vector3[lineRenderer.positionCount];
        lineRenderer.GetPositions(positions);

        for (int i = 0; i < positions.Length - 1; i++)
        {
            Vector3 startPoint = positions[i];
            Vector3 endPoint = positions[i + 1];

            RaycastHit2D[] hits = Physics2D.LinecastAll(startPoint, endPoint);
            
            foreach (RaycastHit2D hit in hits)
            {
                Collider2D collider = hit.collider;
                if (collider)
                {
                    if (collider.GetComponent<NodeController>() && !misstake)
                    {
                        if (collider.gameObject != currentLine.gameObject &&
                            collider.GetComponent<NodeController>().ItemData.Type == currentNode.ItemData.Type &&
                            collider.GetComponent<NodeController>().Type != currentNode.Type && collider.GetComponent<NodeController>().transform.parent.transform.parent != currentNode.transform.parent.transform.parent)
                        {
                            connectedNode = collider.GetComponent<NodeController>();

                            foreach (Line line in connectedNode.AllLines)
                            {
                                if ((line.inNode == currentNode || line.inNode == connectedNode) &&
                                    (line.outNode == connectedNode || line.outNode == currentNode))
                                    return IncorrectColor;
                            }

                            if (connectedNode.Type == Enums.TypeOfNode.In)
                                currentLine.inNode = connectedNode;
                            else
                                currentLine.outNode = connectedNode;

                            connectedNode.AllLines.Add(currentLine);

                            return CorrectColor;
                        }
                        else
                            return IncorrectColor;
                    }
                    else if (collider.gameObject != currentLine.gameObject && collider.tag != "Player" && collider.tag != "Line")
                    {
                        misstake = true;
                        return IncorrectColor;
                    }
                }
            }
        }
        return DragColor;
    }
}