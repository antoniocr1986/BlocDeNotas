using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Office.Interop.Word;
using Word = Microsoft.Office.Interop.Word;

namespace BlocNotasWF
{
    public partial class MainForm : Form
    {
        private string archivoActual = "nuevoArchivo.txt";
        private FormConfiguracion configuracion = null;
        private string tituloVentana = "Bloc de notas: principal";
        int openFormCount = System.Windows.Forms.Application.OpenForms.Count;

        //***A2
        private Button buttonPrintPreview;
        PrintExample printExample;

        public MainForm()
        {
            InitializeComponent();

            aplicarMargenes();

            if (openFormCount < 1)
                this.Text = tituloVentana;

            //PARA CONFIGURACION COLORES MENU STRIP
            //menuStrip1.Renderer = new CustomMenuRenderer(); PENDIENTE DE VER SI USO O NO

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

            // Configura el ContextMenuStrip para el formulario
            this.ContextMenuStrip = contextMenuStripMain;
        }

        public void aplicarMargenes()
        {
            richTextBox1.SelectionIndent = 10; // Establecer el margen izquierdo en 10 píxeles
            richTextBox1.SelectionRightIndent = 10; // Establecer el margen derecho en 10 píxeles    
            //MODIFICAR ALTO richTextBox para intentar que se vea barra horizontal
            //richTextBox1.Height = this.ClientSize.Height - statusStrip1.Height - 26; // Espacio para StatusStrip + margen de 20 píxeles
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
        public bool ArrayEquals(byte[] arr1, byte[] arr2)
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

        //PARA TEMA OSCURO O CLARO
        private void ApplyTheme(bool isDarkMode)
        {
            Color background = isDarkMode ? ThemeColors.DarkBackground : ThemeColors.LightBackground;
            Color text = isDarkMode ? ThemeColors.DarkText : ThemeColors.LightText;
            Color buttonBackground = isDarkMode ? ThemeColors.DarkButtonBackground : ThemeColors.LightButtonBackground;
            Color buttonText = isDarkMode ? ThemeColors.DarkButtonText : ThemeColors.LightButtonText;

            this.BackColor = background;
            foreach (Control control in this.Controls)
            {
                control.BackColor = background;
                control.ForeColor = text;

                if (control is Button || control is ToolStripButton)
                {
                    control.BackColor = buttonBackground;
                    control.ForeColor = buttonText;
                }
            }
        }

        public void AbrirToolStripMenuItem_Click(object sender, EventArgs e)
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
        public void GuardarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (archivoActual == "nuevoArchivo.txt")
            {
                GuardarComoToolStripMenuItem_Click(sender, e);
            }
            else
            {
                GuardarCambiosEnArchivo(archivoActual);
            }

        }

        public void GuardarCambiosEnArchivo(string nombreArchivo)
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

        public void GuardarComoToolStripMenuItem_Click(object sender, EventArgs e)
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


        public void GuardarComoCambiosEnArchivo(string nombreArchivo = null)
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

        public void ConfigurarPáginaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (pageSetupDialog1.ShowDialog() == DialogResult.OK)
            {
                //Podremos modificar la orientación, el tamaño del papel y los márgenes con:
                if (printExample == null)
                {
                    printExample = new PrintExample(richTextBox1);
                }
                printExample.printDocument.DefaultPageSettings.Landscape= pageSetupDialog1.PageSettings.Landscape;
                printExample.printDocument.DefaultPageSettings.PaperSize = pageSetupDialog1.PageSettings.PaperSize;
                printExample.printDocument.DefaultPageSettings.Margins = pageSetupDialog1.PageSettings.Margins;
                
                //pageSettings = pageSetupDialog1.PageSettings;
            }
        }

