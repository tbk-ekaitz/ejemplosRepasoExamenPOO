using System;
using System.Windows.Forms;

public class MiFormularioSimple : Form
{
    public MiFormularioSimple()
    {
        // 1. Crear el contenedor (la barra gris)
        ToolStrip miBarra = new ToolStrip();

        // 2. Crear un botón para la barra
        ToolStripButton btnGuardar = new ToolStripButton("Guardar");
        
        // 3. Suscribir el evento (Lambda simple)
        btnGuardar.Click += (s, e) => MessageBox.Show("Guardando datos...");

        // 4. Meter el botón dentro de la barra
        miBarra.Items.Add(btnGuardar);

        // 5. Meter la barra dentro del formulario
        this.Controls.Add(miBarra);
    }

    // Punto de entrada para probarlo ya mismo
    [STAThread]
    static void Main() 
    {
        Application.EnableVisualStyles();
        Application.Run(new MiFormularioSimple());
    }
}

