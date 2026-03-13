# Menadżer Wydarzeń

Prosta aplikacja do zarządzania codziennymi zadaniami, stworzona w technologii WPF oraz .NET z użyciem Entity Framework Core.

## Funkcje
* Dodawanie nowych zadań z nazwą, datą i godzinami (start/koniec).
* Inteligentne parsowanie godzin.
* Śledzenie statusu zadań (Oczekujące, Trwa, Zrealizowane).
* Automatyczne zliczanie zadań w pasku statusu.
* Filtrowanie i przeglądanie zadań w tabeli.

## Technologie
* C# / WPF
* .NET 10.0
* SQLite (przez Entity Framework Core)

## Jak uruchomić?
1. Upewnij się, że masz zainstalowane .NET SDK.
2. Sklonuj repozytorium lub pobierz pliki.
3. W terminalu, w głównym folderze projektu, wpisz:
   - ```bash dotnet build ```
   - ```bash dotnet ef database update```
   - ```bash dotnet run```