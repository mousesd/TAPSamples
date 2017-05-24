using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp8
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        internal void Initialize()
        {
            Thread.Sleep(3000);
        }

        internal Task InitializeAsync()
        {
            return Task.Delay(3000);
            //throw new NotImplementedException();
        }
    }
}
