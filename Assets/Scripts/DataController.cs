// basic namespace
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;
using System.Linq;
using System;

// Gaming Service Name Space
using Unity.Services.Core;
using Unity.Services.Authentication;
using Unity.Services.CloudSave;


public class Data   // Data class
{
    public string SetKey {get; set;}
    public string data {get; set;}
    public string date {get; set;}
    public string comment {get; set;}
    public int before {get; set;}

    public Data(){}   // Constructor when pulling data from the cloud

    public Data(string data, string date, string comment)   // Constructor when adding new data
    {
        this.data = data;
        this.date = date;
        this.comment = comment;

        DataContoller.PushData(this);
    }
}



// push and pull data from the cloud
public static class DataContoller
{
    public static async Task signIn()
    {
        // Initialize the cloud save service
        await UnityServices.InitializeAsync();

        // Sign in anonymously
        await AuthenticationService.Instance.SignInAnonymouslyAsync();

        Debug.Log("Sign in success");
    }


    // Push data to the cloud
    public static async void PushData(Data data)
    {   
        // Set Pointer to current data
        int pointer;
        if(DataList.keyarr == null) pointer = 0;
        else                        pointer = int.Parse(DataList.enddatakey());

        data.before = pointer;

        pointer++;
        data.SetKey = pointer.ToString();

        // Add to DataList
        DataList.AddData(data);
        if(DataList.keyarr.Count < 5){
            DataList.keyarr.Add(data.SetKey);
        }

        // Push the data to the cloud
        var pushdata = new Dictionary<string, object>{ { data.SetKey, data } };
        await CloudSaveService.Instance.Data.ForceSaveAsync(pushdata);

        // Push the pointer to the cloud
        var pushkey = new Dictionary<string, object>{ { "key", DataList.DataKeys() } };
        await CloudSaveService.Instance.Data.ForceSaveAsync(pushkey);
    }

    // pull data from the cloud
    public static async Task PullData()
    {   
        try{
            // Pull Key data from the cloud
            var keyquery =  await CloudSaveService.Instance.Data.LoadAsync(new HashSet<string> { "key" });
            List<string> keyarray = DataList.getKeys(keyquery["key"]);
            
            // Pull the data from the cloud
            var pulllist = new HashSet<string>();
            for(int i = 0; i < keyarray.Count; i++) pulllist.Add(keyarray[i]);
            var query =  await CloudSaveService.Instance.Data.LoadAsync(pulllist);

            // Add the data to the DataList
            for(int i = 0; i < keyarray.Count; i++){
            var data = query[keyarray[i]];
            DataList.AddData(JsonConvert.DeserializeObject<Data>(data));
            }
        }catch(KeyNotFoundException){
            Debug.Log("No data");
        }
    }

    // Delete data
    public static async Task DeleteData(string key)
    {
        try{
            // change the before component on the next data
            int inext = DataList.keyarr.IndexOf(key) + 1;
            DataList.Datalist[DataList.keyarr[inext]].before = DataList.Datalist[key].before;

            // push the changed data
            var pushdata = new Dictionary<string, object>{ { DataList.keyarr[inext], DataList.Datalist[DataList.keyarr[inext]] } };
            await CloudSaveService.Instance.Data.ForceSaveAsync(pushdata);

        }catch(ArgumentOutOfRangeException){
            
            // if the data is the last data, do nothing
            Debug.Log("Index is out of range");
        }

        // Delete data from DataList
        DataList.Datalist.Remove(key);

        // Delete data from keyarr
        DataList.keyarr.Remove(key);

        // Delete data on cloud 
        await CloudSaveService.Instance.Data.ForceDeleteAsync(key);

        try{
            // Pull previous data
            var query =  await CloudSaveService.Instance.Data.LoadAsync(new HashSet<string> { DataList.Datalist[DataList.keyarr[0]].before.ToString() });
            Data data = JsonConvert.DeserializeObject<Data>(query[DataList.Datalist[DataList.keyarr[0]].before.ToString()]);

            // Add data to DataList
            DataList.AddData(data);

            // Add data key to keyarr
            DataList.keyarr.Insert(0, data.SetKey);

        }catch(KeyNotFoundException){
            Debug.Log("No data");
        }

        // push key to arr
        var pushkey = new Dictionary<string, object>{ { "key", DataList.DataKeys() } };
        await CloudSaveService.Instance.Data.ForceSaveAsync(pushkey);
    }

}

// Manages and saves the data
public static class DataList
{
    // List of Data class
    // Max count is 5
    public static Dictionary<string, Data> Datalist = new Dictionary<string, Data>(); 

    // List of keys
    public static List<string> keyarr;

    public static void AddData(Data data)
    {
        const int INITIAL_INDEX = 0;
        const int LAST_INDEX = 4;

        // if the list is full, remove the oldest data
        if(DataList.Datalist.Count > 4)
        {
            DataList.Datalist.Remove(DataList.keyarr[INITIAL_INDEX]);

            for(int i = 0; i < DataList.keyarr.Count - 1; i++)
            {
                DataList.keyarr[i] = DataList.keyarr[i + 1];
            }
            
            DataList.keyarr[LAST_INDEX] = data.SetKey;
        }
        
        DataList.Datalist.Add(data.SetKey, data);
    }

    public static string DataKeys() // return the keys of the data as string
    {
        string keys = "";
        for(int i = 0; i < DataList.keyarr.Count; i++)
        {
            keys += DataList.Datalist[DataList.keyarr[i]].SetKey + ",";
        }

        keys = keys.Remove(keys.Length - 1, 1);
        return keys;
    }

    public static string enddatakey()   // return the last key of the data
    {
        return DataList.keyarr[DataList.keyarr.Count - 1];
    }

    public static List<string> getKeys(string key)  // return the keys of the data as list
    {
        // Split the key string
        DataList.keyarr = key.Split(',').ToList();;
        return keyarr;
    }
}
