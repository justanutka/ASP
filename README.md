# UniDesk

UniDesk to aplikacja ASP.NET Core MVC/API do obsługi zgłoszeń. Repozytorium jest uporządkowane jako jedno rozwiązanie Visual Studio z trzema projektami:

- `src/UniDesk.Web` - aplikacja webowa MVC, API, Minimal API, EF Core i SQLite.
- `tests/UniDesk.UnitTests` - testy jednostkowe modelu oraz logiki serwisowej.
- `tests/UniDesk.IntegrationTests` - testy integracyjne endpointów HTTP uruchamiane in-memory.

## Uruchomienie w Visual Studio

1. Otwórz plik `UniDesk.sln`.
2. Ustaw projekt `UniDesk.Web` jako projekt startowy.
3. Uruchom aplikację przyciskiem Start albo klawiszem F5.
4. Strona główna powinna wyświetlić napis `Hello UniDesk`.

## Uruchomienie z terminala

```bash
dotnet build UniDesk.sln
dotnet run --project src/UniDesk.Web/UniDesk.Web.csproj
```

Po uruchomieniu aplikacja korzysta z adresów z `src/UniDesk.Web/Properties/launchSettings.json`.

## Testy

```bash
dotnet test UniDesk.sln
```

Rozwiązanie powinno kompilować się z wynikiem `0 warnings` i `0 errors`.

## Struktura projektu

- `Controllers` - kontrolery MVC i API obsługujące żądania HTTP.
- `DTOs` - kontrakty wejścia i wyjścia API, oddzielone od modelu domenowego.
- `Endpoints` - wydzielona konfiguracja endpointów Minimal API `/api/v2/tickets`.
- `Filters` - filtry endpointów, m.in. walidacja i pomiar czasu żądania.
- `Models` - model domenowy zgłoszenia oraz obiekty zapytań i stronicowania.
- `Services` - warstwa usług i dostęp do danych przez EF Core.
- `Views` - widoki Razor dla części MVC.
- `wwwroot` - statyczne zasoby aplikacji, np. CSS, JavaScript i biblioteki frontendowe.

## Przydatne endpointy

- `GET /Tickets` - lista zgłoszeń w MVC.
- `GET /About` - strona informacyjna.
- `GET /api/tickets` - lista zgłoszeń jako JSON.
- `POST /api/tickets` - utworzenie zgłoszenia przez klasyczny kontroler API.
- `GET /api/v2/tickets` - lista zgłoszeń przez Minimal API.
- `POST /api/v2/tickets` - utworzenie zgłoszenia przez Minimal API.
- `PUT /api/v2/tickets/{id}` - aktualizacja zgłoszenia przez Minimal API.
- `DELETE /api/v2/tickets/{id}` - usunięcie zgłoszenia przez Minimal API.
