# Funkční specifikace zápočtového programu

**Autor:** Jan Svojanovský

**Předmět:** NPRG035 a NPRG038

**Téma**: Lite verze ReCodexu

---

## Popis programu

Cílem mého zápočtového programu je vytvořit dva programy - aplikace na straně serveru a klientská aplikace, které umožní vyhodnocení a odevzdání programů napsaných v jazyce Python.

### Server aplikace

Umožňovala by připojení více klientů (studentů) k serveru a paralelně spouštět (pravděpodobně ve virtuálním prostředí) a vyhodnocovat jejich úlohy. Zároveň by tedy musela držet základní údaje o uživatelích (především jejich údaje a vyhodnocení testů). Úlohy studentům by přiděloval speciální klient  ` admin`. Při vytváření úlohy `admin` vytvoří testy. Každý test se skládá ze tří souborů: vstupní data, argumenty z příkazové řádky a očekávaný výstup. Server vytvoří nové vlákno pro každé odevzdání úlohy a tedy vyhodnocování testů bude probíhat paralelně. Dalšími případnými rozšířeními by potom mohly být speciální funkce pro uživatele `admin` jako například manuální oprava úloh nebo vytváření skupin pro lepší manipulaci při přidělování úloh studentům. 

### Klientská aplikace

Měli bychom dva druhy klientů: `student` a `admin`.  Po zadání IP adresy a autorizaci se klient připojí k serveru a může vykonávat operace dle druhu klienta. Student se může zaregistrovat, úlohy mu musí ale přidělit `admin`. Vytvořit nový účet `admin` může pouze libovolný ze stávajících `adminů`. Student může odevzdávat řešení na přidělené úlohy a sledovat stav svých úloh a jejich výsledky (log testů). Admin může vytvářet nové úlohy a přidělovat je studentům. Každý student uvidí všechny své odevzdané úlohy a celkový počet bodů.

---

### Použité technologie z hlediska splnění NPRG038

Nejzásadněji by bylo využito síťování, přičemž obě aplikace alespoň nějak zásadně budou využívat více vláken.

---

### Možná rozšíření

Server aplikaci by šlo rozšířit o podporu různých jazyků. Dalším možným rozšířením by bylo vytvoření GUI pro klientskou aplikaci. Tato dvě rozšíření by ale dle mého názoru klidně i zdvojnásobila rozsah zápočtového programu.
