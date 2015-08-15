using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace XProof
{
    public partial class ExceptionDialog : Form
    {
        public ExceptionDialog()
        {
            InitializeComponent();
            Icon = Application.OpenForms.Cast<Form>().First(f => f.ShowIcon).Icon;
        }

        private Exception _Exception;

        public Exception Exception
        {
            get { return _Exception; }
            set
            {
                _Exception = value;
                message.Text = value.Message;
                details.Text = value.ToString();
            }
        }
    }
}
