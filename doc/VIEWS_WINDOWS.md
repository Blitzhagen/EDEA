# Views und Windows

Diese Dokumentation beschreibt die XAML-Views, WPF-Windows und das Hauptfenster von EDEA. Sie ergänzt die ViewModel-Dokumentation und zeigt, welche ViewModel-Schicht hinter den jeweiligen Benutzeroberflächen steht.

## XAML-Views in `src/EDEA/Views/`

Die Views sind `UserControl`-Implementierungen, die ausschließlich das Layout definieren. Sie werden über DataTemplates in `MainWindow.xaml` oder `HudWindow.xaml` je nach aktivem ViewModel ausgewählt.

| View | Beschreibung | Verwendendes ViewModel |
|------|--------------|------------------------|
| `BodyHudTableView.xaml` | Kompakte Körper-Tabelle für das HUD-Overlay | `BodyTableViewModel` |
| `BodyTableView.xaml` | Haupttabelle der Himmelskörper im Körper-Tab | `BodyTableViewModel` |
| `GenusHudTableView.xaml` | Kompakte biologische Tabelle für das HUD-Overlay | `GenusTableViewModel` |
| `GenusTableView.xaml` | Haupttabelle der Genus-/Art-Daten im Biologisches-Tab | `GenusTableViewModel` |
| `HistoryTableView.xaml` | Übersicht der Historien- und Statistik-Informationen | `HistoryViewModel` |
| `NavRouteHudTableView.xaml` | Kompakte Routen-Tabelle für das HUD-Overlay | `NavRouteTableViewModel` |
| `RouteTableView.xaml` | Haupttabelle der Navigation/Route im Route-Tab | `NavRouteTableViewModel` |
| `SurroundingsTableView.xaml` | Tabelle der umliegenden Systeme im Umgebung-Tab | `SurroundingsTableViewModel` |

## WPF-Windows in `src/EDEA/Windows/`

Dialog- und Hilfsfenster werden von den entsprechenden ViewModels erzeugt, positioniert und geschlossen. Der `DataContext` wird jeweils im ViewModel-Code zugewiesen.

| Window | Beschreibung | DataContext / zugehöriges ViewModel |
|--------|--------------|--------------------------------------|
| `AboutWindow.xaml` | Versions-, Lizenz- und Kurzbeschreibung | `AboutViewModel` |
| `FeedbackReportIssueWindow.xaml` | Feedback-Formular mit Name, E-Mail und Nachricht | `FeedbackReportIssueViewModel` |
| `HudWindow.xaml` | Transparentes HUD-Overlay mit bewegbarem Inhalt | `HudViewModel` |
| `InputStringDialogWindow.xaml` | Eingabe-Dialog für einzelne Texte | Wird per Aufrufparameter gefüllt |
| `InputStringListDialogWindow.xaml` | Auswahldialog für mehrere Texte | Wird per Aufrufparameter gefüllt |
| `JournalHistoryImportWindow.xaml` | Fortschrittsanzeige beim Journal-Historien-Import | `JournalHistoryImportViewModel` |
| `PreferencesWindow.xaml` | Einstellungsfenster mit Farben, HUD, Sprache, Hotkeys und POIs | `PreferencesViewModel` |
| `RoutePlotterWindow.xaml` | Dialog zur Berechnung einer Neutronen-Route über Spansh | `RoutePlotterViewModel` |

## `MainWindow`

`MainWindow.xaml` ist das zentrale Fenster der Anwendung.

### Charakteristika

- Besitzt eine benutzerdefinierte Titelleiste (kein Standardrahmen, `WindowStyle="None"`, `AllowsTransparency="True"`).
- Enthält eine Statusleiste mit dem aktuellen Systemnamen, Erforschungsstatus, Körper-/Nicht-Körper-Signalen, Gesamtfortschritt und aktueller Aktivität.
- Bietet ein Menü mit Zugriff auf HUD, EDSM-Neuladen, Plotter, Einstellungen, About und Feedback.
- Hostet ein `TabControl` mit `TabStripPlacement="Left"`, dessen `ItemsSource` an `MainViewModel.TabViewModels` gebunden ist.

### View-ViewModel-Verknüpfung

`MainWindow` bindet sich an `MainViewModel`. Der konkrete Inhalt eines Tabs wird über `TabTemplateSelector` und DataTemplates im Fenster-`ResourceDictionary` aufgelöst:

| DataTemplate-Schlüssel | View |
|------------------------|------|
| `Route` | `RouteTableView` |
| `Bodies` | `BodyTableView` |
| `Biologicals` | `GenusTableView` |
| `Surroundings` | `SurroundingsTableView` |
| `History` | `HistoryTableView` |

Die `TabItem`-Header und -Sichtbarkeiten kommen direkt aus den jeweiligen `TabViewModel`-Instanzen (`TabHeader`, `TabVisibility`).

### HUD-Window

`HudWindow` verwendet ebenfalls DataTemplates, um zwischen den HUD-Varianten der drei Haupttabs zu wechseln:

| ViewModel | HUD-View |
|-----------|----------|
| `BodyTableViewModel` | `BodyHudTableView` |
| `NavRouteTableViewModel` | `NavRouteHudTableView` |
| `GenusTableViewModel` | `GenusHudTableView` |

`HudViewModel.CurrentViewModel` legt fest, welche View aktuell im HUD angezeigt wird.

## Verknüpfung zwischen ViewModels und Views

- `MainWindow` → `MainViewModel` (zugewiesen in `App.xaml.cs`).
- Tab-Inhalte → `TabViewModel`-basierte DataTemplate-Auswahl über `TabTemplateSelector`.
- `HudWindow` → `HudViewModel`, dessen `CurrentViewModel` durch DataTemplates in `HudWindow.xaml` aufgelöst wird.
- Alle weiteren Fenster werden durch ihre ViewModels instanziiert und bekommen dort über `window.DataContext = this;` den DataContext zugewiesen.
