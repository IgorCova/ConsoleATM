using System;
using System.Collections.Generic;

namespace ConsoleATM
{
    public class Bill
    {
        public int Denomination { get; set; }
        public long Count { get; set; }

        public override string ToString()
        {
            return $"{Count} x Denomination: {Denomination}";
        }
    }

    public class BillStore : IDisposable
    {
        public List<Bill> Bills { get; set; }

        private bool _disposed = false;

        ~BillStore()
        {
            Dispose(disposing: false);
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                Bills = null;
            }

            _disposed = true;
        }
    }
}