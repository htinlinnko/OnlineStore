using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.Text;
using System.Data;
using MySql.Data.MySqlClient;
using System.Configuration;

namespace POSRESTNormal
{
    [ServiceContract(Namespace = "")]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class POS
    {
        string connString = ConfigurationManager.AppSettings.Get("MySQLConnString");

        // To use HTTP GET, add [WebGet] attribute. (Default ResponseFormat is WebMessageFormat.Json)
        // To create an operation that returns XML,
        //     add [WebGet(ResponseFormat=WebMessageFormat.Xml)],
        //     and include the following line in the operation body:
        //         WebOperationContext.Current.OutgoingResponse.ContentType = "text/xml";
        [OperationContract]
        public void DoWork()
        {
            // Add your operation implementation here
            return;
        }

        [OperationContract, WebGet()]
        public string getHello()
        {
            return "Hello World";
        }

        [OperationContract, WebGet()]
        public string getTestParameter(string parameter1)
        {
            try
            {
                return parameter1;
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        [OperationContract, WebGet()]
        public string insertData(string dataValue)
        {
            MySqlConnection sqlConnection = new MySqlConnection(connString);
            string resultData = "";

            try
            {
                MySqlCommand sqlCommand = new MySqlCommand("INSERT INTO test VALUES (@test)", sqlConnection);
                sqlCommand.Parameters.AddWithValue("@test", dataValue);

                sqlConnection.Open();
                sqlCommand.ExecuteNonQuery();
                resultData = "Success";
            }
            catch(Exception ex)
            {
                resultData = ex.Message;
            }
            finally
            {
                sqlConnection.Close();
            }

            return resultData;
        }
        // Add more operations here and mark them with [OperationContract]
    }
}
