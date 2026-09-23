using System;
using System.Data;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using CapaNegocio;
// Usamos iText7 para la generación del PDF
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;


namespace CapaPresentacion
{
    public partial class Form1 : Form
    {
        private readonly IncidenciaCN negocio = new IncidenciaCN();

        // Controles UI
        private DataGridView dgvIncidencias = null!;
        private TextBox txtFiltro = null!;
        private Button btnBuscar = null!;
        private Button btnGuardar = null!;
        private Button btnExportarPDF = null!;

        // Desplegables actualizados para el Enfoque B (Relacional)
        private ComboBox cbVentas = null!;
        private ComboBox cbTiposProblema = null!;
        private TextBox txtObservacion = null!;
        private ComboBox cbEstado = null!;

        public Form1()
        {
            InitializeComponent(); 
            InicializarComponentes();
            CargarListasDesplegables();
            CargarDatos();
        }

        private void InicializarComponentes()
        {
            this.Text = "Mantenedor de Incidencias en Campañas y Ventas";
            this.Size = new Size(900, 600);
            this.StartPosition = FormStartPosition.CenterScreen;

            // Panel Superior: Filtro y Reportes
            Label lblFiltro = new Label() { Text = "Buscar/Filtrar:", Location = new Point(20, 20), AutoSize = true };
            txtFiltro = new TextBox() { Location = new Point(110, 17), Width = 200 };
            btnBuscar = new Button() { Text = "Filtrar", Location = new Point(320, 15), Width = 80 };
            btnExportarPDF = new Button() { Text = "Generar PDF Log", Location = new Point(720, 15), Width = 140, BackColor = Color.LightSteelBlue };

            btnBuscar.Click += (s, e) => CargarDatos(txtFiltro.Text);
            btnExportarPDF.Click += btnExportarPDF_Click;

            // Tabla DataGridView
            dgvIncidencias = new DataGridView()
            {
                Location = new Point(20, 60),
                Size = new Size(840, 280),
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect
            };

            // Panel Inferior: Formulario Agregar
            GroupBox gbAgregar = new GroupBox()
            {
                Text = "Registrar Nueva Incidencia",
                Location = new Point(20, 360),
                Size = new Size(840, 180)
            };

            // ComboBox para Ventas/Campañas (en lugar de TextBox VentaID)
            Label lblVenta = new Label() { Text = "Campaña / Venta:", Location = new Point(20, 30), AutoSize = true };
            cbVentas = new ComboBox() { Location = new Point(130, 27), Width = 180, DropDownStyle = ComboBoxStyle.DropDownList };

            // ComboBox para Tipo Problema (en lugar de TextBox TipoProblemaID)
            Label lblTipoProb = new Label() { Text = "Tipo Problema:", Location = new Point(320, 30), AutoSize = true };
            cbTiposProblema = new ComboBox() { Location = new Point(410, 27), Width = 180, DropDownStyle = ComboBoxStyle.DropDownList };

            // ComboBox para Estado
            Label lblEstado = new Label() { Text = "Estado:", Location = new Point(600, 30), AutoSize = true };
            cbEstado = new ComboBox() { Location = new Point(650, 27), Width = 130, DropDownStyle = ComboBoxStyle.DropDownList };
            cbEstado.Items.AddRange(new string[] { "Pendiente", "En Proceso", "Resuelto" });
            cbEstado.SelectedIndex = 0;

            // Observaciones
            Label lblObs = new Label() { Text = "Observación:", Location = new Point(20, 70), AutoSize = true };
            txtObservacion = new TextBox() { Location = new Point(130, 67), Width = 550, Multiline = true, Height = 50 };

            btnGuardar = new Button() { Text = "Guardar", Location = new Point(700, 67), Width = 110, Height = 50, BackColor = Color.LightGreen };
            btnGuardar.Click += BtnGuardar_Click;

            gbAgregar.Controls.AddRange(new Control[] {
                lblVenta, cbVentas, lblTipoProb, cbTiposProblema,
                lblEstado, cbEstado, lblObs, txtObservacion, btnGuardar
            });

            this.Controls.AddRange(new Control[] {
                lblFiltro, txtFiltro, btnBuscar, btnExportarPDF, dgvIncidencias, gbAgregar
            });
        }

