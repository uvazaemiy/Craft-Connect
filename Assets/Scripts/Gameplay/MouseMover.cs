using UnityEngine;

public class MouseMover : MonoBehaviour
{
    private bool isClick;
    private float time = 0;
    [SerializeField] private Line selectedLine;
    private Camera camera;

    private void Start()
    {
        camera = Camera.main;
    }

    private void Update()
    {
        Vector3 newPosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0);
        transform.position = camera.ScreenToWorldPoint(newPosition);

        if (Input.GetMouseButtonDown(0))
            isClick = true;

        if (isClick)
        {
            if (time < 0.3f)
            {
                time += Time.deltaTime;
                if (time > 0.05f && Input.GetMouseButtonDown(0) && selectedLine)
                {
                    selectedLine.RemoveLine();
                    if (TutorialController.instance.currentStep == 7)
                        TutorialController.instance.stepPassed = true;
                }
            }
            else
            {
                time = 0;
                isClick = false;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Line")
            selectedLine = other.GetComponent<Line>();
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        selectedLine = null;
    }
}
