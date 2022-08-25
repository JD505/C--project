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
using System.Threading;
using System.Xml.Serialization;

namespace WindowsFormsApp1
{
    
    public struct files 
    {
        public int numAdpter;
        public string numAdpter_name;
        public string name;
        public string adpterType;
        public string no;
        public string mfg;
        public string actuations;
        public string insertions;
        public string pass;
        public string fail;
        public string clean;
        public string life;
        public string socket;
        public string yield;
        public int numFiles;
        public DateTime date;

    }
    public partial class Form1 : Form

    {   
        
        files[] dataFiles;  
        public int j;
        Warning[] aviso;
        public Form1()
        {
           
            InitializeComponent();

            this.dataGridView1.DoubleBuffered(true);
            
            if (File.Exists(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\Not_Change_This\\dontChange"))
            {
                aviso = SetAlarme.Deserialize();
            }
            else
            {
              aviso = new Warning[10000];
            }
            dataFiles = NewestFile();

            int p;

            for (p = 0; p < dataFiles[0].numFiles; p++)
            {

                // add Columns name
                dataGridView1.ColumnCount = dataFiles[0].numFiles;
                dataGridView1.Columns[p].Name = dataFiles[p].no;
                dataGridView1.Columns[p].SortMode = DataGridViewColumnSortMode.NotSortable;
                dataGridView1.Columns[p].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dataGridView1.Columns[p].Width = 120;
            }

            // index data grid
            var mfg = dataGridView1.Rows.Add();
            var id = dataGridView1.Rows.Add();
            var actuations = dataGridView1.Rows.Add();
            var insertions = dataGridView1.Rows.Add();
            var alertInsertions = dataGridView1.Rows.Add();
            var pass = dataGridView1.Rows.Add();
            var fail = dataGridView1.Rows.Add();
            var clean = dataGridView1.Rows.Add();
            var life = dataGridView1.Rows.Add();
            var socket = dataGridView1.Rows.Add();
            var yield = dataGridView1.Rows.Add();
            var date = dataGridView1.Rows.Add();
            

            // add rows names
            dataGridView1.Rows[mfg].HeaderCell.Value = "MFG";
            dataGridView1.Rows[id].HeaderCell.Value = "Adptr. ID";
            dataGridView1.Rows[actuations].HeaderCell.Value = "Actuations";
            dataGridView1.Rows[insertions].HeaderCell.Value = "Total Insertions";
            dataGridView1.Rows[pass].HeaderCell.Value = "Pass";
            dataGridView1.Rows[fail].HeaderCell.Value = "Fail";
            dataGridView1.Rows[clean].HeaderCell.Value = "Clean Alert";
            dataGridView1.Rows[life].HeaderCell.Value = "life";
            dataGridView1.Rows[socket].HeaderCell.Value = "Nº de sockets";
            dataGridView1.Rows[yield].HeaderCell.Value = "Yield";
            dataGridView1.Rows[date].HeaderCell.Value = "Information Date";
            dataGridView1.Rows[alertInsertions].HeaderCell.Value = "Alarm Insertions";
           
            // add values
            for (p = 0; p < dataFiles[0].numFiles; p++)
            {

                dataGridView1.Rows[mfg].Cells[dataFiles[p].no].Value = dataFiles[p].mfg;
                dataGridView1.Rows[id].Cells[dataFiles[p].no].Value = dataFiles[p].adpterType;
                dataGridView1.Rows[actuations].Cells[dataFiles[p].no].Value = dataFiles[p].actuations;
                dataGridView1.Rows[insertions].Cells[dataFiles[p].no].Value = dataFiles[p].insertions;
                dataGridView1.Rows[pass].Cells[dataFiles[p].no].Value = dataFiles[p].pass;
                dataGridView1.Rows[fail].Cells[dataFiles[p].no].Value = dataFiles[p].fail;
                dataGridView1.Rows[clean].Cells[dataFiles[p].no].Value = dataFiles[p].clean;
                dataGridView1.Rows[life].Cells[dataFiles[p].no].Value = dataFiles[p].life;
                dataGridView1.Rows[socket].Cells[dataFiles[p].no].Value = dataFiles[p].socket;
                dataGridView1.Rows[yield].Cells[dataFiles[p].no].Value = dataFiles[p].yield;
                dataGridView1.Rows[date].Cells[dataFiles[p].no].Value = dataFiles[p].date;

              

                // avisos yield
                string g =  dataFiles[p].yield;
                g = g.Replace('.', ',');

                double numYield = System.Convert.ToDouble(g);//float.Parse(dataFiles[p].yield);
                if (numYield > 99 && numYield < 99.61)
                {
                    dataGridView1.Rows[yield].Cells[dataFiles[p].no].Style.BackColor = Color.Yellow;
                }
                else if (numYield > 98 && numYield < 99.1)
                {
                    dataGridView1.Rows[yield].Cells[dataFiles[p].no].Style.BackColor = Color.Orange;
                } 
                else if (numYield <= 98)
                {
                    dataGridView1.Rows[yield].Cells[dataFiles[p].no].Style.BackColor = Color.Red;
                }
              

                // set life alarmes
                
                if (aviso[0].numWaring >= 0)
                {
                    for (int k = 0; k < aviso[0].numWaring; k++)
                    {
                        for (int j = 0; j <=aviso[k].idx_aux; j++){
                        
                            
                            if (aviso[k].adaptadores[j].no == dataFiles[p].no)
                            {
                               
                                double total = System.Convert.ToDouble(float.Parse(dataFiles[p].insertions));
                                double numeroAcomparar = System.Convert.ToDouble(float.Parse(aviso[k].adaptadores[j].insertions));

                                if (total - numeroAcomparar >= 0)
                                {
                                    dataGridView1.Rows[alertInsertions].Cells[dataFiles[p].no].Value = total - numeroAcomparar;
                                }
                                if (total > (numeroAcomparar + (0.8 * aviso[k].life)))
                                {
                                    dataGridView1.Rows[alertInsertions].Cells[dataFiles[p].no].Style.BackColor = Color.Yellow;
                                }

                                if (total > (numeroAcomparar + (0.9 * aviso[k].life)))
                                {
                                    dataGridView1.Rows[alertInsertions].Cells[dataFiles[p].no].Style.BackColor = Color.Orange;
                                }
                                if (total > (numeroAcomparar +  aviso[k].life))
                                {

                                    dataGridView1.Rows[alertInsertions].Cells[dataFiles[p].no].Style.BackColor = Color.Red;
                                    dataGridView1.Rows[alertInsertions].Cells[dataFiles[p].no].Value = "+" + ((total - numeroAcomparar)- aviso[k].life);
                                }
                            }                           
                        }                        
                    }

                   
                    
                }
                
            }
            for (p = 0; p < dataFiles[0].numFiles; p++)
            {
                if (dataGridView1.Rows[alertInsertions].Cells[dataFiles[p].no].Value == null ||
                            dataGridView1.Rows[alertInsertions].Cells[dataFiles[p].no].Value == DBNull.Value ||
                            String.IsNullOrWhiteSpace(dataGridView1.Rows[alertInsertions].Cells[dataFiles[p].no].Value.ToString()))
                {

                    dataGridView1.Rows[alertInsertions].Cells[dataFiles[p].no].Value = "------";
                }
            }
             
            
            //select by type
            System.Object[] ItemObject = new System.Object[dataFiles[0].numAdpter+1];
            for (int k = 0; k <dataFiles[0].numAdpter; k++)
            {
                
                ItemObject[k] = dataFiles[k].numAdpter_name;
                
            }
            ItemObject[dataFiles[0].numAdpter] = "all";
           comboBox2.Items.AddRange(ItemObject);


    
           
        }
        public files[] NewestFile()
        {
            dataFiles = GetData();

            int size = 0;
            string oldNo = string.Empty;
            for (int p = dataFiles[0].numFiles; p >= 0; p--)
            {
                // hide equal adpteres               
                if (dataFiles[p].no != oldNo)
                {
                    size++;
                    
                }
                oldNo = dataFiles[p].no;
            }
            
            files[] dataNewFiles = new files[size];          
            int z = 0, l = 0; 
            oldNo = string.Empty;          
            dataNewFiles[0].no = dataFiles[dataFiles[0].numFiles - 1].no;

            for (int p = 0; p < dataFiles[0].numFiles; p++)
            {

                if (dataFiles[p].no != oldNo)
                {

                    dataNewFiles[z] = dataFiles[p];
                    z++;
                }

                oldNo = dataFiles[p].no;

            }
                for (int i = 0; i < z; i++)
                {
                    for (int p = 0; p < dataFiles[0].numFiles; p++)
                    {
                        if ((dataFiles[p].no == dataNewFiles[i].no) && (DateTime.Compare(dataFiles[p].date, dataNewFiles[i].date) > 0))
                        {
                            dataNewFiles[i] = dataFiles[p];
                        }

                    }
                }


            for (int k = 0; k < dataFiles[0].numAdpter; k++)
            {
                dataNewFiles[k].numAdpter_name = dataFiles[k].numAdpter_name;
 

            }

            dataNewFiles[0].numAdpter = dataFiles[0].numAdpter;
            dataNewFiles[0].numFiles = z;


            return dataNewFiles;
        }
        public files[] GetData()
        {
           
            int i = 0, l = 0, num = 0;
            string nameFiles, nameFilesOld = string.Empty;
            string auxText = string.Empty;
            DateTime[] dataTdata = new DateTime[1000];
            
            // Create a reference to the current directory
            DirectoryInfo di = new DirectoryInfo(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\Info\\");
            FileInfo[] fi = di.GetFiles();

           
            int numSize=0; // get number of files to set array size
            foreach (FileInfo fiT in fi)
            {
                if (fiT.Name.Contains(".txt")){
                    numSize++;
                }

            }
           
            //set array size    
            dataFiles = new files[numSize+1];
            string dateAux = string.Empty;
            
            // open current directory
            foreach (FileInfo fiTemp in fi)
            {

                nameFiles = fiTemp.Name;             
                dataFiles[i].name = nameFiles; // save on array name of files
               

                // get number of files
                if (dataFiles[i].name.Contains(".txt"))
                {
                    num++;
                    dataFiles[0].numFiles = num;
                }
                
                //add value to all fields struct
                var path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\Info\\" + dataFiles[i].name;
                auxText = File.ReadAllText(path, Encoding.UTF8);

                dataFiles[i].no = getBetween(auxText, "No ", "\n");
                dataFiles[i].mfg = getBetween(auxText, "MFG ", "\n");
                dataFiles[i].actuations = getBetween(auxText, "Actuations   ", "\n");
                dataFiles[i].insertions = getBetween(auxText, "Insertions   ", "\n");
                dataFiles[i].pass = getBetween(auxText, "Pass   ", "\n");
                dataFiles[i].fail = getBetween(auxText, "Fail   ", "\n");
                dataFiles[i].clean = getBetween(auxText, "Clean Alert   ", "\n");
                dataFiles[i].life = getBetween(auxText, "life   ", "\n");
                dataFiles[i].socket = getBetween(auxText, "socket  ", "\n");
                dataFiles[i].yield = getBetween(auxText, "Yield   ", "\n");
                dataFiles[i].adpterType = getBetween(auxText, "Adptr. ID ", "\n");

                dateAux = getBetween(auxText, "time ", "\n"); //dataFiles[i].date = getBetween(auxText, "time ", "\n");
                dataFiles[i].date=(DateTime.ParseExact(dateAux, "dd-MM-yyyy-HH-mm", null));                              
               
                
                
                bool flag_difrentType = true;
                
                for (int p = 0; p <= l; p++)
                {
                    // numero de adptadores
                    if (flag_difrentType == true)
                    {
                        if (dataFiles[i].adpterType != dataFiles[p].numAdpter_name)
                        {
                            flag_difrentType = true;


                            

                        }
                        else
                        {
                            flag_difrentType = false;
                        }

                    }
                }
                if (flag_difrentType == true)
                {
                    dataFiles[l].numAdpter_name = dataFiles[i].adpterType;
                    l++;
                    dataFiles[0].numAdpter = l;
                   
                }
               

                i++;
            }
      
            return dataFiles;
        }

        public static string getBetween(string strSource, string strStart, string strEnd) //function for get text between
        {
            int Start, End;
            if (strSource.Contains(strStart) && strSource.Contains(strEnd))
            {
                Start = strSource.IndexOf(strStart, 0) + strStart.Length;
                End = strSource.IndexOf(strEnd, Start);
                if (End < 0)
                {
                    End = 0;
                }
                return strSource.Substring(Start, End - Start);
            }
            else
            {
                return "";
            }

        }

   
        public void dataGridView1_CellClick_1(object sender, DataGridViewCellEventArgs e)
        {
            
            

            for (int j = 0; j < dataGridView1.ColumnCount; j++)
            {

                if (e.RowIndex + 1 == 0 && e.ColumnIndex == j)

                {
                    //MessageBox.Show((e.RowIndex + 1) + "  Row  " + (e.ColumnIndex + 1) + "  Column button clicked ");

                    //close this form and open table_info
                
                    this.Hide();
                    table_info cell = new table_info(dataGridView1.Columns[j].Name);
                    cell.Show();
                  

                    
                }



            }

        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (textBox1.Text != string.Empty )
            {
                textBox1.Text = textBox1.Text.Remove(textBox1.Text.Length - textBox1.Text.Length);
            }
          
            if (comboBox2.SelectedItem != null) { 
            
                string colunaOld = string.Empty;


                for (int k = 0; k < dataFiles[0].numFiles; k++)
                {

                    if (comboBox2.SelectedItem.ToString() == dataFiles[k].adpterType || comboBox2.SelectedItem.ToString() == "all")

                    {
                        dataGridView1.Columns[k].Visible = true;
                    }
                    else
                    {
                        dataGridView1.Columns[k].Visible = false;
                    }

                    //hide columns from same adapter
                    if (dataGridView1.Columns[k].Name == colunaOld)

                    {
                       dataGridView1.Columns[k].Visible = false;

                    }

                    colunaOld = dataGridView1.Columns[k].Name;
                }
            }
        }

        private void dataGridView1_CellMouseEnter(object sender, DataGridViewCellEventArgs e)
        {
            for (int k = 0; k < dataFiles[0].numFiles; k++)
            {
                

                if (e.RowIndex + 1 == 0 && e.ColumnIndex == k)

                {
                    
                    dataGridView1.Columns[k].HeaderCell.Style.ForeColor = Color.White;
                    dataGridView1.Columns[k].HeaderCell.Style.BackColor = Color.MediumBlue;
                   
                }
            }
        }

        private void dataGridView1_CellMouseLeave(object sender, DataGridViewCellEventArgs e)
        {
            for (int l = 0; l < dataFiles[0].numFiles; l++)
            {

                if (e.RowIndex + 1 == 0 && e.ColumnIndex == l)

                {
                    
                    dataGridView1.Columns[l].HeaderCell.Style.ForeColor = default;
                    dataGridView1.Columns[l].HeaderCell.Style.BackColor = default;
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            SetAlarme set = new SetAlarme();
            set.Show();
        }

        

        private void button3_Click(object sender, EventArgs e)
        {
            this.Hide();
            Jobs job = new Jobs();
            job.Show();
        }

        private void button4_Click(object sender, EventArgs e)
        {                        

            var counter = dataGridView1.Columns.Count;

            if (textBox1.Text.Length >= 15)
            {
                MessageBox.Show("Serial Number too large. \nPlease Insert a Correct Serial Number.", "DATA IO Manager", MessageBoxButtons.OK, MessageBoxIcon.Error);
                textBox1.Text = textBox1.Text.Remove(textBox1.Text.Length - textBox1.Text.Length);
            }          
            else
            {
                for (int i = 0; i < counter; i++)
                {
                    if (textBox1.Text == null || textBox1.Text == string.Empty)
                    {
                        dataGridView1.Columns[i].Visible = true;                       
                    }
                    else
                    {
                        
                        if (String.Equals(textBox1.Text, dataGridView1.Columns[i].HeaderCell.Value.ToString(),StringComparison.OrdinalIgnoreCase))
                        {

                            dataGridView1.Columns[i].Visible = true;

                        }
                        else
                        {
                            dataGridView1.Columns[i].Visible = false;
                        }
                    }
                }
                
            }
           
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(textBox1.Text) || textBox1.Text!= string.Empty)
            {
                comboBox2.SelectedItem = null;
            }
        }

        private void label5_Click(object sender, EventArgs e)
        {
            this.Hide();
            Form1 frm = new Form1();
            frm.Show();
        }

        private void label5_MouseEnter(object sender, EventArgs e)
        {
           
            label5.ForeColor = Color.White;
            
        }

        private void label5_MouseLeave(object sender, EventArgs e)
        {
            label5.ForeColor = default;
           
        }
    }

    public static class ExtensionMethods
    {
        public static void DoubleBuffered(this DataGridView dgv, bool setting)
        {
            Type dgvType = dgv.GetType();
            PropertyInfo pi = dgvType.GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic);
            pi.SetValue(dgv, setting, null);
        }
    }

}

