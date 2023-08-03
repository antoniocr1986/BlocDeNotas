using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BlocNotasWF
{
    public partial class Form1 : Form
    {
        string archivoActual ="nuevoArchivo.txt";

        public Form1()
        {
            InitializeComponent();

            //CODIGO PARA AÑADIR EVENTO AL CAMBIAR EL TEXTO DEL RICHTEXTBOX
            richTextBox1.TextChanged += RichTextBox1_TextChanged;


            //CODIGO PARA ARCHIVO-CONFIGURAR PAGINA
            pageSetupDialog1 = new PageSetupDialog();
            // Asociar el PageSetupDialog al RichTextBox para obtener sus propiedades de impresión
            pageSetupDialog1.Document = new System.Drawing.Printing.PrintDocument();
            pageSetupDialog1.Document.DefaultPageSettings = new System.Drawing.Printing.PageSettings();
            pageSetupDialog1.EnableMetric = true; // Usar medidas métricas (milímetros) en ves de pulgadas

            #region barra de estado codificacion
            //CODIGO PARA MOSTRAR CODIFICACION EN BARRA DE ESTADO
            richTextBox1.TextChanged += (sender, e) =>
            {
                string text = richTextBox1.Text;

                Encoding encoding = TryDetectEncoding(text);

                if (encoding != null)
                {
                    toolStripStatusLabelCodification.Text = "Codificación: " + encoding.EncodingName;
                }
                else
                {
                    toolStripStatusLabelCodification.Text = "Codificación: No se pudo determinar";
                }
            };
        }

        private Encoding TryDetectEncoding(string text)
        {
            Encoding[] encodings = new Encoding[]
            {
            Encoding.UTF8,
            Encoding.Unicode, // UTF-16 Little Endian
            Encoding.BigEndianUnicode, // UTF-16 Big Endian
            Encoding.ASCII,
            Encoding.GetEncoding("ISO-8859-1") // Latin-1
            };

            foreach (Encoding encoding in encodings)
            {
                byte[] bytes = encoding.GetPreamble();
                if (bytes.Length <= text.Length)
                {
                    string preambleText = text.Substring(0, bytes.Length);
                    byte[] preambleBytes = encoding.GetBytes(preambleText);
                    if (ArrayEquals(bytes, preambleBytes))
                    {
                        return encoding;
                    }
                }
            }

            return null; // No obtenemos codificacion
        }

        // Método auxiliar para comparar dos arreglos de bytes
        private bool ArrayEquals(byte[] arr1, byte[] arr2)
        {
            if (arr1.Length != arr2.Length)
                return false;

            for (int i = 0; i < arr1.Length; i++)
            {
                if (arr1[i] != arr2[i])
                    return false;
            }

            return true;
        }
        #endregion


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
                    archivoActual = openFileDialog1.FileName;
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

        #region metodos guardar y guardar como...
        private void guardarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (archivoActual == "nuevoArchivo.txt")
            {
                guardarComoToolStripMenuItem_Click(sender, e);
            }
            else
            {
                GuardarCambiosEnArchivo(archivoActual);
            }
            
        }

        private void GuardarCambiosEnArchivo(string nombreArchivo)
        {
            try
            {
                using (StreamWriter savefile = new StreamWriter(nombreArchivo))
                {
                    savefile.WriteLine(richTextBox1.Text);
                }

                Console.WriteLine("Cambios guardados correctamente en el archivo: " + nombreArchivo);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al guardar los cambios: " + ex.Message);
            }
        }

        private void guardarComoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            saveFileDialog1.FileName = ".txt";
            var save = saveFileDialog1.ShowDialog();
            if (save == DialogResult.OK)
            {
                using (var savefile = new System.IO.StreamWriter(saveFileDialog1.FileName))
                {
                    GuardarComoCambiosEnArchivo(saveFileDialog1.FileName);
                    archivoActual = saveFileDialog1.FileName;
                    savefile.WriteLine(richTextBox1.Text);
                }
            }
        }


        private void GuardarComoCambiosEnArchivo(string nombreArchivo = null)
        {
            string nombreArchivoGuardar = archivoActual;

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
        #endregion

        private void configurarPáginaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (pageSetupDialog1.ShowDialog() == DialogResult.OK)
            {
                //METODO A COMPLETAR AUN
                /* Podremos modificar la orientación, el tamaño del papel y los márgenes con:
                 pageSetupDialog.Document.DefaultPageSettings.Landscape
                 pageSetupDialog.Document.DefaultPageSettings.PaperSize
                 pageSetupDialog.Document.DefaultPageSettings.Margins*/
            }
        }

        #region metodos imprimir
        private void imprimirToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult result = printDialog1.ShowDialog();

            if (result == DialogResult.OK)
            {
                PrintDocument printDocument = new PrintDocument();
                printDocument.PrintPage += new PrintPageEventHandler(PrintText);
                printDocument.PrinterSettings = printDialog1.PrinterSettings;
                printDocument.Print();
            }
        }

        private void PrintText(object sender, PrintPageEventArgs e)
        {
            string textToPrint = richTextBox1.Text;

            Font font = richTextBox1.Font;
            Brush brush = new SolidBrush(richTextBox1.ForeColor);

            RectangleF bounds = e.MarginBounds;
            e.Graphics.DrawString(textToPrint, font, brush, bounds, StringFormat.GenericTypographic);
        }
        #endregion

        #region metodos seleccionar todo, cortar, copiar y pegar
        private void seleccionarTodoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            richTextBox1.SelectAll();
        }

        private void cortarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            richTextBox1.Cut();
        }

        private void copiarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            richTextBox1.Copy();
        }

        private void pegarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Clipboard.ContainsText())
            {
                richTextBox1.Paste();
            }
        }
        #endregion

        private void buscarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form2 form2 = new Form2();

            form2.ShowDialog();

            string textoBuscado = form2.GetTextBoxData();

            if (!string.IsNullOrWhiteSpace(textoBuscado))
            {
                int index = 0;
                int lastIndex = richTextBox1.TextLength;

                while (index < lastIndex)
                {
                    index = richTextBox1.Find(textoBuscado, index, lastIndex, RichTextBoxFinds.None);
                    if (index == -1)
                        break;

                    richTextBox1.SelectionBackColor = Color.Yellow;
                    index += textoBuscado.Length;
                }
            }
        }

        private void fechaYHoraToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DateTime fechaHoraActual = DateTime.Now;
            string fechaHoraString = fechaHoraActual.ToString("yyyy-MM-dd HH:mm:ss");

            richTextBox1.Text = fechaHoraString + "\n" + richTextBox1.Text;
        }

        private void fuenteToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            var formato = fontDialog1.ShowDialog();
            if (formato == DialogResult.OK)
            {
                richTextBox1.Font = fontDialog1.Font;
            }
        }

        private void colorToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            var color = colorDialog1.ShowDialog();
            if (color == DialogResult.OK)
            {
                richTextBox1.ForeColor = colorDialog1.Color;
            }
        }

        #region metodos de Zoom
        private void aumentarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            richTextBox1.ZoomFactor = richTextBox1.ZoomFactor + 0.2F;
            ZoomStatusLabel_Changed(sender, e);

        }

        private void reducirToolStripMenuItem_Click(object sender, EventArgs e)
        {
            richTextBox1.ZoomFactor = richTextBox1.ZoomFactor - 0.2F;
            ZoomStatusLabel_Changed(sender, e);
        }

        private void restablecerZoomOriginalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            richTextBox1.ZoomFactor = 1;
            ZoomStatusLabel_Changed(sender, e);
        }

        private void ZoomStatusLabel_Changed(object sender, EventArgs e)
        {
            int valor = Convert.ToInt32(richTextBox1.ZoomFactor * 100);
            toolStripStatusLabelZoom.Text = "Zoom: " + valor + " %";
        }
        #endregion

        #region metodos barra de estado
        private void barraDeEstadoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            statusStrip1.Visible = !statusStrip1.Visible;
            barraDeEstadoToolStripMenuItem.Checked = !barraDeEstadoToolStripMenuItem.Checked;
        }

        private void RichTextBox1_TextChanged(object sender, EventArgs e)
        {
            int wordCount = richTextBox1.Text.Split(new char[] { ' ', '\t', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries).Length;
            toolStripStatusLabelWords.Text = "Palabras: " + wordCount;
        }

        #endregion

        #region metodos salir y cerrar
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

        private void salirToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }
        #endregion

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
