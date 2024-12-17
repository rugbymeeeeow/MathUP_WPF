using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Collections.Generic;

namespace WpfApp
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        // Ввод возможностей поставщиков с проверкой на корректность
        private int[] SupplierEnter(string input)
        {
            string[] values = input.Split(' ');
            if (values.All(value => int.TryParse(value, out _)))
            {
                return Array.ConvertAll(values, int.Parse);
            }
            else
            {
                MessageBox.Show("Ошибка: Введите корректные числа для поставщиков.");
                return null;
            }
        }

        // Ввод возможностей потребителей с проверкой на корректность
        private int[] ConsumerEnter(string input)
        {
            string[] values = input.Split(' ');
            if (values.All(value => int.TryParse(value, out _)))
            {
                return Array.ConvertAll(values, int.Parse);
            }
            else
            {
                MessageBox.Show("Ошибка: Введите корректные числа для потребителей.");
                return null;
            }
        }

        // Ввод матрицы затрат
        private int[,] GetTogether(int supplierCount, int consumerCount, string input)
        {
            var lines = input.Split(';');
            int[,] matrix = new int[supplierCount, consumerCount];

            for (int i = 0; i < supplierCount; i++)
            {
                string[] values = lines[i].Split(' ');
                if (values.Length != consumerCount || values.Any(v => !int.TryParse(v, out _)))
                {
                    MessageBox.Show($"Ошибка: строка {i + 1} матрицы введена некорректно.");
                    return null;
                }
                for (int j = 0; j < consumerCount; j++)
                {
                    matrix[i, j] = int.Parse(values[j]);
                }
            }
            return matrix;
        }

        // Метод Северо-западного угла
        private void NorthWestCorner(int[,] matrix, int[] suppliers, int[] consumers)
        {
            int numSuppliers = matrix.GetLength(0);
            int numConsumers = matrix.GetLength(1);
            int[,] result = new int[numSuppliers, numConsumers];
            int score = 0;
            int i = 0, j = 0;

            while (i < numSuppliers && j < numConsumers)
            {
                int min = Math.Min(suppliers[i], consumers[j]);
                score += min * matrix[i, j];
                result[i, j] = min;
                suppliers[i] -= min;
                consumers[j] -= min;

                if (suppliers[i] == 0) i++;
                if (consumers[j] == 0) j++;
            }

            DisplayResult(result, score);
            DisplayMatrix(result);
        }

        // Метод минимальных элементов
        private void Minimum(int[,] matrix, int[] suppliers, int[] consumers)
        {
            int numSuppliers = matrix.GetLength(0);
            int numConsumers = matrix.GetLength(1);
            int[,] result = new int[numSuppliers, numConsumers];
            int score = 0;

            while (true)
            {
                int minValue = int.MaxValue;
                int minI = -1, minJ = -1;

                for (int i = 0; i < numSuppliers; i++)
                {
                    for (int j = 0; j < numConsumers; j++)
                    {
                        if (matrix[i, j] < minValue && suppliers[i] > 0 && consumers[j] > 0)
                        {
                            minValue = matrix[i, j];
                            minI = i;
                            minJ = j;
                        }
                    }
                }

                if (minI == -1 || minJ == -1) break;

                int min = Math.Min(suppliers[minI], consumers[minJ]);
                score += min * minValue;
                result[minI, minJ] = min;
                suppliers[minI] -= min;
                consumers[minJ] -= min;
            }

            DisplayResult(result, score);
            DisplayMatrix(result);
        }

        // Отображение результатов в DataGrid
        private void DisplayResult(int[,] result, int score)
        {
            var resultList = new List<List<int>>();
            int rows = result.GetLength(0);
            int cols = result.GetLength(1);

            for (int i = 0; i < rows; i++)
            {
                var row = new List<int>();
                for (int j = 0; j < cols; j++)
                {
                    row.Add(result[i, j]);
                }
                resultList.Add(row);
            }

            MessageBox.Show($"Общий счет: {score}");
        }

        // Отображение переработанной матрицы в TextBlock
        private void DisplayMatrix(int[,] result)
        {
            int rows = result.GetLength(0);
            int cols = result.GetLength(1);
            string matrixStr = "Переработанная матрица:\n";

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    matrixStr += result[i, j] + "\t";
                }
                matrixStr += "\n";
            }

            TextBlockMatrix.Text = matrixStr;
        }

        // Обработчик кнопки "Вычислить"
        private void BtnCalculate_Click(object sender, RoutedEventArgs e)
        {
            // Ввод данных
            var suppliers = SupplierEnter(TextBoxSuppliers.Text);
            var consumers = ConsumerEnter(TextBoxConsumers.Text);
            var matrix = GetTogether(suppliers.Length, consumers.Length, TextBoxMatrix.Text);

            if (suppliers == null || consumers == null || matrix == null)
            {
                return;
            }

            // Проверка на сбалансированность
            int totalSuppliers = suppliers.Sum();
            int totalConsumers = consumers.Sum();

            if (totalSuppliers != totalConsumers)
            {
                MessageBox.Show("Ошибка: Сумма возможностей поставщиков не равна сумме возможностей потребителей.");
                if (totalSuppliers > totalConsumers)
                {
                    int[] newConsumers = new int[consumers.Length + 1];
                    Array.Copy(consumers, newConsumers, consumers.Length);
                    newConsumers[consumers.Length] = totalSuppliers - totalConsumers;
                    consumers = newConsumers;
                    MessageBox.Show($"Добавлен фиктивный потребитель с возможностью {totalSuppliers - totalConsumers}");
                }
                else
                {
                    int[] newSuppliers = new int[suppliers.Length + 1];
                    Array.Copy(suppliers, newSuppliers, suppliers.Length);
                    newSuppliers[suppliers.Length] = totalConsumers - totalSuppliers;
                    suppliers = newSuppliers;
                    MessageBox.Show($"Добавлен фиктивный поставщик с возможностью {totalConsumers - totalSuppliers}");
                }
            }

            // Выбор метода
            if (ComboBoxMethod.SelectedIndex == 0)
            {
                NorthWestCorner(matrix, suppliers, consumers);
            }
            else if (ComboBoxMethod.SelectedIndex == 1)
            {
                Minimum(matrix, suppliers, consumers);
            }
            else
            {
                MessageBox.Show("Ошибка: Выберите метод из списка.");
            }
        }
    }
}
