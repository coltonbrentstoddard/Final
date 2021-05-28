using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Final
{
    class Log
    {
        public static void UserLoggedIn(int userId)
        {
            string p = "log.text";
            string log = $"User with ID {userId} logged in at {DataLayer.createTimestamp()}" + Environment.NewLine;

            File.AppendAllText(p, log); 
        }

    }
}
