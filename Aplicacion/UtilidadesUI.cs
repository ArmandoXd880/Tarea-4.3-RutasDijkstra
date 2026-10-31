using System.Drawing.Imaging;
namespace Aplicacion;
internal static class UtilidadesUI
{
    public static Button Boton(string texto, Action accion)
    {
        var b = new Button { Text = texto, AutoSize = true, Height = 32, Padding = new Padding(6, 2, 6, 2), Margin = new Padding(4) };
        b.Click += (_, _) => accion(); return b;
    }
    public static DataGridView Tabla()
    {
        return new DataGridView { Dock = DockStyle.Fill, ReadOnly = true, AllowUserToAddRows = false,
            AllowUserToDeleteRows = false, RowHeadersVisible = false, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
            AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells, BackgroundColor = Color.White, SelectionMode = DataGridViewSelectionMode.FullRowSelect,
            DefaultCellStyle = new DataGridViewCellStyle { WrapMode = DataGridViewTriState.True, Padding = new Padding(4) } };
    }
    public static void GuardarTexto(Form padre, string nombre, string texto)
    {
        using var dialogo = new SaveFileDialog { Filter = "Archivo de texto|*.txt", FileName = nombre };
        if (dialogo.ShowDialog(padre) != DialogResult.OK) return;
        try { File.WriteAllText(dialogo.FileName, texto, System.Text.Encoding.UTF8); }
        catch (Exception e) { MessageBox.Show(padre, e.Message, "No se pudo guardar", MessageBoxButtons.OK, MessageBoxIcon.Error); }
    }
    public static void GuardarImagen(Form padre)
    {
        using var dialogo = new SaveFileDialog { Filter = "Imagen PNG|*.png", FileName = "Ventana.png" };
        if (dialogo.ShowDialog(padre) != DialogResult.OK) return;
        try
        {
            using var imagen = new Bitmap(padre.Width, padre.Height);
            padre.DrawToBitmap(imagen, new Rectangle(Point.Empty, padre.Size)); imagen.Save(dialogo.FileName, ImageFormat.Png);
        }
        catch (Exception e) { MessageBox.Show(padre, e.Message, "No se pudo guardar", MessageBoxButtons.OK, MessageBoxIcon.Error); }
    }
}
