using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfBrowserApplication2
{
    //UI datagrid 每一行的集合
    public class items : ObservableCollection<item> { }


    //UI datagrid 每一个cell  对应类
    public class item
    {
        public int Value { get; set; }
        public string Back { get; set; }
        public item()
        {
            Value = 0;
            Back = "";
        }
    }


    /// <summary>
    /// 每个步骤的详细状态
    /// 当前值 current_maxsum_value
    /// 最大值 current_select_value
    /// DataGrid 的数据源 CollectionSource
    /// </summary>
    public class step_detail
    {
        public ObservableCollection<items> CollectionSource { get; set; }
        public int current_maxsum_value { get; set; }
        public int current_select_value { get; set; }
    }
}
