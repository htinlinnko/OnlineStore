using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Web;

/// <summary>
/// Summary description for ITest
/// </summary>
/// 

[ServiceContract]
interface ITest
{
    [OperationContract, WebGet(UriTemplate ="Hello",ResponseFormat =WebMessageFormat.Json)]
    string getHello();

    [OperationContract, WebGet(UriTemplate = "AnotherHello", ResponseFormat = WebMessageFormat.Xml)]
    string anotherHello();
}