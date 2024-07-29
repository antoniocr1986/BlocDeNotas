using System.Drawing;
using System.Windows.Forms;

public class CustomMenuRenderer : ToolStripProfessionalRenderer
{

    /*// Cambia el color de fondo del MenuStrip cuando está desplegado
    protected override void OnRenderToolStripBackground(ToolStripRenderEventArgs e)
    {
        if (e.ToolStrip is MenuStrip)
        {
            using (SolidBrush brush = new SolidBrush(Color.FromArgb(54, 54, 54))) // Color de fondo del menú desplegado
            {
                e.Graphics.FillRectangle(brush, e.AffectedBounds);
            }
        }
        else
        {
            base.OnRenderToolStripBackground(e);
        }
    }*/
    
    // Cambia el color de fondo de los elementos del menú cuando están seleccionados
    protected override void OnRenderMenuItemBackground(ToolStripItemRenderEventArgs e)
    {
        if (e.Item.Selected)
        {
            using (SolidBrush brush = new SolidBrush(Color.FromArgb(54, 54, 54))) // Color de fondo del ítem seleccionado
            {
                e.Graphics.FillRectangle(brush, e.Item.ContentRectangle);
            }
        }
        else
        {
            using (SolidBrush brush = new SolidBrush(Color.FromArgb(54, 54, 54)))
            {
                e.Graphics.FillRectangle(brush, e.Item.ContentRectangle);
            }
        }
    }
    /*
    // Cambia el color del texto de los elementos del menú cuando están seleccionados
    protected override void OnRenderItemText(ToolStripItemTextRenderEventArgs e)
    {
        if (e.Item.Selected)
        {
            e.TextColor = Color.White; // Color del texto cuando el ítem está seleccionado
        }
        else
        {
            e.TextColor = Color.White; // Color del texto cuando el ítem no está seleccionado
        }
        base.OnRenderItemText(e);
    }
    */
}

