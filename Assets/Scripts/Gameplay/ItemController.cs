using System.Collections;
using UnityEngine;

public class ItemController : MonoBehaviour
{
    private Item ItemData;
    public int itemCount;
    private SpriteRenderer image;

    public void Init(Item ItemData, int count)
    {
        itemCount = count;
        image = GetComponent<SpriteRenderer>();
        this.ItemData = ItemData;
        image.sprite = ItemData.image;
    }

    public IEnumerator MoveItem(LineRenderer line, NodeController nextNode)
    {
        float distanceCovered = 0.0f;
        float totalDistance = 0.0f;
        int currentPointIndex = 0;
        float speed = GameManager.instance.itemSpeed;
        
        for (int i = 1; i < line.positionCount; i++)
            totalDistance += Vector3.Distance(line.GetPosition(i - 1), line.GetPosition(i));
        
        do
        {
            Vector3 startPoint = line.GetPosition(currentPointIndex);
            Vector3 endPoint = line.GetPosition(currentPointIndex + 1);
            float segmentDistance = Vector3.Distance(startPoint, endPoint);

            distanceCovered += speed * Time.deltaTime;
            
            Vector3 currentPosition = Vector3.Lerp(startPoint, endPoint, distanceCovered / segmentDistance);
            transform.position = currentPosition;

            if (distanceCovered >= segmentDistance)
            {
                distanceCovered = 0.0f;
                currentPointIndex++;
            }
            yield return new WaitForSeconds(Time.deltaTime);
        } while (currentPointIndex < line.positionCount - 1);

        nextNode.AddItem(itemCount);
        line.GetComponent<Line>().AllItemsOnLine.Remove(this);
        Destroy(gameObject);
    }
}
