using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Newtonsoft.Json;
public class Passport //Класс для данных паспорта
{
    public string series { get; set; }
    public string number { get; set; }
    public DateTime issuedAt { get; set; }
    public string issuer { get; set; }
    public string issuerСode { get; set; }
}
public class Credit /*Класс для данных о кредитах*/
{
    public string type { get; set; }
    public string currency { get; set; }
    public DateTime issuedAt { get; set; }
    public double rate { get; set; }
    public int loanSum { get; set; }
    public int term { get; set; }
    public DateTime? repaidAt { get; set; }
    public int currentOverdueDebt { get; set; }
    public int numberOfDaysOnOverdue { get; set; }
    public int remainingDebt { get; set; }
    public string creditId { get; set; }
}

public class Client /*Класс для данных о клиенте*/
{
    public string firstName { get; set; }
    public string middleName { get; set; }
    public string lastName { get; set; }
    public DateTime birthDate { get; set; }
    public string citizenship { get; set; }
    public Passport passport { get; set; }
    public List<Credit> creditHistory { get; set; }
}

public class Program
{
    public static bool IsCheckIsPassed(string json)
    {
        Client client = JsonConvert.DeserializeObject<Client>(json);
        DateTime now = DateTime.Now;
        int age = now.Year - client.birthDate.Year;
        if (now.DayOfYear < client.birthDate.DayOfYear) age--;

        if (age < 20)//Проверка 1
        {
            return false;
        }

        if ((age > 20 && client.passport.issuedAt < client.birthDate.AddYears(20)) || (age > 45 && client.passport.issuedAt < client.birthDate.AddYears(45)))//Проверка 2
        {
            return false;
        }

        foreach (var credit in client.creditHistory) /*Проверка 3*/
        {
            if (credit.currentOverdueDebt > 0)//Проверка на наличие непогашенной просроченной задолженсти
            {
                return false;
            }

            if (credit.type != "Кредитная карта")//Проверка типа кредита
            {
                if (credit.numberOfDaysOnOverdue > 60) /*Проверка наличие задолжности сроком более 60 дней*/
                { return false; }
                if (client.creditHistory.Count(c => c.type != "Кредитная карта" && c.numberOfDaysOnOverdue > 15) > 2) /*Проверка на наличие двух и более кредитов с непогашенной просроченной задолженсти сроком более 15 дней*/
                { return false; }
            }
            else
            {
                if (credit.numberOfDaysOnOverdue > 30)/*Проверка наличие задолжности сроком более 30 дней*/
                { return false; }
            }
        }
        return true;
    }


    public static void Main()
    {
        string json = @"
        {
            ""firstName"": ""Иван"",
            ""middleName"": ""Иванович"",
            ""lastName"": ""Иванов"",
            ""birthDate"": ""1969-12-31T21:00:00.000Z"",
            ""citizenship"": ""РФ"",
            ""passport"": {
                ""series"": ""12 34"",
                ""number"": ""123456"",
                ""issuedAt"": ""2023-03-11T21:00:00.000Z"",
                ""issuer"": ""УФМС"",
                ""issuerСode"": ""123-456""
            },
            ""creditHistory"": [
                {
                    ""type"": ""Кредит наличными"",
                    ""currency"": ""RUB"",
                    ""issuedAt"": ""2003-02-27T21:00:00.000Z"",
                    ""rate"": 0.13,
                    ""loanSum"": 100000,
                    ""term"": 12,
                    ""repaidAt"": ""2004-02-27T21:00:00.000Z"",
                    ""currentOverdueDebt"": 0,
                    ""numberOfDaysOnOverdue"": 0,
                    ""remainingDebt"": 0,
                    ""creditId"": ""25e8a350-fbbc-11ee-a951-0242ac120002""
                },
                {
                    ""type"": ""Кредитная карта"",
                    ""currency"": ""RUB"",
                    ""issuedAt"": ""2009-03-27T21:00:00.000Z"",
                    ""rate"": 0.24,
                    ""loanSum"": 30000,
                    ""term"": 3,
                    ""repaidAt"": ""2009-06-29T20:00:00.000Z"",
                    ""currentOverdueDebt"": 0,
                    ""numberOfDaysOnOverdue"": 2,
                    ""remainingDebt"": 0,
                    ""creditId"": ""81fb1ff6-fbbc-11ee-a951-0242ac120002""
                },
                {
                    ""type"": ""Кредит наличными"",
                    ""currency"": ""RUB"",
                    ""issuedAt"": ""2009-02-27T21:00:00.000Z"",
                    ""rate"": 0.09,
                    ""loanSum"": 200000,
                    ""term"": 24,
                    ""repaidAt"": ""2011-03-02T21:00:00.000Z"",
                    ""currentOverdueDebt"": 0,
                    ""numberOfDaysOnOverdue"": 14,
                    ""remainingDebt"": 0,
                    ""creditId"": ""c384eea2-fbbc-11ee-a951-0242ac120002""
                },
                {
                    ""type"": ""Кредит наличными"",
                    ""currency"": ""RUB"",
                    ""issuedAt"": ""2024-05-15T21:00:00.000Z"",
                    ""rate"": 0.13,
                    ""loanSum"": 200000,
                    ""term"": 36,
                    ""repaidAt"": null,
                    ""currentOverdueDebt"": 10379,
                    ""numberOfDaysOnOverdue"": 15,
                    ""remainingDebt"": 110000,
                    ""creditId"": ""ebeddfde-fbbc-11ee-a951-0242ac120002""
                }
            ]
        }";
        bool result = IsCheckIsPassed(json);
        Console.WriteLine(result);
    }
}