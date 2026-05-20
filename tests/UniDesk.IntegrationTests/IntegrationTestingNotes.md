# Testy jednostkowe a testy integracyjne

Testy jednostkowe służą do sprawdzania małych fragmentów logiki aplikacji bez uruchamiania całego API, bez routingu i bez prawdziwej bazy danych. W tym projekcie mogą one sprawdzać na przykład logikę zmiany statusu ticketu, walidację danych albo działanie serwisów.

Testy integracyjne sprawdzają, czy cała aplikacja działa poprawnie jako API. Obejmują one endpointy, kody odpowiedzi HTTP, przesyłanie i odbieranie JSON-a, działanie kontrolerów, dependency injection oraz zapis i odczyt danych z testowej bazy danych. Dlatego w tych testach użyto `WebApplicationFactory`, `HttpClient` i bazy SQLite in-memory.

Na tym etapie nie ma sensu pisać testów integracyjnych dla każdego drobnego przypadku, na przykład dla wszystkich możliwych błędnych wartości enuma albo szczegółów mapowania EF Core. Takie rzeczy lepiej sprawdzać testami jednostkowymi. Testy integracyjne powinny skupiać się głównie na najważniejszych scenariuszach działania API.
