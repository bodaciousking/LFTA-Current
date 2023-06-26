using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class EditingUI : MonoBehaviour
{
    FlexibleColorPicker fCP;
    public List<GameObject> models = new List<GameObject>();
    public List<Sprite> modelImages = new List<Sprite>();
    public GameObject modelButton;
    public GameObject picker;
    public GameObject recolorButton;
    public Color modelColor;
    MapEditing mE;
    public static EditingUI instance;
    public Transform modelButtonHolder;
    public GameObject optionsButtonHolder;
    EditorTile hitTile;
    EditorTile previousTile;
    HitFace faceHit;

    private void Start()
    {
        mE = MapEditing.instance;
        fCP = picker.GetComponent<FlexibleColorPicker>();
        CreateModelButtons();
    }
    public void SetOptionsButtons(bool set)
    {
        optionsButtonHolder.SetActive(set);
    }

    private void Update()
    {
        Ray ray;
        RaycastHit hit;
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        int layerMask = 1 << 8;
        if (Physics.Raycast(ray, out hit, 5000, layerMask))
        {
            if (!EventSystem.current.IsPointerOverGameObject())
            {
                if (hit.collider.gameObject.GetComponent<EditorTile>())
                    hitTile = hit.collider.gameObject.GetComponent<EditorTile>();
                else
                    hitTile = hit.collider.gameObject.GetComponentInChildren<EditorTile>();

                if (mE.action == MapEditing.Choice.Placing)
                {
                    previousTile = null;

                    faceHit = GetHitFace(hit);
                    if (previousTile != null)
                    {
                        if (previousTile != hitTile)
                            mE.PreviewNewSect(hitTile.mySect.coord, faceHit);
                    }
                    else if (previousTile == null)
                        mE.PreviewNewSect(hitTile.mySect.coord, faceHit);

                    previousTile = hitTile;
                }

                if (Input.GetKeyDown(KeyCode.Mouse0))
                {
                    if (mE.action == MapEditing.Choice.Placing && hitTile.spawnPointTeam == -1)
                    {
                        mE.AddNewSect(hitTile.mySect.coord, faceHit);
                    }

                    else if (mE.action == MapEditing.Choice.ChoosingSpawnSpots && (hitTile.spawnPointTeam == -1 || hitTile.spawnPointTeam == mE.teamToAddTo))
                    {
                        if (mE.ToggleSpawnPoint(hitTile.mySect.coord))
                        {
                            if (hitTile.spawnPointTeam == -1)
                                hitTile.spawnPointTeam = mE.teamToAddTo;
                            else if (hitTile.spawnPointTeam == mE.teamToAddTo)
                                hitTile.spawnPointTeam = -1;
                        }
                    }
                    else if (mE.action == MapEditing.Choice.Recolor)
                    {
                        mE.Paint(hitTile.mySect);
                        hitTile.myMat.color = EditingUI.instance.modelColor;
                    }

                    else if (mE.action == MapEditing.Choice.Erase)
                    {
                        mE.EraseSect(hitTile.mySect.coord);
                        Destroy(hitTile.gameObject);
                    }
                }
            }
            else
                mE.DestroyPreviewSect();
        }
        else
            mE.DestroyPreviewSect();
    }
    public enum HitFace
    {
        None,
        Up,
        Down,
        East,
        West,
        North,
        South
    }

    public HitFace GetHitFace(RaycastHit hit)
    {
        Vector3 incomingVec = hit.normal - Vector3.up;

        if (incomingVec == new Vector3(0, -1, -1))
        {
            return HitFace.South;
        }

        if (incomingVec == new Vector3(0, -1, 1))
        {
            return HitFace.North;
        }

        if (incomingVec == new Vector3(0, 0, 0))
        {
            return HitFace.Up; 
        }

        if (incomingVec == new Vector3(0, -2, 0))
        {
            return HitFace.Down; 
        }

        if (incomingVec == new Vector3(-1, -1, 0))
        {
            return HitFace.West;
        }

        if (incomingVec == new Vector3(1, -1, 0))
        {
            return HitFace.East; 
        }

        return HitFace.None;
    }
    public void CreateModelButtons()
    {
        foreach (Transform item in modelButtonHolder.transform)
        {
            Destroy(item.gameObject);
        }
        for (int i = 0; i < models.Count; i++)
        {
            GameObject button = Instantiate(modelButton, modelButtonHolder);
            Button newbutton = button.GetComponent<Button>();
            ModelButton mB = button.GetComponentInChildren<ModelButton>();
            mB.previewSprite = modelImages[i];
            mB.Init();
            mB.myModel = models[i];
            mB.myReference = i;
            newbutton.onClick.AddListener(() => mB.SelectModel());
            TextMeshProUGUI tmp = button.GetComponentInChildren<TextMeshProUGUI>();
            tmp.text = mB.myModel.name;
        }
    }

    public void ToggleColor()
    {
        picker.SetActive(!picker.activeSelf);
        modelButtonHolder.gameObject.SetActive(!modelButtonHolder.gameObject.activeSelf);
    }

    private void Awake()
    {
        if (instance != null)
        {
            Debug.Log("Too many Editing Ui scripts!");
            return;
        }
        instance = this;
    }
}
