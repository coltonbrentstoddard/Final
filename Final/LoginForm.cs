using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Final
{
    public partial class loginForm : Form
    {
        public loginForm()
        {
            InitializeComponent();
        }

        static public int MyUser(string userName, string password)
        {
            MySqlConnection con = new MySqlConnection(DataLayer.connString);
            con.Open();
            MySqlCommand cmd = new MySqlCommand($"SELECT userId FROM user WHERE userName = '{userName}' AND password = '{password}'", con);
            MySqlDataReader rdr = cmd.ExecuteReader();

            if (rdr.HasRows)
            {
                rdr.Read();
                DataLayer.SetUserID(Convert.ToInt32(rdr[0]));
                DataLayer.SetUserName(userName);
                rdr.Close();
                return DataLayer.GetUserID(); 
            }
            return 0; 
        }

        private void ValidateUser()
        {

                if(MyUser(usernameTextBox.Text, passwordTextBox.Text) != 0)
                {
                    this.Hide();
                    MainForm mF = new MainForm();
                    Log.UserLoggedIn(DataLayer.GetUserID());
                    mF.Show(); 
                }
                else
                {
                    MessageBox.Show(Resource1.errorUsernameOrPassword);
                    usernameTextBox.BackColor = Color.Salmon;
                    passwordTextBox.BackColor = Color.Salmon; 
                }
            
        }

        private void loginButton_Click(object sender, EventArgs e)
        {

            ValidateUser(); 
 
        }
    }
}
