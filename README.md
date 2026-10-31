# Tarea 4.3 Grafo ponderado y Dijkstra

Aplicación de escritorio en C# con Windows Forms y .NET 8. El proyecto incluye código comentado en español, pruebas reproducibles, reporte PDF y Word y un guion separado para el video.

## Abrir en Windows

1. Extrae el ZIP completo antes de abrir los archivos.
2. Usa Visual Studio 2022 o posterior con la carga de trabajo **Desarrollo de escritorio de .NET** y el SDK de .NET 8.
3. Abre `RutasDijkstra.sln`.
4. Si es necesario, haz clic derecho en **Aplicacion** y elige **Establecer como proyecto de inicio**.
5. Presiona F5. La ventana carga el ejemplo automáticamente.

El formulario se construye desde C#; no requiere un archivo de diseñador. No utiliza paquetes NuGet externos.

También puedes abrir una terminal en esta carpeta y ejecutar:

```powershell
dotnet run --project Aplicacion/Aplicacion.csproj -c Release
```

`Ejecutar_en_Windows.bat` ejecuta ese mismo comando. Requiere el SDK de .NET 8.

## Pruebas

```powershell
dotnet run --project Pruebas/Pruebas.csproj -c Release
```

Las pruebas de consola son independientes de la ventana. El código fuente que se probó es el mismo que utiliza Aplicacion mediante ProjectReference. `Evidencias/pruebas_reales.txt` contiene la salida obtenida. `Evidencias/compilacion.txt` registra el compilador y el alcance de la compilación.

Las fuentes se compilaron con Roslyn del SDK 8.0.427 y referencias .NET y Windows Desktop 8.0.33, sin errores ni advertencias. Las pruebas se ejecutaron con .NET 8.0.33 en Ubuntu 24.04.3 LTS. La ventana no se ejecutó en Linux.

## Archivos de entrega

- Modelo: clases de la estructura y operaciones.
- Aplicacion: inicio, formulario, dibujo y exportación.
- Pruebas: programa de verificación del modelo.
- Evidencias: registros reales de pruebas y compilación.
- Reporte: PDF para entregar y Word editable para agregar el enlace de YouTube.
- Guion: instrucciones de pantalla entre corchetes y texto sencillo para leer.

El espacio para el enlace público del video está al final del reporte. El guion incluye la indicación de mantener la cámara en la esquina inferior derecha.

## Uso del grafo

Elige origen y destino y pulsa Calcular ruta. Para editar una arista, elige sus dos extremos, escribe el peso y pulsa Guardar conexión; el cambio se aplica en ambos sentidos. Quitar conexión elimina la arista. Cada edición recalcula la consulta. Restablecer ejemplo vuelve a la red original y a la consulta A a F.

Los seis lugares son ficticios: A almacén, B centro, C norte, D hospital, E mercado y F campus. Se permiten pesos cero y se rechazan negativos. El modelo usa null para ausencia de arista; la diagonal de la matriz se muestra como cero. La aplicación mantiene seis vértices y permite editar sus conexiones.

Resultado original: A -> C -> B -> D -> E -> F, 13 km. Si cambias D con F a peso 1, el resultado pasa a 9 km. La traza se calcula desde el origen a todos los vértices alcanzables. Un asterisco marca las distancias fijadas. Guardar resultados exporta la ruta, matriz, lista y traza.
