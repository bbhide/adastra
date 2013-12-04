﻿using System;
using System.Collections.Generic;
using System.ComponentModel;

using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Newtonsoft.Json;
using System.Net;
using System.IO;

/// <summary>
/// Used to describe a single file entry
/// </summary>
public struct CurrentSelection
{
    public int UserID;
    public int ExperimentID;
    public int SubjectID;
    public int ModalityID;
}


namespace edb_tool
{
    public partial class MainForm : Form
    {
        //MySql db;

        public CurrentSelection curr;

        string web;

        public MainForm()
        {
            InitializeComponent();
            this.CenterToScreen();

            #region gridview configuration
            //grid experiments
            dataGridView2.ReadOnly = true;
            dataGridView2.ColumnHeadersVisible = false;
            dataGridView2.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridView2.RowHeadersVisible = false;
            dataGridView2.AllowUserToDeleteRows = false;
            dataGridView2.AllowUserToAddRows = false;
            dataGridView2.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            //dataGridView2.CellContentClick += new DataGridViewCellEventHandler(dataGridView2_CellContentClick);
            dataGridView2.SelectionChanged += new EventHandler(dataGridView2_SelectionChanged);
            dataGridView2.AllowUserToResizeRows = false;
            dataGridView2.MultiSelect = false;

            //grid subjects
            dataGridView3.ReadOnly = true;
            dataGridView3.ColumnHeadersVisible = false;
            dataGridView3.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridView3.RowHeadersVisible = false;
            dataGridView3.AllowUserToDeleteRows = false;
            dataGridView3.AllowUserToAddRows = false;
            dataGridView3.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView3.SelectionChanged += new EventHandler(dataGridView3_SelectionChanged);
            dataGridView3.AllowUserToResizeRows = false;
            dataGridView3.MultiSelect = false;
            #endregion

            checkBoxShowSharedToMe.CheckedChanged += new EventHandler(checkBoxShowSharedToMe_CheckedChanged);

            tabControl2.Visible = false;

            curr.ExperimentID = -1;
            curr.SubjectID = -1;

            //Helper.ImportUsers(Helper.Scan("http://www.gipsa-lab.grenoble-inp.fr/annuaire.php?lettre=tous"));
            tabControl1.TabPages.RemoveAt(2);

            //var p1 = new edb_tool.EdbWebService.MyComplexType();
            //var o = new localhost.HelloExample();

            //var myComplexType = new localhost.MyComplexType { iduser = 1, username = "Jon" };
            //localhost.ArrayOfString res = o.batman(myComplexType);
            //localhost.game p = o.gamelist(1999);
            //var products = o.getData();
           // int userid = o.batman();
            //var p2 = new edb_tool.EdbWebService

            //web = "http://localhost/edb-json/";
            //web = "http://si-devel.gipsa-lab.grenoble-inp.fr/edm/";

            //var json = JsonHelper.DownloadJson(web + "user/list.php");

            //List<GUser> users = JsonConvert.DeserializeObject<List<GUser>>(json);

            //var o = JsonConvert.SerializeObject(users);

            //JsonHelper.SendJson(o, web + "user/add.php");

            //DataFactory.GetDataProvider().ListExperiments(1999);
            

        }

        void checkBoxShowSharedToMe_CheckedChanged(object sender, EventArgs e)
        {
            BindExperimentGrid();
        }

        void dataGridView3_SelectionChanged(object sender, EventArgs e)
        {
            //load the modalities for this experimentid/subject id

            if (dataGridView3.SelectedRows.Count > 0)
            {
                panel1.Enabled = true;
                int idcolumn = Helper.LocateColumnInGrid("idsubject", dataGridView3);
                if (idcolumn != -1)
                {
                    object stringid = dataGridView3.SelectedRows[0].Cells[idcolumn].Value;
                    int id = Convert.ToInt32(stringid);
                    curr.SubjectID = id;
                }
            }
            else
            {
                panel1.Enabled = false;
            }
            
            ConstructTabsModalities();

            if (tabControl2.TabPages.Count > 0) tabControl2.SelectedIndex = 0;
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            Login lf = new Login(this);
            lf.ShowDialog();

            BindExperimentGrid();

            comboBox4.DisplayMember = "name";
            comboBox4.ValueMember = "idmodality";
            comboBox4.DataSource = ProviderFactory.GetDataProvider().ListModalities();

           
            //Application.OpenForms["Login"].BringToFront();
            //var list = ProviderFactory.GetDataProvider().ListExperimentsSharedToTheUserByOthers(curr.UserID);
        }

