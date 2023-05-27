using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using Newtonsoft.Json;

// Gaming Service Name Space
using Unity.Services.Core;
using Unity.Services.Authentication;
using Unity.Services.CloudSave;

public static class DataCalculator
{
    // Instance of dataInfo
    public static DataInfo metadata;

    public async static Task PullMetaData()
    {
        // Pull data info from cloud
        
        try{   
            var datainfo =  await CloudSaveService.Instance.Data.LoadAsync(new HashSet<string> { "DataInfo" });
            var data = datainfo["DataInfo"];

            Debug.Log("Pulling data from the cloud");

            metadata = new DataInfo(JsonConvert.DeserializeObject<DataInfo>(data));
            Debug.Log("Pulling data from the cloud");
        }
        catch
        {
            Debug.Log("No data in the cloud");
        }
        // when there is no data in the cloud

    }

    public async static Task PushMetadata(Data data)
    {
        if(metadata == null)
        {
            // when there is no data in the cloud
            metadata = new DataInfo(getseconds(data.data), 0, 1);
        }
        metadata.addData(data);

        // push data info to the cloud
        var pushdata = new Dictionary<string, object>{ { "DataInfo", metadata } };
        await CloudSaveService.Instance.Data.ForceSaveAsync(pushdata);
    }

    public async static Task DeleteMetadata(string key)
    {
        // change key into data object
        int inext = DataList.keyarr.IndexOf(key);
        Data data = DataList.Datalist[DataList.keyarr[inext]];

        // change the metadata info 
        metadata.deleteData(data);

        // push data info to the cloud
        var pushdata = new Dictionary<string, object>{ { "DataInfo", metadata } };
        await CloudSaveService.Instance.Data.ForceSaveAsync(pushdata);

    }

    private static float getseconds(string data) // convert the data to seconds
    {
        // split the data into minutes, and seconds
        string[] split = data.Split('分');
        split[1] = split[1].Remove(split[1].Length - 1, 1);

        // convert to seconds
        float seconds = float.Parse(split[0]) * 60 + float.Parse(split[1]);

        return seconds;
    }

}


public class DataInfo
{
    public int count {get; set;}
    public float mean {get; set;}
    public float sd {get; set;}

    // constructor when pulling data from the cloud
    public DataInfo(){}

    // recive mean and count data each time the app opens
    public DataInfo(DataInfo datainfo)
    {
        this.mean = datainfo.mean;
        this.count = datainfo.count;
        this.sd = datainfo.sd;
    }
    
    public DataInfo(float mean, int count, float sd)
    {
        this.mean = mean;
        this.count = count;
        this.sd = sd;

        // just testing if it works
        calculateSD(mean);  
    }

    // get Zscore
    public string getZscore(Data data)
    {
        // get the data in seconds
        float datavalue = getseconds(data.data);

        // calculate the Zscore
        float Zscore = (((datavalue - this.mean) * 10 )/ this.sd) + 50;

        return Zscore.ToString("F2");
    }

    private void getsd(float newmean, float datavalue)
    {
        // calculate the sd
        if(count < 6)   // when there is less than 6 data
        {
            // calculate sd normally
            calculateSD(newmean);
        }
        else            // when there is more than 5 data
        {
            // calculate sd with a special formula
            calculateSDSP(newmean, datavalue);
        }
    }
    
    // when adding new data
    public void addData(Data data)
    {
        Debug.Log("Adding data");
        // add count
        this.count++;
        float newmean;

        // calculate the new mean
        float datavalue = getseconds(data.data);
        newmean = (this.mean * (count - 1) + datavalue) / count;

        // calculate the new sd through DataInfo constructor
        getsd(newmean, datavalue);
        
    }

    public void deleteData(Data data)
    {
        Debug.Log("Deleting data");
        // subtract count
        this.count--;
        float newmean;

        // calculate the new mean
        float datavalue = getseconds(data.data);
        newmean = (this.mean * (count + 1) - datavalue) / count;

        // calculate the new sd through DataInfo constructor
        getsd(newmean, datavalue);
    }

    private void calculateSD(float mean) // calculate the sd
    {
        // calculate the mean squared error
        float MSR = 0;       
        foreach (KeyValuePair<string, Data> entry in DataList.Datalist){
            MSR += (float) Mathf.Pow((getseconds(entry.Value.data) - mean), 2);
            Debug.Log("in seconds: " +(getseconds(entry.Value.data)));
        }

        // calculate the sd
        MSR /= this.count; 
        // take the square root of the mean squared error
        this.sd = (float) Mathf.Sqrt(MSR);

        // update the mean
        this.mean = mean;
    }

    private void calculateSDSP(float newmean, float datavalue)
    {
        // the difference between the new mean and the old mean
        float difference = Mathf.Abs(newmean - this.mean) + 1;

        // calculate the ratio of count
        float ratio = ((float)this.count-1) / this.count;

        // calculate the new data's mean squared error
        float newMSR = Mathf.Pow((datavalue - newmean), 2) / this.count;

        // calculate the new sd
        this.sd = Mathf.Sqrt((ratio * difference * Mathf.Pow(this.sd, 2)) + newMSR);
        // update the mean
        this.mean = newmean;
    }

    private float getseconds(string data) // convert the data to seconds
    {
        // split the data into minutes, and seconds
        string[] split = data.Split('分');
        split[1] = split[1].Remove(split[1].Length - 1, 1);

        // convert to seconds
        float seconds = float.Parse(split[0]) * 60 + float.Parse(split[1]);

        return seconds;
    }
}