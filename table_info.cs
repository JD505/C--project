using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.IO;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Xml;
using HtmlAgilityPack;
using HtmlDocument = HtmlAgilityPack.HtmlDocument;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Serialization;


namespace WindowsFormsApp1
{
    
    public partial class table_info : Form
    {
        Warning[] aviso;
        public string selectedNo;
        files[] dataFilesNo;

        bool flag_datePicker = true;
        bool auxFlag = true;

        public table_info(string no)
        {
            InitializeComponent();
            label5.Hide();
            label8.Hide();
            panel2.Hide();
            button1.Hide();
            label2.Hide();
            label4.Hide();
            dateTimePicker1.Hide();

            this.dataGridView2.DoubleBuffered(true);
            this.dataGridView3.DoubleBuffered(true);
            aviso = SetAlarme.Deserialize();
            
            selectedNo = no;
            dataFilesNo = SelectNoFiles();

            //set label tex-adapter name
            label3.Text = " Adapter SN: " + dataFilesNo[0].no;

            // add number of Columns 
            dataGridView2.ColumnCount = dataFilesNo[0].numFiles;

            int i = dataFilesNo[0].numFiles - 1, p;
            for (p = 0; p < dataFilesNo[0].numFiles; p++)
            {
                dataGridView2.Columns[p].Name = dataFilesNo[p].no + p;
                dataGridView2.Columns[p].HeaderCell.Value = (dataFilesNo[p].date).ToString();
                i--;

                //not short
                dataGridView2.Columns[p].SortMode = DataGridViewColumnSortMode.NotSortable;
            }

            // index data grid
            var mfg = dataGridView2.Rows.Add();
            var id = dataGridView2.Rows.Add();
            var actuations = dataGridView2.Rows.Add();
            var insertions = dataGridView2.Rows.Add();
            var pass = dataGridView2.Rows.Add();
            var fail = dataGridView2.Rows.Add();
            var clean = dataGridView2.Rows.Add();
            var life = dataGridView2.Rows.Add();
            var socket = dataGridView2.Rows.Add();
            var yield = dataGridView2.Rows.Add();

            // add rows names
            dataGridView2.Rows[mfg].HeaderCell.Value = "MFG";
            dataGridView2.Rows[id].HeaderCell.Value = "Adptr. ID";
            dataGridView2.Rows[actuations].HeaderCell.Value = "Actuations";
            dataGridView2.Rows[insertions].HeaderCell.Value = "Insertions";
            dataGridView2.Rows[pass].HeaderCell.Value = "Pass";
            dataGridView2.Rows[fail].HeaderCell.Value = "Fail";
            dataGridView2.Rows[clean].HeaderCell.Value = "Clean Alert";
            dataGridView2.Rows[life].HeaderCell.Value = "life";
            dataGridView2.Rows[socket].HeaderCell.Value = "Nº de sockets";
            dataGridView2.Rows[yield].HeaderCell.Value = "Yield";

            // add values
            i = 0;
            for (p = 0; p < dataFilesNo[0].numFiles; p++)
            {

                dataGridView2.Rows[mfg].Cells[dataFilesNo[p].no + p].Value = dataFilesNo[p].mfg;
                dataGridView2.Rows[id].Cells[dataFilesNo[p].no + p].Value = dataFilesNo[p].adpterType;
                dataGridView2.Rows[actuations].Cells[dataFilesNo[p].no + p].Value = dataFilesNo[p].actuations;
                dataGridView2.Rows[insertions].Cells[dataFilesNo[p].no + p].Value = dataFilesNo[p].insertions;
                dataGridView2.Rows[pass].Cells[dataFilesNo[p].no + p].Value = dataFilesNo[p].pass;
                dataGridView2.Rows[fail].Cells[dataFilesNo[p].no + p].Value = dataFilesNo[p].fail;
                dataGridView2.Rows[clean].Cells[dataFilesNo[p].no + p].Value = dataFilesNo[p].clean;
                dataGridView2.Rows[life].Cells[dataFilesNo[p].no + p].Value = dataFilesNo[p].life;
                dataGridView2.Rows[socket].Cells[dataFilesNo[p].no + p].Value = dataFilesNo[p].socket;
                dataGridView2.Rows[yield].Cells[dataFilesNo[p].no + p].Value = dataFilesNo[p].yield;

                string g = dataFilesNo[p].yield;
                g = g.Replace('.', ',');

                double numYield = System.Convert.ToDouble(g);//float.Parse(dataFiles[p].yield);
                if (numYield > 99 && numYield < 99.61)
                {
                    dataGridView2.Rows[yield].Cells[dataFilesNo[p].no + p].Style.BackColor = Color.Yellow;
                    
                    i++;
                }
                else if (numYield > 98 && numYield < 99.1)
                {
                    dataGridView2.Rows[yield].Cells[dataFilesNo[p].no + p].Style.BackColor = Color.Orange;
                   
                    i++;
                }
                else if (numYield <= 98)
                {
                    dataGridView2.Rows[yield].Cells[dataFilesNo[p].no + p].Style.BackColor = Color.Red;
                    
                    i++;
                }
            }

            // set life alarmes
            // set inicio alarme 
            //set fim alarme
            if (aviso[0].numWaring >= 0)
            {
                for (int k = 0; k < aviso[0].numWaring; k++)
                {
                    for (int j = 0; j <= aviso[k].idx_aux; j++)
                    {


                        if (aviso[k].adaptadores[j].no == selectedNo)
                        {

                            double total = System.Convert.ToDouble(float.Parse(dataFilesNo[0].insertions));
                            double numeroAcomparar = System.Convert.ToDouble(float.Parse(aviso[k].adaptadores[j].insertions));
                           
                            label8.Show();
                           label5.Show();
                           label2.Show();
                           label2.Text=( total - numeroAcomparar).ToString();
                           label4.Show();
                           label4.Text = (aviso[k].life).ToString();

                            double totalAux;
                            double referencia;

                            for (p = 0; p < dataFilesNo[0].numFiles; p++)
                            {
                                 totalAux = System.Convert.ToDouble(float.Parse(dataFilesNo[p].insertions));                                
                                 referencia = System.Convert.ToDouble(float.Parse(aviso[k].adaptadores[j].insertions));
                              
                                if (dataFilesNo[p].insertions == aviso[k].adaptadores[j].insertions)
                                {
                                    dataGridView2.Rows[insertions].Cells[dataFilesNo[p].no + p].Style.BackColor = Color.CornflowerBlue;
                                }

                            
                                if (totalAux >= (aviso[k].life+ referencia))
                                    {
                                    dataGridView2.Rows[insertions].Cells[dataFilesNo[p].no + p].Style.BackColor = Color.Red;
                                }
                                
                            }
                        }
                    }
                }
            }



                          
            // columns 3---------------------------------            
            dataGridView3.ColumnCount = 5;
            // add Columns name 
            dataGridView3.Columns[0].Name = "Insertions ";
            dataGridView3.Columns[1].Name = "Pass ";
            dataGridView3.Columns[2].Name = "Fail ";
            dataGridView3.Columns[3].Name = "Clean Count ";
            dataGridView3.Columns[4].Name = "Yield";
            //not short
            dataGridView3.Columns[0].SortMode = DataGridViewColumnSortMode.NotSortable;
            dataGridView3.Columns[1].SortMode = DataGridViewColumnSortMode.NotSortable;
            dataGridView3.Columns[2].SortMode = DataGridViewColumnSortMode.NotSortable;
            dataGridView3.Columns[3].SortMode = DataGridViewColumnSortMode.NotSortable;
            dataGridView3.Columns[4].SortMode = DataGridViewColumnSortMode.NotSortable;

            dataGridView3.Rows.Add();
            dataGridView3.Rows.Add();
            dataGridView3.Rows.Add();
            dataGridView3.Rows.Add();
            // columns 3--------------------------------- 
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.Hide();
            Form1 form = new Form1();
            form.Show();
        }
        public moreInfo[] MoreData(files[] dataFiles, int idx)
        {

            moreInfo[] more = new moreInfo[15];


            int numSocket = Int32.Parse(dataFiles[idx].socket);

            //read all text
            var path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\Info\\" + dataFiles[idx].name;
            string auxText = File.ReadAllText(path, Encoding.UTF8);
            label1.Text = (dataFiles[idx].date).ToString();

            int i;
            for (i = 0; i < numSocket; i++)
            {

                string auxText1 = Form1.getBetween(auxText, "Socket " + (i + 1), ";");
                auxText1 = String.Concat(auxText1, ";");
                more[i].socket = i + 1;
                more[i].insertions = Form1.getBetween(auxText1, "insertions ", "\n");
                more[i].pass = Form1.getBetween(auxText1, "pass ", "\n");
                more[i].fail = Form1.getBetween(auxText1, "fail ", "\n");
                more[i].cleanCount = Form1.getBetween(auxText1, "cleanCount ", "\n");
                more[i].yield = Form1.getBetween(auxText1, "yield ", ";");
               
            }

            return more;
        }

