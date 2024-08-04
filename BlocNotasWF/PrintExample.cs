using BlocNotasWF.Properties;
using System;
using System.Drawing;
using System.Drawing.Printing;
using System.Windows.Forms;

namespace BlocNotasWF
{
    //***A2 Clase creada para intentar corregir funcionamiento imprimir y vista previa del documento
    public class PrintExample    
    {
        private RichTextBox richTextBox;
        public PrintDocument printDocument { get; private set; }
        private PrintPreviewDialog printPreviewDialog;
        System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormBuscar));

        public PrintExample(RichTextBox rtb)
        {
            richTextBox = rtb;
            printDocument = new PrintDocument();
            printDocument.PrintPage += new PrintPageEventHandler(PrintDocument_PrintPage);
        }

        private void PrintDocument_PrintPage(object sender, PrintPageEventArgs e)
        {
            e.Graphics.DrawString(richTextBox.Text, richTextBox.Font, Brushes.Black, e.MarginBounds, StringFormat.GenericTypographic);
        }

        public void ShowPrintPreview()
        {
            PrintPreviewDialog printPreviewDialog = new PrintPreviewDialog
            {
                Document = printDocument,
                Width = 500,  // Ancho deseado
                Height = 800,  // Altura deseada
                Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")))

            };
            printPreviewDialog.StartPosition = FormStartPosition.Manual;
            printPreviewDialog.Left = (Screen.PrimaryScreen.WorkingArea.Width - printPreviewDialog.Width) / 2;
            printPreviewDialog.Top = (Screen.PrimaryScreen.WorkingArea.Height - printPreviewDialog.Height) / 2;

            printPreviewDialog.ShowDialog();
        }
    }
}

    

