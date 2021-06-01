﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Oracle.DataAccess.Client;
using Oracle.DataAccess.Types;


namespace SportsClub
{
    public partial class Form1 : Form
    {
        

        string ordb = "data source=orcl; user id=hr; password=hr;";
        OracleConnection conn;
        string eventType;
        int eventID;

        int totalCost;
        int newID;
        string name;
        int cost;


        public Form1()
        {
            InitializeComponent();
        }

        private void label2_Click(object sender, EventArgs e) { }

        private void textBox1_TextChanged(object sender, EventArgs e) { }

        private void label4_Click(object sender, EventArgs e) { }

        private void label3_Click(object sender, EventArgs e) { }

        private void lbl_cost_Click(object sender, EventArgs e){ }

        private void MembersForm_Load(object sender, EventArgs e)
        {
            conn = new OracleConnection(ordb);
            conn.Open();

            OracleCommand cmd = new OracleCommand();
            cmd.Connection = conn;
            cmd.CommandText = "GETSPORT";
            cmd.CommandType = CommandType.StoredProcedure;
           
            cmd.Parameters.Add("name", OracleDbType.RefCursor, ParameterDirection.Output);
            

            OracleDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                Sport_cmb.Items.Add(dr[0]);

            }
            dr.Close();

        }
        
        private void MembersForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            conn.Dispose();
        }

        private void btn_load_Click(object sender, EventArgs e)
        {
            
            if (radioButton_sport.Checked)
            {
                cmb_events.Items.Clear();
                eventType = "Sport";
            }
            else if (radioButton_trips.Checked)
            {
                cmb_events.Items.Clear();
                eventType = "Trip";
            }

            OracleCommand cmd = new OracleCommand();
            cmd.Connection = conn;
            cmd.CommandText = "select eventname from events where eventtype = :type";
            cmd.CommandType = CommandType.Text;
            cmd.Parameters.Add("type", eventType);

            OracleDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                cmb_events.Items.Add(dr[0]);
            }
            dr.Close();
        }

        private void cmb_events_SelectedIndexChanged(object sender, EventArgs e)
        {
            OracleCommand selecEventData = new OracleCommand();
            selecEventData.Connection = conn;
            selecEventData.CommandText = "getEventData";
            selecEventData.CommandType = CommandType.StoredProcedure;

            selecEventData.Parameters.Add("evname", cmb_events.Text);
            selecEventData.Parameters.Add("sd", OracleDbType.Date, ParameterDirection.Output);
            selecEventData.Parameters.Add("ed", OracleDbType.Date, ParameterDirection.Output);
            selecEventData.Parameters.Add("evcost", OracleDbType.Int32, ParameterDirection.Output);
            selecEventData.Parameters.Add("eventID", OracleDbType.Int32, ParameterDirection.Output);

            selecEventData.ExecuteNonQuery();
            try
            {
                lbl_sd.Text = Convert.ToDateTime(selecEventData.Parameters["sd"].Value.ToString()).ToString();
                lbl_ed.Text = Convert.ToDateTime(selecEventData.Parameters["ed"].Value.ToString()).ToString();
                lbl_cost.Text = selecEventData.Parameters["evcost"].Value.ToString();
                eventID = Convert.ToInt32(selecEventData.Parameters["eventID"].Value.ToString());
                
            }
            catch 
            {
                MessageBox.Show("Error");

            }
                
        }

        private void lbl_ed_Click(object sender, EventArgs e){ }

        private void btn_book_Click(object sender, EventArgs e)
        {
            //Calculate total Cost
            int totalCost;
            totalCost = Convert.ToInt32(txt_notick.Text) * Convert.ToInt32(lbl_cost.Text);
            lbl_totalcost.Text = totalCost.ToString();

            //Get book id
            int newID;
            OracleCommand getBookId = new OracleCommand();
            getBookId.Connection = conn;
            getBookId.CommandText = "getbookid";
            getBookId.CommandType = CommandType.StoredProcedure;
            getBookId.Parameters.Add("bid", OracleDbType.Int32, ParameterDirection.Output);
            getBookId.ExecuteNonQuery();
            try
            {
                newID = Convert.ToInt32(getBookId.Parameters["bid"].Value.ToString()) + 1;

            }
            catch
            {
                newID = 1;

            }
               lbl_bookid.Text = newID.ToString();

         
            //Insert
            OracleCommand insertBooking = new OracleCommand();
            insertBooking.Connection = conn;
            insertBooking.CommandText = "insert into bookings values(:bookID,:eventID,:memberID,:noOfPersons,:totalCost)";
            insertBooking.CommandType = CommandType.Text;
            insertBooking.Parameters.Add("bookID", newID);
            insertBooking.Parameters.Add("EventID", eventID);
            insertBooking.Parameters.Add("memberID", txt_memid.Text);
            insertBooking.Parameters.Add("noOfPersons", txt_notick.Text);
            insertBooking.Parameters.Add("totalCost", lbl_totalcost.Text);
            int r = insertBooking.ExecuteNonQuery();
            if(r != -1)
            {
                MessageBox.Show("Has Been Booked Successfully \nThe total Cost: " + totalCost);
            }

            else
            {
                MessageBox.Show("Error");
            }

        }

        private void Sport_cmb_SelectedIndexChanged(object sender, EventArgs e) { }

        private void MemberID_txt_TextChanged_1(object sender, EventArgs e) { }

        private void label1_Click(object sender, EventArgs e) { }

        private void tab_book_Click(object sender, EventArgs e) { }

        private void tabPage2_Click(object sender, EventArgs e) { }

        private void Save_Btn_Click(object sender, EventArgs e)
        {
            OracleCommand getCapName = new OracleCommand();
            getCapName.Connection = conn;
            getCapName.CommandText = "select captinname, costpermon from sports  where sportname = :name and category =:age and gender = :gender";
            getCapName.CommandType = CommandType.Text;
            getCapName.Parameters.Add("name", Sport_cmb.SelectedItem.ToString());

            if (kidRadBtn.Checked)
                getCapName.Parameters.Add("age", "kids");
            else if (teenRadBtn.Checked)
                getCapName.Parameters.Add("age", "teenagers");

            //getCapName.Parameters.Add("age", Age_cmb.SelectedItem.ToString());
            if (Male_rb.Checked)
                getCapName.Parameters.Add("gender", "m");
            else if (Female_rb.Checked)
                getCapName.Parameters.Add("gender", "f");
           
            
            //getCapName.Parameters.Add("gender", Gender_cmb.SelectedItem.ToString());
            OracleDataReader dr = getCapName.ExecuteReader();

            if (dr.Read())
            {
                CapName_lbl.Text = dr[0].ToString();
                Cost_lbl.Text = dr[1].ToString();
                Join_btn.Visible = true;

            }
            else
            {
                MessageBox.Show("No data found");
            }
        }

        private void label18_Click(object sender, EventArgs e){ }

    }
}
