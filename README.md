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
