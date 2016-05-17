using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace WCFServiceLibrary.main_info
{
    /// <summary>
    /// *
    /// *
    /// Created by : HTIN LINN KO
    /// Created on : 17 May 2016
    /// Environment: VS 2015
    /// -----------------------------------------------------------------------
    /// This class is to store main information of the user.
    /// FirstName, LastName, Email, Password, DecryptionKey [Optional], GUID
    /// 
    /// ### ANY CHANGES TO THIS CLASS, PLEASE UPDATE ACCORDINGLY ###
    /// *
    /// *
    /// </summary>
    
    [DataContract]
    public class main_info
    {
        [DataMember]
        string first_name;

        [DataMember]
        string last_name;

        [DataMember]
        string email;

        [DataMember]
        string password;

        [DataMember]
        string decrypt_key;

        [DataMember]
        Guid id_guid;

        public string First_name
        {
            get
            {
                return first_name;
            }

            set
            {
                first_name = value;
            }
        }

        public string Last_name
        {
            get
            {
                return last_name;
            }

            set
            {
                last_name = value;
            }
        }

        public string Email
        {
            get
            {
                return email;
            }

            set
            {
                email = value;
            }
        }

        public string Password
        {
            get
            {
                return password;
            }

            set
            {
                password = value;
            }
        }

        public string Decrypt_key
        {
            get
            {
                return decrypt_key;
            }

            set
            {
                decrypt_key = value;
            }
        }

        public Guid Id_guid
        {
            get
            {
                return id_guid;
            }

            set
            {
                id_guid = value;
            }
        }
    }
}