        private void CargarListasDesplegables()
        {
            try
            {
                // Llenar ComboBox de Ventas
                DataTable dtVentas = negocio.ObtenerVentas();
                if (dtVentas.Rows.Count > 0)
                {
                    cbVentas.DataSource = dtVentas;
                    cbVentas.DisplayMember = "NombreCampana"; // Texto visible para el usuario
                    cbVentas.ValueMember = "VentaID";          // Clave primaria enviada a SQL
                }

                // Llenar ComboBox de Tipos de Problema
                DataTable dtTipos = negocio.ObtenerTiposProblema();
                if (dtTipos.Rows.Count > 0)
                {
                    cbTiposProblema.DataSource = dtTipos;
                    cbTiposProblema.DisplayMember = "Descripcion"; // Texto visible para el usuario
                    cbTiposProblema.ValueMember = "TipoProblemaID"; // Clave primaria enviada a SQL
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar desplegables: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CargarDatos(string filtro = "")
        {
            try
            {
                dgvIncidencias.DataSource = negocio.ListarIncidencias(filtro);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar datos: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnGuardar_Click(object sender, EventArgs e)
        {
            try
            {
                if (cbVentas.SelectedValue == null || cbTiposProblema.SelectedValue == null)
                {
                    MessageBox.Show("Seleccione una campaña y un tipo de problema de las listas.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (string.IsNullOrWhiteSpace(txtObservacion.Text))
                {
                    MessageBox.Show("Por favor, ingrese una observación.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Extraer el ID numérico interno seleccionado en las listas
                int ventaId = Convert.ToInt32(cbVentas.SelectedValue);
                int tipoProblemaId = Convert.ToInt32(cbTiposProblema.SelectedValue);
                string observacion = txtObservacion.Text.Trim();
                string estado = cbEstado.SelectedItem.ToString();

                bool resultado = negocio.RegistrarIncidencia(ventaId, tipoProblemaId, observacion, estado);

                if (resultado)
                {
                    MessageBox.Show("Incidencia registrada correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LimpiarFormulario();
                    CargarDatos();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al guardar: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LimpiarFormulario()
        {
            if (cbVentas.Items.Count > 0) cbVentas.SelectedIndex = 0;
            if (cbTiposProblema.Items.Count > 0) cbTiposProblema.SelectedIndex = 0;
            txtObservacion.Clear();
            cbEstado.SelectedIndex = 0;
        }

private void btnExportarPDF_Click(object sender, EventArgs e)
{
    try
    {
        // Verificar si la tabla de la pantalla tiene datos
        if (dgvIncidencias.Rows.Count == 0 || (dgvIncidencias.Rows.Count == 1 && dgvIncidencias.Rows[0].IsNewRow))
        {
            MessageBox.Show("No hay registros en la tabla para exportar.", "Atención", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        SaveFileDialog sfd = new SaveFileDialog()
        {
            Filter = "Archivos PDF (*.pdf)|*.pdf",
            FileName = "Reporte_Incidencias.pdf"
        };

        if (sfd.ShowDialog() == DialogResult.OK)
        {
            using (PdfWriter writer = new PdfWriter(sfd.FileName))
            using (PdfDocument pdf = new PdfDocument(writer))
            using (Document doc = new Document(pdf))
            {
                // Encabezado
                doc.Add(new Paragraph("Reporte de Incidencias Registradas").SetFontSize(16));
                doc.Add(new Paragraph("Fecha de generacion: " + DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss")).SetFontSize(10));
                doc.Add(new Paragraph("\n"));

                // Anchos de las columnas para dar espacio a la observación
                float[] columnWidths = new float[] { 1.5f, 1.5f, 1.5f, 4f, 2f, 2.5f };
                Table table = new Table(iText.Layout.Properties.UnitValue.CreatePercentArray(columnWidths));
                table.SetWidth(iText.Layout.Properties.UnitValue.CreatePercentValue(100));

                // 1. Agregar títulos desde las columnas del DataGridView
                foreach (DataGridViewColumn col in dgvIncidencias.Columns)
                {
                    Cell headerCell = new Cell().Add(new Paragraph(col.HeaderText).SetFontSize(9));
                    table.AddHeaderCell(headerCell);
                }

                // 2. Agregar los datos desde las filas del DataGridView
                foreach (DataGridViewRow row in dgvIncidencias.Rows)
                {
                    if (!row.IsNewRow)
                    {
                        foreach (DataGridViewCell cell in row.Cells)
                        {
                            string valor = cell.Value != null ? cell.Value.ToString() : "";
                            Cell dataCell = new Cell().Add(new Paragraph(valor).SetFontSize(8));
                            table.AddCell(dataCell);
                        }
                    }
                }

                doc.Add(table);
            }

            MessageBox.Show("Reporte PDF generado exitosamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
    catch (Exception ex)
    {
        MessageBox.Show("Error al generar el PDF: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
    }
}
    }
}