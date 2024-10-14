﻿
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


[Serializable]
public class IResponse
{
    public bool result;
    public string message;
    public double complete;   // in milliseconds
    public IModel[] package;  // list of returned objusts

    public IResponse(bool _result, string _message)
    {
        result = _result;
        message = _message;
    }


    public void SetResponse(bool _result, string _message, IModel[] _package = null)
    {
        result = _result;
        message = _message;
        package = _package;

        if(_package == null)
        {
            _package = new List<IModel>().ToArray();
        }
        {
            package = _package;
        }

    }

    public string ToJson()
    {
        return JsonConvert.SerializeObject(this);
    }
}