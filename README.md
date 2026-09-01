# CLOPE

Реализация алгоритма кластеризации CLOPE на C#.

## Требования

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- **Windows** или **Linux**

## Запуск

Из корня репозитория:

```
dotnet run
```

По умолчанию используется датасет грибов `DataStorage/mooh_with_ids.txt` с параметром репульсии **r = 2.6**.

## Параметры данных

Для `mooh_with_ids.txt` в `Program.cs` задано:

- разделитель: `,`
- пропускаемые столбцы: `0` (id строки), `1` (классы грибов)
- пустые значения: `?`
- нормализация: отдельный словарь на каждый столбец

## Тесты

`Tests/ClusterTests.cs` — unit тесты на MSTest, проверяют `AddTransaction`, `RemoveTransaction`, восстановление N/S/W после удаления и добавления транзакций.

## Структура

| Папка / файл | Назначение |
|---|---|
| `Core/` | `ClopeEngine` (Init, Iter), `TransactionClusterMap` |
| `Clusters/` | `Cluster`, `ClusterSet` — N, S, W, Add/Remove |
| `Transactions/` | `Transaction`, `TransactionSet` — загрузка транзакций |
| `Import/` | чтение текстового файла |
| `Helpers/` | вывод в консоль, пути к данным |
| `Tests/` | unit-тесты кластера |
| `DataStorage/` | тестовые наборы данных |

## Ссылки

- [Loginom — CLOPE](https://loginom.ru/blog/clope)
- [Algowiki — псевдокод](https://algowiki-project.org/ru/Участник:Артем_Карпухин/Алгоритм_CLOPE_кластеризации_категориальных_данных)
