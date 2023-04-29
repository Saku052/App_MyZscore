
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Gaming Service Name Space
using Unity.Services.Core;
using Unity.Services.Authentication;

public class Test : MonoBehaviour
{

    // Start is called before the first frame update
    
    public void PushData()
    {
        Data data = new Data("00分00秒", "2020/01/01", "push(this)のデータ");
        
    }

    public async void Deldata()
    {
        await DataContoller.DeleteData("9");
        Debug.Log("Delete success");
    }

    public void showdata()
    {
        Debug.Log("This is the data");
        for(int i = 0; i < DataList.keyarr.Count; i++)
        {
            Debug.Log(DataList.Datalist[DataList.keyarr[i]].SetKey);
        }
        Debug.Log("This is the end of the data");
    }
}
