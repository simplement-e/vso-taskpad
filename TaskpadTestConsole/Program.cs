using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskpadTestConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            string s = "{ 'subscriptionId': '00000000-0000-0000-0000-000000000000','notificationId': 8,'id': 'f9b4c23e-88dd-4516-b04d-849787304e32','eventType': 'tfvc.checkin','publisherId': 'tfs'}";
            s = RestWebServiceClientHelper.Raw("http://localhost:62691/hooks/vso/checkedin", "POST", s);
        }
    }
}
