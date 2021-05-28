using MySql.Data.MySqlClient;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Final
{
    class DataLayer
    {
        public static string connString = "server=wgudb.ucertify.com; Uid=U07wZG; password=53689155350; database=U07wZG;";
        private static int userID;
        private static string userName;
        private static Dictionary<int, Hashtable> Appointments = new Dictionary<int, Hashtable>();

        public static int GetUserID()
        {
            return userID;
        }

        public static void SetUserID(int currentuserID)
        {
            userID = currentuserID;
        }

        public static string GetUserName()
        {
            return userName;
        }

        public static void SetUserName(string currentUserName)
        {
            userName = currentUserName;
        }

        public static string createTimestamp()
        {
            return DateTime.Now.ToString("u");
        }


        public static Dictionary<int, Hashtable> GetAppointments()
        {
            return Appointments;
        }

        public static void SetAppointments(Dictionary<int, Hashtable> appointments)
        {
            Appointments = appointments;
        }

        public static string ConvertToLocalTime(string dateTime)
        {

            DateTime utcDateTime = DateTime.Now;
            string localDateTime = utcDateTime.ToString("MM/dd/yyyy hh:mm tt");

            return localDateTime;
        }

        public static int NewID(List<int> idlist)
        {
            int highestID = 0;
            foreach (int id in idlist)
            {
                if (id > highestID)
                    highestID = id;
            }
            return highestID + 1;
        }

        public static int CreateID(string table)
        {
            MySqlConnection c = new MySqlConnection(connString);
            c.Open();
            MySqlCommand cmd = new MySqlCommand($"SELECT {table + "Id"} FROM {table}", c);
            MySqlDataReader rdr = cmd.ExecuteReader();
            List<int> idlist = new List<int>();

            while (rdr.Read())
            {
                idlist.Add(Convert.ToInt32(rdr[0]));
            }
            rdr.Close();
            c.Close();
            return NewID(idlist);
        }


        public static int BuildRecord(string timestamp, string userName, string table, string partOfQuery, int userId = 0)
        {
            int recordId = CreateID(table);
            string recordInsert;
            if (userId == 0)
            {
                recordInsert = $"INSERT INTO {table}" +
                $" VALUES ('{recordId}', {partOfQuery}, '{timestamp}', '{userName}', '{timestamp}', '{userName}')";
            }
            else
            {
                recordInsert = $"INSERT INTO {table} (appointmentId, customerId, start, end, type, userId, createDate, createdBy, lastUpdate, lastUpdateBy)" +
                $" VALUES ('{recordId}', {partOfQuery}, '{userId}', '{timestamp}', '{userName}', '{timestamp}', '{userName}')";
            }

            MySqlConnection c = new MySqlConnection(connString);
            c.Open();
            MySqlCommand cmd = new MySqlCommand(recordInsert, c);
            cmd.ExecuteNonQuery();

            return recordId;

        }

        public static int FindCustomer(string search)
        {
            int customerId;
            string query;
            if (int.TryParse(search, out customerId))
            {
                query = $"SELECT customerId FROM customer WHERE customerid = '{search.ToString()}'";
            }
            else
            {
                query = $"SELECT customerId FROM customer WHERE customerName LIKE '{search}'";
            }

            MySqlConnection con = new MySqlConnection(connString);
            con.Open();
            MySqlCommand cmd = new MySqlCommand(query, con);
            MySqlDataReader rdr = cmd.ExecuteReader();

            if (rdr.HasRows)
            {
                rdr.Read();
                customerId = Convert.ToInt32(rdr[0]);
                rdr.Close();
                con.Close();
                return customerId;
            }
            return 0;

        }

        public static Dictionary<string, string> GetCustomerDetails(int customerId)
        {
            string query = $"SELECT * FROM customer WHERE customerId = '{customerId.ToString()}'";
            MySqlConnection con = new MySqlConnection(connString);
            con.Open();
            MySqlCommand cmd = new MySqlCommand(query, con);
            MySqlDataReader rdr = cmd.ExecuteReader();
            rdr.Read();

            Dictionary<string, string> CustomerDict = new Dictionary<string, string>();
            CustomerDict.Add("customerName", rdr[1].ToString());
            CustomerDict.Add("addressId", rdr[2].ToString());
            CustomerDict.Add("active", rdr[3].ToString());
            rdr.Close();

            query = $"SELECT * FROM address WHERE addressId = '{CustomerDict["addressId"]}'";
            cmd = new MySqlCommand(query, con);
            rdr = cmd.ExecuteReader();
            rdr.Read();

            CustomerDict.Add("address", rdr[1].ToString());
            CustomerDict.Add("cityId", rdr[3].ToString());
            CustomerDict.Add("zip", rdr[4].ToString());
            CustomerDict.Add("phone", rdr[5].ToString());
            rdr.Close();

            query = $"SELECT * FROM city WHERE cityId = '{CustomerDict["cityId"]}'";
            cmd = new MySqlCommand(query, con);
            rdr = cmd.ExecuteReader();
            rdr.Read();

            CustomerDict.Add("city", rdr[1].ToString());
            CustomerDict.Add("countryId", rdr[2].ToString());
            rdr.Close();

            query = $"SELECT * FROM country WHERE countryId = '{CustomerDict["countryId"]}'";
            cmd = new MySqlCommand(query, con);
            rdr = cmd.ExecuteReader();
            rdr.Read();

            CustomerDict.Add("country", rdr[1].ToString());
            rdr.Close();
            con.Close();

            return CustomerDict;
        }

        public static Dictionary<string, string> GetAppointmentDetails(string appointmentId)
        {
            string query = $"SELECT * FROM appointment WHERE appointmentId = '{appointmentId}'";
            MySqlConnection con = new MySqlConnection(connString);
            con.Open();
            MySqlCommand cmd = new MySqlCommand(query, con);
            MySqlDataReader rdr = cmd.ExecuteReader();
            rdr.Read();

            Dictionary<string, string> AppointmentDict = new Dictionary<string, string>();
            AppointmentDict.Add("appointmentId", appointmentId);
            AppointmentDict.Add("customerId", rdr[1].ToString());
            AppointmentDict.Add("type", rdr[13].ToString());
            AppointmentDict.Add("start", rdr[7].ToString());
            AppointmentDict.Add("end", rdr[8].ToString());

            rdr.Close();

            return AppointmentDict;

        }
    }
}
