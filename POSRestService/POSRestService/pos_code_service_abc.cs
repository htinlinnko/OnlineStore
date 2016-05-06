using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.Text;
using System.Threading.Tasks;

namespace POSRestService
{
    [AspNetCompatibilityRequirements
    (RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]

    [ServiceBehavior(InstanceContextMode =InstanceContextMode.Single)]
    public class pos_code_service_abc //: Ipos_code
    {
        public string GetHelloCode()
        {
            return "Hello from Pos Code";
            //throw new NotImplementedException();
        }

        public List<pos_code> GetPOSCode()
        {
            throw new NotImplementedException();
        }

        public void InsertPOSCode(string title, string first_name, string last_name, string display_order, string profile_name, Guid uqid, string mobile_no, string email, string pwd, bool active, string created_by)
        {
            throw new NotImplementedException();
        }
    }
}
