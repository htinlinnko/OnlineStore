using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.Text;

namespace POSRestService
{
    [AspNetCompatibilityRequirements
    (RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]

    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]

    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "pos_code_service" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select pos_code_service.svc or pos_code_service.svc.cs at the Solution Explorer and start debugging.
    public class pos_code_service : Ipos_code
    {
        public string GetHelloCode()
        {
            return "Hello from pos";
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
