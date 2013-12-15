using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Data;
using System.IO;
using System.Threading;
using System.Collections.ObjectModel;
using System.Media;
using System.Xml;
using System.Windows.Markup;
using System.Windows.Threading;

namespace WpfBrowserApplication2
{
    /// <summary>
    /// Page1.xaml 的交互逻辑
    /// </summary>
    public partial class Page1 : Page
    {

        //所有步骤的引用
        private List<step_detail> AllStep, AllStepAny, AllStepMaxtrix;
        //lenArray 当前矩阵的Rank值，用于生成前台DataGrid
        //Nstep  执行到了哪一步
        private int lenArray = 0, Nstep = 0;

        private static int Nstepmax = 100000;

        //当前的所有步骤 类型 任意  和 矩阵
        private List<step_detail> CurrentAllStepAny = new List<step_detail>();
        private List<step_detail> CurrentAllStepMaxtrix = new List<step_detail>();

       
        private string CurrentFilePath=null;//当前矩阵目录，若随机生成则为null

        private int[,] CurrentMatrix;//当前矩阵


        //计时器，用于播放
        private DispatcherTimer timer = new DispatcherTimer();

        //当前播放状态 是否是自动播放
        private bool IsAutoPlay = false;

        //时间间隔
        private static int[] AutoTime = new int[] { 1, 100, 500, 1000, 2000, 3000, 5000, 10000 };
        
        //用于保存最近使用的3文件

        private List<RecentlyMatrix> recentlymatrix = new List<RecentlyMatrix>();
        
        
        
        

        /// <summary>
        /// 
        /// 最近使用文件的数据
        /// 
        /// 包含文件目录，名字，矩阵
        /// 最大矩阵的所有步骤 allstepmaxtrix
        /// 最大任意的所有步骤 allstepany
        /// </summary>
        /// 
        public class RecentlyMatrix
        {
            public string sfilename { get; set; }
            public string sfilepath { get; set; }
            public int[,] matrix { get; set; }
            public List<step_detail> allstepany { get; set; }
            public List<step_detail> allstepmaxtrix { get; set; }
        }

        public Page1()
        {
            InitializeComponent();
            _time();
        }


        //初始化计时器
        private void _time()
        {
            timer.Interval = TimeSpan.FromMilliseconds(1000);
            timer.Tick += setui;
        }


        //用于自动播放，当前步骤Nstep+1  并调用前台UI设置函数
        private void setui(object sender, EventArgs e)
        {
            if (AllStep==null||Nstep + 2 >= AllStep.Count)
            {
                timer.Stop();
                MessageBox.Show("演示完毕");
                return;
            }
            Nstep++;
            SetDataGrid();
        }


        //生成随机矩阵，并设置当前矩阵的相关数据，掉用 设置最近[使用的文件]的函数，
        //调用Getdata 得到  所有步骤

        private void RandomMatrix(int m, int n)
        {
            int i,j;
            Random x=new Random();
            int[,] a = new int[m, n];
            for (i = 0; i < m; i++)
                for (j = 0; j < n; j++)
                {
                    int t = x.Next(2);
                    a[i, j] = x.Next(101);
                    if (t == 1) a[i, j] = -a[i, j];
                }
            SetRecentlyData();

            GetData(m, n, a);

            CurrentFilePath = null;
            UICurrentData.Content = "Random" + " [" + m + '*' + n + "]";
            CurrentMatrix = new int[m, n];
            CurrentMatrix = (int[,])a.Clone();
        }


        //设置当前需要播放 的  步骤类型   矩阵  还是 任意
        private void SetAllStep()
        {
            int x = UIType.SelectedIndex;
            Nstep = 0;
            if (x == 1)
                AllStep = AllStepAny;
            else
                AllStep = AllStepMaxtrix;
            SetDataGrid();
        }


        //实体化 maxsum类  获取所有步骤
        private void GetData(int m, int n, int[,] a)
        {
            maxsum _maxsum = new maxsum(m, n, a);
            lenArray = n;

            CurrentAllStepMaxtrix.Clear();
            _maxsum.result_maxsum.ForEach(s => CurrentAllStepMaxtrix.Add(s));

            CurrentAllStepAny.Clear();
            _maxsum.result_any_maxsum.ForEach(s => CurrentAllStepAny.Add(s));

            AllStepMaxtrix = CurrentAllStepMaxtrix;
            AllStepAny = CurrentAllStepAny;

            UIMainGrid.Background = UIcolor_white.Background;
            reset();
            SetAllStep();
        }


