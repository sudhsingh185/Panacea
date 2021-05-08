using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;
namespace Panacea_project
{
    public partial class loginpage : Form
    {
        
        //Change DB file path in dbfilepath if not found
        private static string dbFilePath = @"C:\Users\Deepak Maurya\Desktop\Panacea\Panacea_project\Panacea_project\PatientDatabaseTable.mdf";
        //Connect local Database table to Main Page
        SqlConnection connection = new SqlConnection(@"Data Source = (LocalDB)\MSSQLLocalDB; AttachDbFilename=" + dbFilePath + ";Integrated Security = True");
        string userid = string.Empty;
        string passwd = string.Empty;
        public loginpage()
        {
            InitializeComponent();
        }

        private void loginBtn_Click(object sender, EventArgs e)
        {
            //user id and password checking using local database table
            connection.Open();
            SqlCommand cmd = connection.CreateCommand();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = "select * from [UserLoginDetails]";
            cmd.ExecuteNonQuery();
            DataTable dt = new DataTable();
            SqlDataAdapter dtApt = new SqlDataAdapter(cmd);
            dtApt.Fill(dt);
            foreach (DataRow dataRow in dt.Rows)
            {
                userid=dataRow[1].ToString();
                passwd = dataRow[2].ToString();

                if (useridTextBox.Text == userid && passwdTextBox.Text == passwd)
                {
                    this.Hide();
                    mainPage.LoginSuccess = true;
                }                
            }
            if (mainPage.LoginSuccess == false)
            {
                MessageBox.Show("Invalid id or Password", "Warnning");
            }
            connection.Close();           
        }      
    }
}
