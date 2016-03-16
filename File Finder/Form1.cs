using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Threading;
using System.Diagnostics;

namespace File_Finder
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            folderBrowserDialog1.ShowDialog();

            if (folderBrowserDialog1.SelectedPath != "")
            {
                this.txtPath.Text = folderBrowserDialog1.SelectedPath;
                findFiles();
            }
        }       
        
        private void btnDeleteAll_Click(object sender, EventArgs e)
        {
            if (chkFiles.Items.Count > 0)
            {
                if (MessageBox.Show("Are you sure you wish to delete ALL items?", "Confirm delete", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    string results = "";

                    for (int i = 0; i < chkFiles.Items.Count; i++)
                    {
                        new FileInfo(chkFiles.Items[i].ToString()).Delete();
                        results += chkFiles.Items[i].ToString() + " - Deleted. \r\n";
                    }
                    MessageBox.Show(results);
                    ResetForm();
                }
            }
        }

        private void btnDeleteSelected_Click(object sender, EventArgs e)
        {
            int chkNum = 0;
            for (int j = 0; j < chkFiles.Items.Count; j++)
            {
                if (chkFiles.GetItemChecked(j) == true) { chkNum++; }
            }

            if(chkNum == 0)
            {
                MessageBox.Show("Please check at least one box");
                return;
            }

            if (chkFiles.Items.Count > 0)
            {
                if (MessageBox.Show("Are you sure you wish to delete the selected items?", "Confirm delete", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    string results = "";

                    for (int i = 0; i < chkFiles.Items.Count; i++)
                    {
                        if (chkFiles.GetItemChecked(i) == true)
                        {
                            new FileInfo(chkFiles.Items[i].ToString()).Delete();
                            results += chkFiles.Items[i].ToString() + " - Deleted. \r\n";
                        }
                    }
                    MessageBox.Show(results);
                    ResetForm();
                }
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            ResetForm();
            findFiles();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }



        private void ResetForm()
        {
            prgProgress.Value = 0;
            chkFiles.Items.Clear();
            lblTime.Text = "";
        }
        
        private List<String> DirSearch(string sDir)
        {
            List<String> files = new List<String>();
            string txtFiles = "";
            try
            {
                foreach (string file in Directory.GetFiles(sDir))
                {
                    if (file.StartsWith(sDir + "\\" + "._"))
                    {
                        files.Add(file);
                    }
                }
                foreach (string dir in Directory.GetDirectories(sDir))
                {
                    files.AddRange(DirSearch(dir));
                }
            }
            catch (System.Exception exc)
            {
                txtFiles += "\r\n" + exc.Message;
            }
            return files;
        } 
        
        private void findFiles()
        {
            Stopwatch stp = new Stopwatch();
            stp.Start();
            DirectoryInfo dir = new DirectoryInfo(folderBrowserDialog1.SelectedPath);

            string TopDIR = dir.ToString();
            List<String> files = DirSearch(TopDIR);
            prgProgress.Maximum = files.Count + 50;
            prgProgress.Value = 50;

            foreach (string file in files)
            {
                this.chkFiles.Items.Add(new CheckBox().Text = file);
                if (prgProgress.Value != prgProgress.Maximum) { prgProgress.Value += 1; lblTime.Text = "Time Taken: " + stp.Elapsed; }
            }

            stp.Stop();
            TimeSpan ts = stp.Elapsed;
            string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}", ts.Hours, ts.Minutes, ts.Seconds, ts.Milliseconds / 10);
            lblTime.Text = "Time Taken: " + elapsedTime + "   Files: " + files.Count.ToString();
        }
    }
}
