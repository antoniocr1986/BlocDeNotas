using System;
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
    public partial class CustomPrintDialog : Form
    {
        private RichTextBox richTextBox;
        private PrintDocument printDocument;
        private PrintPreviewControl printPreviewControl;
        private Button printButton;
        private Button printerSettingsButton;

        public CustomPrintDialog(RichTextBox richTextBox1)
        {
            this.richTextBox = richTextBox1;

            // Configurar PrintDocument
            printDocument = new PrintDocument();
            printDocument.PrintPage += new PrintPageEventHandler(PrintDocument_PrintPage);

            // Configurar PrintPreviewControl
            printPreviewControl = new PrintPreviewControl
            {
                Document = printDocument,
                Dock = DockStyle.Fill
            };

            // Configurar Botón de Impresión
            printButton = new Button
            {
                Text = "Imprimir",
                Dock = DockStyle.Bottom
            };
            printButton.Click += PrintButton_Click;

            // Configurar Botón de Configuración de la Impresora
            printerSettingsButton = new Button
            {
                Text = "Configuración de la Impresora",
                Dock = DockStyle.Bottom
            };
            printerSettingsButton.Click += PrinterSettingsButton_Click;

            // Agregar controles al formulario
            this.Controls.Add(printPreviewControl);
            this.Controls.Add(printButton);
            this.Controls.Add(printerSettingsButton);

            this.Text = "Vista Previa e Impresión";
            this.WindowState = FormWindowState.Maximized;
        }

        private void PrintDocument_PrintPage(object sender, PrintPageEventArgs e)
        {
            string textToPrint = richTextBox.Text;

            Font font = richTextBox.Font;
            Brush brush = new SolidBrush(richTextBox.ForeColor);

            RectangleF bounds = e.MarginBounds;
            e.Graphics.DrawString(textToPrint, font, brush, bounds, StringFormat.GenericTypographic);
        }

        private void PrintButton_Click(object sender, EventArgs e)
        {
            PrintDialog printDialog = new PrintDialog
            {
                Document = printDocument
            };

            if (printDialog.ShowDialog() == DialogResult.OK)
            {
                printDocument.PrinterSettings = printDialog.PrinterSettings;
                printDocument.Print();
            }
        }

        private void PrinterSettingsButton_Click(object sender, EventArgs e)
        {
            PageSetupDialog pageSetupDialog = new PageSetupDialog
            {
                Document = printDocument
            };

            if (pageSetupDialog.ShowDialog() == DialogResult.OK)
            {
                printPreviewControl.InvalidatePreview();
            }
        }
    }
}