        public struct moreInfo
        {
            public int socket;
            public string insertions;
            public string pass;
            public string fail;
            public string cleanCount;
            public string yield;

        }

        public void dataGridView2_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            int j;
            //  Panel dynamicPanel = newPanel();

            for (j = 0; j < dataGridView2.ColumnCount; j++)
            {

                if (e.RowIndex + 1 == 0 && e.ColumnIndex == j)

                {
                    //MessageBox.Show((e.RowIndex + 1) + "  Row  " + (e.ColumnIndex + 1) + "  Column button clicked ");

                    panel2.Hide();
                    files[] dataFile = SelectNoFiles();
                    Add_griew3(dataFile, j);
                    
                }
            }
        }

        public void Add_griew3(files[] dataFiles, int idx)
        {   
            panel2.Show();
            button1.Show();
            

            string columnIdx = dataGridView2.Columns[idx].Name;
            string realIdx = columnIdx.Substring(columnIdx.Length - 1, 1);
            int realIdx_int = Int32.Parse(realIdx);

            moreInfo[] more = new moreInfo[33];
            more = MoreData(dataFiles, realIdx_int);

            int i;
            int numSocket = Int32.Parse(dataFiles[idx].socket);

            for (i = 0; i < numSocket; i++)
            {

                dataGridView3.Rows[i].HeaderCell.Value = "Socket " + (i + 1);
                dataGridView3.Rows[i].Cells[0].Value = more[i].insertions;
                dataGridView3.Rows[i].Cells[1].Value = more[i].pass;
                dataGridView3.Rows[i].Cells[2].Value = more[i].fail;
                dataGridView3.Rows[i].Cells[3].Value = more[i].cleanCount;
                dataGridView3.Rows[i].Cells[4].Value = more[i].yield;

                string g = more[i].yield;
                g = g.Replace('.', ',');

                double numYield = System.Convert.ToDouble(float.Parse(g));
                if (numYield > 99.1 && numYield < 99.61)
                {
                    dataGridView3.Rows[i].Cells[4].Style.BackColor = Color.Yellow;
                }
                else if (numYield > 98.7 && numYield < 99.10)
                {
                    dataGridView3.Rows[i].Cells[4].Style.BackColor = Color.Orange;
                }
                else if (numYield <= 98.7)
                {
                    dataGridView3.Rows[i].Cells[4].Style.BackColor = Color.Red;
                }
            }




        }

