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
    public partial class Configuracion : Form
    {
        public Configuracion()
        {
            InitializeComponent();
        }

        private void Configuracion_Load(object sender, EventArgs e)
        {

        }

        private void italicToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void labelFormato_Click(object sender, EventArgs e)
        {
            contextMenuStripFormato.Show(Cursor.Position);
        }

        private void buttonSoporteTecnico_Click(object sender, EventArgs e)
        {
            // Número de teléfono en formato internacional sin signos de más, espacios ni guiones
            string phoneNumber = "628054646";
            string url = $"https://wa.me/{phoneNumber}";

            // Abre el enlace en el navegador predeterminado
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
            {
                FileName = url,
                UseShellExecute = true
            });
        }
    }
}
