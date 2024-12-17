using System;
using System.Linq;

public static class TransportProblem
{
    // Метод для решения задачи методом Северо-западного угла
    public static (int[,] distribution, int totalCost) SolveNorthWestCorner(int[,] costMatrix, int[] suppliers, int[] consumers)
    {
        int numSuppliers = suppliers.Length;
        int numConsumers = consumers.Length;
        int[,] distribution = new int[numSuppliers, numConsumers];
        int totalCost = 0;

        int i = 0, j = 0;
        while (i < numSuppliers && j < numConsumers)
        {
            int amount = Math.Min(suppliers[i], consumers[j]);
            distribution[i, j] = amount;
            totalCost += amount * costMatrix[i, j];
            suppliers[i] -= amount;
            consumers[j] -= amount;

            if (suppliers[i] == 0) i++;
            if (consumers[j] == 0) j++;
        }

        return (distribution, totalCost);
    }

    // Метод для решения задачи методом минимальных элементов
    public static (int[,] distribution, int totalCost) SolveMinElementMethod(int[,] costMatrix, int[] suppliers, int[] consumers)
    {
        int numSuppliers = suppliers.Length;
        int numConsumers = consumers.Length;
        int[,] distribution = new int[numSuppliers, numConsumers];
        int totalCost = 0;

        // Сначала проверим, сбалансирована ли задача
        int totalSuppliers = suppliers.Sum();
        int totalConsumers = consumers.Sum();

        if (totalSuppliers != totalConsumers)
        {
            // Если не сбалансировано, добавляем фиктивную переменную
            if (totalSuppliers > totalConsumers)
            {
                // Добавляем фиктивного потребителя
                numConsumers++;
                Array.Resize(ref consumers, numConsumers);
                consumers[numConsumers - 1] = totalSuppliers - totalConsumers; // фиктивный потребитель
            }
            else
            {
                // Добавляем фиктивного поставщика
                numSuppliers++;
                Array.Resize(ref suppliers, numSuppliers);
                suppliers[numSuppliers - 1] = totalConsumers - totalSuppliers; // фиктивный поставщик
            }

            // Обновляем матрицу затрат, добавляем фиктивные строки или столбцы
            int[,] newCostMatrix = new int[numSuppliers, numConsumers];
            for (int i = 0; i < numSuppliers - 1; i++)
            {
                for (int j = 0; j < numConsumers - 1; j++)
                {
                    newCostMatrix[i, j] = costMatrix[i, j];
                }
            }

            // Заполняем фиктивные строки/столбцы нулевыми затратами
            if (numSuppliers > suppliers.Length) // добавляем фиктивного поставщика
            {
                for (int j = 0; j < numConsumers; j++)
                {
                    newCostMatrix[numSuppliers - 1, j] = 0;
                }
            }
            else if (numConsumers > consumers.Length) // добавляем фиктивного потребителя
            {
                for (int i = 0; i < numSuppliers; i++)
                {
                    newCostMatrix[i, numConsumers - 1] = 0;
                }
            }

            costMatrix = newCostMatrix;
        }

        while (true)
        {
            int minCost = int.MaxValue;
            int minI = -1, minJ = -1;

            // Поиск минимального элемента
            for (int i = 0; i < numSuppliers; i++)
            {
                for (int j = 0; j < numConsumers; j++)
                {
                    if (costMatrix[i, j] < minCost && suppliers[i] > 0 && consumers[j] > 0)
                    {
                        minCost = costMatrix[i, j];
                        minI = i;
                        minJ = j;
                    }
                }
            }

            if (minI == -1 || minJ == -1)
                break; // Если не осталось подходящих позиций

            // Расчет минимального количества товара
            int amount = Math.Min(suppliers[minI], consumers[minJ]);
            distribution[minI, minJ] = amount;
            totalCost += amount * costMatrix[minI, minJ];
            suppliers[minI] -= amount;
            consumers[minJ] -= amount;
        }

        return (distribution, totalCost);
    }
}
