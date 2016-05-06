using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace POSRestService
{
    [DataContract]
    public class pos_code
    {
        [DataMember]
        int id;

        [DataMember]
        string title;

        [DataMember]
        string first_name;

        [DataMember]
        string last_name;

        [DataMember]
        string display_order;

        [DataMember]
        string profile_name;

        [DataMember]
        Guid unique_id;

        [DataMember]
        string mobile_no;

        [DataMember]
        string email_address;

        [DataMember]
        string password;

        [DataMember]
        bool active;

        [DataMember]
        string created_by;

        [DataMember]
        string created_on;

        [DataMember]
        string updated_by;

        [DataMember]
        string updated_on;

        public int Id
        {
            get
            {
                return id;
            }

            set
            {
                id = value;
            }
        }

        public string Title
        {
            get
            {
                return title;
            }

            set
            {
                title = value;
            }
        }

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

        public string Display_order
        {
            get
            {
                return display_order;
            }

            set
            {
                display_order = value;
            }
        }

        public string Profile_name
        {
            get
            {
                return profile_name;
            }

            set
            {
                profile_name = value;
            }
        }

        public Guid Unique_id
        {
            get
            {
                return unique_id;
            }

            set
            {
                unique_id = value;
            }
        }

        public string Mobile_no
        {
            get
            {
                return mobile_no;
            }

            set
            {
                mobile_no = value;
            }
        }

        public string Email_address
        {
            get
            {
                return email_address;
            }

            set
            {
                email_address = value;
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

        public bool Active
        {
            get
            {
                return active;
            }

            set
            {
                active = value;
            }
        }

        public string Created_by
        {
            get
            {
                return created_by;
            }

            set
            {
                created_by = value;
            }
        }

        public string Created_on
        {
            get
            {
                return created_on;
            }

            set
            {
                created_on = value;
            }
        }

        public string Updated_by
        {
            get
            {
                return updated_by;
            }

            set
            {
                updated_by = value;
            }
        }

        public string Updated_on
        {
            get
            {
                return updated_on;
            }

            set
            {
                updated_on = value;
            }
        }
    }
}
