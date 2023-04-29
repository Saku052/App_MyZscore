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
    [SerializeField] private GameObject newData;    // GameObject for adding new data
    [SerializeField] private Transform dataParent;  // parent of the data

    private async void Start()  // sign in and pull data as the app opens
    {
        // Initialize the cloud save service and pull data
        await Task.WhenAll(DataContoller.signIn());
        await Task.WhenAll(DataContoller.PullData());

        // Display the data pulled
        DisplayDataList();
    }

    private void DisplayDataList()  // Display the data pulled
    {
        // Display the data
        for(int i = DataList.keyarr.Count - 1; i > -1; i--)
        {
            GameObject data = Instantiate(dataPrefab, dataParent);
            data.transform.Find("DataComment").GetComponent<Text>().text = DataList.Datalist[DataList.keyarr[i]].comment;
            data.transform.Find("DataValue").GetComponent<Text>().text = DataList.Datalist[DataList.keyarr[i]].data;
            data.transform.Find("DataDelBut").GetComponent<Button>().onClick.AddListener(() => DeleteData());
            data.transform.name = DataList.keyarr[i];
        }

        // Display GameObject for adding new data at the end
        Instantiate(newData, dataParent);
    }

    public async void DeleteData()
    {
        // Get pressed button object
        eventSystem=EventSystem.current;
        GameObject button_ob = eventSystem.currentSelectedGameObject;

        // Delete data from the cloud
        await Task.WhenAll(DataContoller.DeleteData(button_ob.transform.parent.name));

        // Delete all data displayed
        foreach(Transform child in button_ob.transform.parent.parent)  
            Destroy(child.gameObject);

        //Display the data again
        DisplayDataList();

        Debug.Log("Data deleted");
    }

}
