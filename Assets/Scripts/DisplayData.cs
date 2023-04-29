using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class DisplayData : MonoBehaviour
{

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


    private void DisplayDataList()
    {
        // Display the data
        for(int i = DataList.keyarr.Count - 1; i > -1; i--)
        {
            GameObject data = Instantiate(dataPrefab, dataParent);
            data.transform.Find("DataComment").GetComponent<Text>().text = DataList.Datalist[DataList.keyarr[i]].comment;
            data.transform.Find("DataValue").GetComponent<Text>().text = DataList.Datalist[DataList.keyarr[i]].data;
            data.transform.name = DataList.keyarr[i];
        }

        // Display GameObject for adding new data at the end
        Instantiate(newData, dataParent);
    }

}
