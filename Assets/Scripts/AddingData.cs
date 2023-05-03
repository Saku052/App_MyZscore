using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class AddingData : MonoBehaviour
{
[SerializeField] private Dropdown dropSeconds;  // drop down for seconds
[SerializeField] private Dropdown dropMinutes;  // drop down for minutes

public static AddingData instance;  // make an instance of this class

public void UploadData()
{
    // Get the data to be uploaded
    string data = dropMinutes.options[dropMinutes.value].text +"分" +
                    dropSeconds.options[dropSeconds.value].text + "秒";

    // Upload the data
    new Data(data, "2022/01/01", "UploadDataのデータ");

    Debug.Log("Data uploaded");
}


public Task setDropDown()
{
    // just to make the value 0 to be 00 / 1 to be 01 and on
    string zero = "";

    // Set Maximum value for drop down
    const int MAXSECOND = 60;
    const int MAXMINUTE = 100;

    // Set drop down for seconds
    dropSeconds.options.Clear();
    for(int i = 0; i < MAXSECOND; i++)
    {   
        if(i < 10)  zero = "0";
        else        zero = "";
        dropSeconds.options.Add(new Dropdown.OptionData(zero + i.ToString()));
    }
    dropSeconds.value = 0;

    // Set drop down for minutes
    dropMinutes.options.Clear();
    for(int i = 0; i < MAXMINUTE; i++)
    {
        if(i < 10)  zero = "0";
        else        zero = "";
        dropMinutes.options.Add(new Dropdown.OptionData(zero + i.ToString()));
    }
    dropMinutes.value = 0;

    return Task.CompletedTask;
}


private void Awake()    // Set instance of this class
{
    if(instance == null)    instance = this;
    else                    Destroy(gameObject);
}
}