        // 设置前台DataGrid 和 状态
        private void SetDataGrid()
        {
            if (AllStep==null)
            {
                return;
            }
            if (Nstep >= AllStep.Count)
            {
                MessageBox.Show("已经是最后一步了");
                return;
            }
            if (Nstep < 0)
            {
                MessageBox.Show("已经是第一步了");
                return;
            }
            dataGrid.ItemsSource = null;
            dataGrid.ItemsSource = AllStep[Nstep].CollectionSource;
            UImaxsum.Text = AllStep[Nstep].current_maxsum_value.ToString();
            UIsum.Text = AllStep[Nstep].current_select_value.ToString();
        }


        // 设置最近使用文件的数据
        private void SetRecentlyData()
        {
            if (CurrentFilePath != null)
            {
                RecentlyMatrix temp_maxtrix = new RecentlyMatrix
                {
                    matrix = new int[CurrentMatrix.GetLength(1), CurrentMatrix.GetLength(0)],
                    sfilename = System.IO.Path.GetFileNameWithoutExtension(CurrentFilePath),
                    sfilepath = CurrentFilePath,
                    allstepany = new List<step_detail>(),
                    allstepmaxtrix = new List<step_detail>()
                };

                temp_maxtrix.matrix = (int[,])CurrentMatrix.Clone();
                CurrentAllStepAny.ForEach(x => temp_maxtrix.allstepany.Add(x));
                CurrentAllStepMaxtrix.ForEach(x => temp_maxtrix.allstepmaxtrix.Add(x));

                if (recentlymatrix.Count >= 3)
                    recentlymatrix.RemoveAt(0);
                recentlymatrix.Add(temp_maxtrix);

                SetUI_RecentlyFile();
            }
        }


        // 从文件读入数据
        private void FileDataInput(string ss)
        {
            string s = "";
            int i, j;

            var file = File.OpenText(ss);

            s = file.ReadToEnd();
            s = s.Replace("\r\n", " ");

            string[] sa = s.Split(' ', ',', (char)9);

            int kk = 0, m = 0, n = 0;

            while (!int.TryParse(sa[kk], out m)) kk++;
            kk++;

            while (!int.TryParse(sa[kk], out n)) kk++;
            kk++;


            int[,] aa = new int[m, n];

            for (i = 0; i < m; i++)
                for (j = 0; j < n; j++)
                {
                    int tt = 0;
                    if (kk >= sa.Length)
                    {
                        MessageBox.Show("此文件数据格式错误或数据数量错误");
                        return;
                    }
                    while (!int.TryParse(sa[kk], out tt)) kk++;
                    kk++;
                    aa[i, j] = tt;
                }


            SetRecentlyData();
            UICurrentData.Content = System.IO.Path.GetFileNameWithoutExtension(ss);

            GetData(m, n, aa);

            CurrentMatrix = new int[m, n];
            CurrentMatrix = (int[,])aa.Clone();
            CurrentFilePath = ss;

        }

       
        //设置前台 最近使用文件相关的控件
        private void SetUI_RecentlyFile()
        {
            if (recentlymatrix.Count >= 1)
                UIRecentlyFile1.Content = recentlymatrix[recentlymatrix.Count-1].sfilename;
            if (recentlymatrix.Count >= 2)
                UIRecentlyFile2.Content = recentlymatrix[recentlymatrix.Count - 2].sfilename;
            if (recentlymatrix.Count >= 3)
                UIRecentlyFile3.Content = recentlymatrix[recentlymatrix.Count - 3].sfilename;
        }

        //开启计时器
        private void AutoPlayBeign()
        {
            timer.Start();
        }

        //重置当前播放
        private void reset()
        {
            try
            {
                Nstep = 0;
                IsAutoPlay = false;
                UIPre.IsEnabled = true;
                UINext.IsEnabled = true;
                UIBegin.Content = "自动播放";
                timer.Stop();
                SetDataGrid();
            }
            catch { }
        }

        //从  list   recentlymatrix 提取最近使用的文件
        private void GetRecentlyData(int index)
        {
            lenArray = recentlymatrix[index].matrix.GetLength(1);
            AllStepMaxtrix = recentlymatrix[index].allstepmaxtrix;
            AllStepAny = recentlymatrix[index].allstepany;
            SetAllStep();
        }


       
        
        /// <summary>
        /// UI 相关的函数
        /// </summary>

        private void UIPre_Click(object sender, RoutedEventArgs e)
        {
            Nstep--;
            SetDataGrid();
        }

        private void UINext_Click(object sender, RoutedEventArgs e)
        {
            Nstep++;
            SetDataGrid();
        }

