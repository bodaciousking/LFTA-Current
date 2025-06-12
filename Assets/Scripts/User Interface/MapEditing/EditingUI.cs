using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class EditingUI : MonoBehaviour
{
    public List<GameObject> models = new List<GameObject>();
    public List<Sprite> modelImages = new List<Sprite>();
    public GameObject modelButton;

    MapEditing mE;
    E_TeamSpawns e_ts;
    E_ModelTerrain e_mt;
    E_Colors e_c;

    public static EditingUI instance;
    public Transform modelButtonHolder;
    public GameObject optionsButtonHolder;
    public GameObject colorPicker;
    public GameObject recolorButton;

    TMP_InputField brush_x, brush_y, brush_z;

    private void Start()
    {
        mE = MapEditing.instance;
        e_ts = E_TeamSpawns.instance;
        e_mt = E_ModelTerrain.instance;
        e_c = E_Colors.instance;

        CreateModelButtons();

        colorPicker = GameObject.Find("p_colors");
        recolorButton = GameObject.Find("p_");
        brush_x = GameObject.Find("bXField").GetComponent<TMP_InputField>();
        brush_y = GameObject.Find("bYField").GetComponent<TMP_InputField>();
        brush_z = GameObject.Find("bZField").GetComponent<TMP_InputField>();
    }
    public void SetOptionsButtons(bool set)
    {
        optionsButtonHolder.SetActive(set);
    }

    private void Update()
    {
        if (brush_x.text != null && brush_y.text != null && brush_z.text != null)
        {
            e_c.ChangeBrushSize(new Vector3(int.Parse(brush_x.text), int.Parse(brush_y.text), int.Parse(brush_z.text)));
        }
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
            B_Models mB = button.GetComponentInChildren<B_Models>();
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
        colorPicker.SetActive(!colorPicker.activeSelf);
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
