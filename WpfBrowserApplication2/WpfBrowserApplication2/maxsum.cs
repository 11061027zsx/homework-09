using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfBrowserApplication2
{
    public class maxsum
    {
        //使用的颜色定义
        private static string scolor_none = "White",
            scolor_selectd = "LightSkyBlue",
            scolor_extend = "LightGreen",
            scolor_ban = "Salmon",
            scolor_result = "Gold",
            scolor_noway = "Gray",
            scolor_way = "LightGreen";

        /// <summary>
        /// 结果存储于下面两个list
        /// result_maxsum 矩阵
        /// result_any_maxsum 任意
        /// </summary>
        public List<step_detail> result_maxsum { get; set; }
        public List<step_detail> result_any_maxsum { get; set; }

        /// <summary>
        /// 矩阵的相关数据
        /// m行值
        /// n列值
        /// g 图形
        /// </summary>
        /// 
        private int m;
        private int n;
        private int[,] g;

        // 当前最大的矩阵
        current_maxsum ans = new current_maxsum { v_end = 0, v_start = 0, h_end = 0, h_start = 0 };

        // 当前最大的任意
        current_any_maxsum ans_any = new current_any_maxsum { };

        //每个步骤的  矩阵 的详细

        item[,] item_temp;

        /// <summary>
        /// 当前最大的矩阵
        /// 包含 值和 范围 
        /// </summary>
        class current_maxsum
        {
            public int value { get; set; }
            public int v_start { get; set; }
            public int v_end { get; set; }
            public int h_start { get; set; }
            public int h_end { get; set; }
        }

        /// <summary>
        /// 当前最大的任意
        /// 包含值 和 矩阵 状态
        /// </summary>
        class current_any_maxsum
        {
            public int value { get; set; }
            public bool[,] graph { get; set; }
            public int ncount { get; set; }
        }

        //初始化
        public maxsum(int _m, int _n, int[,] _g)
        {
            int i, j;
            m = _m;
            n = _n;
            g = new int[m, n];
            item_temp = new item[m, n];
            for (i = 0; i < m; i++)
                for (j = 0; j < n; j++) item_temp[i, j] = new item();


            result_maxsum = new List<step_detail>();
            result_maxsum.Clear();
            result_any_maxsum = new List<step_detail>();
            result_any_maxsum.Clear();

            for (i = 0; i < m; i++)
                for (j = 0; j < n; j++) g[i, j] = _g[i, j];

            Set_item_temp();
            AddStep(0, 0);

            cal_maxsum();
            Set_item_temp();
            for (i = ans.h_start; i <= ans.h_end; i++)
                for (j = ans.v_start; j <= ans.v_end; j++)
                    item_temp[i, j].Back = scolor_result;

            AddStep(ans.value, ans.value);

            cal_any_maxsum();

            Set_item_temp();
            AddStep(0, 0);
            Set_item_temp();
            addstepany(0, 0);
        }


        //矩阵 添加步骤
        private void AddStep(int currentvalue, int maxsumvalue)
        {
            int i, j;
            step_detail _step = new step_detail { current_select_value = currentvalue, current_maxsum_value = maxsumvalue };

            _step.CollectionSource = new ObservableCollection<items>();
            _step.CollectionSource.Clear();

            for (i = 0; i < m; i++)
            {
                items dr = new items();
                dr.Clear();
                for (j = 0; j < n; j++)
                {
                    item tt = new item { Back = item_temp[i, j].Back, Value = item_temp[i, j].Value };
                    dr.Add(tt);
                }
                _step.CollectionSource.Add(dr);
            }
            result_maxsum.Add(_step);
        }


        //任意添加步骤
        private void addstepany(int currentvalue, int maxsumvalue)
        {
            int i, j;
            step_detail _step = new step_detail { current_select_value = currentvalue, current_maxsum_value = maxsumvalue };

            _step.CollectionSource = new ObservableCollection<items>();
            _step.CollectionSource.Clear();

            for (i = 0; i < m; i++)
            {
                items dr = new items();
                dr.Clear();
                for (j = 0; j < n; j++)
                {
                    item tt = new item { Back = item_temp[i, j].Back, Value = item_temp[i, j].Value };
                    dr.Add(tt);
                }
                _step.CollectionSource.Add(dr);
            }
            result_any_maxsum.Add(_step);
        }

        //初始化  当前步骤  矩阵 状态值
        private void Set_item_temp()
        {
            int i, j;
            for (i = 0; i < m; i++)
                for (j = 0; j < n; j++)
                {
                    item_temp[i, j].Value = g[i, j];
                    item_temp[i, j].Back = scolor_none;
                }
        }


        //计算 最大矩阵的所有步骤
        private void cal_maxsum()
        {
            int[, ,] sum_g = new int[m, n, n];
            int[, ,] f = new int[m, n, n];
            int[, ,] len = new int[m, n, n];

            int i, j, k;

            for (i = 0; i < m; i++)
                for (j = 0; j < n; j++)
                    for (k = 0; k < n; k++)
                    {
                        f[i, j, k] = 0;
                        sum_g[i, j, k] = 0;
                    }

            for (i = 0; i < m; i++)
                for (j = 0; j < n; j++)
                    for (k = j; k < n; k++)
                        try
                        {
                            sum_g[i, j, k] = sum_g[i, j, k - 1] + g[i, k];
                        }
                        catch { sum_g[i, j, k] = g[i, k]; }




            int u, v;

            for (k = 0; k < m; k++)
            {
                for (i = 0; i < n; i++)
                {
                    for (j = i; j < n; j++)
                    {
                        Set_item_temp();

                        f[k, i, j] = sum_g[k, i, j];
                        for (u = i; u <= j; u++) item_temp[k, u].Back = scolor_selectd;

                        AddStep(sum_g[k, i, j], ans.value);

                        len[k, i, j] = 1;

                        if (k > 0)
                        {
                            string scolor, scolorcom;
                            if (f[k - 1, i, j] > 0)
                            {
                                len[k, i, j] += len[k - 1, i, j];
                                f[k, i, j] += f[k - 1, i, j];
                                scolor = scolor_extend;
                                scolorcom = scolor_selectd;
                            }
                            else
                            {
                                scolorcom = scolor_none;
                                scolor = scolor_ban;
                            }

                            for (u = 0; u < len[k - 1, i, j]; u++)
                                for (v = i; v <= j; v++)
                                    item_temp[k - 1 - u, v].Back = scolor;
                            AddStep(sum_g[0, i, j], ans.value);

                            for (u = 0; u < len[k - 1, i, j]; u++)
                                for (v = i; v <= j; v++)
                                    item_temp[k - 1 - u, v].Back = scolorcom;
                            AddStep(sum_g[0, i, j], ans.value);
                        }

                        int t_ans = (ans.h_end - ans.h_start + 1) * (ans.v_end - ans.v_start + 1);
                        int t_current = len[k, i, j] * (j - i + 1);

                        if (ans.value < f[k, i, j] || ans.value == f[k, i, j] && t_ans < t_current)
                        {
                            ans.value = f[k, i, j];
                            ans.v_start = i;
                            ans.v_end = j;
                            ans.h_end = k;
                            ans.h_start = k - len[k, i, j] + 1;
                            for (u = ans.h_start; u <= ans.h_end; u++)
                                for (v = ans.v_start; v <= ans.v_end; v++) item_temp[u, v].Back = scolor_result;
                            AddStep(sum_g[0, i, j], ans.value);
                        }
                    }
                }
            }

        }



        /// <summary>
        /// 计算最大任意的所有步骤
        /// </summary>

        private bool[,] gTemp, gResult;
        private int sumall = 0, amax, nn = 0, cpuload = 0;
        private int sum = -10000, nsum = 0;

        private int[,] a, asum, gmain;
        private int[] dx = { -1, 0, 1, 0 }, dy = { 0, -1, 0, 1 };

        private bool isinvh(int x, int y)
        {
            return (x >= 0 && x < m && y >= 0 && y < n);
        }

        private void isUnicom(int x, int y)
        {
            int tx, ty, i;

            gTemp[x, y] = true;

            for (i = 0; i < 4; i++)
            {
                tx = x + dx[i];
                ty = y + dy[i];

                if (isinvh(tx, ty) && !gTemp[tx, ty] && gmain[tx, ty] < 2)
                    isUnicom(tx, ty);
            }
        }

        private void fany(int x, int y, int z)
        {
            int tx, ty, tt, i, j;
            cpuload++;
            if (cpuload > 300000) return;
            addstepany(z, sum);


            for (i = 0; i <= x; i++)
                for (j = 0; j < n; j++, nn++)
                    if (gmain[i, j] == 1)
                    {
                        tx = i; ty = j;
                        for (i = 0; i < m; i++)
                            for (j = 0; j < n; j++, nn++) gTemp[i, j] = false;
                        isUnicom(tx, ty);
                        for (i = 0; i <= x; i++)
                            for (j = 0; j < n; j++, nn++) if (!gTemp[i, j] && gmain[i, j] == 1)
                                {
                                    for (i = 0; i <= x; i++)
                                        for (j = 0; j < n; j++) if (gmain[i, j] == 1)
                                                item_temp[i, j].Back = scolor_ban;
                                    addstepany(z, sum);

                                    for (i = 0; i <= x; i++)
                                        for (j = 0; j < n; j++) if (gmain[i, j] == 1)
                                                item_temp[i, j].Back = scolor_selectd;
                                    return;
                                }
                        i = x + 1;
                        break;
                    }


            if (x == m - 1 && y == n - 1)
            {


                if (sum > z)
                {
                    for (i = 0; i < m; i++)
                        for (j = 0; j < n; j++)
                            if (gmain[i, j] == 1) item_temp[i, j].Back = scolor_extend;
                    addstepany(z, sum);

                    for (i = 0; i < m; i++)
                        for (j = 0; j < n; j++)
                            if (gmain[i, j] == 1) item_temp[i, j].Back = scolor_selectd;
                    return;
                }


                tt = 0;
                for (i = 0; i < m; i++)
                    for (j = 0; j < n; j++, nn++) if (gmain[i, j] == 1)
                            tt++;

                if (sum == z && tt > nsum)
                {
                    for (i = 0; i < m; i++)
                        for (j = 0; j < n; j++)
                            if (gmain[i, j] == 1) item_temp[i, j].Back = scolor_extend;
                    addstepany(z, sum);

                    for (i = 0; i < m; i++)
                        for (j = 0; j < n; j++)
                            if (gmain[i, j] == 1) item_temp[i, j].Back = scolor_selectd;
                    return;
                }

                for (i = 0; i < m; i++)
                    for (j = 0; j < n; j++)
                        if (gmain[i, j] == 1) item_temp[i, j].Back = scolor_extend;
                addstepany(z, sum);

                for (i = 0; i < m; i++)
                    for (j = 0; j < n; j++)
                        if (gmain[i, j] == 1) item_temp[i, j].Back = scolor_result;
                addstepany(z, sum);

                for (i = 0; i < m; i++)
                    for (j = 0; j < n; j++)
                        if (gmain[i, j] == 1) item_temp[i, j].Back = scolor_selectd;

                for (i = 0; i < m; i++)
                    for (j = 0; j < n; j++, nn++) if (gmain[i, j] == 1)
                            gResult[i, j] = true;
                        else gResult[i, j] = false;

                sum = z;
                nsum = tt;
                return;
            }

            ty = (y + 1) % n;
            tx = x + (y + 1) / n;

            tt = sumall - asum[tx, ty] + z;

            if (tt + a[tx, ty] >= sum)
            {
                gmain[tx, ty] = 1;
                item_temp[tx, ty].Back = scolor_selectd;
                fany(tx, ty, z + a[tx, ty]);
                item_temp[tx, ty].Back = scolor_none;
                gmain[tx, ty] = 0;
            }

            if (tt >= sum)
            {
                gmain[tx, ty] = 2;
                item_temp[tx, ty].Back = scolor_noway;
                fany(tx, ty, z);
                gmain[tx, ty] = 0;
                item_temp[tx, ty].Back = scolor_none;
            }
        }

        private void cal_any_maxsum()
        {
            gTemp = new bool[m, n];
            gmain = new int[m, n];
            gResult = new bool[m, n];

            a = new int[m, n];
            asum = new int[m, n];
            a = (int[,])g.Clone();

            for (int i = 0; i < m; i++)
                for (int j = 0; j < n; j++)
                {
                    gTemp[i, j] = gResult[i, j] = false;
                    gmain[i, j] = 0;
                }

            for (int i = 0; i < m; i++)
                for (int j = 0; j < n; j++)
                {
                    if (a[i, j] > 0) sumall += a[i, j];
                    asum[i, j] = sumall;
                    if (amax < a[i, j]) amax = a[i, j];
                }
            Set_item_temp();
            fany(0, -1, 0);

            Set_item_temp();
            for (int i = 0; i < m; i++)
                for (int j = 0; j < n; j++)
                {
                    if (gResult[i, j]) item_temp[i, j].Back = scolor_result;
                }
            addstepany(sum, sum);
        }


    }
}
