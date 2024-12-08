namespace List
{
    public class LinkedList
    {
        public class ListHead
        {
            public ListHead? next;
            public ListHead? prev;
            public ListHead()
            {
                next = this;
                prev = this;
            }
        }

        public static void INIT_LIST_HEAD(ref ListHead list)
        {
            list.next = list;
            list.prev = list;
        }

        private static void _list_add(ref ListHead _new, ref ListHead prev, ref ListHead next)
        {
            next.prev = _new;
            _new.next = next;
            _new.prev = prev;
            prev.next = _new;
        }

        public static void list_add_tail(ref ListHead _new, ref ListHead head)
        {
            _list_add(ref _new, ref head.prev, ref head);
        }
    }
}
