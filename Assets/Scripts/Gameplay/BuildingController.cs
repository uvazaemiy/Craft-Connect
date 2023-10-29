using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class BuildingController : MonoBehaviour
{
    [Header("General")]
    public Building BuildingData;
    public bool AvailableToBuild;
    public bool AvailableToUse;
    [SerializeField] private BuildingController[] NextBuilding;
    public SpriteRenderer[] BuildingSprites;
    public Text[] BuildingTexts;
    [Space]
    [Header("Nodes")]
    public NodeController[] inNodes_U;
    [Space]
    public NodeController[] inNodes_C;
    public NodeController[] outNodes_C;
    [Space]
    [Header("Builing sprites")]
    [SerializeField] private SpriteRenderer Shadow;
    [SerializeField] private SpriteRenderer AddSprite;
    [SerializeField] private SpriteRenderer TextSprite;
    [Header("Progress sprites")]
    [SerializeField] private Slider slider;
    [SerializeField] private Image[] imagesOfSlider;
    [SerializeField] private Image ProgressCircle;
    [SerializeField] private Text ProgressText;

    private int rec_items;

    private void Start()
    {
        for (int i = 0; i < inNodes_U.Length; i++)
        {
            inNodes_U[i].item_count = BuildingData.AllComponents[i].ComponentCount;
            inNodes_U[i].max_items = BuildingData.AllComponents[i].ComponentCount;
            inNodes_U[i].ItemText.text = "X" + BuildingData.AllComponents[i].ComponentCount;
        }
        
        if (outNodes_C.Length > 0)
            rec_items = outNodes_C[0].max_items;
        for (int i = 0; i < inNodes_C.Length; i++)
            inNodes_C[i].max_items = BuildingData.Type == Enums.TypeOfBuildins.Factory ? outNodes_C[0].max_items * 5 : outNodes_C[0].max_items;
        if (inNodes_C.Length > 0)
            outNodes_C[0].max_items = inNodes_C[0].max_items;

        
        if (BuildingData.Type == Enums.TypeOfBuildins.Storage)
            foreach (NodeController node in outNodes_C)
                node.ItemText.text = "X" + 0;

        if (!AvailableToUse)
        {
            foreach (Image image in imagesOfSlider)
                image.color = new Color(1, 1, 1, 0);
            slider.gameObject.SetActive(false);
        }

        StartCoroutine(PredStartGenerating());
    }

    private IEnumerator GenerateItems(NodeController node)
    {
        if (AvailableToUse)
        {
            if (node.lineID >= node.AllLines.Count)
                node.lineID = 0;

            rec_items = BuildingData.Type == Enums.TypeOfBuildins.Factory ? 1 : rec_items;
            int itemProdaction = BuildingData.Type == Enums.TypeOfBuildins.Factory ? rec_items : node.max_items;
            if (BuildingData.Type == Enums.TypeOfBuildins.Storage)
                itemProdaction = node.item_count > node.max_items ? node.max_items : node.item_count;
            
            if ((node.item_count <= node.max_items && BuildingData.Type != Enums.TypeOfBuildins.Storage) || (BuildingData.Type == Enums.TypeOfBuildins.Storage && node.item_count > 0))
            {
                if (node.item_count < node.max_items && BuildingData.Type != Enums.TypeOfBuildins.Storage)
                {
                    yield return StartCoroutine(FillSlider(BuildingData.TimeToCraft));
                    node.item_count += itemProdaction;
                }
                
                if (node.lineID >= node.AllLines.Count)
                    node.lineID = 0;
                
                NodeController nextNode = null;

                for (int i = node.lineID; i < node.AllLines.Count; i++)
                {
                    if (node.AllLines[i].inNode)
                    {
                        if (!node.AllLines[i].inNode.ignore)
                        {
                            nextNode = node.AllLines[node.lineID].inNode;
                            node.lineID = i;
                            break;
                        }
                    }
                }

                if (nextNode && node.lineID < node.AllLines.Count)
                {
                    if (!node.AllLines[node.lineID].inNode.ignore)
                    {
                        if (BuildingData.Type == Enums.TypeOfBuildins.Factory && node.item_count > 0)
                            itemProdaction = node.item_count;
                        node.item_count -= itemProdaction;
                        CreateItem(node, itemProdaction);
                        slider.value = 0;
                    }
                }

                node.lineID++;
            }
        }
    }

    private void CreateItem(NodeController node, int count)
    {
        ItemController newItem = Instantiate(GameManager.instance.ItemPrefab, node.transform.position, Quaternion.identity);
        newItem.transform.SetParent(node.transform);
        newItem.Init(node.ItemData, count);
        node.AllLines[node.lineID].AllItemsOnLine.Add(newItem);

        float distance =
            Vector3.Distance(
                node.AllLines[node.lineID].line.GetPosition(node.AllLines[node.lineID].line.positionCount - 1),
                node.transform.position);

        if (distance <= 3f)
        {
            Vector3[] positions = new Vector3[node.AllLines[node.lineID].line.positionCount];
            node.AllLines[node.lineID].line.GetPositions(positions);
            System.Array.Reverse(positions);
            node.AllLines[node.lineID].line.SetPositions(positions);
        }
                    
        StartCoroutine(newItem.MoveItem(node.AllLines[node.lineID].line, node.AllLines[node.lineID].inNode));
    }

    private IEnumerator PredStartGenerating()
    {
        yield return new WaitForSeconds(1);
        if (BuildingData.Type != Enums.TypeOfBuildins.Building && BuildingData.Type != Enums.TypeOfBuildins.Storage)
            if (outNodes_C[0].item_count >= outNodes_C[0].max_items)
                StartCoroutine(FillSlider(0, true));
        
        StartCoroutine(StartGenerating());
    }

    private IEnumerator StartGenerating()
    {
        yield return new WaitForSeconds(0.02f);
        
        if (AvailableToUse)
        {
            switch (BuildingData.Type)
            {
                case Enums.TypeOfBuildins.Prodaction:
                    yield return StartCoroutine(GenerateItems(outNodes_C[0]));
                    break;
                case Enums.TypeOfBuildins.Factory:
                    if (inNodes_C[0].item_count >= 1)
                    {
                        if (inNodes_C.Length == 2)
                        {
                            if (inNodes_C[1].item_count >= 1 && inNodes_C[0].item_count >= 1)
                                yield return StartCoroutine(GenerateItems(outNodes_C[0]));
                            
                            break;
                        }
                        
                        yield return StartCoroutine(GenerateItems(outNodes_C[0]));
                    }
                    else if (outNodes_C[0].item_count > 0 && outNodes_C[0].AllLines.Count > 0)
                    {
                        NodeController nextNode = null;
                        
                        for (int i = outNodes_C[0].lineID; i < outNodes_C[0].AllLines.Count; i++)
                        {
                            if (outNodes_C[0].AllLines[i].inNode)
                            {
                                if (!outNodes_C[0].AllLines[i].inNode.ignore)
                                {
                                    nextNode = outNodes_C[0].AllLines[outNodes_C[0].lineID].inNode;
                                    outNodes_C[0].lineID = i;
                                    break;
                                }
                            }
                        }

                        if (nextNode)
                        {
                            outNodes_C[0].item_count = 0;
                            CreateItem(outNodes_C[0], outNodes_C[0].item_count);
                            slider.value = 0;
                            outNodes_C[0].lineID++;
                        }

                        if (outNodes_C[0].lineID >= outNodes_C[0].AllLines.Count)
                            outNodes_C[0].lineID = 0;

                    }
                    break;
                case Enums.TypeOfBuildins.Storage:
                    for (int i = 0; i < outNodes_C.Length; i++)
                    {
                        StartCoroutine(GenerateItems(outNodes_C[i]));
                        inNodes_C[i].item_count = outNodes_C[i].item_count;
                        
                        outNodes_C[i].ItemText.text = "X" + outNodes_C[i].item_count;
                    }
                    yield return new WaitForSeconds(BuildingData.TimeToCraft); 
               break;
            }
        }

        yield return new WaitForEndOfFrame();
        StartCoroutine(StartGenerating());
    }

    private IEnumerator FillSlider(float time, bool preload = false)
    {
        int n = 0;
        
        do
        {
            if (preload)
            {
                slider.value = 100;
                break;
            }
            
            slider.value = n++;
            yield return new WaitForSeconds(time / 100);
        } while (n != 100);

        if (AvailableToUse && BuildingData.Type != Enums.TypeOfBuildins.Storage && !preload)
            foreach (NodeController node in inNodes_C)
            {
                node.item_count -= rec_items;
                node.ignore = false;
            }
        
        yield return new WaitForEndOfFrame();
    }

    private IEnumerator FillCircle(float time)
    {
        ProgressCircle.gameObject.SetActive(true);
        ProgressText.gameObject.SetActive(true);

        ProgressCircle.DOFade(1, DOTweenManager.instance.time);
        ProgressText.DOFade(1, DOTweenManager.instance.time);

        float n = 0;
        
        do
        {
            ProgressCircle.fillAmount = n;
            ProgressText.text = Mathf.Round(n * 100) + "%";
            n += 0.01f;
            
            yield return new WaitForSeconds(time / 100);
        } while (n <= 1);
        
        ProgressCircle.DOFade(0, DOTweenManager.instance.time);
        ProgressText.DOFade(0, DOTweenManager.instance.time);
        
        ProgressCircle.gameObject.SetActive(false);
        ProgressText.gameObject.SetActive(false);
    }

    public void CheckResourcesForBuild()
    {
        if (!AvailableToUse && inNodes_U[0].item_count <= 0 && inNodes_U[1].item_count <= 0)
        {
            if (inNodes_U.Length > 2)
            {
                if (inNodes_U[2].item_count <= 0)
                    StartCoroutine(Build());
            }
            else
                StartCoroutine(Build());
        }
        else if (BuildingData.Type == Enums.TypeOfBuildins.Storage)
            for (int i = 0; i < inNodes_C.Length; i++)
            {
                outNodes_C[i].item_count = inNodes_C[i].item_count;
                if (outNodes_C[i].ItemText)
                    outNodes_C[i].ItemText.text = "X" + outNodes_C[i].item_count;
            }
    }

    public IEnumerator Build(bool save = false)
    {
        gameObject.SetActive(true);
        foreach (NodeController node in inNodes_U)
        {
            foreach (Line line in node.AllLines)
                line.RemoveLine(true);

            node.item_count = 0;
            node.AllLines.Clear();
            StartCoroutine(node.Fade(0, save));
        }

        if (!save)
            yield return StartCoroutine(FillCircle(BuildingData.TimeToBuild));
        yield return StartCoroutine(ChangeMainSprite(save));

        if (TutorialController.instance.currentStep == 5)
            TutorialController.instance.stepPassed = true;

        if (BuildingData.Type != Enums.TypeOfBuildins.Storage && BuildingData.Type != Enums.TypeOfBuildins.Building)
        {
            slider.gameObject.SetActive(true);
            foreach (Image image in imagesOfSlider)
                image.DOFade(1, DOTweenManager.instance.time);
        }

        foreach (Image image in imagesOfSlider)
        {
            if (!save)
                image.DOFade(1, 0.2f);
            else
                image.color = new Color(1, 1, 1, 1);
        }
        
        foreach (NodeController node in inNodes_C)
            StartCoroutine(node.Fade(1, save));
        foreach (NodeController node in outNodes_C)
            StartCoroutine(node.Fade(1, save));

        AvailableToUse = true;

        foreach (BuildingController building in NextBuilding)
            if (!building.AvailableToBuild)
            {
                building.gameObject.SetActive(true);
                building.AvailableToBuild = true;

                foreach (SpriteRenderer sprite in building.BuildingSprites)
                {
                    if (!save)
                        sprite.DOFade(1, DOTweenManager.instance.time);
                    else
                        sprite.color = Color.white;
                }

                foreach (Text text in building.BuildingTexts)
                {
                    if (!save)
                        text.DOFade(1, DOTweenManager.instance.time);
                    else
                        text.color = Color.white;
                }
            }

        yield return null;
    }

    private IEnumerator ChangeMainSprite(bool save)
    {
        SpriteRenderer mainSprite = GetComponent<SpriteRenderer>();

        AddSprite.sprite = BuildingData.Sprite;
        AddSprite.gameObject.SetActive(true);

        if (Shadow)
        {
            Shadow.gameObject.SetActive(true);
            if (!save)
                Shadow.DOFade(1, DOTweenManager.instance.time);
            else
                Shadow.color = new Color(1, 1, 1, 1);
        }
        
        if (!save)
            yield return AddSprite.DOFade(1, DOTweenManager.instance.time).WaitForCompletion();
        mainSprite.sprite = BuildingData.Sprite;
        mainSprite.color = Color.white;
        AddSprite.gameObject.SetActive(false);

        if (BuildingData.Type == Enums.TypeOfBuildins.Building)
        {
            gameObject.tag = "Line";
            mainSprite.sortingOrder = -3;
            if (!save)
                yield return TextSprite.DOFade(0, DOTweenManager.instance.time).WaitForCompletion();
            TextSprite.gameObject.SetActive(false);
        }

        yield return null;
    }
}