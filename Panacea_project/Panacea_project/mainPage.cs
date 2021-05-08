using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace Panacea_project
{
    public partial class mainPage : Form
    {

        //Change DB file path in dbfilepath if not found
        private static string dbFilePath = @"C:\Users\Deepak Maurya\Desktop\Panacea\Panacea_project\Panacea_project\PatientDatabaseTable.mdf";
        //Connect local Database table to Main Page
        SqlConnection connection = new SqlConnection(@"Data Source = (LocalDB)\MSSQLLocalDB; AttachDbFilename="+dbFilePath+";Integrated Security = True");
        int patien_id = 0;
        public static bool LoginSuccess = false;
        public mainPage()
        {
            
            //Open Login Page For login with id and password
            loginpage logpg = new loginpage();
            logpg.ShowDialog();
            if (LoginSuccess == true)
            {
                InitializeComponent();
                deleteBtn.Enabled = false;
                AutoCreateId();
                FillDataGridView();
            }
                      
        }

        //This mathed used for Auto generate Id
        private void AutoCreateId()
        {
            //Local Connection open For Database Connectivity
            connection.Open();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = "select Patient_ID from PatientDetails";
            cmd.Connection = connection;
            SqlDataAdapter sqlDataAdapter = new SqlDataAdapter();
            sqlDataAdapter.SelectCommand = cmd;
            DataSet ds = new DataSet();
            sqlDataAdapter.Fill(ds);

            //Local Database Connection Close
            connection.Close();

            //If Table empty then update id = 1
            if (ds.Tables[0].Rows.Count < 1)
            {               
                patien_id = 1;
            }

            //If table have data then 1 increament
            else
            {
                connection.Open();
                SqlCommand cmd1 = new SqlCommand();
                cmd1.CommandText = "Select max(Patient_ID) from PatientDetails";
                cmd1.Connection = connection;
                SqlDataAdapter sqlDataAdapter1 = new SqlDataAdapter();
                sqlDataAdapter1.SelectCommand = cmd1;
                DataSet ds1 = new DataSet();
                sqlDataAdapter1.Fill(ds1);
                int id = Convert.ToInt32(ds1.Tables[0].Rows[0][0]);
                patien_id = id+1;
                connection.Close();
            }
        }

        private void saveBtn_Click(object sender, EventArgs e)
        {
            try
            {
               //Check gender Male or Female and add Local Database table
                string gender = string.Empty;
                bool isChecked = male.Checked;
                if (isChecked)
                    gender = male.Text;
                else
                    gender = female.Text;

                if (connection.State == ConnectionState.Closed)
                {
                    
                    if (saveBtn.Text == "Save" )
                    {
                        if (nametxt.Text != string.Empty && ageTxt.Text != string.Empty && addressTxt.Text != string.Empty)
                        {

                            //sql query and command for save patient details
                            AutoCreateId();
                            connection.Open();
                            SqlCommand sqlcmd = new SqlCommand("PatientEdit", connection);
                            sqlcmd.CommandType = CommandType.StoredProcedure;
                            sqlcmd.Parameters.AddWithValue("@mode", "Add");
                            sqlcmd.Parameters.AddWithValue("@Patient_ID", patien_id);
                            sqlcmd.Parameters.AddWithValue("@Name", nametxt.Text.Trim());
                            sqlcmd.Parameters.AddWithValue("@Age", ageTxt.Text.Trim());
                            sqlcmd.Parameters.AddWithValue("@Gender", gender.Trim());
                            sqlcmd.Parameters.AddWithValue("@Address", addressTxt.Text.Trim());
                            sqlcmd.ExecuteNonQuery();
                            MessageBox.Show("Saved Patient");
                            nametxt.Text = ageTxt.Text = addressTxt.Text = "";
                            male.Checked = female.Checked = false;
                            connection.Close();
                            FillDataGridView();
                        }
                        else
                        {
                            MessageBox.Show("Fill all Details", "Information");
                        }

                    }
                    else
                    { 

                        //sql query and command for update patient details
                        connection.Open();
                        SqlCommand sqlcmd = new SqlCommand("PatientEdit", connection);
                        sqlcmd.CommandType = CommandType.StoredProcedure;
                        sqlcmd.Parameters.AddWithValue("@mode", "Edit");
                        sqlcmd.Parameters.AddWithValue("@Patient_ID", patien_id);
                        sqlcmd.Parameters.AddWithValue("@Name", nametxt.Text.Trim());
                        sqlcmd.Parameters.AddWithValue("@Age", ageTxt.Text.Trim());
                        sqlcmd.Parameters.AddWithValue("@Gender", gender.Trim());
                        sqlcmd.Parameters.AddWithValue("@Address", addressTxt.Text.Trim());
                        sqlcmd.ExecuteNonQuery();
                        MessageBox.Show("Updated Patient Details");                     
                        nametxt.Text = ageTxt.Text = addressTxt.Text = "";
                        male.Checked = female.Checked = false;
                        connection.Close();
                        FillDataGridView();

                    }
                    Reset();                   
                }
             
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Somthing Wrong");
            }

        }

        private void FillDataGridView()
        {
            if (connection.State == ConnectionState.Closed)
            {
                //sql query and command for display patient details
                connection.Open();
                SqlCommand cmd = connection.CreateCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "select * from [PatientDetails]";
                cmd.ExecuteNonQuery();
                DataTable dt = new DataTable();
                SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(cmd);
                sqlDataAdapter.Fill(dt);
                dataGridView1.DataSource = dt;
                connection.Close();
            }
        }

        private void dataGridView1_DoubleClick(object sender, EventArgs e)
        {
            if(dataGridView1.CurrentRow.Index != -1)
            {
                //select data from datagridview to set patient details for update/delete
                patien_id = Convert.ToInt32(dataGridView1.CurrentRow.Cells[0].Value.ToString());
                nametxt.Text = dataGridView1.CurrentRow.Cells[1].Value.ToString();
                ageTxt.Text = dataGridView1.CurrentRow.Cells[2].Value.ToString();
                if (dataGridView1.CurrentRow.Cells[3].Value.ToString() == "Male")
                {
                    male.Checked = true;
                }
                else
                {
                    female.Checked = true;
                }
                
                addressTxt.Text = dataGridView1.CurrentRow.Cells[4].Value.ToString();
                saveBtn.Text = "Update";
                deleteBtn.Enabled = true;
                
            }
        }

        private void resetBtn_Click(object sender, EventArgs e)
        {
            Reset();
        }

        private void Reset()
        {
            //Reset page
            nametxt.Text = ageTxt.Text = addressTxt.Text = "";
            saveBtn.Text = "Save";
            male.Checked = female.Checked = false;
            patien_id = 0;
        }

        private void deleteBtn_Click(object sender, EventArgs e)
        {
            try
            {
                if (connection.State == ConnectionState.Closed)
                {
                    //remove patient details sql query and command
                    connection.Open();
                    SqlCommand sqlcmd = new SqlCommand("PatientDelete", connection);
                    sqlcmd.CommandType = CommandType.StoredProcedure;
                    sqlcmd.Parameters.AddWithValue("@Patient_ID", patien_id);
                    sqlcmd.ExecuteNonQuery();
                    MessageBox.Show("Patient Details Deleted");
                    Reset();                 
                    connection.Close();
                    FillDataGridView();
                    deleteBtn.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message,"Error Message");
            }
        }

    }
}
