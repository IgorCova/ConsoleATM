using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConsoleATM
{
    public static class BillExtensions
    {
        public static string Stringify(this List<Bill> bills)
        {
            var result = new StringBuilder();
            foreach (var bill in bills)
            {
                result.Append(bill);
                result.Append(Environment.NewLine);
            }

            return result.ToString();
        }
        
        public static string StringifyDenomination(this IEnumerable<Bill> bills)
        {
            var result = new StringBuilder();
            foreach (var bill in bills.Where(x => x.Count > 0))
            {
                if (result.Length > 0)
                {
                    result.Append(", ");
                }
                result.Append(bill.Denomination);
                
            }

            return result.ToString();
        }
    }
}