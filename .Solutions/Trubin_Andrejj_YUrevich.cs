//http://acm.timus.ru/author.aspx?id=102314
//усложнения №1,2,3

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace _1843
{
    class Program
    {
        static int[,] tc = new int[24, 9];//turn_cube - tc // описываем все возможные варианты как можно повернуть куб;
        //первые 6 чисел описывают перестановку граней блока, следующие 3 перестановку размеров
        static int[, ,] ctc = new int[6, 6, 4];//clever_twist_cube - ctc // умный поворот куба, если мы знаем цвет одной из граней итоговаого блока("6 цветов" * "6 граней"),
        // то нам досточно перебрать всего 4 варианта из массива tc вместо 24, чтобы понять подходить ли блок на проверяемое место
        static string[,] edge_color_string = new string[3, 4];//опишсывает цвета всех граней в виде строк("3 измерения" * "4 грани")
        static int[, ,] edge_color = new int[3, 4, 2];//описывает цвета всех граней в виде пары чисел("3 измерения" * "4 грани" * "2 цвета для грани")
        static int[,] edge_color_ends = new int[3, 2];//запоминаем для каждой грани два ребра которые ограничевают грань с двух сторон("3 измерения" * "2 ребра")
        static string color_base;//цвета граней параллелипипеда до разрезания
        static int[] type_color = { 32, 16, 8, 4, 2, 1 };//тип каждой грани
        static short[] vwc = { 0, 1, 8, 9, 16, 17 };//version_without_color - vwc, если блоки вообще не ракрашены, то нам достаточно проверить 6 вариантов поворотов чтобы узнать одинаковые ли блоки между собой или нет
        static int[] size_wdh = new int[3];//количество блоков в гранях
        static int[] size_block_w, size_block_d, size_block_h;//размеры блоков вдоль граней

        static void turn_cube()
        {//заполняем массив tc, зачем он нужен написано при его объявлении
            /////11111
            tc[0, 0] = 0; tc[0, 1] = 1; tc[0, 2] = 2; tc[0, 3] = 3; tc[0, 4] = 4; tc[0, 5] = 5; tc[0, 6] = 0; tc[0, 7] = 1; tc[0, 8] = 2;
            tc[1, 0] = 0; tc[1, 1] = 1; tc[1, 2] = 4; tc[1, 3] = 5; tc[1, 4] = 3; tc[1, 5] = 2; tc[1, 6] = 0; tc[1, 7] = 2; tc[1, 8] = 1;
            tc[2, 0] = 0; tc[2, 1] = 1; tc[2, 2] = 3; tc[2, 3] = 2; tc[2, 4] = 5; tc[2, 5] = 4; tc[2, 6] = 0; tc[2, 7] = 1; tc[2, 8] = 2;
            tc[3, 0] = 0; tc[3, 1] = 1; tc[3, 2] = 5; tc[3, 3] = 4; tc[3, 4] = 2; tc[3, 5] = 3; tc[3, 6] = 0; tc[3, 7] = 2; tc[3, 8] = 1;
            /////22222
            tc[4, 0] = 1; tc[4, 1] = 0; tc[4, 2] = 2; tc[4, 3] = 3; tc[4, 4] = 5; tc[4, 5] = 4; tc[4, 6] = 0; tc[4, 7] = 1; tc[4, 8] = 2;
            tc[5, 0] = 1; tc[5, 1] = 0; tc[5, 2] = 4; tc[5, 3] = 5; tc[5, 4] = 2; tc[5, 5] = 3; tc[5, 6] = 0; tc[5, 7] = 2; tc[5, 8] = 1;
            tc[6, 0] = 1; tc[6, 1] = 0; tc[6, 2] = 3; tc[6, 3] = 2; tc[6, 4] = 4; tc[6, 5] = 5; tc[6, 6] = 0; tc[6, 7] = 1; tc[6, 8] = 2;
            tc[7, 0] = 1; tc[7, 1] = 0; tc[7, 2] = 5; tc[7, 3] = 4; tc[7, 4] = 3; tc[7, 5] = 2; tc[7, 6] = 0; tc[7, 7] = 2; tc[7, 8] = 1;
            /////33333
            tc[8, 0] = 2; tc[8, 1] = 3; tc[8, 2] = 1; tc[8, 3] = 0; tc[8, 4] = 4; tc[8, 5] = 5; tc[8, 6] = 1; tc[8, 7] = 0; tc[8, 8] = 2;
            tc[9, 0] = 2; tc[9, 1] = 3; tc[9, 2] = 5; tc[9, 3] = 4; tc[9, 4] = 1; tc[9, 5] = 0; tc[9, 6] = 1; tc[9, 7] = 2; tc[9, 8] = 0;
            tc[10, 0] = 2; tc[10, 1] = 3; tc[10, 2] = 0; tc[10, 3] = 1; tc[10, 4] = 5; tc[10, 5] = 4; tc[10, 6] = 1; tc[10, 7] = 0; tc[10, 8] = 2;
            tc[11, 0] = 2; tc[11, 1] = 3; tc[11, 2] = 4; tc[11, 3] = 5; tc[11, 4] = 0; tc[11, 5] = 1; tc[11, 6] = 1; tc[11, 7] = 2; tc[11, 8] = 0;
            /////44444
            tc[12, 0] = 3; tc[12, 1] = 2; tc[12, 2] = 5; tc[12, 3] = 4; tc[12, 4] = 0; tc[12, 5] = 1; tc[12, 6] = 1; tc[12, 7] = 2; tc[12, 8] = 0;
            tc[13, 0] = 3; tc[13, 1] = 2; tc[13, 2] = 1; tc[13, 3] = 0; tc[13, 4] = 5; tc[13, 5] = 4; tc[13, 6] = 1; tc[13, 7] = 0; tc[13, 8] = 2;
            tc[14, 0] = 3; tc[14, 1] = 2; tc[14, 2] = 4; tc[14, 3] = 5; tc[14, 4] = 1; tc[14, 5] = 0; tc[14, 6] = 1; tc[14, 7] = 2; tc[14, 8] = 0;
            tc[15, 0] = 3; tc[15, 1] = 2; tc[15, 2] = 0; tc[15, 3] = 1; tc[15, 4] = 4; tc[15, 5] = 5; tc[15, 6] = 1; tc[15, 7] = 0; tc[15, 8] = 2;
            /////55555
            tc[16, 0] = 4; tc[16, 1] = 5; tc[16, 2] = 1; tc[16, 3] = 0; tc[16, 4] = 3; tc[16, 5] = 2; tc[16, 6] = 2; tc[16, 7] = 0; tc[16, 8] = 1;
            tc[17, 0] = 4; tc[17, 1] = 5; tc[17, 2] = 2; tc[17, 3] = 3; tc[17, 4] = 1; tc[17, 5] = 0; tc[17, 6] = 2; tc[17, 7] = 1; tc[17, 8] = 0;
            tc[18, 0] = 4; tc[18, 1] = 5; tc[18, 2] = 0; tc[18, 3] = 1; tc[18, 4] = 2; tc[18, 5] = 3; tc[18, 6] = 2; tc[18, 7] = 0; tc[18, 8] = 1;
            tc[19, 0] = 4; tc[19, 1] = 5; tc[19, 2] = 3; tc[19, 3] = 2; tc[19, 4] = 0; tc[19, 5] = 1; tc[19, 6] = 2; tc[19, 7] = 1; tc[19, 8] = 0;
            /////66666
            tc[20, 0] = 5; tc[20, 1] = 4; tc[20, 2] = 3; tc[20, 3] = 2; tc[20, 4] = 1; tc[20, 5] = 0; tc[20, 6] = 2; tc[20, 7] = 1; tc[20, 8] = 0;
            tc[21, 0] = 5; tc[21, 1] = 4; tc[21, 2] = 0; tc[21, 3] = 1; tc[21, 4] = 3; tc[21, 5] = 2; tc[21, 6] = 2; tc[21, 7] = 0; tc[21, 8] = 1;
            tc[22, 0] = 5; tc[22, 1] = 4; tc[22, 2] = 2; tc[22, 3] = 3; tc[22, 4] = 0; tc[22, 5] = 1; tc[22, 6] = 2; tc[22, 7] = 1; tc[22, 8] = 0;
            tc[23, 0] = 5; tc[23, 1] = 4; tc[23, 2] = 1; tc[23, 3] = 0; tc[23, 4] = 2; tc[23, 5] = 3; tc[23, 6] = 2; tc[23, 7] = 0; tc[23, 8] = 1;
        }

        static void clever_twist_cube()
        {//заполняем массив ctc, зачем он нужен написано при его объявлении
            
            for (int i = 0; i < 6; i++)
                for (int j = 0; j < 6; j++)
                {
                    for (int n = 0, k = 0; k < 24; k++)
                        if (tc[k, i] == j)
                        {
                            ctc[i, j, n] = k;
                            n++;
                        }
                }
        }

        static void fill_edge_color_string()
        {//заполняем массив edge_color_string, зачем он нужен написано при его объявлении
            size_wdh[0] = size_wdh[1] = size_wdh[2] = 1;//задаем такие значения чтобы легче было работать с функцией "color_block"
            //грань W
            edge_color_string[0, 0] = color_block(1, 1, 0, 1, 0, 1);
            edge_color_string[0, 1] = color_block(1, 1, 0, 1, 1, 0);
            edge_color_string[0, 2] = color_block(1, 1, 1, 0, 1, 0);
            edge_color_string[0, 3] = color_block(1, 1, 1, 0, 0, 1);
            //грань D
            edge_color_string[1, 0] = color_block(0, 1, 1, 1, 0, 1);
            edge_color_string[1, 1] = color_block(1, 0, 1, 1, 0, 1);
            edge_color_string[1, 2] = color_block(1, 0, 1, 1, 1, 0);
            edge_color_string[1, 3] = color_block(0, 1, 1, 1, 1, 0);
            //грань H
            edge_color_string[2, 0] = color_block(0, 1, 0, 1, 1, 1);
            edge_color_string[2, 1] = color_block(1, 0, 0, 1, 1, 1);
            edge_color_string[2, 2] = color_block(1, 0, 1, 0, 1, 1);
            edge_color_string[2, 3] = color_block(0, 1, 1, 0, 1, 1);
        }

        static void fill_edge_color()
        {//заполняем массив edge_color, зачем он нужен написано при его объявлении
            //грани вдоль оси W
            edge_color[0, 0, 0] = 4; edge_color[0, 0, 1] = 2;
            edge_color[0, 1, 0] = 2; edge_color[0, 1, 1] = 5;
            edge_color[0, 2, 0] = 5; edge_color[0, 2, 1] = 3;
            edge_color[0, 3, 0] = 3; edge_color[0, 3, 1] = 4;
            //грани вдоль оси D
            edge_color[1, 0, 0] = 0; edge_color[1, 0, 1] = 4;
            edge_color[1, 1, 0] = 4; edge_color[1, 1, 1] = 1;
            edge_color[1, 2, 0] = 1; edge_color[1, 2, 1] = 5;
            edge_color[1, 3, 0] = 5; edge_color[1, 3, 1] = 0;
            //грани вдоль оси H
            edge_color[2, 0, 0] = 0; edge_color[2, 0, 1] = 2;
            edge_color[2, 1, 0] = 2; edge_color[2, 1, 1] = 1;
            edge_color[2, 2, 0] = 1; edge_color[2, 2, 1] = 3;
            edge_color[2, 3, 0] = 3; edge_color[2, 3, 1] = 0;
        }

        static void fill_edge_color_ends()
        {//заполняем массив fill_edge_color_ends, зачем он нужен написано при его объявлении
            edge_color_ends[0, 0] = 0; edge_color_ends[0, 1] = 1;
            edge_color_ends[1, 0] = 2; edge_color_ends[1, 1] = 3;
            edge_color_ends[2, 0] = 4; edge_color_ends[2, 1] = 5;
        }

        static int[] func_nearest_plane(string f, string s)
        {//возвращает номера граней из блоков "f" и "s" имеющих одинаковый цвет
            int[] nearest_plane = { -1, -1 };
            for (int i = 0; i < 6; i++)
                if (s[i] != '.')
                {
                    nearest_plane[0] = i;
                    for (int j = 0; j < 6; j++)
                        if (s[i] == f[j])
                        {
                            nearest_plane[1] = j;
                            break;
                        }
                    break;
                }
            return nearest_plane;
        }

        static int rotation_wdh_extended(string f, string s, int w, int d, int h)
        {
            int[] wdh_input = { w, d, h };

            int[] nearest_plane = func_nearest_plane(f, s);

            for (int i = 0; i < 4; i++)
                if (   (f[tc[ctc[nearest_plane[0], nearest_plane[1], i], 0]] == s[0] || s[0] == '.')
                    && (f[tc[ctc[nearest_plane[0], nearest_plane[1], i], 1]] == s[1] || s[1] == '.')
                    && (f[tc[ctc[nearest_plane[0], nearest_plane[1], i], 2]] == s[2] || s[2] == '.')
                    && (f[tc[ctc[nearest_plane[0], nearest_plane[1], i], 3]] == s[3] || s[3] == '.')
                    && (f[tc[ctc[nearest_plane[0], nearest_plane[1], i], 4]] == s[4] || s[4] == '.')
                    && (f[tc[ctc[nearest_plane[0], nearest_plane[1], i], 5]] == s[5] || s[5] == '.'))
                    return ctc[nearest_plane[0], nearest_plane[1], i];
            return -1;
        }

        static int[] turn(string f, string s, int f_w, int f_d, int f_h, int s_w, int s_d, int s_h)
        {//f(color figure);;s(color segment)
            int[] type_turn = { -1, -1 };
            int[] f_wdh = { f_w, f_d, f_h };
            int[] s_wdh = { s_w, s_d, s_h };

            int[] nearest_plane = func_nearest_plane(f, s);

            if (nearest_plane[0] != -1)
            {
                if (nearest_plane[1] == -1)
                    return type_turn;
                for (int i = 0; i < 4; i++)
                    if (f_wdh[tc[ctc[nearest_plane[0], nearest_plane[1], i], 6]] == s_wdh[0] && f_wdh[tc[ctc[nearest_plane[0], nearest_plane[1], i], 7]] == s_wdh[1] && f_wdh[tc[ctc[nearest_plane[0], nearest_plane[1], i], 8]] == s_wdh[2] && f[tc[ctc[nearest_plane[0], nearest_plane[1], i], 0]] == s[0] && f[tc[ctc[nearest_plane[0], nearest_plane[1], i], 1]] == s[1] && f[tc[ctc[nearest_plane[0], nearest_plane[1], i], 2]] == s[2] && f[tc[ctc[nearest_plane[0], nearest_plane[1], i], 3]] == s[3] && f[tc[ctc[nearest_plane[0], nearest_plane[1], i], 4]] == s[4] && f[tc[ctc[nearest_plane[0], nearest_plane[1], i], 5]] == s[5])
                    {
                        type_turn[0] = tc[ctc[nearest_plane[0], nearest_plane[1], i], 0];
                        type_turn[1] = tc[ctc[nearest_plane[0], nearest_plane[1], i], 2];
                        return type_turn;
                    }
            }
            else
                for (int i = 0; i < 6; i++)
                    if (f_wdh[tc[vwc[i], 6]] == s_wdh[0] && f_wdh[tc[vwc[i], 7]] == s_wdh[1] && f_wdh[tc[vwc[i], 8]] == s_wdh[2])
                    {
                        type_turn[0] = tc[vwc[i], 0];
                        type_turn[1] = tc[vwc[i], 2];
                        return type_turn;
                    }
            return type_turn;
        }

        static void swap(int[] myArray, int i, int j)
        {            
            int temp = myArray[i];
            myArray[i] = myArray[j];
            myArray[j] = temp;
        }

        static void swap(long[] myArray, long i, long j)
        {
            long temp = myArray[i];
            myArray[i] = myArray[j];
            myArray[j] = temp;
        }

        static long blocks_id(string color_segment,int w,int d,int h)
        {
            long id=0;
            for (int b = 0; b < 6; b++)
                if (color_segment.IndexOf(color_base[b]) >= 0)
                    id += type_color[b];

            int[] wdh = { w, d, h };
            if (wdh[0] > wdh[1])
                swap(wdh, 0, 1);
            if (wdh[1] > wdh[2])
                swap(wdh, 1, 2);
            if (wdh[0] > wdh[1])
                swap(wdh, 0, 1);
            if (wdh[1] > wdh[2])
                swap(wdh, 1, 2);

            id = id * 1001 * 1001 * 1001 + wdh[0] * 1001 * 1001 + wdh[1] * 1001 + wdh[2];
            return id;
        }

        static void pyramid(long[] block_id, int[] number_start, int j, int n)
        {            
            while (j * 2 + 2 <= n)
            {
                if (j * 2 + 2 == n)
                {
                    if (block_id[2 * j + 1] > block_id[j])
                    {
                        swap(block_id, j, 2 * j + 1);
                        swap(number_start, j, 2 * j + 1);

                        j = j * 2 + 1;
                    }
                    break;
                }
                else
                {
                    if (block_id[2 * j + 1] > block_id[j] && block_id[2 * j + 1] >= block_id[2 * j + 2])
                    {
                        swap(block_id, j, 2 * j + 1);
                        swap(number_start, j, 2 * j + 1);

                        j = j * 2 + 1;

                        continue;
                    }
                    if (block_id[2 * j + 2] > block_id[j])
                    {
                        swap(block_id, j, 2 * j + 2);
                        swap(number_start, j, 2 * j + 2);

                        j = j * 2 + 2;

                        continue;
                    }
                    break;
                }
            }
        }

        static void sort_pyramid(long[] block_id, int[] number_start, int n)
        {
            for (int i = (n - 2) / 2; i >= 0; i--)
                pyramid(block_id, number_start, i, n);
            for (int i = n - 1; i > 0; i--)
            {
                swap(block_id, 0, i);
                swap(number_start, 0, i);
                pyramid(block_id, number_start, 0, i);
            }
        }

        static string color_block(int w1, int w2, int d1, int d2, int h1, int h2)
        {
            string color_segment="";
            if (w1 == 0)
                color_segment += color_base[0];
            else
                color_segment += '.';
            if (w2 == size_wdh[0] - 1)
                color_segment += color_base[1];
            else
                color_segment += '.';
            if (d1 == 0)
                color_segment += color_base[2];
            else
                color_segment += '.';
            if (d2 == size_wdh[1] - 1)
                color_segment += color_base[3];
            else
                color_segment += '.';
            if (h1 == 0)
                color_segment += color_base[4];
            else
                color_segment += '.';
            if (h2 == size_wdh[2] - 1)
                color_segment += color_base[5];
            else
                color_segment += '.';
            return color_segment;
        }

        static void finish_rotaion_func(string[] color_figure, int[,] wdh_figure, int number_figure, int type_rotaion)
        {//зная как нужно повернуть блок в итоговом паралелипипеде, изменим его параметры
            int[] wdh = { wdh_figure[number_figure, 0], wdh_figure[number_figure, 1], wdh_figure[number_figure, 2] };
            wdh_figure[number_figure, 0] = wdh[tc[type_rotaion, 6]];
            wdh_figure[number_figure, 1] = wdh[tc[type_rotaion, 7]];
            wdh_figure[number_figure, 2] = wdh[tc[type_rotaion, 8]];

            string color = color_figure[number_figure];
            color_figure[number_figure] = "";
            for (int j = 0; j < 6; j++)
                color_figure[number_figure] += color[tc[type_rotaion, j]];
        }

        static void remember_size_blocks(string[] color_figure, int[,] wdh_figure, int[] size_block, int k, int j, int n)   
        {
            for (int n_edge = 0, i = 0; i < n; i++)
                if (color_figure[i].IndexOf(color_base[edge_color[k, j, 0]]) >= 0 && color_figure[i].IndexOf(color_base[edge_color[k, j, 1]]) >= 0)
                {
                    if (color_figure[i].IndexOf(color_base[edge_color_ends[k, 0]]) < 0 && color_figure[i].IndexOf(color_base[edge_color_ends[k, 1]]) < 0)
                        size_block[++n_edge] = wdh_figure[i, k];
                    if (color_figure[i].IndexOf(color_base[edge_color_ends[k, 0]]) >= 0)
                        size_block[0] = wdh_figure[i, k];
                    if (color_figure[i].IndexOf(color_base[edge_color_ends[k, 1]]) >= 0)
                        size_block[size_wdh[k] - 1] = wdh_figure[i, k];
                }
        }

        static void passing_pointer_size_blocks(int[] size_block, int k)
        {
            if (k == 0)
                size_block_w = size_block;
            if (k == 1)
                size_block_d = size_block;
            if (k == 2)
                size_block_h = size_block;
        }

        static void sizing_blocks(int k, string[] color_figure, int[,] wdh_figure, int length_standard, int n, int[,] edge, int[,] length_edge, int[, ,] edge_color, int[,] edge_color_ends)
        {
            int[] size_block;
            for (int j = 0; j < 4; j++)
                if (length_edge[k, j] == length_standard)
                {
                    size_wdh[k] = edge[k, j];
                    size_block = new int[size_wdh[k]];
                    remember_size_blocks(color_figure, wdh_figure, size_block, k, j, n);
                    passing_pointer_size_blocks(size_block, k);
                    return;//если мы уже нашли размеры блоков в грани, то работа функции должна быть завершенна
                }
            if (length_edge[k, 0] < length_standard)
            {
                size_wdh[k] = edge[k, 0] + 1;
                size_block = new int[size_wdh[k]];
                remember_size_blocks(color_figure, wdh_figure, size_block, k, 0, n);
                if (size_block[0] == 0)
                    size_block[0] = length_standard - length_edge[k, 0];
                else
                {
                    if (size_block[size_wdh[k] - 1] == 0)
                        size_block[size_wdh[k] - 1] = length_standard - length_edge[k, 0];
                    else
                        size_block[size_wdh[k] - 2] = length_standard - length_edge[k, 0];
                }
            }
            else
            {
                size_wdh[k] = edge[k, 0] - 1;
                size_block = new int[size_wdh[k]];
                if (size_wdh[k] == 1)
                    size_block[0] = length_standard;
                else
                {
                    int edge_start_n = 0, edge_center_n = 0, edge_end_n = 0;
                    int len_start_n = 0, len_center_n = 0, len_end_n = 0;
                    int j = 0;
                    for (int i = 0; i < n; i++)
                        if (color_figure[i].IndexOf(color_base[edge_color[k, j, 0]]) >= 0 && color_figure[i].IndexOf(color_base[edge_color[k, j, 1]]) >= 0)
                        {
                            if (color_figure[i].IndexOf(color_base[edge_color_ends[k, 0]]) < 0 && color_figure[i].IndexOf(color_base[edge_color_ends[k, 1]]) < 0)
                            {
                                edge_center_n++;
                                len_center_n += wdh_figure[i, 0];
                            }
                            if (color_figure[i].IndexOf(color_base[edge_color_ends[k, 0]]) >= 0 && color_figure[i].IndexOf(color_base[edge_color_ends[k, 1]]) < 0)
                            {
                                edge_start_n++;
                                len_start_n += wdh_figure[i, 0];
                            }
                            if (color_figure[i].IndexOf(color_base[edge_color_ends[k, 1]]) >= 0 && color_figure[i].IndexOf(color_base[edge_color_ends[k, 0]]) < 0)
                            {
                                edge_end_n++;
                                len_end_n += wdh_figure[i, 0];
                            }
                        }

                    int check = -1;
                    if (edge_start_n == 1 && edge_end_n == 1)
                    {
                        check = len_start_n + len_center_n + len_end_n - length_standard;
                        size_block[0] = len_start_n;
                        size_block[size_wdh[k] - 1] = len_end_n;
                    }
                    if (edge_start_n == 2)
                    {
                        size_block[0] = length_standard - len_center_n - len_end_n;
                        size_block[size_wdh[k] - 1] = len_end_n;
                    }
                    if (edge_end_n == 2)
                    {
                        size_block[0] = len_start_n;
                        size_block[size_wdh[k] - 1] = length_standard - len_center_n - len_start_n;
                    }

                    j = 0;
                    for (int n_edge = 0, i = 0; i < n; i++)
                        if (color_figure[i].IndexOf(color_base[edge_color[k, j, 0]]) >= 0 && color_figure[i].IndexOf(color_base[edge_color[k, j, 1]]) >= 0 && color_figure[i].IndexOf(color_base[edge_color_ends[k, 0]]) < 0 && color_figure[i].IndexOf(color_base[edge_color_ends[k, 1]]) < 0)
                        {
                            if (check == wdh_figure[i, 0])
                                check = -1;
                            else
                                size_block[++n_edge] = wdh_figure[i, 0];
                        }
                }
            }
            passing_pointer_size_blocks(size_block, k);
        }

        static void blocks_in_edges(int w, int d, int h, int n, string[] color_figure, int[,] wdh_figure, int[] finish_rotaion)
        //w,d,h размеры итогового параллелепипеда; n сколько всего блоков; color_figure цвета блоков; wdh_figure размеры входных блоков
        {//В этой функции установим сколько блоков в каждой из итоговых граней, для этого будем рассматривать все 12 граней
            int[,] edge = new int[3, 4];//количество блоков в каждой грани
            int[,] length_edge = new int[3, 4];//длина граней     
       
            size_wdh[0] = size_wdh[1] = size_wdh[2] = -1;//чтобы в будущем точно знать какие из данных заполнены, а какие нет присвоим всем зведомо недопустимое значение
            
            //ищем размер каждой грани и количество блоков в ней(проверяя есть ли на блоки цвета соответствующеие грани)
            for (int i = 0; i < n; i++)
                for (int j = 0; j < 3; j++)
                    for (int k = 0; k < 4; k++)
                        if (color_figure[i].IndexOf(color_base[edge_color[j, k, 0]]) >= 0 && color_figure[i].IndexOf(color_base[edge_color[j, k, 1]]) >= 0)
                        {
                            if (finish_rotaion[i] == -1)
                            {
                                finish_rotaion[i] = rotation_wdh_extended(color_figure[i], edge_color_string[j, k], wdh_figure[i, 0], wdh_figure[i, 1], wdh_figure[i, 2]);
                                finish_rotaion_func(color_figure, wdh_figure, i, finish_rotaion[i]);
                            }
                            edge[j, k]++; length_edge[j, k] += wdh_figure[i, j];
                        }
            
            //грань W
            sizing_blocks(0, color_figure, wdh_figure, w, n, edge, length_edge, edge_color, edge_color_ends);
            //грань D
            sizing_blocks(1, color_figure, wdh_figure, d, n, edge, length_edge, edge_color, edge_color_ends);
            //грань H
            sizing_blocks(2, color_figure, wdh_figure, h, n, edge, length_edge, edge_color, edge_color_ends);
        }

        private static void Main()
        {
            turn_cube();//заполняем массив tc(всевозможных поворотов блока)
            clever_twist_cube();//заполняем массив ctc("умного" поворота), при объявлений данного массива написанно для чего он нужен            

            string[] tokens = Console.In.ReadToEnd().Split(' ', '\n');
            //string[] tokens = Console.ReadLine().Split(' ');
            int w_base, d_base, h_base;

            w_base = int.Parse(tokens[0]);
            d_base = int.Parse(tokens[1]);
            h_base = int.Parse(tokens[2]);
            color_base = tokens[3];

            fill_edge_color_string();//заполняем массив edge_color_string, описывающий в виде строка цвета граней
            fill_edge_color();//заполняем массив edge_color, описывающий в виде строка цвета граней
            fill_edge_color_ends();//заполняем массив edge_color_ends, описывающий в виде строка цвета граней

            int n = int.Parse(tokens[4]);

            int[,] wdh_figure = new int[n, 3];

            string[] color_figure = new string[n];
            int[] finish_rotaion = new int[n];
            for (int i = 0; i < n; i++)
            {
                wdh_figure[i, 0] = int.Parse(tokens[5 + 4 * i]);
                wdh_figure[i, 1] = int.Parse(tokens[6 + 4 * i]);
                wdh_figure[i, 2] = int.Parse(tokens[7 + 4 * i]);
                color_figure[i] = tokens[8 + 4 * i];
                finish_rotaion[i] = -1;
            }
            
            blocks_in_edges(w_base, d_base, h_base, n, color_figure, wdh_figure, finish_rotaion);

            int[] coordinate_block_w = new int[size_wdh[0]];
            int[] coordinate_block_d = new int[size_wdh[1]];
            int[] coordinate_block_h = new int[size_wdh[2]];
            coordinate_block_w[0] = 0;
            for (int i = 1; i < size_wdh[0]; i++)
                coordinate_block_w[i] = coordinate_block_w[i - 1] + size_block_w[i - 1];
            coordinate_block_d[0] = 0;
            for (int i = 1; i < size_wdh[1]; i++)
                coordinate_block_d[i] = coordinate_block_d[i - 1] + size_block_d[i - 1];
            coordinate_block_h[0] = 0;
            for (int i = 1; i < size_wdh[2]; i++)
                coordinate_block_h[i] = coordinate_block_h[i - 1] + size_block_h[i - 1];
            
            string[] color_segment = new string[n + 1];//(n+1) - т.к. итоговых блоков может оказаться на один больше, чем входных данных(случай потерянного блока)
            int[,] size_block = new int[n + 1, 3];
            int[,] coordinate_block = new int[n + 1, 3];
            Int64[] block_id = new Int64[n + 1];
            int[] number_start = new int[n + 1];

            for (int a = 0, i = 0; i < size_wdh[0]; i++)
                for (int j = 0; j < size_wdh[1]; j++)
                    for (int k = 0; k < size_wdh[2]; k++)
                    {
                        color_segment[a] = color_block(i, i, j, j, k, k);

                        coordinate_block[a, 0] = coordinate_block_w[i];
                        coordinate_block[a, 1] = coordinate_block_d[j];
                        coordinate_block[a, 2] = coordinate_block_h[k];

                        size_block[a, 0] = size_block_w[i];
                        size_block[a, 1] = size_block_d[j];
                        size_block[a, 2] = size_block_h[k];

                        block_id[a] = blocks_id(color_segment[a], size_block_w[i], size_block_d[j], size_block_h[k]);
                        number_start[a] = a;
                        a++;
                    }

            int n_blocks = size_wdh[0] * size_wdh[1] * size_wdh[2];
            sort_pyramid(block_id, number_start, n_blocks);

            Int64[] unique_id = new Int64[n_blocks];//массив уникальных идентификаторов итоговых блоков
            int[] unique_id_start = new int[n_blocks];
            int[] unique_id_end = new int[n_blocks];//нужно для случая с лишнем кубиком
            int unique_n = 1;
            unique_id_start[0] = 0;
            unique_id[0] = block_id[0];
            for (int a = 1; a < n_blocks; a++)
                if (unique_id[unique_n - 1] != block_id[a])
                {
                    unique_id_end[unique_n - 1] = a - 1;
                    unique_id[unique_n] = block_id[a];
                    unique_id_start[unique_n] = a;
                    unique_n++;                     
                }
            unique_id_end[unique_n - 1] = n_blocks - 1;

            string symbol_output = "FBDULR";
            for (int a = 0; a < n; a++)
            {
                Int64 id = blocks_id(color_figure[a], wdh_figure[a,0], wdh_figure[a,1], wdh_figure[a,2]);

                int[] type_turn = new int[2];
                int start = 0, end = unique_n - 1, center;
                while(true)
                {
                    center = (end - start) / 2 + start;
                    if (unique_id[center] == id)
                    {                                                
                        if (unique_id_start[center] <= unique_id_end[center])
                        {
                            type_turn = turn(color_figure[a], color_segment[number_start[unique_id_start[center]]], wdh_figure[a, 0], wdh_figure[a, 1], wdh_figure[a, 2], size_block[number_start[unique_id_start[center]], 0], size_block[number_start[unique_id_start[center]], 1], size_block[number_start[unique_id_start[center]], 2]);

                            if (type_turn[0] != -1 && type_turn[1] != -1)
                            {
                                if (finish_rotaion[a] != -1)
                                {
                                    type_turn[0] = tc[finish_rotaion[a], 0];
                                    type_turn[1] = tc[finish_rotaion[a], 2];
                                }
                                Console.Write(symbol_output[type_turn[0]] + " ");
                                Console.Write(symbol_output[type_turn[1]] + " ");
                                Console.Write(coordinate_block[number_start[unique_id_start[center]], 0] + " ");
                                Console.Write(coordinate_block[number_start[unique_id_start[center]], 1] + " ");
                                Console.Write(coordinate_block[number_start[unique_id_start[center]], 2] + " ");
                                unique_id_start[center]++;
                            }
                        }
                        break;
                    }
                    else
                    {
                        if (unique_id[center] < id)
                            start = center + 1;
                        else
                            end = center - 1;
                        if (end < start)//для случая с лишним кубиком
                            break;
                    }
                }
                Console.Write('\n');
            }
            //string[] tokens1 = Console.ReadLine().Split(' ');
        }
    }
}