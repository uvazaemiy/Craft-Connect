using System.Collections;
using DG.Tweening;
using UnityEngine;

public class CameraMover : MonoBehaviour
{
    public static CameraMover instance;

    public bool allowDrag;
    Vector3 touchStart;
    [SerializeField] private float zoomOutMin = 1;
    public float zoomOutMax = 8;
    [SerializeField] private float leftBorder = -31.1f;
    [SerializeField] private float rightBorder = -112.2f;
    [SerializeField] private float upBorder = 26.8f;
    [SerializeField] private float downBorder = -7.4f;

    private float aspectRatio;

    private Camera camera;
    
    
    
    
    private bool drag = false;
    private bool zoom = false;

    private Vector3 initialTouchPosition;
    private Vector3 initialCameraPosition;

    private Vector3 initialTouch0Position;
    private Vector3 initialTouch1Position;
    private Vector3 initialMidPointScreen;
    private float initialOrthographicSize;

    private void Start()
    {
        instance = this;
        
        camera = Camera.main;
        aspectRatio = (float)Screen.width / (float)Screen.height;
        
        Application.targetFrameRate = 120;
        QualitySettings.vSyncCount = 0;
    }

    private void Update () 
    {
        if (allowDrag && !TutorialController.instance.tutorialMoving)
        {
            if (Input.touchCount == 2)
            {
                drag = false;

                Touch touch0 = Input.GetTouch(0);
                Touch touch1 = Input.GetTouch(1);

                if (!zoom)
                {
                    initialTouch0Position = touch0.position;
                    initialTouch1Position = touch1.position;
                    initialCameraPosition = this.transform.position;
                    initialOrthographicSize = camera.orthographicSize;
                    initialMidPointScreen = (touch0.position + touch1.position) / 2;

                    zoom = true;
                }
                else
                {
                    this.transform.position = initialCameraPosition;
                    camera.orthographicSize = initialOrthographicSize;

                    float scaleFactor = GetScaleFactor(touch0.position,
                                                       touch1.position,
                                                       initialTouch0Position,
                                                       initialTouch1Position);

                    Vector2 currentMidPoint = (touch0.position + touch1.position) / 2;
                    Vector3 initialMidPointWorldBeforeZoom = camera.ScreenToWorldPoint(initialMidPointScreen);

                    camera.orthographicSize = initialOrthographicSize / scaleFactor;

                    Vector3 initialMidPointWorldAfterZoom = camera.ScreenToWorldPoint(initialMidPointScreen);
                    Vector2 initialMidPointDelta = initialMidPointWorldBeforeZoom - initialMidPointWorldAfterZoom;

                    Vector2 oldAndNewMidPointDelta =
                        camera.ScreenToWorldPoint(currentMidPoint) -
                        camera.ScreenToWorldPoint(initialMidPointScreen);

                    Vector3 newPos = initialCameraPosition;
                    newPos.x -= oldAndNewMidPointDelta.x - initialMidPointDelta.x;
                    newPos.y -= oldAndNewMidPointDelta.y - initialMidPointDelta.y;
                    newPos.z = -10;

                    camera.transform.position = newPos;
                    
                    camera.orthographicSize = Mathf.Clamp(camera.orthographicSize, zoomOutMin, zoomOutMax);
                }
            }
            else 
            {
                zoom = false;
            }
        }
        else if (Input.GetMouseButton(0) && !TutorialController.instance.tutorialMoving)
        {
            if (Input.mousePosition.x < Screen.width * 0.15f)
                transform.position += Vector3.left * GameManager.instance.cameraSpeed * Time.deltaTime;
            if (Input.mousePosition.x > Screen.width * 0.85f)
                transform.position += Vector3.right * GameManager.instance.cameraSpeed * Time.deltaTime;
            if (Input.mousePosition.y < Screen.height * 0.15f)
                transform.position += Vector3.down * GameManager.instance.cameraSpeed * Time.deltaTime;
            if (Input.mousePosition.y > Screen.height * 0.85f)
                transform.position += Vector3.up * GameManager.instance.cameraSpeed * Time.deltaTime;
        }

        if (Input.GetMouseButtonUp(0))
            allowDrag = true;
        
        CheckLimits();
    }

    private void CheckLimits()
    {
        Vector3 newPosition = camera.transform.position;
        if (transform.position.x - camera.orthographicSize * aspectRatio < leftBorder)
            newPosition.x = leftBorder + camera.orthographicSize * aspectRatio;
        if (transform.position.x + camera.orthographicSize * aspectRatio > rightBorder)
            newPosition.x = rightBorder - camera.orthographicSize * aspectRatio;
        if (transform.position.y + camera.orthographicSize > upBorder)
            newPosition.y = upBorder - camera.orthographicSize;
        if (transform.position.y - camera.orthographicSize < downBorder)
            newPosition.y = downBorder + camera.orthographicSize;

        camera.transform.position = newPosition;
    }

    public IEnumerator MoveAndZoomCamera(Vector3 position, float zoom)
    {
        int i = 0;
        camera.transform.DOMove(new Vector3(position.x, position.y, -10), 1);
        
        do
        {
            camera.orthographicSize = Mathf.Lerp(camera.orthographicSize, zoom, 0.08f);
            i++;
            yield return new WaitForSeconds(1 / 60f);
        } while (i != 60);
    }
    
    
    static bool IsTouching(Touch touch)
    {
        return touch.phase == TouchPhase.Began ||
               touch.phase == TouchPhase.Moved ||
               touch.phase == TouchPhase.Stationary;
    }

    public static float GetScaleFactor(Vector2 position1, Vector2 position2, Vector2 oldPosition1, Vector2 oldPosition2)
    {
        float distance = Vector2.Distance(position1, position2);
        float oldDistance = Vector2.Distance(oldPosition1, oldPosition2);

        if (oldDistance == 0 || distance == 0)
        {
            return 1.0f;
        }

        return distance / oldDistance;
    }
}