using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Data;
using MySql.Data.MySqlClient;

namespace WCFServiceLibrary.main_info
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
    public class main_infoService : Imain_info
    {
        string connString = ConfigurationManager.AppSettings.Get("mysqlConnString");

        public main_info getMainUserInformationByEmail(string _emailAddress)
        {
            MySqlConnection sqlConnection = new MySqlConnection(connString);

            main_info mInfo = new main_info();

            try
            {
                MySqlCommand sqlCommand = new MySqlCommand("SELECT * FROM main_info WHERE email = @em", sqlConnection);
                sqlCommand.Parameters.AddWithValue("@em", _emailAddress);
                sqlConnection.Open();

                MySqlDataReader dReader = sqlCommand.ExecuteReader();
                if (dReader.HasRows)
                {
                    while (dReader.Read())
                    {
                        mInfo.Id_guid = Guid.Parse(dReader["m_guid"].ToString());
                        mInfo.First_name = dReader["first_name"].ToString();
                        mInfo.Last_name = dReader["last_name"].ToString();
                        mInfo.Email = dReader["email"].ToString();
                        mInfo.Password = dReader.IsDBNull(4) ? dReader["password"].ToString() : "";
                        mInfo.Send_email = dReader.IsDBNull(5) ? (int?)null : int.Parse(dReader["send_email"].ToString());
                        mInfo.Created_on = DateTime.Parse(dReader["created_on"].ToString());
                    }
                }
                dReader.Close();
            }
            catch (Exception ex) { throw ex; }
            finally { sqlConnection.Close(); }

            return mInfo ?? null;
        }

        public main_info getMainUserInformationByEmailAndPassword(string _emailAddress, string _password)
        {
            MySqlConnection sqlConnection = new MySqlConnection(connString);

            main_info mInfo = new main_info();

            try
            {
                MySqlCommand sqlCommand = new MySqlCommand("SELECT * FROM main_info WHERE email = @em and password = @pwd", sqlConnection);
                sqlCommand.Parameters.AddWithValue("@em", _emailAddress);
                sqlCommand.Parameters.AddWithValue("@pwd", _password);
                sqlConnection.Open();

                MySqlDataReader dReader = sqlCommand.ExecuteReader();
                if (dReader.HasRows)
                {
                    while (dReader.Read())
                    {
                        mInfo.Id_guid = Guid.Parse(dReader["m_guid"].ToString());
                        mInfo.First_name = dReader["first_name"].ToString();
                        mInfo.Last_name = dReader["last_name"].ToString();
                        mInfo.Email = dReader["email"].ToString();
                        mInfo.Password = dReader.IsDBNull(4) ? dReader["password"].ToString() : "";
                        mInfo.Send_email = dReader.IsDBNull(5) ? (int?)null : int.Parse(dReader["send_email"].ToString());
                        mInfo.Created_on = DateTime.Parse(dReader["created_on"].ToString());
                    }
                }
                dReader.Close();
            }
            catch (Exception ex) { throw ex; }
            finally { sqlConnection.Close(); }

            return mInfo ?? null;
        }

        public string removeMainUserInformation(string _emailAddress, Guid _guid)
        {
            throw new NotImplementedException();
        }

        public string setMainUserInformation(string _firstName, string _lastName, string _emailAddress)
        {
            MySqlConnection sqlConnection = new MySqlConnection(connString);

            try
            {
                int tempPassword = new Random().Next();

                MySqlCommand sqlCommand = new MySqlCommand("INSERT INTO main_info(first_name, last_name, email, m_guid, password, created_on) VALUES (@fn, @ln, @em, @guid, @pwd, NOW())", sqlConnection);
                sqlCommand.Parameters.AddWithValue("@fn", _firstName);
                sqlCommand.Parameters.AddWithValue("@ln", _lastName);
                sqlCommand.Parameters.AddWithValue("@em", _emailAddress);
                sqlCommand.Parameters.AddWithValue("@guid", Guid.NewGuid().ToString());
                sqlCommand.Parameters.AddWithValue("@pwd", tempPassword.ToString());
                sqlConnection.Open();
                sqlCommand.ExecuteNonQuery();
                return "Success";
            }
            catch (Exception ex) { return ex.Message; }
            finally { sqlConnection.Close(); }
        }

        public string updateEmailSendStatus(bool _sendEmail)
        {
            MySqlConnection sqlConnection = new MySqlConnection(connString);

            try
            {
                MySqlCommand sqlCommand = new MySqlCommand("UPDATE main_info SET send_email = @se  WHERE email = @em", sqlConnection);
                sqlCommand.Parameters.AddWithValue("@se", _sendEmail ? "1" : "0");
                sqlConnection.Open();
                sqlCommand.ExecuteNonQuery();
                return "Success";
            }
            catch (Exception ex) { return ex.Message; }
            finally { sqlConnection.Close(); }
        }

        public string updatePassword(string _password, Guid _guid, string _emailAddress)
        {
            MySqlConnection sqlConnection = new MySqlConnection(connString);

            try
            {
                MySqlCommand sqlCommand = new MySqlCommand("UPDATE main_info SET password = @pwd WHERE m_guid = @guid and email = @em", sqlConnection);
                sqlCommand.Parameters.AddWithValue("@pwd", _password);
                sqlCommand.Parameters.AddWithValue("@em", _emailAddress);
                sqlCommand.Parameters.AddWithValue("@guid", _guid.ToString());
                sqlConnection.Open();
                sqlCommand.ExecuteNonQuery();
                return "Success";
            }
            catch (Exception ex) { return ex.Message; }
            finally { sqlConnection.Close(); }
        }

        public string updateMainUserInformation(string _firstName, string _lastName, string _emailAddress, Guid _guid, string _password)
        {
            MySqlConnection sqlConnection = new MySqlConnection(connString);

            try
            {
                MySqlCommand sqlCommand = new MySqlCommand("UPDATE main_info SET first_name = @fn, last_name = @ln WHERE m_guid = @guid and email = @em", sqlConnection);
                sqlCommand.Parameters.AddWithValue("@fn", _firstName);
                sqlCommand.Parameters.AddWithValue("@ln", _lastName);
                sqlCommand.Parameters.AddWithValue("@em", _emailAddress);
                sqlCommand.Parameters.AddWithValue("@guid", _guid.ToString());
                sqlConnection.Open();
                sqlCommand.ExecuteNonQuery();
                return "Success";
            }
            catch (Exception ex) { return ex.Message; }
            finally { sqlConnection.Close(); }
        }

        public string setTestData(string _testData)
        {
            MySqlConnection sqlConnection = new MySqlConnection(connString);

            try
            {
                MySqlCommand sqlCommand = new MySqlCommand("INSERT INTO test(test) VALUES (@test)", sqlConnection);
                sqlCommand.Parameters.AddWithValue("@test", _testData);
                sqlConnection.Open();
                sqlCommand.ExecuteNonQuery();
                return "Success";
            }
            catch (Exception ex) { return ex.Message; }
            finally { sqlConnection.Close(); }
        }

        public string getHello(string _para)
        {
            return "Hello " + _para;
        }
    }
}
