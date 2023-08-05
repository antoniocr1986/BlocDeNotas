using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BlocNotasWF
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }

        public string GetTextBoxData()
        {
            return textBoxBuscar.Text;
        }

        private void ButtonBuscar_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
