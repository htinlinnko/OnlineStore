using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace WCFServiceLibrary.main_info
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
    public class main_infoService : Imain_info
    {
        public main_info getMainUserInformationByEmail(string _emailAddress)
        {
            throw new NotImplementedException();
        }

        public main_info getMainUserInformationByEmailAndPassword(string _emailAddress, string _password)
        {
            throw new NotImplementedException();
        }

        public string removeMainUserInformation(string _emailAddress, Guid _guid)
        {
            throw new NotImplementedException();
        }

        public string setMainUserInformation(string _firstName, string _lastName, string _emailAddress)
        {
            throw new NotImplementedException();
        }

        public string updateMainUserInformation(string _firstName, string _lastName, string _emailAddress, Guid _guid, string _password)
        {
            throw new NotImplementedException();
        }

        public string getHello(string _para)
        {
            return "Hello " + _para;
        }
    }
}
