using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DisplayData : MonoBehaviour
{
    [SerializeField] private EventSystem eventSystem; // getting event system

    [SerializeField] private GameObject dataPrefab; // prefab for displaying data
    [SerializeField] private GameObject settingsPrefab;  // panel for displaying settings
    [SerializeField] private GameObject newData;    // GameObject for adding new data
    [SerializeField] private Transform dataParent;  // parent of the data
    
    [SerializeField] private GameObject loadPanel;  // panel for loading data

    [SerializeField] private GameObject infoPanel;  // panel for displaying data

    // make an instance of this class
    public static DisplayData instance;

    private async void Start()  // sign in and pull data as the app opens
    {
        // Initialize the cloud save service and pull data
        await Task.WhenAll(DataContoller.signIn(), 
            AddingData.instance.setDropDown(), 
            AddingData.instance.CloseDataPanel());

        await Task.WhenAll(DataContoller.PullData(),
            DataCalculator.PullMetaData());

        // Display the data pulled
        DisplayDataList();

        // Close the loading panel
        loadPanel.SetActive(false);
    }




    private bool isSettings = false;    // check if the settings panel is displayed
    public void DisplaySettingsButton() // Display the settings panel
    {
        if(isSettings)  return;
        DisplaySettings();
        isSettings = true;
    }
    public void CloseSettingsButton()
    {
        if(!isSettings) return;
        Destroy(GameObject.Find("Option(Clone)"));
        isSettings = false;
    }

    private void DisplaySettings()
    {
        // Display the settings panel
        GameObject settings = Instantiate(settingsPrefab, dataParent);
        settings.transform.SetSiblingIndex(0);
        settings.transform.Find("PrivacyPolicy").GetComponent<Button>().onClick.AddListener(() => OpenBrowser());
        settings.transform.Find("ColorChange").GetComponent<Button>().onClick.AddListener(() => ChangeColor());
        settings.transform.Find("ClosePanel").GetComponent<Button>().onClick.AddListener(() => CloseSettingsButton());
    }

    private void OpenBrowser()
    {
        Application.OpenURL("https://special-chimpanzee-20a.notion.site/GooglePlay-83b6558362f946c6b6f5b2ac529975ac?pvs=4");
    }

    public void ChangeColor()
    {
        // swap posColor and negColor
        Color32 temp = posColor;
        posColor = negColor;
        negColor = temp;

        // Display the data again
        DisplayDataList();
    }

    private Color32 posColor = new Color32(29, 107, 163, 255); // blue
    private Color32 negColor = new Color32(163, 29, 29, 255);  // red


    public void DisplayDataList()  // Display the data pulled
    {   
        // delete all data displayed
        foreach(Transform child in dataParent)
            Destroy(child.gameObject);

        // Display the settings panel if the settings panel is displayed
        if(isSettings)  DisplaySettings();

        // Display the data if the data exist
        try{
            for(int i = DataList.keyarr.Count - 1; i > -1; i--){
                // instantiate data prefab
                GameObject data = Instantiate(dataPrefab, dataParent);

                // display basic data
                data.transform.Find("DataComment").GetComponent<Text>().text = DataList.Datalist[DataList.keyarr[i]].comment;
                data.transform.Find("DataValue").GetComponent<Text>().text = DataList.Datalist[DataList.keyarr[i]].data;
                data.transform.Find("DataScore").GetComponent<Text>().text = DataCalculator.metadata.getZscore(DataList.Datalist[DataList.keyarr[i]]);
                data.transform.Find("DataDelBut").GetComponent<Button>().onClick.AddListener(() => DeleteData());
                data.transform.name = DataList.keyarr[i];

                // change color of the data score
                // if the data Z score is over 50, change the color to blue
                float score = float.Parse(DataCalculator.metadata.getZscore(DataList.Datalist[DataList.keyarr[i]]));
                Color32 color;
                if(score >= 50.0f)
                    color = posColor;   // default color is blue
                else
                    color = negColor;   // default color is red
                
                data.transform.Find("DataScore").GetComponent<Text>().color = color;
                data.transform.Find("AvgDiff").GetComponent<Image>().color = color;

                float second = DataCalculator.metadata.getseconds(DataList.Datalist[DataList.keyarr[i]].data);
                float mean = DataCalculator.metadata.mean;

                data.transform.Find("AvgDiff").Find("AvgDiffText").GetComponent<Text>().text = 
                    DataCalculator.metadata.getminseconds(second - mean);
            }

            // show mean of data
            infoPanel.transform.Find("Mean").GetComponent<Text>().text =
            "平均 | "+ DataCalculator.metadata.getminseconds(DataCalculator.metadata.mean);

        }catch(System.NullReferenceException){
            Debug.Log("No data");
        }

        

        // Display GameObject for adding new data at the end
        GameObject addingData = Instantiate(newData, dataParent);
        addingData.GetComponent<Button>().onClick.AddListener(() => AddingData.instance.OpenDataPanel());
    }

    public async void DeleteData()  // Delete data when button pressed
    {
        // Show loading panel
        loadPanel.SetActive(true);

        // Get pressed button object
        eventSystem=EventSystem.current;
        GameObject button_ob = eventSystem.currentSelectedGameObject;

        // Delete data from the cloud
        await Task.WhenAll(DataCalculator.DeleteMetadata(button_ob.transform.parent.name));
        await Task.WhenAll(DataContoller.DeleteData(button_ob.transform.parent.name));

        // Delete all data displayed
        foreach(Transform child in button_ob.transform.parent.parent)  
            Destroy(child.gameObject);

        //Display the data again
        DisplayDataList();

        // Close the loading panel
        loadPanel.SetActive(false);

        Debug.Log("Data deleted");
    }


    private void Awake() {
        if(instance == null)    instance = this;
        else                    Destroy(gameObject);
    }
}
