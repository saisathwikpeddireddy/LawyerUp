using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LawyerUp
{
    public class ClientInfo
    {
        private string firstname;
        private string lastname;

        public ClientInfo(string firstname, string lastname)
        {
            firstName = firstname;
            lastName = lastname;
        }

        public String firstName
        {
            get { return firstname; }
            set { firstname = value; }
        }

        public String lastName
        {
            get { return lastname; }
            set { lastname = value; }
        }
    }
}

