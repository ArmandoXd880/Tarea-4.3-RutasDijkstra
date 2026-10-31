using Modelo;
using System.Text;
namespace Aplicacion;

public sealed class FormPrincipal : Form
{
    private GrafoPonderado grafo = GrafoPonderado.CrearEjemplo();
    private ResultadoDijkstra? resultado;
    private readonly ComboBox origen = Selector(), destino = Selector(), extremoA = Selector(), extremoB = Selector();
    private readonly NumericUpDown peso = new() { Minimum = 0, Maximum = 1000000, Value = 4, Width = 90 };
    private readonly Label estado = new() { Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft, Padding = new Padding(6), AutoEllipsis = true };
    private readonly DataGridView matriz = UtilidadesUI.Tabla(), lista = UtilidadesUI.Tabla(), traza = UtilidadesUI.Tabla();
    private readonly LienzoGrafo lienzo = new() { Dock = DockStyle.Fill };
    private static readonly string[] Lugares = { "A - Almacén", "B - Centro", "C - Norte", "D - Hospital", "E - Mercado", "F - Campus" };
    private static ComboBox Selector() => new() { DropDownStyle = ComboBoxStyle.DropDownList, Width = 145 };
    public FormPrincipal()
    {
        Text = "Tarea 4.3 | Rutas de distribución con Dijkstra"; Width = 1400; Height = 900; MinimumSize = new Size(1120, 760);
        StartPosition = FormStartPosition.CenterScreen; Font = new Font("Segoe UI", 10); AutoScaleMode = AutoScaleMode.Dpi;
        foreach (var combo in new[] { origen, destino, extremoA, extremoB }) { combo.Items.AddRange(Lugares); combo.SelectedIndex = 0; }
        destino.SelectedIndex = 5; extremoB.SelectedIndex = 1;
        var raiz = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 5, Padding = new Padding(10) };
        raiz.RowStyles.Add(new RowStyle(SizeType.Absolute, 60)); raiz.RowStyles.Add(new RowStyle(SizeType.Absolute, 60));
        raiz.RowStyles.Add(new RowStyle(SizeType.Absolute, 58)); raiz.RowStyles.Add(new RowStyle(SizeType.Percent, 100)); raiz.RowStyles.Add(new RowStyle(SizeType.Absolute, 62));
        var consulta = new FlowLayoutPanel { Dock = DockStyle.Fill, AutoScroll = true };
        Agregar(consulta, "Origen:", origen); Agregar(consulta, "Destino:", destino);
        consulta.Controls.Add(UtilidadesUI.Boton("Calcular ruta", Calcular));
        consulta.Controls.Add(UtilidadesUI.Boton("Restablecer ejemplo", () => { grafo = GrafoPonderado.CrearEjemplo(); origen.SelectedIndex = 0; destino.SelectedIndex = 5; Calcular(); }));
        var editar = new FlowLayoutPanel { Dock = DockStyle.Fill, AutoScroll = true };
        Agregar(editar, "Conexión:", extremoA); Agregar(editar, "con", extremoB); Agregar(editar, "km:", peso);
        editar.Controls.Add(UtilidadesUI.Boton("Guardar conexión", () => Editar(true)));
        editar.Controls.Add(UtilidadesUI.Boton("Quitar conexión", () => Editar(false)));
        editar.Controls.Add(UtilidadesUI.Boton("Guardar resultados", () => UtilidadesUI.GuardarTexto(this, "Resultados_Dijkstra.txt", Exportar())));
        editar.Controls.Add(UtilidadesUI.Boton("Guardar imagen", () => UtilidadesUI.GuardarImagen(this)));
        var cuerpo = new TableLayoutPanel { Dock = DockStyle.Fill, RowCount = 2, ColumnCount = 1 };
        cuerpo.RowStyles.Add(new RowStyle(SizeType.Percent, 52)); cuerpo.RowStyles.Add(new RowStyle(SizeType.Percent, 48));
        var tabs = new TabControl { Dock = DockStyle.Fill };
        foreach (var (titulo, tabla) in new[] { ("Matriz de adyacencia", matriz), ("Lista de adyacencia", lista), ("Traza de Dijkstra", traza) })
        { var tab = new TabPage(titulo); tab.Controls.Add(tabla); tabs.TabPages.Add(tab); }
        cuerpo.Controls.Add(lienzo, 0, 0); cuerpo.Controls.Add(tabs, 0, 1);
        raiz.Controls.Add(consulta, 0, 0); raiz.Controls.Add(editar, 0, 1); raiz.Controls.Add(estado, 0, 2); raiz.Controls.Add(cuerpo, 0, 3);
        raiz.Controls.Add(new Label { Dock = DockStyle.Fill, Text = "Red ficticia: distancias de ejemplo en kilómetros. Conexiones de doble sentido y pesos no negativos.\nLa traza calcula desde el origen a todos los lugares. INF significa inalcanzable o aún sin distancia conocida.", Padding = new Padding(4), AutoSize = false }, 0, 4);
        Controls.Add(raiz); Calcular();
    }
    private static void Agregar(FlowLayoutPanel panel, string texto, Control control)
    { panel.Controls.Add(new Label { Text = texto, AutoSize = true, Margin = new Padding(4, 8, 4, 0) }); panel.Controls.Add(control); }
    private void Editar(bool conectar)
    {
        try
        {
            if (conectar) grafo.Conectar(extremoA.SelectedIndex, extremoB.SelectedIndex, (int)peso.Value);
            else grafo.Desconectar(extremoA.SelectedIndex, extremoB.SelectedIndex);
            Calcular();
        }
        catch (ArgumentException e) { MessageBox.Show(this, e.Message, "Revisa la conexión", MessageBoxButtons.OK, MessageBoxIcon.Information); }
    }
    private static string Distancia(long x) => x == long.MaxValue ? "INF" : x.ToString();
    private void Calcular()
    {
        resultado = grafo.Dijkstra(origen.SelectedIndex); var ruta = resultado.RutaHacia(destino.SelectedIndex);
        estado.Text = ruta.Count == 0 ? "No existe una ruta entre el origen y el destino seleccionados."
            : $"Ruta mínima: {string.Join(" -> ", ruta.Select(i => grafo.Nombres[i]))}   |   Distancia: {resultado.Distancias[destino.SelectedIndex]} km";
        matriz.Columns.Clear(); matriz.Rows.Clear(); matriz.Columns.Add("Lugar", "Lugar");
        foreach (string n in grafo.Nombres) matriz.Columns.Add(n, n);
        for (int i = 0; i < grafo.Cantidad; i++)
        {
            var fila = new object[grafo.Cantidad + 1]; fila[0] = grafo.Nombres[i];
            for (int j = 0; j < grafo.Cantidad; j++) fila[j + 1] = i == j ? "0" : grafo.Peso(i, j)?.ToString() ?? "—";
            matriz.Rows.Add(fila);
        }
        lista.Columns.Clear(); lista.Rows.Clear(); lista.Columns.Add("Lugar", "Lugar"); lista.Columns.Add("Vecinos", "Vecinos y distancias");
        for (int i = 0; i < grafo.Cantidad; i++)
            lista.Rows.Add(grafo.Nombres[i], string.Join(", ", grafo.Vecinos(i).Select(v => $"{grafo.Nombres[v.Vecino]} ({v.Peso} km)")));
        traza.Columns.Clear(); traza.Rows.Clear(); traza.Columns.Add("Paso", "Paso"); traza.Columns.Add("Fijo", "Vértice fijado");
        foreach (string n in grafo.Nombres) traza.Columns.Add(n, "d(" + n + ")");
        traza.Columns.Add("Cambios", "Mejoras de distancia"); traza.Columns[traza.Columns.Count - 1].FillWeight = 330;
        for (int k = 0; k < resultado.Pasos.Count; k++)
        {
            var paso = resultado.Pasos[k]; var fila = new object[grafo.Cantidad + 3];
            fila[0] = k; fila[1] = paso.VerticeFijado < 0 ? "Inicio" : grafo.Nombres[paso.VerticeFijado];
            for (int j = 0; j < grafo.Cantidad; j++) fila[j + 2] = Distancia(paso.Distancias[j]) + (paso.Fijados[j] ? " *" : "");
            fila[^1] = paso.Cambios; traza.Rows.Add(fila);
        }
        traza.Columns[1].HeaderText = "Fijado (* = definitivo)";
        lienzo.Mostrar(grafo, ruta, origen.SelectedIndex, destino.SelectedIndex);
    }
    private string Exportar()
    {
        var s = new StringBuilder(); s.AppendLine("TAREA 4.3 | Dijkstra | Red ficticia de distribución"); s.AppendLine(estado.Text);
        foreach (var (titulo, tabla) in new[] { ("MATRIZ", matriz), ("LISTA", lista), ("TRAZA", traza) })
        {
            s.AppendLine(titulo); s.AppendLine(string.Join(" | ", tabla.Columns.Cast<DataGridViewColumn>().Select(c => c.HeaderText)));
            foreach (DataGridViewRow fila in tabla.Rows) s.AppendLine(string.Join(" | ", fila.Cells.Cast<DataGridViewCell>().Select(c => c.Value)));
        }
        return s.ToString();
    }
}

