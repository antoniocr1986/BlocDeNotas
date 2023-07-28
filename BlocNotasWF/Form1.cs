using System;
using System.IO;
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
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void abrirToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string abrir;
            openFileDialog1.ShowDialog();

            try
            {
                System.IO.StreamReader file = new System.IO.StreamReader(openFileDialog1.FileName);
                if (openFileDialog1.FileName.Contains(".txt"))
                {
                    abrir = file.ReadToEnd();
                    richTextBox1.Text = abrir;
                }
                else
                {
                    MessageBox.Show("El archivo elejido ha de ser un .txt");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al abrir archivo: " + ex.Message);
            }
        }

        private void guardarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            saveFileDialog1.FileName = "nuevoTexto.txt";
            var save = saveFileDialog1.ShowDialog();
            if(save == DialogResult.OK)
            {
                using(var savefile = new System.IO.StreamWriter(saveFileDialog1.FileName))
                {
                    GuardarCambiosEnArchivo(saveFileDialog1.FileName);
                    //savefile.WriteLine(richTextBox1.Text);
                }
            }
        }

        private void GuardarCambiosEnArchivo(string nombreArchivo = null)
        {
            string nombreArchivoGuardar = "nuevoTexto.txt";

            try
            {
                using (StreamWriter savefile = new StreamWriter(nombreArchivoGuardar))
                {
                    savefile.WriteLine(richTextBox1.Text);
                }

                Console.WriteLine("Cambios guardados correctamente en el archivo: " + nombreArchivoGuardar);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al guardar los cambios: " + ex.Message);
            }
        }

        private void colorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var color = colorDialog1.ShowDialog();
            if (color == DialogResult.OK)
            {
                richTextBox1.ForeColor = colorDialog1.Color;
            }
        }

        private void formatoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var formato = fontDialog1.ShowDialog();
            if (formato == DialogResult.OK)
            {
                richTextBox1.Font = fontDialog1.Font;
            }
        }


        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                DialogResult result = MessageBox.Show("¿Quiere cerrar sin guardar?",
                                                      "Cerrar aplicación",
                                                      MessageBoxButtons.YesNoCancel,
                                                      MessageBoxIcon.Question);

                if (result == DialogResult.No)
                {
                    e.Cancel = true;
                    guardarToolStripMenuItem_Click(sender, e);
                }
                else if (result == DialogResult.Yes)

                {
                    Environment.Exit(0);
                }
                else
                {
                    e.Cancel = true;
                }
            }
        }
    }
}
