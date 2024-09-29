namespace List
{
    public class LinkedList
    {
        public class ListHead
        {
            public ListHead? next;
            public ListHead? prev;
        }

        public static void INIT_LIST_HEAD(ref ListHead list)
        {
            list.next = null;
            list.prev = null;
        }
    }
}