internal sealed class LienzoGrafo : Panel
{
    private GrafoPonderado? grafo; private HashSet<(int, int)> ruta = new(); private int origen, destino;
    public LienzoGrafo() { DoubleBuffered = true; BackColor = Color.White; BorderStyle = BorderStyle.FixedSingle; ResizeRedraw = true; }
    public void Mostrar(GrafoPonderado g, IReadOnlyList<int> camino, int a, int b)
    {
        grafo = g; origen = a; destino = b; ruta.Clear();
        for (int i = 1; i < camino.Count; i++) ruta.Add((Math.Min(camino[i - 1], camino[i]), Math.Max(camino[i - 1], camino[i])));
        Invalidate();
    }
    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e); if (grafo is null) return;
        var g = e.Graphics; g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
        PointF[] relativos = { new(.08f,.5f), new(.31f,.18f), new(.31f,.82f), new(.65f,.18f), new(.65f,.82f), new(.92f,.5f) };
        PointF[] posiciones = relativos.Select(p => new PointF(30 + p.X * (ClientSize.Width - 60), 25 + p.Y * (ClientSize.Height - 50))).ToArray();
        using var formato = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
        for (int a = 0; a < 6; a++) for (int b = a + 1; b < 6; b++)
        {
            if (grafo.Peso(a, b) is not int peso) continue;
            bool elegida = ruta.Contains((a, b)); using var pluma = new Pen(elegida ? Color.SeaGreen : Color.SlateGray, elegida ? 5 : 2);
            g.DrawLine(pluma, posiciones[a], posiciones[b]);
            PointF centro = new((posiciones[a].X + posiciones[b].X) / 2, (posiciones[a].Y + posiciones[b].Y) / 2);
            RectangleF caja = new(centro.X - 35, centro.Y - 12, 70, 24); g.FillRectangle(Brushes.White, caja); g.DrawString(peso.ToString(), Font, Brushes.Black, caja, formato);
        }
        using var fuente = new Font("Segoe UI", 12, FontStyle.Bold);
        for (int a = 0; a < 6; a++)
        {
            using var color = new SolidBrush(a == origen ? Color.RoyalBlue : a == destino ? Color.DarkOrange : Color.FromArgb(24, 89, 148));
            RectangleF caja = new(posiciones[a].X - 22, posiciones[a].Y - 22, 44, 44);
            g.FillEllipse(color, caja); g.DrawString(grafo.Nombres[a], fuente, Brushes.White, caja, formato);
        }
    }
}