        private void UIBegin_Click(object sender, RoutedEventArgs e)
        {

            if (IsAutoPlay)
            {
                IsAutoPlay = false;
                UIPre.IsEnabled = true;
                UINext.IsEnabled = true;
                UIBegin.Content = "自动播放";
                timer.Stop();
                return;
            }
            if (AllStep.Count > Nstepmax)
            {
                string message = "总的步骤已经超过"+Nstepmax+'['+AllStep.Count+']'+"，按最快播放速度，将要播放 ";
                message += (AllStep.Count / (50.0 * 60)).ToString("f3")+' ';
                message += "分钟，还是要继续播放吗？";
                string caption = "小提示";
                MessageBoxButton buttons = MessageBoxButton.YesNo;

                var result = MessageBox.Show(message, caption, buttons);

                if (result == MessageBoxResult.No)
                {
                    return;
                }
            }
            IsAutoPlay = true;
            UIPre.IsEnabled = false;
            UINext.IsEnabled = false;
            UIBegin.Content = "手动控制";
            timer.Start();
        }

        private void UIReset_Click(object sender, RoutedEventArgs e)
        {
            reset();
        }

        private void UITime_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int x ;
            try
            {
                x= UITime.SelectedIndex;
            }
            catch { x = 3; }

            timer.Interval = TimeSpan.FromMilliseconds(AutoTime[x]);
        }

        private void UIType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SetAllStep();
            reset();
        }

        private void UIRandom_Click(object sender, RoutedEventArgs e)
        {
            int n = UIRandomV.SelectedIndex + 1;
            int m = UIRandomH.SelectedIndex + 1;

            if (m * n > 50)
            {
                string message = "行列值过大(m*n超过50)将导致AnyMaxsum的步骤过多，而无法查看，还是要生成这样的矩阵吗？";
                string caption = "小提示";
                MessageBoxButton buttons = MessageBoxButton.YesNo;

                var result = MessageBox.Show(message, caption, buttons);

                if (result == MessageBoxResult.No)
                {
                    return;
                }
            }

            RandomMatrix(m, n);
        }

        private void UIFilePick_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.FileName = "Document"; // Default file name
            dlg.DefaultExt = ".txt"; // Default file extension
            dlg.Filter = "Text documents (.txt)|*.txt"; // Filter files by extension

            // Show open file dialog box
            Nullable<bool> result = dlg.ShowDialog();

            // Process open file dialog box results
            if (result == true)
            {
                // Open document
                string filename = dlg.FileName;
                FileDataInput(filename);
            }
        }

        private void UICurrentData_Click(object sender, RoutedEventArgs e)
        {
            lenArray = CurrentMatrix.GetLength(1);
            AllStepMaxtrix = CurrentAllStepMaxtrix;
            AllStepAny = CurrentAllStepAny;
            SetAllStep();
            UIMainGrid.Background = UIcolor_white.Background;
        }

        private void UIRecentlyFile1_Click(object sender, RoutedEventArgs e)
        {
            if (recentlymatrix.Count < 1) return;
            UIMainGrid.Background = UIcolor1.Background;
            GetRecentlyData(recentlymatrix.Count-1);
        }

        private void UIRecentlyFile2_Click(object sender, RoutedEventArgs e)
        {
            if (recentlymatrix.Count < 2) return;
            UIMainGrid.Background = UIcolor2.Background;
            GetRecentlyData(recentlymatrix.Count - 2);
        }

        private void UIRecentlyFile3_Click(object sender, RoutedEventArgs e)
        {
            if (recentlymatrix.Count < 3) return;
            UIMainGrid.Background = UIcolor3.Background;
            GetRecentlyData(recentlymatrix.Count - 3);
        }


        /// <summary>
        /// UI_DataGrid 相关的函数
        /// </summary>

        private void dataGrid_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = e.Row.GetIndex() + 1;
        }

        private void dataGrid_AutoGeneratedColumns(object sender, EventArgs e)
        {
            for (int i = 0; i < dataGrid.Columns.Count; i++)
            {
                dataGrid.Columns[i].Header = (i + 1).ToString();

            }
        }

        private void dataGrid_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            (sender as DataGrid).Columns.Clear();
            for (int columnIndex = 0; columnIndex < lenArray; columnIndex++)
            {
                DataGridTemplateColumn column = new DataGridTemplateColumn();
                XmlTextReader sr = new XmlTextReader(
                  new StringReader(
                    "<DataTemplate xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\" xmlns:x=\"http://schemas.microsoft.com/winfx/2006/xaml\">" +
                      "<TextBlock Background=\"{Binding [" + columnIndex + "].Back}\" Text=\"{Binding [" + columnIndex + "].Value}\"  TextAlignment=\"Center\"  Foreground=\"Black\"/>" +
                    "</DataTemplate>"));

                column.CellTemplate = (DataTemplate)XamlReader.Load(sr);

                (sender as DataGrid).Columns.Add(column);
            }
            e.Column = null;
        }



    }





}
