using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelIcon : MonoBehaviour
{
    public LevelData levelData;
    public LevelHistory levelHistory;
    [Space(25), Header("References")]
    public MapMenuManager mapMenuManager;
    public SceneLoader sceneLoader;
    public MouseOverCheck mouseOverCheck = new MouseOverCheck();
    public GameObject leadLineRendererPrefab;
    public List<LineRenderer> leadsToLineRendererList = new List<LineRenderer>();
    public List<LineRenderer> leadsFromLineRendererList = new List<LineRenderer>();
    public Animator levelIconAnimator;
    public Image levelIcon;
    public bool isLoaded
    {
        get
        {
            if (mapMenuManager.levelIcon == this)
                return (true);
            else
                return (false);
        }
    }
    private bool hasStarted;

    bool clickOnce;

    void Start()
    {
        if (levelData != null)
        {
            levelIcon.sprite = levelData.levelIcon;
            levelHistory = levelData.levelHistory;
        }
        hasStarted = true;
        if (levelData != null && levelData.levelHistory != null)
            CreateLineRenderers();

        if (levelHistory.levelCompletionStatus == LevelHistory.LevelCompletionStatus.Locked)
            levelIcon.color = mapMenuManager.levelLockedIconColor;
    }
    void Update()
    {
        if (mouseOverCheck.mouseOver == true)
        {
            if (Input.GetMouseButtonDown(0) && clickOnce == false)
                clickOnce = true;
            else if (Input.GetMouseButtonDown(0) && clickOnce == true)
                mapMenuManager.StartValidation();

            if (mapMenuManager.levelIcon == null)
            {
                mapMenuManager.SetNewLevelIcon(this);
                //startButtonText.color = new Color(255, 228, 57, 255);
            }
            else if (mapMenuManager.levelIcon != this)
            {
                mapMenuManager.SetNewLevelIcon(this);
            }
        }
        else
            clickOnce = false;

        if (levelHistory.levelCompletionStatus != LevelHistory.LevelCompletionStatus.Locked)
            if (levelData != null && levelData.levelHistory != null)
                RefreshLines();
        switch (levelHistory.levelCompletionStatus)
        {
            case LevelHistory.LevelCompletionStatus.Locked:
                break;
            case LevelHistory.LevelCompletionStatus.Unlocked:
                break;
            case LevelHistory.LevelCompletionStatus.Previewed:
                break;
            case LevelHistory.LevelCompletionStatus.Withdrawn:
                break;
            case LevelHistory.LevelCompletionStatus.Completion_Detected:
                break;
            case LevelHistory.LevelCompletionStatus.Completion_Undetected:
                break;
            default:
                break;
        }
    }

    public void Unload()
    {
        //isLoaded = false;
        //levelBoxAnimator.SetBool("Load", false);
        //sceneLoader.sceneName = null;
        //startButtonText.color = new Color(255, 255, 255, 255);
    }

    public void RefreshIconInfo()
    {

    }

    public void CreateLineRenderers()
    {
        LevelHistory history = levelData.levelHistory;
        Debug.Log("Creating Renderers for " + history.levelData.levelName);

        foreach (LevelData leadTo in history.levelLeadsTo)
        {
            mapMenuManager.levelIconDict.TryGetValue(leadTo.levelHistory, out LevelIcon leadIcon);
            GameObject lineRendererObject = Instantiate(leadLineRendererPrefab, Vector3.zero, Quaternion.identity, mapMenuManager.lineRendererChild.transform);
            LineRenderer lineRenderer = lineRendererObject.GetComponent<LineRenderer>();
            leadsToLineRendererList.Add(lineRenderer);
            lineRenderer.startWidth = 0f;
            lineRenderer.endWidth = 0f;
            lineRenderer.positionCount = 2;
            lineRenderer.sharedMaterial = mapMenuManager.lineToMaterial;
            lineRenderer.SetPosition(0, transform.position);
            lineRenderer.SetPosition(1, leadIcon.transform.position);
            lineRendererObject.GetComponent<Animator>().SetTrigger("Blink");
        }

        foreach (LevelData leadFrom in history.levelLeadFrom)
        {
            mapMenuManager.levelIconDict.TryGetValue(leadFrom.levelHistory, out LevelIcon leadIcon);
            GameObject lineRendererObject = Instantiate(leadLineRendererPrefab, Vector3.zero, Quaternion.identity, mapMenuManager.lineRendererChild.transform);
            LineRenderer lineRenderer = lineRendererObject.GetComponent<LineRenderer>();
            leadsFromLineRendererList.Add(lineRenderer);
            lineRenderer.startWidth = 0f;
            lineRenderer.endWidth = 0f;
            lineRenderer.positionCount = 2;
            lineRenderer.sharedMaterial = mapMenuManager.lineFromMaterial;
            lineRenderer.SetPosition(0, transform.position);
            lineRenderer.SetPosition(1, leadIcon.transform.position);
        }
    }

    public void RefreshLines()
    {
        int counter = 0;
        foreach (LevelData leadTo in levelHistory.levelLeadsTo)
        {
            if (levelHistory.hasCompletedLevel && leadTo.levelHistory.levelCompletionStatus != LevelHistory.LevelCompletionStatus.Locked)
            {
                mapMenuManager.levelIconDict.TryGetValue(leadTo.levelHistory, out LevelIcon leadIcon);
                if (isLoaded == true)
                {
                    leadsToLineRendererList[counter].startWidth = 10f;
                    leadsToLineRendererList[counter].endWidth = 10f;
                }
                else
                {
                    leadsToLineRendererList[counter].startWidth = 0f;
                    leadsToLineRendererList[counter].endWidth = 0f;
                }
                leadsToLineRendererList[counter].positionCount = 2;
                leadsToLineRendererList[counter].SetPosition(0, transform.position);
                leadsToLineRendererList[counter].SetPosition(1, leadIcon.transform.position);
                counter++;
            }
        }

        int counter2 = 0;
        foreach (LevelData leadFrom in levelHistory.levelLeadFrom)
        {
            if (levelHistory.levelCompletionStatus != LevelHistory.LevelCompletionStatus.Locked && leadFrom.levelHistory.hasCompletedLevel)
            {
                mapMenuManager.levelIconDict.TryGetValue(leadFrom.levelHistory, out LevelIcon leadIcon);
                if (isLoaded == true)
                {
                    leadsFromLineRendererList[counter2].startWidth = 10f;
                    leadsFromLineRendererList[counter2].endWidth = 10f;
                }
                else
                {
                    leadsFromLineRendererList[counter2].startWidth = 0f;
                    leadsFromLineRendererList[counter2].endWidth = 0f;
                }
                leadsFromLineRendererList[counter2].positionCount = 2;
                leadsFromLineRendererList[counter2].SetPosition(0, transform.position);
                leadsFromLineRendererList[counter2].SetPosition(1, leadIcon.transform.position);
                counter2++;
            }
        }
    }

    public void OnDrawGizmos()
    {
        if (hasStarted == false && levelData != null)
        {
            levelIcon.sprite = levelData.levelIcon;
            levelIcon.gameObject.name = "LevelIcon (" + levelData.name.ToString() + ")";
        }
    }
}
