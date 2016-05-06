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
        [WebGet(ResponseFormat =WebMessageFormat.Json)]
        List<pos_code> GetPOSCode();

        [OperationContract]
        [WebInvoke(Method ="POST", BodyStyle =WebMessageBodyStyle.Wrapped, UriTemplate ="")]
        void InsertPOSCode(string title, string first_name, string last_name, string display_order, string profile_name,
            Guid uqid, string mobile_no, string email, string pwd, bool active, string created_by);

        [OperationContract]
        [WebGet(ResponseFormat =WebMessageFormat.Json)]
        string GetHelloCode();
    }
}