        private void button1_Click(object sender, EventArgs e)
        {
            panel2.Hide();
            button1.Hide();
        }

        public files[] SelectNoFiles()
        {
            Form1 frm = new Form1();
            files[] dataFiles = frm.GetData();

            files[] dataFilesNo = new files[dataFiles[0].numFiles];

            int p, z = 0;
            for (p = 0; p < dataFiles[0].numFiles; p++)
            {
                // hide not selected adpteres
                if (dataFiles[p].no == selectedNo)
                {
                    dataFilesNo[z] = dataFiles[p];
                    z++;
                }
                dataFilesNo[0].numFiles = z;

            }
            files auxOrder = new files();
            for (int i = 0; i < z; i++)
            {
                for (int k = 0; k < z; k++)
                {

                    if (DateTime.Compare(dataFilesNo[i].date, dataFilesNo[k].date) > 0)
                    {
                        auxOrder = dataFilesNo[k];
                        dataFilesNo[k] = dataFilesNo[i];
                        dataFilesNo[i] = auxOrder;
                    }
                }
                
            }
            dataFilesNo[0].numFiles = z;
            return dataFilesNo;
        }

        private void button2_Click(object sender, EventArgs e)
        {
          //  statistc stats = new statistc(selectedNo);
           // stats.Show();
        }
        