        #region metodos imprimir
        public void ImprimirToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (printExample == null)
            {
                printExample = new PrintExample(richTextBox1);
            }
            printExample.ShowPrintPreview();
        }

        public void PrintText(object sender, PrintPageEventArgs e)
        {
            string textToPrint = richTextBox1.Text;

            System.Drawing.Font font = richTextBox1.Font;
            Brush brush = new SolidBrush(richTextBox1.ForeColor);

            RectangleF bounds = e.MarginBounds;
            e.Graphics.DrawString(textToPrint, font, brush, bounds, StringFormat.GenericTypographic);
        }

        //*** A2 vista previa
        private void ButtonPrintPreview_Click(object sender, EventArgs e)
        {
            if (printExample == null)
            {
                printExample = new PrintExample(richTextBox1);
            }         
            printExample.ShowPrintPreview();
        }

        #endregion

        #region metodos seleccionar todo, cortar, copiar y pegar
        public void SeleccionarTodoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            richTextBox1.SelectAll();
        }

        private void seleccionarTodoToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            richTextBox1.SelectAll();
        }

        public void CortarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            richTextBox1.Cut();
        }

        public void CopiarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            richTextBox1.Copy();
        }

        public void PegarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Clipboard.ContainsText())
            {
                richTextBox1.Paste();
            }
        }


        private void cortarToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (richTextBox1.SelectedText.Length > 0)
            {
                richTextBox1.Cut();
            }
        }

        private void copiarToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (richTextBox1.SelectedText.Length > 0)
            {
                richTextBox1.Copy();
            }
        }

        private void pegarToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (Clipboard.ContainsText())
            {
                richTextBox1.Paste();
            }
        }
        #endregion

        public void BuscarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormBuscar form2 = new FormBuscar();

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

                    richTextBox1.SelectionBackColor = Color.SteelBlue;
                    index += textoBuscado.Length;
                }
            }
        }

        public void FechaYHoraToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DateTime fechaHoraActual = DateTime.Now;
            string fechaHoraString = fechaHoraActual.ToString("yyyy-MM-dd HH:mm:ss");

            // Obtener la posición actual del cursor
            int posicionCursor = richTextBox1.SelectionStart;

            // Insertar el texto en la posición del cursor
            richTextBox1.Text = richTextBox1.Text.Insert(posicionCursor, fechaHoraString);

            // Restaurar la posición del cursor
            richTextBox1.SelectionStart = posicionCursor + fechaHoraString.Length;
        }

        public void FuenteToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            var formato = fontDialog1.ShowDialog();
            if (formato == DialogResult.OK)
            {
                richTextBox1.Font = fontDialog1.Font;
            }
        }

        public void ColorToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            var color = colorDialog1.ShowDialog();
            if (color == DialogResult.OK)
            {
                richTextBox1.ForeColor = colorDialog1.Color;
            }
        }

        #region metodos de Zoom
        public void AumentarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            richTextBox1.ZoomFactor = richTextBox1.ZoomFactor + 0.2F;
            ZoomStatusLabel_Changed(sender, e);

        }

        public void ReducirToolStripMenuItem_Click(object sender, EventArgs e)
        {
            richTextBox1.ZoomFactor = richTextBox1.ZoomFactor - 0.2F;
            ZoomStatusLabel_Changed(sender, e);
        }

        public void RestablecerZoomOriginalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            richTextBox1.ZoomFactor = 1;
            ZoomStatusLabel_Changed(sender, e);
        }

        public void ZoomStatusLabel_Changed(object sender, EventArgs e)
        {
            int valor = Convert.ToInt32(richTextBox1.ZoomFactor * 100);
            toolStripStatusLabelZoom.Text = $" {valor} %";
        }
        #endregion

        #region metodos barra de estado
        public void BarraDeEstadoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            statusStrip1.Visible = !statusStrip1.Visible;
            barraDeEstadoToolStripMenuItem.Checked = !barraDeEstadoToolStripMenuItem.Checked;
        }

        public void RichTextBox1_TextChanged(object sender, EventArgs e)
        {
            int wordCount = richTextBox1.Text.Split(new char[] { ' ', '\t', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries).Length;
            toolStripStatusLabelWords.Text = "Palabras: " + wordCount;
            if (wordCount < 10)
            {
                toolStripStatusLabelWords.Text = toolStripStatusLabelWords.Text + "  ";
            }
            else if (wordCount < 100)
            {
                toolStripStatusLabelWords.Text = toolStripStatusLabelWords.Text + " ";
            }
        }

        #endregion

        #region metodos salir y cerrar
        public void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Form closingForm = sender as Form;

            int openFormCount = System.Windows.Forms.Application.OpenForms.Count;

            if (openFormCount > 1)
            {
                //if (closingForm.Text != tituloVentana)
                //  e.Cancel = true;
                //else    
                //    MessageBox.Show("No se puede cerrar la ventana principal mientras tengas otras ventanas abiertas en la aplicación, actualmente tienes abiertas " + openFormCount + " ventanas.");
            }
            else
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
                        GuardarToolStripMenuItem_Click(sender, e);
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

        public void SalirToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }
        #endregion

        private void pictureBoxConfig_Click(object sender, EventArgs e)
        {
            if (configuracion == null || configuracion.IsDisposed)
            {
                configuracion = new FormConfiguracion(this);

                configuracion.Show();
            }
            else
            {
                configuracion.BringToFront();
            }

        }

        private void deshacerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (richTextBox1.CanUndo)
            {
                richTextBox1.Undo();
            }
        }
        #region métodosOrtografia

        public void activarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CheckSpelling(richTextBox1);
        }

        private void CheckSpelling(RichTextBox richTextBox)
        {
            // Crear una instancia de la aplicación de Word
            Word.Application wordApp = new Word.Application();
            wordApp.Visible = false; // Hacer que Word sea invisible

            // Crear un nuevo documento
            Word.Document document = wordApp.Documents.Add();

            // Poner el texto del RichTextBox en el documento
            document.Content.Text = richTextBox.Text;

            // Revisar la ortografía del documento
            document.CheckSpelling();

            // Obtener el texto corregido
            string correctedText = document.Content.Text;

            // Colocar el texto corregido de vuelta en el RichTextBox
            richTextBox.Text = correctedText;

            // Cerrar el documento sin guardar cambios
            document.Close(false);

            // Cerrar la aplicación de Word
            wordApp.Quit();

            // Liberar recursos
            System.Runtime.InteropServices.Marshal.ReleaseComObject(document);
            System.Runtime.InteropServices.Marshal.ReleaseComObject(wordApp);

            MessageBox.Show("Revisión ortográfica completa.", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        #endregion

        #region metodosEliminar
        private void eliminarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            eliminarTexto(sender, e);
        }

        private void eliminarToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            eliminarTexto(sender, e);
        }

        public void eliminarTexto(object sender, EventArgs e)
        {
            // Eliminar el texto seleccionado en el RichTextBox
            if (!string.IsNullOrEmpty(this.richTextBox1.SelectedText))
            {
                this.richTextBox1.SelectedText = "";
            }
        }
        #endregion

        private void richTextBox1_SelectionChanged(object sender, EventArgs e)
        {
            // Obtener la posición actual del cursor
            int index = richTextBox1.SelectionStart;
            int line = richTextBox1.GetLineFromCharIndex(index);
            int firstCharIndex = richTextBox1.GetFirstCharIndexFromLine(line);
            int column = index - firstCharIndex;

            // Mostrar la información en una etiqueta
            toolStripStatusLabelLineaCol.Text = $"Ln {line + 1}, Col {column + 1}";
            if (line + 1 < 10)
            {
                toolStripStatusLabelLineaCol.Text = toolStripStatusLabelLineaCol.Text + "  ";
            }
            else if (line + 1 < 100)
            {
                toolStripStatusLabelLineaCol.Text = toolStripStatusLabelLineaCol.Text + " ";
            }
            if (column + 1 < 10)
            {
                toolStripStatusLabelLineaCol.Text = toolStripStatusLabelLineaCol.Text + "  ";
            }
            else if (column + 1 < 100)
            {
                toolStripStatusLabelLineaCol.Text = toolStripStatusLabelLineaCol.Text + " ";
            }
        }

        private void nuevaVentanaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MainForm mainform2 = new MainForm();
            mainform2.Show();
        }

        private void richTextBox1_MouseClick(object sender, MouseEventArgs e)
        {
            //Guardar posicion actual en el richTextBox
            int posicionCursor = richTextBox1.SelectionStart;

            // Seleccionar todo el texto en el RichTextBox
            richTextBox1.SelectAll();

            // Establecer el color de fondo del texto seleccionado
            richTextBox1.SelectionBackColor = Color.FromArgb(44, 44, 44);

            // Deseleccionar el texto para evitar que quede resaltado
            richTextBox1.DeselectAll();

            // Restaurar la posición del cursor
            richTextBox1.SelectionStart = posicionCursor;

            //***A1
            //***A1CheckStatuStripLocation();
        }

        //***A1
        /*private void CheckStatusStripLocation()
        {
            // Obtener la ubicación y el tamaño del RichTextBox
            var richTextBoxRect = richTextBox1.Bounds;

            // Obtener la ubicación y el tamaño del StatusStrip
            var statusStripRect = statusStrip1.Bounds;

            // Verificar si el StatusStrip está dentro del RichTextBox
            bool isInsideRichTextBox = richTextBoxRect.IntersectsWith(statusStripRect);

            // Verificar si el StatusStrip está en la parte inferior del formulario
            bool isDockedToBottom = statusStrip1.Dock == DockStyle.Bottom;

            // Mostrar resultados
            MessageBox.Show($"StatusStrip dentro del RichTextBox: {isInsideRichTextBox}\nStatusStrip anclado a la parte inferior: {isDockedToBottom}");
        }*/

        public void buscarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormBuscar form2 = new FormBuscar();

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

        private void buscarToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            {
                FormBuscar form2 = new FormBuscar();

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
                            MessageBox.Show("No se ha encontrado ningun texto que coincida con el texto buscado");
                            break;

                        richTextBox1.SelectionBackColor = Color.Yellow;
                        index += textoBuscado.Length;
                    }
                }
            }
        }

        private void ajusteDeLineaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            richTextBox1.WordWrap = !richTextBox1.WordWrap;
            ajusteDeLineaToolStripMenuItem.Checked = !ajusteDeLineaToolStripMenuItem.Checked;

            //***A1 INTENTAR HACER VER QUE SE VEA LA SCROLLBAR HORIZONTAL
             /* if (richTextBox1.WordWrap == false)
            {
                richTextBox1.ScrollBars = RichTextBoxScrollBars.Both; // Mostrar barras de desplazamiento vertical y horizontal
            }
            else
                richTextBox1.ScrollBars = RichTextBoxScrollBars.Vertical;*/
        }
    }
}
