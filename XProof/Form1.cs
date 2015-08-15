﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace XProof
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            Disposed += Form1_Disposed;
            Application.ThreadException += Application_ThreadException;
        }

        private readonly Proofreader Proof = new Proofreader();

        void Form1_Disposed(object sender, EventArgs e)
        {
            Proof.Dispose();
        }

        void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
        {
            using (var dlg = new ExceptionDialog())
            {
                dlg.Exception = e.Exception;
                dlg.ShowDialog(this);
            }
        }

        private void chooseButton_Click(object sender, EventArgs e)
        {
            if (DialogResult.OK == openFileDialog.ShowDialog(this))
            {
                if (openFileDialog.FileNames.Length == 1)
                {
                    filenames.Text = openFileDialog.FileName;
                }
                else
                {
                    filenames.Text = "\"" + string.Join("\", \"", openFileDialog.FileNames) + "\"";
                }
            }
        }

        private void runButton_Click(object sender, EventArgs e)
        {
            UseWaitCursor = true;
            runButton.Enabled = false;
            backgroundWorker.RunWorkerAsync(openFileDialog.FileNames);
        }

        private void backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            var filenames = e.Argument as string[];
            if (filenames != null)
            {
                Proof.DoProof(filenames);
                e.Result = false;
            }
            else
            {
                Proof.Shutdown();
                e.Result = true;
            }
        }

        private void backgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            var after_shutdown = (bool)e.Result;
            if (after_shutdown) return;

            bool shutdown;
            if (e.Error == null)
            {
                this.Activate();
                var result = MessageBox.Show(this, "Grammar check completed.\r\nDo you want to close the Word window?", "Message - XProof", MessageBoxButtons.YesNo);
                shutdown = (result == DialogResult.Yes);
            }
            else
            {
                using (var dlg = new ExceptionDialog())
                {
                    dlg.Exception = e.Error;
                    dlg.ShowDialog(this);
                }
                shutdown = true;
            }
            backgroundWorker.RunWorkerAsync(null);
            runButton.Enabled = true;
            UseWaitCursor = false;
        }

        private void closeButton_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
