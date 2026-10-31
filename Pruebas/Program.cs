using Modelo;
using System.Runtime.InteropServices;

int verificaciones = 0;
void Verificar(bool c, string mensaje) { verificaciones++; if (!c) throw new Exception(mensaje); }
void Rechaza(Action accion, string mensaje)
{
    bool rechazo = false; try { accion(); } catch (ArgumentException) { rechazo = true; }
    Verificar(rechazo, mensaje);
}
var ejemplo = GrafoPonderado.CrearEjemplo(); var r = ejemplo.Dijkstra(0);
Verificar(r.Distancias.SequenceEqual(new long[] {0,3,2,8,10,13}), "Distancias del ejemplo");
Verificar(r.RutaHacia(5).SequenceEqual(new[] {0,2,1,3,4,5}), "Ruta óptima");
Verificar(r.Pasos.Select(p => p.VerticeFijado).SequenceEqual(new[] {-1,0,2,1,3,4,5}), "Orden de fijación");
Verificar(r.Pasos[1].Distancias[1] == 4 && r.Pasos[2].Distancias[1] == 3, "Mejora de B");
Verificar(r.Pasos[4].Distancias[5] == 14 && r.Pasos[5].Distancias[5] == 13, "Mejora de F");
Verificar(r.Pasos[0].Distancias[1] == long.MaxValue, "Instantánea inicial independiente");
Verificar(r.RutaHacia(0).SequenceEqual(new[] {0}) && r.Distancias[0] == 0, "Origen igual a destino");
var g0 = new GrafoPonderado(new[] {"A","B","C"}); g0.Conectar(0,1,0);
Verificar(g0.Dijkstra(0).Distancias[1] == 0, "Peso cero");
Verificar(g0.Dijkstra(0).Distancias[2] == long.MaxValue && g0.Dijkstra(0).RutaHacia(2).Count == 0, "Destino aislado");
g0.Conectar(0,1,int.MaxValue); g0.Conectar(1,2,int.MaxValue);
Verificar(g0.Dijkstra(0).Distancias[2] == 2L * int.MaxValue, "Distancia long sin desbordamiento int");
Verificar(g0.Desconectar(0,1) && g0.Peso(1,0) is null && !g0.Desconectar(0,1), "Eliminar conexión simétrica");
Rechaza(() => g0.Conectar(0,1,-1), "Rechazar negativo");
Rechaza(() => g0.Conectar(0,0,1), "Rechazar lazo");
Rechaza(() => g0.Dijkstra(-1), "Origen fuera de rango");
Rechaza(() => r.RutaHacia(9), "Destino fuera de rango");
Rechaza(() => new GrafoPonderado(Array.Empty<string>()), "Sin vértices");
Rechaza(() => new GrafoPonderado(new[] {"A","A"}), "Nombres repetidos");
var editado = GrafoPonderado.CrearEjemplo(); editado.Conectar(3,5,1);
Verificar(editado.Dijkstra(0).Distancias[5] == 9 && editado.Dijkstra(0).RutaHacia(5).SequenceEqual(new[] {0,2,1,3,5}), "Editar peso D F a 1");
editado = GrafoPonderado.CrearEjemplo(); editado.Desconectar(3,4);
Verificar(editado.Dijkstra(0).Distancias[5] == 14, "Quitar conexión D E");
editado.Desconectar(3,5); editado.Desconectar(4,5);
Verificar(editado.Dijkstra(0).RutaHacia(5).Count == 0, "Aislar F");
// Oráculo independiente: Floyd-Warshall calcula todos los pares por programación dinámica.
var azar = new Random(4302026); int grafos = 50, consultas = 0;
for (int caso = 0; caso < grafos; caso++)
{
    int n = azar.Next(1, 11); var g = new GrafoPonderado(Enumerable.Range(0,n).Select(i => "V"+i));
    long[,] d = new long[n,n];
    for (int i = 0; i < n; i++) for (int j = 0; j < n; j++) d[i,j] = i == j ? 0 : long.MaxValue;
    for (int i = 0; i < n; i++) for (int j = i+1; j < n; j++) if (azar.NextDouble() < .35)
    { int peso = azar.Next(0,31); g.Conectar(i,j,peso); d[i,j] = d[j,i] = peso; }
    for (int k = 0; k < n; k++) for (int i = 0; i < n; i++) for (int j = 0; j < n; j++)
        if (d[i,k] != long.MaxValue && d[k,j] != long.MaxValue) d[i,j] = Math.Min(d[i,j], d[i,k]+d[k,j]);
    for (int origen = 0; origen < n; origen++)
    {
        var resultado = g.Dijkstra(origen); consultas++;
        for (int destino = 0; destino < n; destino++)
        {
            Verificar(resultado.Distancias[destino] == d[origen,destino], "Dijkstra contra Floyd-Warshall");
            var ruta = resultado.RutaHacia(destino);
            if (d[origen,destino] == long.MaxValue) { Verificar(ruta.Count == 0, "Sin ruta a aislado"); continue; }
            Verificar(ruta[0] == origen && ruta[^1] == destino && ruta.Distinct().Count() == ruta.Count, "Extremos y sin ciclo en ruta");
            long costo = 0;
            for (int i = 1; i < ruta.Count; i++) { int? peso = g.Peso(ruta[i-1],ruta[i]); Verificar(peso.HasValue, "Arista de ruta existente"); costo += peso!.Value; }
            Verificar(costo == resultado.Distancias[destino], "Costo reconstruido");
        }
    }
}
Console.WriteLine("TAREA 4.3 | Pruebas reales del modelo de grafos");
Console.WriteLine($"Entorno: .NET {Environment.Version} | {RuntimeInformation.OSDescription}");
Console.WriteLine("CORRECTO: distancias desde A: A=0 B=3 C=2 D=8 E=10 F=13.");
Console.WriteLine("CORRECTO: ruta A -> C -> B -> D -> E -> F; costo 13 km.");
Console.WriteLine("CORRECTO: fijación A,C,B,D,E,F y mejora de F: 14 -> 13.");
Console.WriteLine("CORRECTO: pesos cero, desconexión, origen=destino, negativos rechazados y distancias long.");
Console.WriteLine($"CORRECTO: {grafos} grafos aleatorios y {consultas} orígenes contrastados con Floyd-Warshall.");
Console.WriteLine($"TOTAL: {verificaciones} verificaciones superadas.");
Console.WriteLine("TRAZA DEL EJEMPLO | Paso | Fijado | A B C D E F");
for (int i = 0; i < r.Pasos.Count; i++)
{
    var p = r.Pasos[i]; Console.WriteLine($"{i} | {(p.VerticeFijado < 0 ? "Inicio" : ejemplo.Nombres[p.VerticeFijado])} | {string.Join(" ",p.Distancias.Select(x => x == long.MaxValue ? "INF" : x.ToString()))} | {p.Cambios}");
}
