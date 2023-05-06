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
}