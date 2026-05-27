# 📋 Menadżer Wydarzeń

> Desktopowa aplikacja WPF do zarządzania wydarzeniami, zadaniami i terminami — z automatyczną zmianą statusów, powiadomieniami, filtrowaniem i eksportem CSV.

---

## ✨ Funkcje

| Ikona | Funkcja | Opis |
|:---:|---|---|
| ➕ | **Dodawanie wydarzeń** | Przycisk „+" otwiera dialog z pełnym formularzem |
| ✏️ | **Pełna edycja** | Przycisk „Edytuj" lub **dwuklik** na wierszu |
| 🗑️ | **Usuwanie** | Usuwanie z potwierdzeniem |
| 🔍 | **Filtrowanie** | Szukaj po tekście, filtruj po statusie i kategorii |
| 🎨 | **Kolorowanie wierszy** | Wizualne oznaczenie stanu każdego wydarzenia |
| 🔄 | **Auto-status** | Status zmienia się automatycznie w oparciu o datę i godzinę |
| 📅 | **Widok kalendarza** | Wybierz datę i przeglądaj wydarzenia w danym dniu |
| 🔔 | **Powiadomienia** | Popup 15 min przed startem wydarzenia (włącz/wyłącz) |
| 📊 | **Statystyki** | Licznik oczekujących, aktywnych, zrealizowanych i przeterminowanych |
| 💾 | **Eksport CSV** | Zapis całej listy wydarzeń do pliku `.csv` |
| 🔢 | **Sortowanie** | Kliknij nagłówek kolumny, aby posortować |
| 📭 | **Pusta lista** | Komunikat gdy żadne wydarzenie nie pasuje do filtrów |

---

## 🔄 Automatyczna zmiana statusu

Aplikacja co minutę (i przy starcie) sprawdza każde wydarzenie i aktualizuje jego status:

| Warunek | Nowy status |
|---|---|
| Data planowana < dziś | ✅ **Zrealizowane** |
| Data = dziś, godzina ≥ startu i < końca | 🔵 **Trwa** |
| Data = dziś, godzina ≥ końca | ✅ **Zrealizowane** |

> ⚠️ Status zmienia się tylko **do przodu** (Oczekujące → Trwa → Zrealizowane). Ręcznie ustawione „Zrealizowane" nigdy nie jest cofane.

---

## 🎨 Legenda kolorów

| Kolor | Znaczenie |
|:---:|---|
| 🟩 Zielony | Wydarzenie **zrealizowane** |
| 🟨 Żółty | Wydarzenie **w trakcie** |
| 🟥 Czerwony | Wydarzenie **przeterminowane** (data minęła, status ≠ Zrealizowane) |
| ⬜ Biały | Wydarzenie **oczekujące**, termin jeszcze nie minął |

---

## 🔢 Priorytety

| Priorytet | Kiedy używać |
|---|---|
| **Niski** | Zadania nieistotne czasowo |
| **Średni** *(domyślny)* | Standardowe zadania |
| **Wysoki** | Ważne, wymaga uwagi |
| **Krytyczny** | Musi być wykonane natychmiast |

---

## 🖥️ Technologie

| Technologia | Wersja | Zastosowanie |
|---|---|---|
| **C#** | 13 | Język programowania |
| **.NET** | 10 | Platforma uruchomieniowa |
| **WPF** | — | Interfejs użytkownika (tylko Windows) |
| **Entity Framework Core** | 10.0.5 | Dostęp do bazy danych |
| **SQLite** | — | Lokalna baza danych (`MojaBaza.db`) |

---

## 📦 Wymagania

- ✅ System **Windows 10** lub nowszy
- ✅ [**.NET 10 Runtime**](https://dotnet.microsoft.com/download/dotnet/10.0)
- ✅ Visual Studio 2022+ lub `dotnet CLI`

---

## 🚀 Uruchomienie

### Przez Visual Studio
1. Otwórz `MenadzerWydarzen.csproj`
2. Naciśnij `F5`

### Przez terminal
```bash
cd MenadzerWydarzen
dotnet run
```

### Budowanie wersji Release
```bash
dotnet publish -c Release -r win-x64 --self-contained
```

---

## 📁 Struktura projektu

```
ToDoList/
├── MenadzerWydarzen/
│   ├── 📄 Wydarzenie.cs              # Model danych (event + enums)
│   ├── 📄 AppDbContext.cs            # EF Core + auto-migracja nowych kolumn
│   ├── 📄 WydarzenieHelper.cs        # Współdzielone metody pomocnicze (DRY)
│   │
│   ├── 🖼️  MainWindow.xaml           # Główny interfejs: lista, filtry, kalendarz, FAB
│   ├── 📄 MainWindow.xaml.cs         # Logika: filtrowanie, auto-status, powiadomienia
│   │
│   ├── 🖼️  AddWindow.xaml            # Dialog dodawania nowego wydarzenia (FAB +)
│   ├── 📄 AddWindow.xaml.cs          # Walidacja + tworzenie obiektu Wydarzenie
│   │
│   ├── 🖼️  EditWindow.xaml           # Dialog edycji wydarzenia
│   ├── 📄 EditWindow.xaml.cs         # Walidacja + zapis zmian
│   │
│   ├── 🖼️  NotificationPopup.xaml    # Popup powiadomienia (bottom-right)
│   ├── 📄 NotificationPopup.xaml.cs  # Auto-zamknięcie po 8 s
│   │
│   ├── 📄 StatusConverter.cs         # EventStatus  →  tekst PL
│   ├── 📄 PriorityConverter.cs       # EventPriority → tekst PL
│   └── 📄 RowColorConverter.cs       # Status + Data  → kolor wiersza
│
├── .gitignore
└── README.md
```

---

## 🔔 Powiadomienia

Aplikacja sprawdza co minutę, czy któreś z wydarzeń zaczyna się **w ciągu najbliższych 15 minut**.
Jeśli tak — wyświetla popup w prawym dolnym rogu ekranu, który znika po **8 sekundach**.

- Przycisk **🔔 Powiadomienia: WŁĄCZONE / 🔕 WYŁĄCZONE** przełącza tę funkcję
- Każde wydarzenie jest notyfikowane tylko **raz na sesję**

---

## 📤 Eksport CSV

Kliknij **„Eksport CSV"** → wybierz lokalizację zapisu.  
Plik zawiera kolumny: `Id, Nazwa, Data, Start, Koniec, Status, Priorytet, Kategoria, Opis`.

---

## 🗄️ Baza danych

| Cecha | Szczegół |
|---|---|
| Silnik | SQLite |
| Plik | `MojaBaza.db` (katalog roboczy aplikacji) |
| Tworzenie | Automatyczne przy pierwszym uruchomieniu |
| Migracje | Nowe kolumny dodawane automatycznie bez utraty danych |

---

## 🧱 Zasady projektowania

| Zasada | Jak jest spełniona |
|---|---|
| **SRP** | Każdy konwerter i klasa ma jedną odpowiedzialność |
| **DRY** | `WydarzenieHelper` — jedna implementacja `ParseTime` i `NullIfBlank` |
| **DRY** | `LoadAll()` — enkapsulacja wzorca ładowania danych |
| **KISS** | Brak zbędnych warstw abstrakcji dla projektu tej skali |
| **YAGNI** | Kod odpowiada tylko na realne, zaimplementowane potrzeby |

---

*Autor: Gracjan*
