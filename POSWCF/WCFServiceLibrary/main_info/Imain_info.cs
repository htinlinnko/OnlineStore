using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace WCFServiceLibrary.main_info
{
    [ServiceContract]
    interface Imain_info
    {
        [OperationContract]
        main_info getMainUserInformationByEmail(string _emailAddress);

        [OperationContract]
        main_info getMainUserInformationByEmailAndPassword(string _emailAddress, string _password);

        [OperationContract]
        string setMainUserInformation(string _firstName, string _lastName, string _emailAddress);

        [OperationContract]
        string updateMainUserInformation(string _firstName, string _lastName, string _emailAddress, Guid _guid, string _password);

        [OperationContract]
        string removeMainUserInformation(string _emailAddress, Guid _guid);

        [OperationContract]
        string getHello(string _para);
    }
}