        void dataGridView2_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridView2.SelectedRows.Count > 0)
            {
                int idexperiment = Convert.ToInt32(dataGridView2.SelectedRows[0].Cells[Helper.LocateColumnInGrid("idexperiment", dataGridView2)].Value);
                curr.ExperimentID = idexperiment;
                
                //load subjects for this experiment
                dataGridView3.DataSource = ProviderFactory.GetDataProvider().ListSubjectsByExperimentId(idexperiment, curr.UserID);
                if (dataGridView3.Rows.Count > 0)
                    dataGridView3.Rows[0].Selected = true;

                //set visibility
                dataGridView3.Columns[0].Visible = false;
                dataGridView3.Columns[2].Visible = false;
                dataGridView3.Columns[3].Visible = false;
                dataGridView3.Columns[4].Visible = false;
                dataGridView3.Columns[5].Visible = false;
            }
            else //no subjects available in this experiment
            {
                curr.SubjectID = -1; 
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ManageExperiments me = new ManageExperiments(this);
            me.Show();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            ManageSubjects ms = new ManageSubjects(this, false, -1);
            ms.Show();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            ManageModalities mm = new ManageModalities(this);
            mm.Show();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            ManageTags mt = new ManageTags();
            mt.Show();
        }

        /// <summary>
        /// Add a tab per modality and lists the files for each
        /// </summary>
        public void ConstructTabsModalities()
        {
            int selectedTab = tabControl2.SelectedIndex;

            tabControl2.TabPages.Clear();
            List<GModality> modalities = ProviderFactory.GetDataProvider().ListModalitiesByExperimentID(curr.ExperimentID);

            if (modalities.Count > 0 && curr.SubjectID != -1)
            {
                tabControl2.Visible = true;

                foreach(GModality m in modalities)
                {
                    TabPage tb = new TabPage(m.name);
                    tabControl2.TabPages.Add(tb);
                    tb.Tag = m.idmodality;

                    //add control gridview
                    DataGridView dgv = new DataGridView();

                    #region grid settings
                    //dgv.ReadOnly = true;
                    dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                    dgv.RowHeadersVisible = false;
                    dgv.AllowUserToDeleteRows = false;
                    dgv.AllowUserToAddRows = false;
                    dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                    dgv.Size = new Size(tb.Size.Width - 1,tb.Size.Height-1);
                    dgv.AllowUserToResizeRows = false;
                    dgv.MultiSelect = false;

                    System.Windows.Forms.DataGridViewCheckBoxColumn Column1;
                    Column1 = new System.Windows.Forms.DataGridViewCheckBoxColumn();
                    Column1.Name = "checkbox1";

                    #endregion

                    tb.Controls.Add(dgv);

                    //load files
                    dgv.DataSource = ProviderFactory.GetDataProvider().ListFilesByExperimentSubjectModalityID(curr.ExperimentID, curr.SubjectID, m.idmodality);

                    if (dgv.Columns.Count>0)
                            dgv.Columns[0].Visible = false; //hide id

                    dgv.Columns.Insert(1, Column1); //add checkbox
                    dgv.Columns[1].Width = 28;

                    foreach(DataGridViewColumn column in dgv.Columns) //needed because otherwise the checkbox can not be set
                    {
                        if (column.DisplayIndex >1)
                           column.ReadOnly = true;

                        if (column.Name == "filename" || column.Name == "pathname" || column.Name == "checkbox1" || column.Name == "tags")
                        {
                            column.Visible = true;
                        }
                        else
                        {
                            column.Visible = false;
                        }
                    }

                    #region button location
                    DataGridViewLinkColumn Editlink = new DataGridViewLinkColumn();
                    Editlink.UseColumnTextForLinkValue = true;
                    Editlink.HeaderText = "Location";
                    Editlink.DataPropertyName = "lnkColumn";
                    Editlink.LinkBehavior = LinkBehavior.SystemDefault;
                    Editlink.Text = "Open";
                    dgv.Columns.Add(Editlink);
                    dgv.Columns[dgv.Columns.Count - 1].Width = 50;
                    #endregion

                    dgv.CellContentClick += new DataGridViewCellEventHandler(dgv_CellContentClick);
                }

                tabControl2.SelectedIndex = selectedTab;
            }
            else
            {
                tabControl2.Visible = false;
            }
        }

