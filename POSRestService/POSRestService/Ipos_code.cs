using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Threading.Tasks;

namespace POSRestService
{
    [ServiceContract]
    public interface Ipos_code
    {
        [OperationContract]
        [WebGet(ResponseFormat = WebMessageFormat.Json)]
        pos_code[] GetPOSCode(string userId, string password);

        [OperationContract]
        [WebInvoke(Method ="PUT", BodyStyle =WebMessageBodyStyle.WrappedRequest)]
        void InsertPOSCode(string title, string first_name, string last_name, string display_order, string profile_name,
            string mobile_no, string email, string pwd, bool active, string created_by, string userId, string password);

        [OperationContract]
        [WebInvoke(Method = "PUT")]
        void InsertTest(string test);

        [OperationContract]
        [WebGet(ResponseFormat = WebMessageFormat.Json)]
        string GetHelloCode(string userId, string password);

        [OperationContract]
        [WebGet(ResponseFormat = WebMessageFormat.Json)]
        string[] GetAnotherCode(string userId, string password);
    }
}
