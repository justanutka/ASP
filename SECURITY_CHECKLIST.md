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