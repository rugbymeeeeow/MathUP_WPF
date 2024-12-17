using System.Linq;
using System;

public static class InputValidation
{
    // Метод для парсинга ввода поставщиков и потребителей
    public static int[] ParseInput(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            throw new ArgumentException("Ввод не может быть пустым.");
        }

        // Разделяем строку по пробелам и пытаемся преобразовать каждый элемент в число
        var values = input.Split(' ')
                          .Select(value =>
                          {
                              if (int.TryParse(value, out int result))
                                  return result;
                              else
                                  throw new ArgumentException($"Неверное значение: {value}");
                          })
                          .ToArray();

        return values;
    }

    // Метод для парсинга матрицы затрат
    public static int[,] ParseCostMatrix(string input, int numSuppliers, int numConsumers)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            throw new ArgumentException("Матрица затрат не может быть пустой.");
        }

        var rows = input.Split(';');
        if (rows.Length != numSuppliers)
        {
            throw new ArgumentException("Число строк матрицы не соответствует числу поставщиков.");
        }

        int[,] matrix = new int[numSuppliers, numConsumers];
        for (int i = 0; i < numSuppliers; i++)
        {
            var values = rows[i].Split(' ');
            if (values.Length != numConsumers)
            {
                throw new ArgumentException($"Число столбцов в строке {i + 1} не соответствует числу потребителей.");
            }

            for (int j = 0; j < numConsumers; j++)
            {
                if (!int.TryParse(values[j], out int cost))
                {
                    throw new ArgumentException($"Неверное значение в матрице: {values[j]}.");
                }
                matrix[i, j] = cost;
            }
        }

        return matrix;
    }
}