        void dgv_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            DataGridView dgv = (DataGridView)sender;

            if (dgv.Columns[e.ColumnIndex].HeaderText.ToLower() == "location")
            {
                int columnindex  = Helper.LocateColumnInGrid("pathname",dgv);
                string fullpath = Convert.ToString(dgv.SelectedRows[0].Cells[columnindex].Value);

                string command = fullpath.Substring(0, fullpath.LastIndexOf("\\"));
                if (fullpath.Length>0)
                    System.Diagnostics.Process.Start(command);
            }
        }

        /// <summary>
        /// Add files
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button13_Click(object sender, EventArgs e)
        {
            if (tabControl2.TabCount == 0 || tabControl2.Visible == false)
            {
                MessageBox.Show("You need at least one modality linked before you can add files!",
                "Warning",
                MessageBoxButtons.OK);
                return;
            }
           
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Multiselect = true;

            DialogResult result = ofd.ShowDialog();
            if (result == DialogResult.OK) // Test result.
            {
                foreach (String f in ofd.FileNames)
                {
                    //insert in db only name for now
                    GFile file = new GFile(-1, Helper.GetFileShortName(f), f);
                    
                    long idfile = ProviderFactory.GetDataProvider().AddFile(file);

                    int idmodality = Convert.ToInt32(tabControl2.SelectedTab.Tag);//gets the selected tab modality id
                    ProviderFactory.GetDataProvider().AssociateFile(curr.ExperimentID, curr.SubjectID, idmodality, idfile);
                    
                    //add in table list_file
                }
            }

            ConstructTabsModalities();
        }

        /// <summary>
        /// Remove modality
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button12_Click(object sender, EventArgs e)
        {
            //show warning

            int idmodality = Convert.ToInt32(tabControl2.SelectedTab.Tag);

            //the ids should be already loaded in the grid for thi modality

            //delete from file - removes the main information about the file entry

            //delete from list_file - - it detaches the files from the experiment and the modality
            //db.DeleteFilesByExperimentIdSubjectIdModalityId(curr.ExperimentID, curr.SubjectID, idmodality);

            //delete from list_modalities - it detaches this modality from the experiment
        }

        /// <summary>
        /// Delete files
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button11_Click(object sender, EventArgs e)
        {
            if (tabControl2.TabCount == 0 || tabControl2.Visible == false)
            {
                MessageBox.Show("You need at least one modality linked before you can add files!",
                "Warning",
                MessageBoxButtons.OK);
                return;
            }

            TabPage tp = tabControl2.SelectedTab;
            DataGridView dgv = (DataGridView)tp.Controls[0];
            
            int cbindex = Helper.LocateColumnInGrid("checkbox1", dgv);
            int[] fileids = GetSelectedItems(dgv, cbindex);
            int count = fileids.Length;

            if (fileids.Length > 0)
            {
                var confirmResult = MessageBox.Show("Are you sure you want to delete " + count + " item(s)??", "Confirm Delete!!", MessageBoxButtons.YesNo);
                if (confirmResult == DialogResult.Yes)
                {
                    int idfileindex = Helper.LocateColumnInGrid("idfile", dgv);

                    foreach (DataGridViewRow raw in dgv.Rows)
                    {
                        bool isForDelete = (raw.Cells[cbindex].Value == null) ? false : Convert.ToBoolean(raw.Cells[cbindex].Value);
                        if (isForDelete)
                        {
                            int idfile = Convert.ToInt32(raw.Cells[idfileindex].Value);

                            //remove from list_file table (provides the connection between the file and the experiment, subject)
                            ProviderFactory.GetDataProvider().DeleteFilesByFileIdFromListFile(idfile);

                            //remove associated tags
                            ProviderFactory.GetDataProvider().RemoveTags(idfile);

                            //remove from file table
                            ProviderFactory.GetDataProvider().DeleteFilesByFileId(idfile);

                            
                        }
                    }

                    ConstructTabsModalities();
                }
            }
            else
            {
                MessageBox.Show("No items selected for deletion. Please use the checkboxes next to each item.", "Warning!", MessageBoxButtons.OK);
            }
        }

        private void button15_Click(object sender, EventArgs e)
        {
            if (tabControl2.TabCount == 0 || tabControl2.Visible == false)
            {
                MessageBox.Show("You need at least one modality linked before you can select files!",
                "Warning",
                MessageBoxButtons.OK);
                return;
            }

            TabPage tp = tabControl2.SelectedTab;
            DataGridView dgv = (DataGridView)tp.Controls[0];
            int cbindex = Helper.LocateColumnInGrid("checkbox1", dgv);

            foreach (DataGridViewRow raw in dgv.Rows)
            {
                raw.Cells[cbindex].Value = true;
            }
        }

        private void button16_Click(object sender, EventArgs e)
        {
            if (tabControl2.TabCount == 0 || tabControl2.Visible == false)
            {
                MessageBox.Show("You need at least one modality linked before you can deselect files!",
                "Warning",
                MessageBoxButtons.OK);
                return;
            }

            TabPage tp = tabControl2.SelectedTab;
            DataGridView dgv = (DataGridView)tp.Controls[0];
            int cbindex = Helper.LocateColumnInGrid("checkbox1", dgv);

            foreach (DataGridViewRow raw in dgv.Rows)
            {
                raw.Cells[cbindex].Value = false;
            }
        }

        private void button17_Click(object sender, EventArgs e)
        {
            ManageSubjects ms = new ManageSubjects(this, true, curr.ExperimentID);
            ms.Show();
        }

        private void button14_Click(object sender, EventArgs e)
        {
            if (tabControl2.TabCount == 0 || tabControl2.Visible == false)
            {
                MessageBox.Show("You need at least one modality linked before you can add files!",
                "Warning",
                MessageBoxButtons.OK);
                return;
            }

            int[] fileids = GetSelectedFiles();

            if (fileids.Length <= 0)
            {
                MessageBox.Show("No files selected. Use the checkbox next to each file!",
                "Warning",
                MessageBoxButtons.OK);
                return;
            }

            //int fileid = 
            TagSelector ts = new TagSelector(this);
            ts.Show();
        }

        public int[] GetSelectedItems(DataGridView dgv,int selectedColumnIndex)
        {
            List<int> ids = new List<int>();

            int count = 0;

            foreach (DataGridViewRow raw in dgv.Rows)
            {
                bool isSelected = (raw.Cells[selectedColumnIndex].Value == null) ? false : Convert.ToBoolean(raw.Cells[selectedColumnIndex].Value);
                if (isSelected)
                {
                    ids.Add(Convert.ToInt32(raw.Cells[0].Value));
                    count++;
                }
            }

            return ids.ToArray();
        }

        public int[] GetSelectedFiles()
        {
            TabPage tp = tabControl2.SelectedTab;
            DataGridView dgv = (DataGridView)tp.Controls[0];
            int cbindex = Helper.LocateColumnInGrid("checkbox1", dgv);

            int[] fileids = GetSelectedItems(dgv, cbindex);

            return fileids;
        }

        private void buttonSwitchTabFiles_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedIndex = 1;
        }

        /// <summary>
        /// Add modality
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button10_Click(object sender, EventArgs e)
        {
            int idmodality = Convert.ToInt32(comboBox4.SelectedValue);
            ProviderFactory.GetDataProvider().AddModalityToExperiment(idmodality, curr.ExperimentID);

            ConstructTabsModalities();

            tabControl2.SelectedIndex = tabControl2.TabPages.Count - 1;
        }

        public void BindExperimentGrid()
        {
            var ownExp = ProviderFactory.GetDataProvider().ListExperiments(curr.UserID);
            List<GExperiment> allExp;

            if (checkBoxShowSharedToMe.Checked)
            {
                var sharedExp = ProviderFactory.GetDataProvider().ListExperimentsSharedToTheUserByOthers(curr.UserID);
                allExp = ownExp.Union(sharedExp).ToList();
            }
            else
            {
                allExp = ownExp;
            }

            dataGridView2.DataSource = allExp;

            //experiment datagridview layout
            dataGridView2.Columns[0].Visible = false;
            dataGridView2.Columns[2].Visible = false;
            dataGridView2.Columns[3].Visible = false;
            dataGridView2.Columns[4].Visible = false;
            dataGridView2.Columns[5].Visible = false;
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}