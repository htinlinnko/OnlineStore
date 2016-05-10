using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.Text;
using System.Configuration;

namespace POSRestService
{
    [AspNetCompatibilityRequirements
    (RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]

    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]

    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "pos_code_service" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select pos_code_service.svc or pos_code_service.svc.cs at the Solution Explorer and start debugging.
    public class pos_code_service : Ipos_code
    {
        string connString = ConfigurationManager.AppSettings.Get("connString");

        public string GetHelloCode(string userId, string password)
        {
            if (userId == "mouseInputUser" && password == "mouseCrystalTecl1234")
            {
                return "Hello from pos";
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        public string[] GetAnotherCode(string userId, string password)
        {
            if (userId == "mouseInputUser" && password == "mouseCrystalTecl1234")
            {
                string[] strArray = new string[] { "List1", "List2" };

                return strArray;
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        public pos_code[] GetPOSCode(string userId, string password)
        {
            throw new NotImplementedException();
        }

        public void InsertPOSCode(string title, string first_name, string last_name, string display_order, string profile_name, string mobile_no, string email, string pwd, bool active, string created_by
            , string userId, string password)
        {
            if (userId == "mouseInputUser" && password == "mouseCrystalTecl1234")
            {
                try
                {
                    using (SqlConnection sqlConnection = new SqlConnection(connString))
                    {
                        string cmdText = "INSERT INTO basic_user (title, first_name, last_name, display_order, profile_name, unique_id, mobile_no, email_adderss, password, active, created_by, created_date) " +
                            " VALUES (@ti, @fn, @ln, @do, @pn, @ui, @mn, @ea, @pwd, @ac, @cb, GETDATE())";

                        SqlCommand sqlCommand = new SqlCommand(cmdText, sqlConnection);
                        sqlCommand.Parameters.AddWithValue("@ti", title);
                        sqlCommand.Parameters.AddWithValue("@fn", first_name);
                        sqlCommand.Parameters.AddWithValue("@ln", last_name);
                        sqlCommand.Parameters.AddWithValue("@do", display_order);
                        sqlCommand.Parameters.AddWithValue("@pn", profile_name);
                        sqlCommand.Parameters.AddWithValue("@ui", Guid.NewGuid());
                        sqlCommand.Parameters.AddWithValue("@mn", mobile_no);
                        sqlCommand.Parameters.AddWithValue("@ea", email);
                        sqlCommand.Parameters.AddWithValue("@pwd", pwd);
                        sqlCommand.Parameters.AddWithValue("@ac", active);
                        sqlCommand.Parameters.AddWithValue("@cb", created_by);                        

                        sqlCommand.ExecuteNonQuery();
                    }

                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        public void InsertTest(string test)
        {
            using (SqlConnection sqlConnection = new SqlConnection(connString))
            {
                //sqlConnection.Open();
                string cmdText = "INSERT INTO Test (Test) " +
                    " VALUES (@Test)";

                SqlCommand sqlCommand = new SqlCommand(cmdText, sqlConnection);
                sqlCommand.Parameters.AddWithValue("@Test", test);

                sqlCommand.ExecuteNonQuery();
                sqlConnection.Close();
            }
        }
    }
}
