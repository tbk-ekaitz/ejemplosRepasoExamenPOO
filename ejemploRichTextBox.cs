using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace Practica6TOO
{
    // --- PUNTO DE ENTRADA DE LA APLICACIÓN ---
    internal class Program
    {
        [STAThread] // Obligatorio para que funcionen los Diálogos Comunes
        public static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            // Iniciamos el formulario
            Application.Run(new Prueba1());
        }
    }

    // --- CLASE DEL FORMULARIO ---
    public class Prueba1 : Form
    {
        private RichTextBox richTextBox1;
        private string rutaArchivoActual = null;

        public Prueba1()
        {
            // Configuración básica del formulario por código
            this.Text = "Editor Práctica 6 - Nuevo Archivo";
            this.Size = new Size(600, 400);

            // Inicializar RichTextBox
            richTextBox1 = new RichTextBox();
            richTextBox1.Dock = DockStyle.Fill;
            this.Controls.Add(richTextBox1);

            // Suscribir el evento de cierre
            this.FormClosing += new FormClosingEventHandler(Prueba1_FormClosing);
            
            // Forzar que empiece sin modificaciones
            richTextBox1.Modified = false;
        }

        // --- LÓGICA DE ARCHIVOS ---

        private bool Guardar()
        {
            if (string.IsNullOrEmpty(rutaArchivoActual))
            {
                return GuardarComo();
            }
            else
            {
                File.WriteAllText(rutaArchivoActual, richTextBox1.Text);
                richTextBox1.Modified = false;
                return true;
            }
        }

        private bool GuardarComo()
        {
            using (SaveFileDialog saveDialog = new SaveFileDialog())
            {
                saveDialog.Filter = "Archivos de texto (*.txt)|*.txt";
                if (saveDialog.ShowDialog() == DialogResult.OK)
                {
                    rutaArchivoActual = saveDialog.FileName;
                    File.WriteAllText(rutaArchivoActual, richTextBox1.Text);
                    richTextBox1.Modified = false;
                    this.Text = "Editor - " + Path.GetFileName(rutaArchivoActual);
                    return true;
                }
            }
            return false; 
        }

        // --- LÓGICA DE CONTROL DE CAMBIOS ---

        private bool ValidarCambiosPendientes()
        {
            // Si no hay cambios (Modified == false), permitimos la acción directamente
            if (!richTextBox1.Modified) return true;

            DialogResult resultado = MessageBox.Show(
                "¿Desea guardar los cambios antes de continuar?",
                "Cambios no guardados",
                MessageBoxButtons.YesNoCancel,
                MessageBoxIcon.Question);

            if (resultado == DialogResult.Yes) return Guardar();
            if (resultado == DialogResult.No) return true; // El usuario ignora los cambios
            
            return false; // El usuario dio a "Cancelar", detenemos el cierre o la acción
        }

        // --- MANEJO DEL EVENTO FORM CLOSING ---

        private void Prueba1_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Si la validación devuelve 'false', cancelamos el evento de cierre
            if (!ValidarCambiosPendientes())
            {
                e.Cancel = true;
            }
        }
    }
}
