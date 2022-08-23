using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Configuration;

namespace ConsoleATM
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("config.json", optional: false);

            IConfiguration config = builder.Build();

            using var billsStore = new BillStore { Bills = config.GetSection("Bills").Get<List<Bill>>() };
            Console.WriteLine($"ATM has: {billsStore.Bills.Where(x => x.Count > 0).Sum(x => x.Denomination * x.Count)}\n");

            // Console.WriteLine($"The answer is always {bills.Count}");
            while (true)
            {
                if (long.TryParse(Console.ReadLine(), out var moneyToWithdraw))
                {
                    Console.WriteLine($"Need to withdraw {moneyToWithdraw}");

                    var availableToWithdraw = billsStore.Bills.Any(x => (moneyToWithdraw % x.Denomination) == 0);

                    if (!availableToWithdraw)
                    {
                        Console.WriteLine(
                            $"ATM can't withdraw {moneyToWithdraw}, please enter amount in multiples of any ({billsStore.Bills.StringifyDenomination()})");
                        continue;
                    }

                    var withdrawBills = new List<Bill>();

                    var oneBill = billsStore.Bills.FirstOrDefault(x => x.Denomination == moneyToWithdraw && x.Count > 0);

                    if (oneBill != null)
                    {
                        withdrawBills.Add(new Bill { Denomination = oneBill.Denomination, Count = 1 });
                        oneBill.Count--;
                    }
                    else
                    {
                        var balanceToBeIssued = moneyToWithdraw;
                        foreach (var bill in billsStore.Bills.OrderByDescending(x => x.Denomination))
                        {
                            if (bill.Denomination > balanceToBeIssued)
                            {
                                continue;
                            }

                            var neededCountOfThisDenomination = balanceToBeIssued / bill.Denomination;
                            var possibleCountDenomination = neededCountOfThisDenomination < bill.Count
                                ? neededCountOfThisDenomination
                                : bill.Count;

                            if (possibleCountDenomination == 0)
                            {
                                continue;
                            }

                            balanceToBeIssued -= bill.Denomination * possibleCountDenomination;
                            withdrawBills.Add(new Bill { Denomination = bill.Denomination, Count = possibleCountDenomination });
                            bill.Count -= possibleCountDenomination;
                        }
                    }

                    if (withdrawBills.Sum(x => x.Denomination * x.Count) == moneyToWithdraw)
                    {
                        Console.WriteLine($"ATM Withdraw\n{withdrawBills.Stringify()}");
                    }
                    else
                    {
                        // Back money to store
                        foreach (var bill in withdrawBills)
                        {
                            foreach (var billsInStore in billsStore.Bills.Where(billsInStore => billsInStore.Denomination == bill.Denomination))
                            {
                                billsInStore.Count += bill.Count;
                            }
                        }
                        Console.WriteLine($"ATM can't withdraw {moneyToWithdraw}, please enter amount in multiples of any ({billsStore.Bills.StringifyDenomination()}\n)");
                    }

                    Console.WriteLine($"ATM has: {billsStore.Bills.Where(x => x.Count > 0).Sum(x => x.Denomination * x.Count)}\n");
                }
                else
                {
                    Console.WriteLine(
                        $"ATM can't withdraw {moneyToWithdraw}, please enter amount in multiples of any ({billsStore.Bills.StringifyDenomination()}\n)");
                }
            }
        }
    }
}