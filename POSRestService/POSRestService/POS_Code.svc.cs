using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

namespace POSRestService
{
    [ServiceContract(Namespace = "")]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class POS_Code
    {
        string connString = ConfigurationManager.AppSettings.Get("connString");

        // To use HTTP GET, add [WebGet] attribute. (Default ResponseFormat is WebMessageFormat.Json)
        // To create an operation that returns XML,
        //     add [WebGet(ResponseFormat=WebMessageFormat.Xml)],
        //     and include the following line in the operation body:
        //         WebOperationContext.Current.OutgoingResponse.ContentType = "text/xml";
        [OperationContract]
        [WebGet(ResponseFormat =WebMessageFormat.Json, RequestFormat =WebMessageFormat.Json)]
        public string GetHello()
        {
            // Add your operation implementation here
            return "Hello World";
        }

        [OperationContract, WebInvoke()]
        public string InsertTest(string test)
        {
            SqlConnection sqlConnection = new SqlConnection(connString);
            string statusCode = "";
            try
            {
                sqlConnection.Open();
                string cmdText = "INSERT INTO Test (Test) VALUES (@Test)";

                SqlCommand sqlCommand = new SqlCommand(cmdText, sqlConnection);
                sqlCommand.Parameters.AddWithValue("@Test", test);

                sqlCommand.ExecuteNonQuery();
                statusCode = "Success";
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                sqlConnection.Close();
            }

            return statusCode;
        }

        // Add more operations here and mark them with [OperationContract]
    }
}
