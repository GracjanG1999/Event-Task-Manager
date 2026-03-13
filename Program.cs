using System;
using System.Collections.Generic;

var zadania = new List<string> { "Kupić mleko", "Nauczyć się C#" };

Console.WriteLine("--- TWOJA LISTA ZADAŃ ---");
foreach (var zadanie in zadania)
{
    Console.WriteLine($"- [ ] {zadanie}");
}