namespace O2Decrapper
{
    public struct Info
    {
        public Info(string epoch, string lastDelete, int counter)
        {
            Epoch = epoch;
            LastDelete = lastDelete;
            Counter = counter;
        }

        public string Epoch;
        public string LastDelete;
        public int Counter;
    }
}