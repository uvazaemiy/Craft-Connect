using System.Collections;
using DG.Tweening;
using Kilosoft.Tools;
using UnityEngine;

public class CinemaMoving : MonoBehaviour
{
    [SerializeField] private Ease TypeOfEase;
    [SerializeField] private Transform FirstPosition;
    [SerializeField] private Transform SecondPosition;
    [SerializeField] private float time;
    [SerializeField] private float TargetScale;

    private Camera camera;

    private void Start()
    {
        camera = Camera.main;
    }

    [EditorButton("Move camera")]
    public void MoveCamera()
    {
        FirstPosition.position = new Vector3(FirstPosition.position.x, FirstPosition.position.y, -10);
        SecondPosition.position = new Vector3(SecondPosition.position.x, SecondPosition.position.y, -10);

        camera.transform.position = FirstPosition.position;
        camera.transform.DOMove(SecondPosition.position, time).SetEase(TypeOfEase);

        StartCoroutine(ChangeScale());
    }

    private IEnumerator ChangeScale()
    {
        int i = 0;
        do
        {
            camera.orthographicSize = Mathf.Lerp(camera.orthographicSize, TargetScale, 0.08f);
            i++;
            yield return new WaitForSeconds(1 / 60f);
        } while (i != time * 60);
    }
}
