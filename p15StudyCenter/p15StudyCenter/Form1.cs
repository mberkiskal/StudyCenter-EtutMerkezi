using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.IO;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace p15StudyCenter
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        SqlConnection conn = new SqlConnection("Data Source=DESKTOP-23T2RIK\\SQLEXPRESS;Initial Catalog=p15EtütMerkezi;Integrated Security=True");

        void lessonlist()
        {
            SqlDataAdapter da = new SqlDataAdapter("select * from Table_Dersler",conn);
            DataTable dt = new DataTable();
            da.Fill(dt);
            cmbLesson.ValueMember = "DERSID";
            cmbLesson.DisplayMember = "DERSAD";
            cmbLesson.DataSource = dt;
            cmbLesson.Text = "";

            cmbTeacherLesson.ValueMember= "DERSID";
            cmbTeacherLesson.DisplayMember = "DERSAD";
            cmbTeacherLesson.DataSource = dt;
            cmbTeacherLesson.Text = "";
        }
        void lessonlist2()
        {
            SqlDataAdapter da = new SqlDataAdapter("select * from Table_Dersler", conn);
            DataTable dt = new DataTable();
            da.Fill(dt);
            
            cmbTeacherLesson.ValueMember = "DERSID";
            cmbTeacherLesson.DisplayMember = "DERSAD";
            cmbTeacherLesson.DataSource = dt;
            cmbTeacherLesson.Text = "";
        }
        void studylist()
        {
            SqlDataAdapter da3 = new SqlDataAdapter("execute p15Etut", conn);
            DataTable dt3 = new DataTable();
            da3.Fill(dt3);
            dataGridView1.DataSource = dt3;

            for(int i=0; i<dt3.Rows.Count; i++)
            {
                bool status1 = Convert.ToBoolean(dt3.Rows[i]["DURUM"]);
                if(status1 == true)
                {
                    dataGridView1.Rows[i].DefaultCellStyle.BackColor = Color.Green;
                }
                else
                {
                    dataGridView1.Rows[i].DefaultCellStyle.BackColor = Color.Red;

                }
            }
        }
        bool status2;
        void recurrent()
        {
            conn.Open();
            SqlCommand cmd3 = new SqlCommand("select * from Table_Dersler where DERSAD=@p1", conn);
            cmd3.Parameters.AddWithValue("@p1", txtLesson.Text);
            SqlDataReader reader = cmd3.ExecuteReader();
            if (reader.Read())
            {
                status2 = false;
            }
            else
            {
                status2 = true;
            }
            conn.Close();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            lessonlist();
            lessonlist2();
            studylist();
        }      
        private void cmbLesson_SelectedIndexChanged(object sender, EventArgs e)
        {
            SqlDataAdapter da2 = new SqlDataAdapter("select * from Table_Ogretmenler where BRANSID="+cmbLesson.SelectedValue, conn);
            DataTable dt2 = new DataTable();
            da2.Fill(dt2);
            cmbTeacher.ValueMember = "OGRTID";
            dt2.Columns.Add("FullName",typeof(string),"AD+' '+SOYAD");
            cmbTeacher.DisplayMember = "FullName";
            cmbTeacher.DataSource = dt2;
            cmbTeacher.Text = "";
        }

        private void btnCreate_Click(object sender, EventArgs e)
        {
            conn.Open();
            SqlCommand cmd = new SqlCommand("insert into Table_Etut (DERSID,OGRETMENID,TARIH,SAAT) values (@p1,@p2,@p3,@p4)", conn);
            cmd.Parameters.AddWithValue("@p1",cmbLesson.SelectedValue);
            cmd.Parameters.AddWithValue("@p2", cmbTeacher.SelectedValue);
            cmd.Parameters.AddWithValue("@p3", mskDate.Text);
            cmd.Parameters.AddWithValue("@p4", mskClock.Text);
            cmd.ExecuteNonQuery();
            conn.Close();
            MessageBox.Show("Study Successfully Added!", "Process Successfully",MessageBoxButtons.OK,MessageBoxIcon.Information);
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            int secilen = dataGridView1.SelectedCells[0].RowIndex;
            txtStudyId.Text = dataGridView1.Rows[secilen].Cells[0].Value.ToString();
        }

        private void btnGive_Click(object sender, EventArgs e)
        {
            conn.Open();
            SqlCommand cmd1 = new SqlCommand("update Table_Etut set OGRENCIID=@p1, DURUM=@p2 where ETUTID=@p3", conn);
            cmd1.Parameters.AddWithValue("@p1", txtStudent.Text);
            cmd1.Parameters.AddWithValue("@p2", "True");
            cmd1.Parameters.AddWithValue("@p3", txtStudyId.Text);
            cmd1.ExecuteNonQuery();
            conn.Close();
            MessageBox.Show("Study Was Given To The Student Successfully!", "Process Successfully", MessageBoxButtons.OK, MessageBoxIcon.Information);
            studylist();
        }

        private void btnPhoto_Click(object sender, EventArgs e)
        {
            openFileDialog1.ShowDialog();
            pictureBox1.ImageLocation = openFileDialog1.FileName;
        }

        private void btnAddStudent_Click(object sender, EventArgs e)
        {
            conn.Open();
            SqlCommand cmd2 = new SqlCommand("insert into Table_Ogrenciler (AD,SOYAD,FOTOGRAF,SINIF,TELEFEON,MAIL) values (@p1,@p2,@p3,@p4,@p5,@p6)", conn);
            cmd2.Parameters.AddWithValue("@p1", txtName.Text);
            cmd2.Parameters.AddWithValue("@p2", txtSurname.Text);
            cmd2.Parameters.AddWithValue("@p3", pictureBox1.ImageLocation);
            cmd2.Parameters.AddWithValue("@p4", txtClass.Text);
            cmd2.Parameters.AddWithValue("@p5", mskPhoneN.Text); 
            cmd2.Parameters.AddWithValue("@p6", txtMail.Text);
            cmd2.ExecuteNonQuery();
            conn.Close();
            MessageBox.Show("Student Successfully Added!", "Process Successfully", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnAddLesson_Click(object sender, EventArgs e)
        {
            recurrent();
            if (status2 == true)
            {
                conn.Open();
                SqlCommand cmd4 = new SqlCommand("insert into Table_Dersler (DERSAD) values (@p1)", conn);
                cmd4.Parameters.AddWithValue("@p1", txtLesson.Text);
                cmd4.ExecuteNonQuery();
                conn.Close();
                MessageBox.Show("Lesson Successfully Added!", "Process Successfully", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show(txtLesson.Text+" Lesson Already Exists! Try Again!","Process didn't occur", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnTeacher_Click(object sender, EventArgs e)
        {
           
            conn.Open();
            SqlCommand cmd5 = new SqlCommand("insert into Table_Ogretmenler (AD,SOYAD,BRANSID) values (@p1,@p2,@p3)", conn);
            cmd5.Parameters.AddWithValue("@p1", txtTeacherName.Text);
            cmd5.Parameters.AddWithValue("@p2", txtTeacherSurname.Text);
            cmd5.Parameters.AddWithValue("@p3", cmbTeacherLesson.SelectedValue);
            cmd5.ExecuteNonQuery();
            conn.Close();
            MessageBox.Show("Teacher Successfully Added!", "Process Successfully", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
