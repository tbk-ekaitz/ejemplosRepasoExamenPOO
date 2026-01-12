using System;
using System.Drawing;
using System.Windows.Forms;

namespace FusionMenusMDI
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new FormularioPadre());
        }
    }

    // =========================================================================
    // FORMULARIO PADRE MDI
    // =========================================================================
    public class FormularioPadre : Form
    {
        private MenuStrip menuPadre;
        private StatusStrip barraEstado;
        private ToolStripStatusLabel lblEstado;
        private int contadorDocumentos = 0;

        public FormularioPadre()
        {
            // Configurar como contenedor MDI
            this.IsMdiContainer = true;
            this.Text = "Editor - Fusión de Menús";
            this.Size = new Size(800, 600);
            this.StartPosition = FormStartPosition.CenterScreen;

            CrearMenuPadre();
            CrearBarraEstado();
        }

        private void CrearMenuPadre()
        {
            menuPadre = new MenuStrip();
            
            // ================================================================
            // IMPORTANTE: Habilitar fusión en el MenuStrip del PADRE
            // ================================================================
            menuPadre.AllowMerge = true;

            // ================================================================
            // MENÚ ARCHIVO (del padre)
            // ================================================================
            ToolStripMenuItem menuArchivo = new ToolStripMenuItem("&Archivo");
            
            ToolStripMenuItem menuNuevo = new ToolStripMenuItem("&Nuevo Documento");
            menuNuevo.ShortcutKeys = Keys.Control | Keys.N;
            menuNuevo.Click += delegate(object s, EventArgs e)
            {
                contadorDocumentos++;
                FormularioHijo hijo = new FormularioHijo(contadorDocumentos);
                hijo.MdiParent = this;
                hijo.Show();
            };

            ToolStripMenuItem menuSalir = new ToolStripMenuItem("&Salir");
            menuSalir.Click += delegate { this.Close(); };

            menuArchivo.DropDownItems.Add(menuNuevo);
            menuArchivo.DropDownItems.Add(new ToolStripSeparator());
            menuArchivo.DropDownItems.Add(menuSalir);

            // ================================================================
            // MENÚ VER (del padre)
            // ================================================================
            ToolStripMenuItem menuVer = new ToolStripMenuItem("&Ver");
            
            ToolStripMenuItem menuCascada = new ToolStripMenuItem("&Cascada");
            menuCascada.Click += delegate { this.LayoutMdi(MdiLayout.Cascade); };
            
            menuVer.DropDownItems.Add(menuCascada);

            // ================================================================
            // MENÚ VENTANA (del padre)
            // ================================================================
            ToolStripMenuItem menuVentana = new ToolStripMenuItem("&Ventana");
            menuPadre.MdiWindowListItem = menuVentana;

            // Agregar menús al MenuStrip
            menuPadre.Items.Add(menuArchivo);
            menuPadre.Items.Add(menuVer);
            menuPadre.Items.Add(menuVentana);

            this.MainMenuStrip = menuPadre;
            this.Controls.Add(menuPadre);
        }

        private void CrearBarraEstado()
        {
            barraEstado = new StatusStrip();
            lblEstado = new ToolStripStatusLabel();
            lblEstado.Text = "Sin documentos abiertos";
            lblEstado.Spring = true;
            barraEstado.Items.Add(lblEstado);
            this.Controls.Add(barraEstado);

            this.MdiChildActivate += delegate
            {
                if (this.ActiveMdiChild != null)
                    lblEstado.Text = "Documento activo: " + this.ActiveMdiChild.Text;
                else
                    lblEstado.Text = "Sin documentos abiertos";
            };
        }
    }

    // =========================================================================
    // FORMULARIO HIJO - CON MENÚ QUE SE FUSIONA
    // =========================================================================
    public class FormularioHijo : Form
    {
        private MenuStrip menuHijo;
        private RichTextBox txtEditor;
        private int numeroDocumento;

        public FormularioHijo(int numero)
        {
            this.numeroDocumento = numero;
            this.Text = string.Format("Documento {0}", numero);
            this.Size = new Size(600, 400);

            CrearMenuHijo();
            CrearEditor();
        }

        private void CrearMenuHijo()
        {
            menuHijo = new MenuStrip();
            
            // ================================================================
            // IMPORTANTE: Habilitar fusión en el MenuStrip del HIJO
            // ================================================================
            menuHijo.AllowMerge = true;

            // ================================================================
            // MENÚ EDITAR (del hijo) - SE INSERTARÁ en el padre
            // ================================================================
            ToolStripMenuItem menuEditar = new ToolStripMenuItem("&Editar");
            
            // ================================================================
            // MergeAction: Cómo se fusiona este menú
            // MergeIndex: En qué posición se inserta
            // ================================================================
            menuEditar.MergeAction = MergeAction.Insert;
            menuEditar.MergeIndex = 1;  // Posición 1 (después de Archivo)

            ToolStripMenuItem menuCopiar = new ToolStripMenuItem("&Copiar");
            menuCopiar.ShortcutKeys = Keys.Control | Keys.C;
            menuCopiar.Click += delegate
            {
                if (txtEditor.SelectedText.Length > 0)
                    Clipboard.SetText(txtEditor.SelectedText);
            };

            ToolStripMenuItem menuPegar = new ToolStripMenuItem("&Pegar");
            menuPegar.ShortcutKeys = Keys.Control | Keys.V;
            menuPegar.Click += delegate
            {
                if (Clipboard.ContainsText())
                    txtEditor.SelectedText = Clipboard.GetText();
            };

            menuEditar.DropDownItems.Add(menuCopiar);
            menuEditar.DropDownItems.Add(menuPegar);

            // ================================================================
            // MENÚ FORMATO (del hijo) - SE INSERTARÁ después de Editar
            // ================================================================
            ToolStripMenuItem menuFormato = new ToolStripMenuItem("F&ormato");
            menuFormato.MergeAction = MergeAction.Insert;
            menuFormato.MergeIndex = 2;  // Posición 2

            ToolStripMenuItem menuNegrita = new ToolStripMenuItem("&Negrita");
            menuNegrita.ShortcutKeys = Keys.Control | Keys.B;
            menuNegrita.Click += delegate
            {
                Font fuenteActual = txtEditor.SelectionFont;
                FontStyle nuevoEstilo = fuenteActual.Style ^ FontStyle.Bold;
                txtEditor.SelectionFont = new Font(fuenteActual, nuevoEstilo);
            };

            ToolStripMenuItem menuCursiva = new ToolStripMenuItem("&Cursiva");
            menuCursiva.ShortcutKeys = Keys.Control | Keys.I;
            menuCursiva.Click += delegate
            {
                Font fuenteActual = txtEditor.SelectionFont;
                FontStyle nuevoEstilo = fuenteActual.Style ^ FontStyle.Italic;
                txtEditor.SelectionFont = new Font(fuenteActual, nuevoEstilo);
            };

            menuFormato.DropDownItems.Add(menuNegrita);
            menuFormato.DropDownItems.Add(menuCursiva);

            // Agregar menús al MenuStrip del hijo
            menuHijo.Items.Add(menuEditar);
            menuHijo.Items.Add(menuFormato);

            this.Controls.Add(menuHijo);
        }

        private void CrearEditor()
        {
            txtEditor = new RichTextBox();
            txtEditor.Dock = DockStyle.Fill;
            txtEditor.Font = new Font("Consolas", 11);
            txtEditor.Text = string.Format(
                "=== DOCUMENTO {0} ===\n\n" +
                "Este documento demuestra la FUSIÓN DE MENÚS.\n\n" +
                "Observa que cuando este documento está activo:\n" +
                "• Aparece el menú 'Editar' (del hijo)\n" +
                "• Aparece el menú 'Formato' (del hijo)\n" +
                "• El menú 'Archivo' sigue siendo del padre\n\n" +
                "Cuando cierres este documento o cambies a otro:\n" +
                "• Los menús 'Editar' y 'Formato' desaparecerán\n\n" +
                "Prueba:\n" +
                "1. Abre varios documentos (Ctrl+N)\n" +
                "2. Cambia entre ellos\n" +
                "3. Observa cómo los menús aparecen/desaparecen\n",
                numeroDocumento
            );

            this.Controls.Add(txtEditor);
        }
    }
}
