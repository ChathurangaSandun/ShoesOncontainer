using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProductCatelogApi.ViewModels
{
    public class PagenatedItemsViewModel<TEntitiy> where TEntitiy : class 
    {
        public int PageSize { get; private set; }

        public int PageIndex { get; private set; }

        public long Count { get; private set; }

        public IEnumerable<TEntitiy> Data { get; private set; }

        public PagenatedItemsViewModel(int pageSize, int pageIndex, long count, IEnumerable<TEntitiy> data)
        {
            this.PageSize = pageSize;
            this.PageIndex = pageIndex;
            this.Count = count;
            this.Data = data;
        }
        
    }
}
