# goboosting-AFK

## Opis
Plugin wyświetla boostującym graczom menu. Gracz musi wpisać komendę z odpowiednim numerem na czacie albo zabić w tym czasie przeciwnika, aby pomyślnie przejść test. W przeciwnym razie - w zależności od konfiguracji - gracz otrzymuje kicka, bana albo nic.

## Wymagania
- [CounterStrikeSharp](https://github.com/roflmuffin/CounterStrikeSharp/releases) v201
- [Metamod:Source](https://www.sourcemm.net/downloads.php/?branch=master)

## Instalacja
1. Pobierz [najnowszą wersję](https://github.com/CS-GEJMERZY/goboosting-AFK/releases/latest)
2. Wypakuj pliki i wgraj do **_csgo/addons/counterstrikesharp/plugins_**

## Konfiguracja
Przy pierwszym uruchomieniu pluginu, plik **_goboosting-AFK.json_** zostanie automatycznie utworzony w **_csgo/addons/counterstrikesharp/configs/plugins/Plugin_**
```
{
  "KluczApi": "",
  "TypReakcji": 0,
  "BanKomenda": "css_ban #{USERID} {CZAS_W_MINUTACH} {POWOD}",
  "BanCzasMinuty": 5,
  "BanPowod": "GO-BOOSTING AFK",
  "KickKomenda": "css_kick #{PLAYER_USERID} {REASON}",
  "KickPowod": "GO-BOOSTING AFK",
  "CzasMenuAfk": 15,
  "ConfigVersion": 1
}
```

- Klucz API - znajdziesz na stronie https://goboosting.pl/?profil
- Typ Reakcji:
  - 0 = Zakończ boosting, bez dalszej akcji
  - 1 = wyrzuć gracza
  - 2 = zbanuj gracza=
- Formatowanie komend ban i kick:
  - _NICK_ - nick gracza
  - _USERID_ - userid, np. 123
  - _ADRES_IP_ - Adres IP gracza, np 192.168.0.1
  - _POWOD_ - powód kicka, bana; np. GO-BOOSTING AFK
  - _CZAS_W_MINUTACH_ -  Czas banu w minutach
- CzasMenuAfk -  Czas wyświetlania menu testu AFK w sekundach

## Komendy
- css_gb_testafk - Uruchamia test AFK dla graczy, dostępny dla @css/root

  