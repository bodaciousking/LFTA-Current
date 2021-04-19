using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EditingUI : MonoBehaviour
{
    FlexibleColorPicker fCP;
    public List<GameObject> models = new List<GameObject>();
    public List<Image> modelImages = new List<Image>();
    public GameObject modelButton;
    public GameObject picker;
    public GameObject recolorButton;
    public Color modelColor;
    MapEditing mE;
    public static EditingUI instance;
    public Transform buttonHolder;

    private void Start()
    {
        mE = MapEditing.instance;
        fCP = picker.GetComponent<FlexibleColorPicker>();
    }
    public void CreateModelButtons()
    {
        foreach (Transform item in buttonHolder.transform)
        {
            Destroy(item);
        }
        for (int i = 0; i < models.Count; i++)
        {
            GameObject button = Instantiate(modelButton, buttonHolder);
            Button newbutton = button.GetComponent<Button>();
            ModelButton mB = button.GetComponent<ModelButton>();
            mB.previewImage.sprite = modelImages[i].sprite;
            newbutton.onClick.AddListener(() => mB.SelectModel());
            TextMeshProUGUI tmp = button.GetComponent<TextMeshProUGUI>();
            tmp.text = mB.myModel.name;
        }
    }

    public void ShowColor()
    {
        picker.SetActive(true);

    }

    public void ToggleColor()
    {
        picker.SetActive(!picker.activeSelf);
        buttonHolder.gameObject.SetActive(!buttonHolder.gameObject.activeSelf);
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
