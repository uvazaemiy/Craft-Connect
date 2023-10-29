using UnityEngine;
using System.Collections.Generic;

public class DrawBezier : MonoBehaviour 
{
	
	public enum Mode
	{
		Line, //Draws Line Segments at points
		Bezier, //Interprets points as control points of Bezier curve
		BezierInterpolated, //Interpolates 
		BezierReduced
	}
	
	public Mode mode;
	private List<Vector3> points;
    private List<Vector3> gizmos;	
	public LineRenderer lineRenderer;

	// Use this for initialization
	void Start () 
	{
		points = new List<Vector3>();
		
		mode = Mode.BezierReduced;
	}
	
	// Update is called once per frame
	void Update () 
	{
		//ProcessInput();
		//Render();
	}
	
	private void ProcessInput()
	{
        if (mode == Mode.BezierReduced)
        {
            if (Input.GetMouseButtonDown(0))
            {
                //points.Clear();
            }
            if (Input.GetMouseButton(0))
            {
                Vector2 screenPosition = Input.mousePosition;
                Vector3 worldPosition = Camera.main.ScreenToWorldPoint(new Vector3(screenPosition.x, screenPosition.y, 4));

                points.Add(worldPosition);
            }
        }
        else
        {
            if (Input.GetMouseButtonDown(0))
            {
                Vector2 screenPosition = Input.mousePosition;
                Vector3 worldPosition = Camera.main.ScreenToWorldPoint(new Vector3(screenPosition.x, screenPosition.y, 4));

                points.Add(worldPosition);
            }
        }
	}
	

    ///Note: this file merely illustrate the algorithms.
    ///Generally, they should NOT be called each frame!
	private void Render()
	{
		switch(mode)
		{
			case Mode.Line:
				RenderLineSegments();
			break;
			case Mode.Bezier:
				RenderBezier();
			break;
            case Mode.BezierInterpolated:
                //BezierInterpolate();
            break;
            case Mode.BezierReduced:
				//BezierReduce();
            break;


		}
	}
	
	private void RenderLineSegments()
	{
        gizmos = points;
        SetLinePoints(points);
	}

	private void RenderBezier()
	{
		BezierPath bezierPath = new BezierPath();
		
		bezierPath.SetControlPoints(points);
		List<Vector3> drawingPoints = bezierPath.GetDrawingPoints2();

        gizmos = drawingPoints;

        SetLinePoints(drawingPoints);
	}

    public void BezierInterpolate(List<Vector3> points)
    {
        BezierPath bezierPath = new BezierPath();
        bezierPath.Interpolate(points, DrawPathController.instance.scale);

        List<Vector3> drawingPoints = bezierPath.GetDrawingPoints2();
        
        gizmos = bezierPath.GetControlPoints();  

        SetLinePoints(drawingPoints);
    }

    public void BezierReduce(List<Vector3> points)
    {
        BezierPath bezierPath = new BezierPath();
        bezierPath.SamplePoints(points, 1, 1000, 0.33f);
        
        List<Vector3> drawingPoints = bezierPath.GetDrawingPoints2();
        //Debug.Log(gizmos.Count);

        SetLinePoints(drawingPoints);
    }

    private void SetLinePoints(List<Vector3> drawingPoints)
    {
        lineRenderer.SetVertexCount(drawingPoints.Count);

        for (int i = 0; i < drawingPoints.Count; i++)
        {
            lineRenderer.SetPosition(i, drawingPoints[i]);
        }
    }
}
