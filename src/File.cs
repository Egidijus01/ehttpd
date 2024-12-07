using List;
using MainStructures;

namespace FileHandling
{
	public class FileHandler
	{
		private LinkedList.ListHead indexFiles;
		private LinkedList.ListHead dispatchHandlers;
		private LinkedList.ListHead pendingRequests;

		public FileHandler() {
			indexFiles = new LinkedList.ListHead();
			LinkedList.INIT_LIST_HEAD(ref indexFiles);

			dispatchHandlers = new LinkedList.ListHead();
			LinkedList.INIT_LIST_HEAD(ref dispatchHandlers);

			pendingRequests = new LinkedList.ListHead();
			LinkedList.INIT_LIST_HEAD(ref pendingRequests);
		}
		public struct deferredRequest
		{
			public LinkedList.ListHead list;
			public MainStructure.dispatchHandler d;
			public MainStructure.Client cl;
			public MainStructure.PathInfo pi;
			public string url;
			public bool called, path;
		}

		public struct indexFile
		{
			public LinkedList.ListHead list;
			public string name;
		}

		private enum filHdr
		{
			HDR_AUTHORIZATION,
			HDR_IF_MODIFIED_SINCE,
			HDR_IF_UNMODIFIED_SINCE,
			HDR_IF_MATCH,
			HDR_IF_NONE_MATCH,
			HDR_IF_RANGE,
			__HDR_MAX
		}

		public void dispatchAdd(ref MainStructure.dispatchHandler d)
		{
			LinkedList.list_add_tail(ref d.list, ref dispatchHandlers);
		}

		public void indexAdd(string filename) {
            indexFile idx = new indexFile
            {
                name = filename,
                list = new LinkedList.ListHead()
            };			
			LinkedList.INIT_LIST_HEAD(ref idx.list);
			LinkedList.list_add_tail(ref idx.list, ref indexFiles);
		}
	}
}
