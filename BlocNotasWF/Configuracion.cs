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
        MainForm mainform;

        public Configuracion(MainForm ventanaMain)
        {
            InitializeComponent();
            mainform = ventanaMain;
            
        }

        private void Configuracion_Load(object sender, EventArgs e)
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

        #region FormatoTexto
        private void cambiarFamiliaTexto(object sender, EventArgs e, string newFamily)
        {
            mainform.richTextBox1.SelectAll();
            Font currentFont = mainform.richTextBox1.SelectionFont;
            Font newFont = new Font(newFamily, currentFont.Size, currentFont.Style);
            mainform.richTextBox1.SelectionFont = newFont;
        }

        private void cambiarEstiloTexto(object sender, EventArgs e, FontStyle newStyle)
        {
            mainform.richTextBox1.SelectAll();
            Font currentFont = mainform.richTextBox1.SelectionFont;
            Font newFont = new Font(currentFont.FontFamily, currentFont.Size, newStyle);
            mainform.richTextBox1.SelectionFont = newFont;
        }

        private void cambiarTamañoTexto(object sender, EventArgs e, float newSize) 
        {
            mainform.richTextBox1.SelectAll();
            Font currentFont = mainform.richTextBox1.SelectionFont;
            Font newFont = new Font(currentFont.FontFamily, newSize, currentFont.Style);
            mainform.richTextBox1.SelectionFont = newFont;
        }
        #endregion

        #region cambiarTamaño métodosClick
        private void toolStripFormatoTamaño8_Click(object sender, EventArgs e)
        {
            cambiarTamañoTexto(sender, e, 8 );
        }

        private void toolStripFormatoTamaño10_Click(object sender, EventArgs e)
        {
            cambiarTamañoTexto(sender, e, 10);
        }

        private void toolStripFormatoTamaño12_Click(object sender, EventArgs e)
        {
            cambiarTamañoTexto(sender, e, 12);
        }

        private void toolStripFormatoTamaño14_Click(object sender, EventArgs e)
        {
            cambiarTamañoTexto(sender, e, 14);
        }

        private void toolStripFormatoTamaño18_Click(object sender, EventArgs e)
        {
            cambiarTamañoTexto(sender, e, 18);
        }

        private void toolStripFormatoTamaño24_Click(object sender, EventArgs e)
        {
            cambiarTamañoTexto(sender, e, 24);
        }

        private void toolStripFormatoTamaño36_Click(object sender, EventArgs e)
        {
            cambiarTamañoTexto(sender, e, 36);
        }
        #endregion

        #region cambiar tipo fuente métodosClick

        private void timesNewRomanToolStripMenuItem_Click(object sender, EventArgs e)
        {
            cambiarFamiliaTexto(sender,e , "Times New Roman");
        }

        private void wideLatinToolStripMenuItem_Click(object sender, EventArgs e)
        {
            cambiarFamiliaTexto(sender, e, "Wide Latin");
        }

        private void lucidaSansToolStripMenuItem_Click(object sender, EventArgs e)
        {
            cambiarFamiliaTexto(sender, e, "Lucida Sans");
        }

        private void consolasToolStripMenuItem_Click(object sender, EventArgs e)
        {
            cambiarFamiliaTexto(sender, e, "Consolas");
        }
        #endregion

        #region cambiar estilo fuente métodosClick
        private void regularToolStripMenuItem_Click(object sender, EventArgs e)
        {
            cambiarEstiloTexto(sender,e,FontStyle.Regular);
        }

        private void negritaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            cambiarEstiloTexto(sender, e, FontStyle.Bold);
        }

        private void italicToolStripMenuItem_Click(object sender, EventArgs e)
        {
            cambiarEstiloTexto(sender, e, FontStyle.Italic);
        }

        private void negritaCursivaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            cambiarEstiloTexto(sender, e, FontStyle.Bold | FontStyle.Italic);
        }
        #endregion
    }
}
