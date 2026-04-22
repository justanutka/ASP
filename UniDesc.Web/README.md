# UniDesc Web Application

W tym pliku znajdziesz instrukcje, jak uruchomić projekt oraz opis folderów

## Jak uruchomić projekt z poziomu Visual Studio

1. Otwórz projekt w Visual Studio
2. Kliknij na przycisk "Start" lub naciśnij F5, aby uruchomić aplikację w trybie debugowania
3. Projekt uruchomi się w domyślnej przeglądarce pod adresem http://localhost:5000.

## Jak uruchomić projekt z poziomu terminala

1. Otwórz terminal i przejdź do folderu z projektem
2. Wpisz polecenie:

    ```bash
    dotnet run
    ```

3. Aplikacja powinna uruchomić się na porcie http://localhost:5000

## Opis folderów:

### **Controllers**
Folder zawiera kontrolery, które odpowiadają za logikę aplikacji, przetwarzanie żądań HTTP i zwracanie odpowiednich widoków

### **Views**
Folder zawiera pliki widoków, które są renderowane i wyświetlane użytkownikowi w przeglądarkach

### **wwwroot**
Folder zawierający zasoby statyczne, takie jak pliki CSS, JavaScript, obrazy, które są dostępne publicznie w aplikacji


## Techniczne notatki: Separacja modeli

Rozdzielenie modeli w aplikacjach jest ważnym elementem dobrego projektowania architektury systemu. W przypadku Entity Framework (EF) i modelu domenowego, warto rozdzielić te dwa modele z kilku powodów:

1. **Model danych (Entity Framework)** jest bezpośrednio połączony z bazą danych i zawiera informacje, które są związane z technologią bazy danych. Ma szczegóły dotyczące kluczy głównych, indeksów i innych reguł potrzebnych do pracy z bazą danych.
   
2. **Model domenowy** (model biznesowy) jest niezależny od technologii bazy danych i reprezentuje logikę aplikacji. Jest bardziej elastyczny, ponieważ nie zależy od struktury bazy danych, co pozwala łatwiej wdrażać i testować logikę aplikacji.

Podsumowując, rozdzielenie modeli sprawia, że aplikacja jest bardziej elastyczna i łatwiejsza do zarządzania w dłuższej perspektywie


## Notatka techniczna lab 5 
W części AMBITNE zastosowano mechanizm whitelisty pól dozwolonych do sortowania. Oznacza to, że system akceptuje jedynie ściśle określone wartości parametru SortBy, a wszystkie pozostałe są odrzucane. W aktualnej implementacji dozwolone są tylko pola: CreatedAt, Title oraz Status.

Takie rozwiązanie zostało wprowadzone ze względów bezpieczeństwa, stabilności oraz czytelności kontraktu API. Przyjmowanie surowego string jako nazwy pola do sortowania byłoby zbyt elastyczne i mogłoby prowadzić do niepożądanych skutków. Użytkownik mógłby próbować sortować po polach, które nie są przewidziane w publicznym API, na przykład po polach technicznych, pomocniczych lub wewnętrznych. W praktyce zwiększa to ryzyko ujawnienia struktury modelu danych (Information Disclosure), ponieważ sam fakt istnienia lub braku danego pola może dostarczać informacji o wewnętrznej budowie encji i logice systemu.

Dodatkowo zbyt ogólny mechanizm sortowania utrudnia kontrolę poprawności działania aplikacji. Pozwala przekazywać wartości, których system nie powinien obsługiwać, co może prowadzić do błędów wykonania, nieprzewidywalnych rezultatów lub konieczności rozbudowy kodu o dodatkowe zabezpieczenia. Z punktu widzenia utrzymania projektu bardziej przewidywalne i bezpieczne jest jawne określenie listy pól, po których wolno sortować dane.

Zastosowana whitelist zapewnia, że system działa w sposób kontrolowany i zgodny z założeniami projektowymi. Jeśli użytkownik poda niedozwoloną wartość, aplikacja nie próbuje wykonywać takiego sortowania, lecz zwraca błąd 400 Bad Request. Dzięki temu mechanizm jest odporny na nieautoryzowane lub błędne parametry wejściowe.

Takie podejście jest prostsze i bezpieczniejsze niż budowanie całkowicie uniwersalnego mechanizmu sortowania. Whitelist ogranicza możliwe operacje tylko do tych, które zostały wcześniej zaprojektowane, przetestowane i zaakceptowane. W efekcie poprawia bezpieczeństwo aplikacji, zmniejsza ryzyko błędów oraz ułatwia dalszy rozwój systemu.

## Notatka techniczna lab 6 
W modelu Ticket używane są DateTime.Now oraz DateTime.UtcNow, co powoduje niedeterministyczne testy.

Nie zmieniałam istniejącego kodu, aby nie zepsuć wcześniejszych zadań

Zamiast tego wprowadzono:
- interfejs ISystemClock, który definiuje sposób pobierania aktualnego czasu. Dzięki temu logika biznesowa nie musi bezpośrednio korzystać z DateTime.Now
- jego implementację SystemClock, klasa która jest implementacją interfejsu i zwraca rzeczywisty czas systemowy DateTime.Now
- możliwość użycia FakeClock w testach, pozwala ustawić stałą, kontrolowaną wartość czasu

Pozwala to w przyszłości łatwo kontrolować czas i poprawić testowalność systemu.


## Notatka techniczna lab 8 - AMBITME

### Co framework zapewnia automatycznie
ASP.NET Core automatycznie wspiera serializację JSON, model binding, walidację modeli, obsługę kontrolerów oraz generowanie dokumentacji Swagger/OpenAPI. Framework pomaga również w zwracaniu ustandaryzowanych odpowiedzi błędów, takich jak ProblemDetails, dzięki czemu komunikacja API jest bardziej spójna.

### Co nadal wymaga decyzji programisty
Programista nadal musi sam zdecydować, jakie kody HTTP są poprawne, które odpowiedzi powinny być jawnie opisane oraz w jakich miejscach 404 Not Found powinno być częścią kontraktu API. To programista wybiera również DTO wystawiane na zewnątrz i odpowiada za to, czy wygenerowana dokumentacja rzeczywiście odzwierciedla zamierzony kontrakt systemu.

### Notatka inżynierska
Framework bardzo pomaga, ale nie projektuje kontraktu API za programistę. Jeżeli metadata są niepełne albo semantyka odpowiedzi została źle dobrana, Swagger nadal pokaże dokumentację, ale nie musi ona w pełni odpowiadać temu, co rzeczywiście chcieliśmy wystawić.
