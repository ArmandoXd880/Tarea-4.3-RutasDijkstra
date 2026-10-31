namespace Modelo;

public sealed class PasoDijkstra
{
    public int VerticeFijado { get; }
    public IReadOnlyList<long> Distancias { get; }
    public IReadOnlyList<bool> Fijados { get; }
    public string Cambios { get; }
    internal PasoDijkstra(int vertice, long[] distancias, bool[] fijados, string cambios)
    {
        VerticeFijado = vertice;
        Distancias = Array.AsReadOnly((long[])distancias.Clone());
        Fijados = Array.AsReadOnly((bool[])fijados.Clone());
        Cambios = cambios;
    }
}

public sealed class ResultadoDijkstra
{
    public int Origen { get; }
    public IReadOnlyList<long> Distancias { get; }
    public IReadOnlyList<int> Anteriores { get; }
    public IReadOnlyList<PasoDijkstra> Pasos { get; }
    internal ResultadoDijkstra(int origen, long[] distancias, int[] anteriores, List<PasoDijkstra> pasos)
    {
        Origen = origen; Distancias = Array.AsReadOnly((long[])distancias.Clone());
        Anteriores = Array.AsReadOnly((int[])anteriores.Clone()); Pasos = pasos.AsReadOnly();
    }
    public IReadOnlyList<int> RutaHacia(int destino)
    {
        if (destino < 0 || destino >= Distancias.Count) throw new ArgumentOutOfRangeException(nameof(destino));
        var ruta = new List<int>();
        if (Distancias[destino] == long.MaxValue) return ruta.AsReadOnly();
        for (int n = destino; n != -1; n = Anteriores[n]) ruta.Add(n);
        ruta.Reverse(); return ruta.AsReadOnly();
    }
}

// Grafo no dirigido. null indica ausencia de arista; cero sí puede ser un peso.
public sealed class GrafoPonderado
{
    private readonly int?[,] pesos;
    public IReadOnlyList<string> Nombres { get; }
    public int Cantidad => Nombres.Count;
    public GrafoPonderado(IEnumerable<string> nombres)
    {
        ArgumentNullException.ThrowIfNull(nombres);
        string[] datos = nombres.ToArray();
        if (datos.Length == 0 || datos.Length > 100 || datos.Any(string.IsNullOrWhiteSpace)
            || datos.Distinct(StringComparer.Ordinal).Count() != datos.Length)
            throw new ArgumentException("Use entre 1 y 100 nombres únicos y no vacíos.");
        Nombres = Array.AsReadOnly(datos); pesos = new int?[Cantidad, Cantidad];
    }
    private void Validar(int v)
    {
        if (v < 0 || v >= Cantidad) throw new ArgumentOutOfRangeException(nameof(v));
    }
    public int? Peso(int a, int b) { Validar(a); Validar(b); return pesos[a, b]; }
    public void Conectar(int a, int b, int peso)
    {
        Validar(a); Validar(b);
        if (a == b) throw new ArgumentException("El ejemplo no utiliza conexiones de un lugar consigo mismo.");
        if (peso < 0) throw new ArgumentOutOfRangeException(nameof(peso), "Dijkstra requiere pesos no negativos.");
        pesos[a, b] = pesos[b, a] = peso;
    }
    public bool Desconectar(int a, int b)
    {
        Validar(a); Validar(b); bool existia = pesos[a, b].HasValue;
        pesos[a, b] = pesos[b, a] = null; return existia;
    }
    public IReadOnlyList<(int Vecino, int Peso)> Vecinos(int v)
    {
        Validar(v); var lista = new List<(int, int)>();
        for (int j = 0; j < Cantidad; j++) if (pesos[v, j] is int peso) lista.Add((j, peso));
        return lista.AsReadOnly();
    }
    public ResultadoDijkstra Dijkstra(int origen)
    {
        Validar(origen);
        long[] distancias = Enumerable.Repeat(long.MaxValue, Cantidad).ToArray();
        int[] anteriores = Enumerable.Repeat(-1, Cantidad).ToArray();
        bool[] fijados = new bool[Cantidad]; var pasos = new List<PasoDijkstra>();
        distancias[origen] = 0;
        pasos.Add(new PasoDijkstra(-1, distancias, fijados, "Inicio: origen con distancia 0."));
        for (int vuelta = 0; vuelta < Cantidad; vuelta++)
        {
            int u = -1;
            // Los empates se resuelven por el índice menor para reproducir la traza.
            for (int i = 0; i < Cantidad; i++)
                if (!fijados[i] && distancias[i] != long.MaxValue
                    && (u == -1 || distancias[i] < distancias[u])) u = i;
            if (u == -1) break; // Los restantes son inalcanzables.
            fijados[u] = true; var cambios = new List<string>();
            for (int v = 0; v < Cantidad; v++)
            {
                if (fijados[v] || pesos[u, v] is not int peso) continue;
                long alternativa = distancias[u] + peso;
                if (alternativa < distancias[v])
                {
                    string previo = distancias[v] == long.MaxValue ? "INF" : distancias[v].ToString();
                    distancias[v] = alternativa; anteriores[v] = u;
                    cambios.Add($"{Nombres[v]}: {previo} -> {alternativa} vía {Nombres[u]}");
                }
            }
            pasos.Add(new PasoDijkstra(u, distancias, fijados, cambios.Count == 0 ? "Sin mejoras." : string.Join("; ", cambios)));
        }
        return new ResultadoDijkstra(origen, distancias, anteriores, pasos);
    }
    public static GrafoPonderado CrearEjemplo()
    {
        var g = new GrafoPonderado(new[] { "A", "B", "C", "D", "E", "F" });
        foreach (var (a, b, peso) in new[] { (0,1,4), (0,2,2), (1,2,1), (1,3,5), (2,3,8), (2,4,10), (3,4,2), (3,5,6), (4,5,3) })
            g.Conectar(a, b, peso);
        return g;
    }
}
