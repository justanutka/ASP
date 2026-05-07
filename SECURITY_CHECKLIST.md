# Security Checklist UniDesk - LAB 9

## 1. Walidacja danych wejściowych MVC
Formularze MVC muszą używać walidacji po stronie serwera.
Pola krytyczne, takie jak tytuł zgłoszenia, powinny mieć atrybuty Required oraz StringLength.

## 2. Walidacja danych wejściowych API
Endpointy API nie mogą akceptować pustych albo niepoprawnych danych.
Niepoprawne dane powinny zwracać odpowiedź 400 Bad Request.

## 3. Ochrona formularzy przed CSRF
Formularze POST w MVC muszą być zabezpieczone tokenem anty-CSRF.
Akcja POST powinna posiadać atrybut ValidateAntiForgeryToken.

## 4. Ryzyko formularzowe
Formularz bez tokenu anty-CSRF może zostać wywołany z zewnętrznej strony.
Taki atak może wykorzystać aktywną sesję użytkownika i wykonać operację bez jego świadomej zgody.

## 5. Nagłówki bezpieczeństwa HTTP
Aplikacja powinna zwracać podstawowe nagłówki bezpieczeństwa:
- X-Content-Type-Options: nosniff
- X-Frame-Options: DENY

## 6. Weryfikacja błędnych scenariuszy
Należy sprawdzać nie tylko poprawne żądania, ale też błędne przypadki.
Przykłady:
- pusty tytuł zgłoszenia,
- zbyt krótki tytuł,
- niepoprawny status,
- POST formularza bez tokenu CSRF.

---

# LAB 9 AMBITNE

## 1. Czego jeszcze nie robimy w tym kursie

W LAB 9 dodaliśmy podstawowe zabezpieczenia aplikacji UniDesk.
Aplikacja sprawdza dane wejściowe, formularz tworzenia zgłoszenia ma token anty-CSRF, a odpowiedzi HTTP zawierają podstawowe nagłówki bezpieczeństwa.
Na tym etapie nie robimy jeszcze pełnego systemu bezpieczeństwa jak w prawdziwej aplikacji produkcyjnej.

Nie zostały jeszcze wdrożone na przykład:
- pełne logowanie użytkowników,
- role użytkowników,
- sprawdzanie, kto ma dostęp do danego zgłoszenia,
- pełna polityka Content Security Policy,
- ograniczanie liczby zapytań do API,
- pełny zapis historii działań użytkownika.

Oznacza to, że aplikacja ma już podstawowe zabezpieczenia, ale nie jest jeszcze w pełni zabezpieczonym systemem produkcyjnym.

## 2. Co trzeba byłoby jeszcze poprawić w prawdziwym systemie

W prawdziwym systemie najważniejsze byłoby dodanie logowania i autoryzacji.
Obecnie użytkownik może korzystać z widoków MVC i endpointów API bez dokładnego sprawdzania jego uprawnień.
W aplikacji produkcyjnej należałoby sprawdzać, czy użytkownik jest zalogowany oraz czy ma prawo zobaczyć, utworzyć albo zmienić dane zgłoszenie.

## 3. Wniosek

Brak pełnego logowania nie oznacza, że temat bezpieczeństwa jest pomijany.
W LAB 9 skupiliśmy się na podstawach: walidacji danych, ochronie formularza przed CSRF oraz nagłówkach bezpieczeństwa.
To są pierwsze kroki, które przygotowują aplikację do dalszego zabezpieczania w kolejnych etapach.