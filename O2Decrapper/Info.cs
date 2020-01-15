using System;

namespace O2Decrapper
{
    public struct Info
    
    {
        public Info(DateTime epoch, DateTime lastDelete, int counter)
        {
            Epoch = epoch;
            LastDelete = lastDelete;
            Counter = counter;
        }

        public DateTime Epoch;
        public DateTime LastDelete;
        public int Counter;
    }
}