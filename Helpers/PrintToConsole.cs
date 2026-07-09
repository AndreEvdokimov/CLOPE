using CLOPE.Clusters;
using CLOPE.Transactions;

namespace CLOPE.Helpers
{
    internal static class PrintToConsole
    {
        /// <summary>
        /// Выводит в консоль транзакции. По умолчанию выводит 50 транзакций
        /// </summary>
        /// <param name="transactions">Транзакции</param>
        internal static void Transactions(TransactionSet transactions)
        {
            if (transactions.Count == 0)
            {
                Console.WriteLine("Набор транзакций пуст");
                return;
            }

            Console.WriteLine(String.Format("|{0,15}|{1,15}", "ID Транзакции", "ID кластера"));

            foreach (var transaction in transactions)
            {
                Console.WriteLine(String.Format("|{0,15}|{1,15}", transaction.Id, transaction));
            }

            Console.WriteLine(String.Format("|{0,15}|{1,15}", "Кол-во транзакций", transactions.Count));
        }

        /// <summary>
        /// Выводит в консоль характеристики кластеров
        /// </summary>
        internal static void ClusterCharacteristics(ClusterSet clusters)
        {
            if (clusters.Count == 0)
            {
                Console.WriteLine("Набор кластеров пуст");
                return;
            }

            Console.WriteLine();

            foreach (var cluster in clusters)
            {
                Console.WriteLine(cluster.ToString());
            }

            Console.WriteLine();
        }

        /// <summary>
        /// Выводит в консоль таблицу [Кластер : Количество транзакций]
        /// </summary>
        internal static void ClopeResultTable(Core.TransactionClusterMap table)
        {
            if (table.RowsCount() == 0)
            {
                Console.WriteLine("Таблица результатов работы алгоритма пуста");
                return;
            }

            var res = new Dictionary<int, int>();

            foreach (var row in table)
            {
                var transId = row.Key;
                var clusterId = row.Value;

                if (res.ContainsKey(clusterId))
                {
                    res[clusterId]++;
                }
                else 
                {
                    res.Add(clusterId, 1);
                }
            }

            Console.WriteLine(String.Format($"|Всего кластеров|{res.Count,10}|"));

            Console.WriteLine();

            Console.WriteLine(String.Format("|{0,10}|{1,10}|", "№ Кластера", "Количество транзакций"));

            foreach (var row in res.OrderBy(p => p.Key))
            {
                Console.WriteLine(String.Format($"|{row.Key,10}|{row.Value,10}|"));
            }

            Console.WriteLine();
        }

        /// <summary>
        /// Выводит кластеры, у которых N изменился после Iter
        /// </summary>
        internal static void IterChanges(Dictionary<int, int> before, ClusterSet after)
        {
            var afterN = new Dictionary<int, int>();

            foreach (var cluster in after)
            {
                afterN[cluster.Id] = cluster.N;
            }

            var changedIds = before.Keys
                .Union(afterN.Keys)
                .Where(id =>
                {
                    before.TryGetValue(id, out int nBefore);
                    afterN.TryGetValue(id, out int nAfter);
                    return nBefore != nAfter;
                })
                .OrderBy(id => id)
                .ToList();

            Console.WriteLine("=== Изменения после Iter (только где N изменился) ===");
            Console.WriteLine();

            if (changedIds.Count == 0)
            {
                Console.WriteLine("Изменений нет.");
                Console.WriteLine();
                return;
            }

            Console.WriteLine(String.Format("|{0,10}|{1,10}|{2,10}|", "Кластер", "N до", "N после"));

            foreach (var id in changedIds)
            {
                before.TryGetValue(id, out int nBefore);
                bool existsAfter = afterN.TryGetValue(id, out int nAfter);
                string nAfterStr = existsAfter ? nAfter.ToString() : "—";

                Console.WriteLine(String.Format("|{0,10}|{1,10}|{2,10}|", id, nBefore, nAfterStr));
            }

            int nonEmptyBefore = before.Count(p => p.Value > 0);
            Console.WriteLine();
            Console.WriteLine($"Изменено: {changedIds.Count} кластер(ов); непустых до Iter: {nonEmptyBefore}, после: {afterN.Count}");
            Console.WriteLine();
        }
    }
}
