using System;
using System.Collections.Generic;
using System.Linq;

// Класс продукта
public class Product
{
    public string Name { get; set; }
    public decimal Price { get; set; }
    public int Quantity { get; set; }

    // Контруктор класса для создания объектов
    public Product(string name, decimal price, int quantity)
    {
        Name = name;
        Price = price;
        Quantity = quantity;
    }
}

// Класс для вендингового автомата
public class Machine
{
    private List<Product> products = new List<Product>();
    private decimal balance = 0;
    private decimal collectedMoney = 0;
    private bool isAdminMode = false;
    private const string AdminPassword = "qwerty123";

    private List<decimal> insertedCoins = new List<decimal>();

    public Machine()
    {
        products.Add(new Product("Haribo мармелад", 50, 10));
        products.Add(new Product("Мороженое Bahroma", 30, 5));
        products.Add(new Product("Вода Tassay", 40, 8));
    }

    // Доступ админа
    public bool EnterAdminMode(string password)
    {
        if (password == AdminPassword)
        {
            isAdminMode = true;
            return true;
        }
        return false;
    }

    public void ExitAdminMode()
    {
        isAdminMode = false;
    }

    // Добавление товара
    public bool AddProduct(string name, decimal price, int quantity)
    {
        if (!isAdminMode)
        {
            Console.WriteLine("Требуется доступ админа");
            return false;
        }

        products.Add(new Product(name, price, quantity));
        Console.WriteLine($"Товар '{name}' добавлен");
        return true;
    }

    // Удаление товара
    public bool RemoveProduct(string name)
    {
        if (!isAdminMode)
        {
            Console.WriteLine("Требуется доступ админа");
            return false;
        }

        var product = products.FirstOrDefault(p => p.Name == name);
        if (product != null)
        {
            products.Remove(product);
            Console.WriteLine($"Товар '{name}' удален");
            return true;
        }

        Console.WriteLine($"Товар '{name}' не найден");
        return false;
    }

    public decimal CollectMoney()
    {
        if (!isAdminMode)
        {
            Console.WriteLine("Требуется доступ админа");
            return 0;
        }

        decimal moneyToCollect = collectedMoney;
        collectedMoney = 0;
        Console.WriteLine($"Собрано {moneyToCollect} руб.");
        return moneyToCollect;
    }

    // Вывод о товарах для пользователя
    public void DisplayProducts()
    {
        Console.WriteLine("Доступные товары:");
        for (int i = 0; i < products.Count; i++)
        {
            Console.WriteLine($"{i + 1}. {products[i].Name} - {products[i].Price} руб. Остаток: {products[i].Quantity}");
        }
    }

    // Логика выдачи товара 
    public void InsertMoney(decimal money)
    {
        if (money > 0)
        {
            balance += money;
            insertedCoins.Add(money);
            Console.WriteLine($"Внесено: {money} руб. Текущий баланс: {balance} руб");
        }
        else
        {
            Console.WriteLine("Внесите сумму больше 0");
        }
    }

    public void SelectProduct(int index)
    {
        if (index < 1 || index > products.Count)
        {
            Console.WriteLine("Неверный номер товара");
            return;
        }

        Product selected = products[index - 1];
        if (selected.Quantity <= 0)
        {
            Console.WriteLine("Товар закончился");
            return;
        }

        if (balance < selected.Price)
        {
            Console.WriteLine("Недостаточно средств");
            return;
        }

        balance -= selected.Price;
        collectedMoney += selected.Price;
        selected.Quantity--;
        Console.WriteLine($"Ваш товар: {selected.Name}. Ваша сдача: {balance} руб");

        ReturnChange();
    }

    public void ReturnChange()
    {
        if (balance > 0)
        {
            Console.WriteLine($"Возвращено сдачи: {balance} руб");
            balance = 0;
        }
        insertedCoins.Clear();
    }

    // если отменяется операция
    public void CancelOperation()
    {
        if (insertedCoins.Count > 0)
        {
            Console.WriteLine("Отмена операции. Возврат монет:");
            foreach (var coin in insertedCoins)
            {
                Console.WriteLine($"- {coin} руб.");
            }
            Console.WriteLine($"Возвращено: {insertedCoins.Sum()} руб");
        }
        else
        {
            Console.WriteLine("Нет средств для возврата");
        }

        balance = 0;
        insertedCoins.Clear();
    }
    public decimal GetBalance() => balance;
}

class Program
{
    static void Main()
    {
        Machine machine = new Machine();
        bool isRunning = true;

        Console.WriteLine("Вендинговый автомат");
        
        while (isRunning)
        {
            Console.WriteLine("\nМеню");
            Console.WriteLine("1. Просмотреть товары");
            Console.WriteLine("2. Купить товар");
            Console.WriteLine("3. Вернуть сдачу");
            Console.WriteLine("4. Отменить операцию");
            Console.WriteLine("5. Режим администратора");
            Console.WriteLine("0. Выйти");
            Console.Write("Выберите действие: ");

            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    machine.DisplayProducts();
                    break;

                case "2":
                    Console.Write("Сумма внесения: ");
                    decimal amount = decimal.Parse(Console.ReadLine());
                    machine.InsertMoney(amount);
                    break;

                    Console.Write("Введите номер товара: ");
                    if (int.TryParse(Console.ReadLine(), out int productNumber))
                    {
                        machine.SelectProduct(productNumber);
                    }
                    else
                    {
                        Console.WriteLine("Ошибка: введите корректный номер");
                    }
                    break;

                case "4":
                    machine.ReturnChange();
                    break;

                case "5":
                    machine.CancelOperation();
                    break;

                case "6":
                    Console.Write("Введите пароль администратора: ");
                    string password = Console.ReadLine();
                    if (machine.EnterAdminMode(password))
                    {
                        AdminMenu(machine);
                    }
                    else
                    {
                        Console.WriteLine("Неверный пароль");
                    }
                    break;

                case "0":
                    isRunning = false;
                    Console.WriteLine("Завершение работы автомата");
                    break;
            }
        }
    }

    static void AdminMenu(Machine machine)
    {
        bool inAdminMode = true;

        while (inAdminMode)
        {
            Console.WriteLine("\nРежим администратора");
            Console.WriteLine("1. Добавить товар");
            Console.WriteLine("2. Удалить товар");
            Console.WriteLine("3. Собрать деньги");
            Console.WriteLine("4. Просмотреть товары");
            Console.WriteLine("0. Выйти из режима администратора");

            Console.Write("Выберите действие: ");

            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    Console.Write("Введите название товара: ");
                    string name = Console.ReadLine();
    
                    Console.Write("Введите цену: ");
                    decimal price = decimal.Parse(Console.ReadLine());
    
                    Console.Write("Введите количество: ");
                    int quantity = int.Parse(Console.ReadLine());
    
                    machine.AddProduct(name, price, quantity);
                    break;

                case "2":
                    Console.Write("Введите название товара для удаления: ");
                    string productName = Console.ReadLine();
                    machine.RemoveProduct(productName);
                    break;

                case "3":
                    machine.CollectMoney();
                    break;

                case "4":
                    machine.DisplayProducts();
                    break;

                case "0":
                    machine.ExitAdminMode();
                    inAdminMode = false;
                    Console.WriteLine("Выход из режима администратора");
                    break;
            }
        }
    }
}