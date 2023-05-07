using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

public static class DataCalculator
{
    
    public static Task PullMetaData()
    {
        // Pull data info from cloud
        try{

            // get data count and data mean
            
            // DataInfo(data.mean, data.count);
            
            return Task.CompletedTask;
        }catch
        {

            Debug.Log("No data in the cloud");
        }
        // when there is no data in the cloud

        return Task.CompletedTask;
    }

    public static Task PushMetadata()
    {
        // push count and mean to the cloud
        return Task.CompletedTask;
    }
}

public class DataInfo
{
    public int count;
    public float mean;
    public float sd;

    // recive mean and count data each time the app opens
    public DataInfo(float mean, int count)
    {
        this.mean = mean;
        this.count = count;

        // just testing if it works
        calculateSD(mean, count);  

        // calculate the sd
        if(count < 6)   // when there is less than 6 data
        {
            // calculate sd normally
        }
        else            // when there is more than 5 data
        {
            // calculate sd with a special formula
        }
    }

    // when adding new data
    public void addData(float data)
    {
        // add count


        // calculate the new mean


        // calculate the new sd through DataInfo constructor
        // this = new DataInfo(newMean, newCount);


        // push the new data to the cloud
        
    }

    private void calculateSD(float mean, int count) // calculate the sd
    {
        // calculate the mean squared error
        float MSR = 0;       
        foreach (KeyValuePair<string, Data> entry in DataList.Datalist){
            MSR += (float) Mathf.Pow((getseconds(entry.Value.data) - mean), 2);
            Debug.Log("in seconds: " +(getseconds(entry.Value.data)));
        }

        // calculate the sd
        MSR /= count; 
        // take the square root of the mean squared error
        sd = (float) Mathf.Sqrt(MSR);
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