        private void dataGridView2_CellMouseEnter(object sender, DataGridViewCellEventArgs e)
        {
            for (int k = 0; k < dataFilesNo[0].numFiles; k++)
            {


                if (e.RowIndex + 1 == 0 && e.ColumnIndex == k)

                {

                    dataGridView2.Columns[k].HeaderCell.Style.ForeColor = Color.White;
                    dataGridView2.Columns[k].HeaderCell.Style.BackColor = Color.MediumBlue;

                }
            }
        }

        private void dataGridView2_CellMouseLeave(object sender, DataGridViewCellEventArgs e)
        {
            for (int l = 0; l < dataFilesNo[0].numFiles; l++)
            {

                if (e.RowIndex + 1 == 0 && e.ColumnIndex == l)

                {

                    dataGridView2.Columns[l].HeaderCell.Style.ForeColor = default;
                    dataGridView2.Columns[l].HeaderCell.Style.BackColor = default;
                }

            }
        }

       
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked == true)
            {
                dateTimePicker1.Show();
               
            }
            else
            {
                dateTimePicker1.Hide();
                
            }
            
            
            dateSelect();
        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            if (flag_datePicker == true)
            {
                flag_datePicker = false;
            }
            else
            {
                flag_datePicker = true;
            }
            dateSelect();
        }
        public void dateSelect()
        {
            panel2.Hide();
            button1.Hide();

            var counter = dataGridView2.Columns.Count;
            if (checkBox1.Checked == true || flag_datePicker!= auxFlag)
            {
                auxFlag = flag_datePicker;

                for (int i = 0; i < counter; i++)
                {

                    var dateAux = dataGridView2.Columns[i].HeaderCell.Value.ToString();
                    dateAux = dateAux.Remove(dateAux.Length - 9);

                    string datePicker = dateTimePicker1.Value.ToString();
                    datePicker = datePicker.Remove(dateAux.Length - 0);

                    if (dateAux == datePicker)
                    {

                        dataGridView2.Columns[i].Visible = true;
                    }
                    else
                    {

                        dataGridView2.Columns[i].Visible = false;
                    }
                }
            }
            else
            {
                for (int i = 0; i < counter; i++)
                {
                    dataGridView2.Columns[i].Visible = true;
                }
                }
        }

        private void label3_Click(object sender, EventArgs e)
        {
            this.Hide();
            table_info frm = new table_info(selectedNo);
            frm.Show();
        }

        private void label3_MouseEnter(object sender, EventArgs e)
        {
            label3.ForeColor = Color.White;
        }

        private void label3_MouseLeave(object sender, EventArgs e)
        {
            label3.ForeColor = default;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            this.Hide();
            Jobs job = new Jobs();
            job.Show();
        }
    }
}
