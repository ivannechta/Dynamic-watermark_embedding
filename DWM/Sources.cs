using System;
using System.IO;

namespace DWM
{
    /// <summary>
    /// Класс предназначен для генерации исходного кода, содержащего функции построения цифрового водяного знака
    /// Построение ЦВЗ происходит не одной функцией, а несколькими, которые вызываются из разных участков будующей 
    /// защищенной программы (для скрытности).
    /// </summary>
    public class Sources
    {
        private String Path;    // Имя генерируемого файла *.cs
        public long DWMSize;    //Размер ЦВЗ
        public long BlockSize;  //Размер блока ЦВЗ
        public byte[] DWMBuffer;//Буфер с ЦВЗ
        public int[] Merge;     //Вспомогательный массив для учета числа функций, строящих ЦВЗ в памяти
                                //Сериями в Merge обозначаются фрагменты ЦВЗ, которые строит одна функция (в исходном коде)
        public int MergeSize;   //Размер вспомогательного массива
        public int RunLenCnt;   //Число серий во вспомогательном массиве
        public int RunLenMax;   //Максимально разрешенное число серий во вспомогательном массиве 
                                //оно же число функций (в исходном коде), строящих ЦВЗ в памяти

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="path">Путь к генерируемому файлу</param>
        /// <param name="dwmBuffer">Буфер с ЦВЗ</param>
        /// <param name="dwmSize">Размер буфера с ЦВЗ</param>
        /// <param name="blocksize">Размер блока ЦВЗ</param>
        /// <param name="DWMSubfunctions">Число функций (в исходном коде), строящих ЦВЗ в памяти</param>
        public Sources(String path, byte[] dwmBuffer, long dwmSize, long blocksize, int DWMSubfunctions)
        {
            Path = path;
            DWMBuffer = dwmBuffer;
            DWMSize = dwmSize;
            BlockSize = blocksize;
            RunLenMax = DWMSubfunctions;

            MergeSize = RunLenCnt = (int)(DWMSize / BlockSize);
            Merge = new int[RunLenCnt];
            for (int i = 0; i < RunLenCnt; i++)
            {
                Merge[i] = i;
            }
        }

        /// <summary>
        /// Уменьшает количество серий в массиве Merge до RunLenMax
        /// Серии сливаются случайным образом
        /// </summary>
        public void MakeRunLen()
        {
            Random rnd = new Random();
            while (GetRunLenCnt() > RunLenMax)
            {
                MakeMerge(rnd.Next() % MergeSize);
            }
        }

        /// <summary>
        /// Слияние текущей серии со следующей
        /// </summary>
        /// <param name="p"></param>
        public void MakeMerge(int p)
        {
            int t = Merge[p];
            while ((p < MergeSize) && (t == Merge[p])) { p++; }
            if (p >= MergeSize) return;
            int d = Merge[p];
            while ((p < MergeSize) && (d == Merge[p]))
            {
                Merge[p] = t;
                p++;
            }
        }

        /// <summary>
        /// Считает количество серий в Merge
        /// </summary>
        /// <returns>Количество серий</returns>
        public int GetRunLenCnt() 
        {
            int tmp = 1;
            int d = Merge[0];
            for (int i = 0; i < MergeSize; i++)
            {
                if (d != Merge[i])
                {
                    tmp++;
                    d = Merge[i];                    
                }
            }
            return tmp;
        }

        /// <summary>
        /// Вычисляет размер текущей серии
        /// </summary>
        /// <param name="p">Позиция начала текущей серии в Merge</param>
        /// <returns>Размер серии</returns>
        public int GetRunLenSize(int p)
        {
            int tmp = 0;
            int a=Merge[p];
            for (int i = p; i < (int)(DWMSize / BlockSize); i++)
            {
                if (a != Merge[i])
                {
                    return tmp;
                }
                tmp++;
            }
            return tmp;
        }

        /// <summary>
        /// Возвращает индекс начала следующей серии
        /// </summary>
        /// <param name="p">Позиция начала текущей серии в Merge</param>
        /// <returns>Позиция серии или -1, если серии закончились</returns>
        public int GetNextRunLen(int p)
        {
            if (p >= MergeSize) return -1;
            int d = Merge[p];            
            while ((p < MergeSize)&&(d == Merge[p]))
            {
                p++;
            }
            return (p < MergeSize)? p : -1;
        }

        /// <summary>
        /// Пишет в файл одну функцию с фрагментом водяного знака
        /// </summary>
        /// <param name="p">Позиция в массиве Merge с соответствующей серией</param>
        /// <param name="fCnt">Номер функции</param>
        public void WriteOneFunction(int p,int fCnt)
        {
            int Next=GetNextRunLen(p);
            int LenSize = (Next == -1) ? MergeSize - p : Next - p; //Длина текущей серии в Merge
            using (StreamWriter writer = File.AppendText(Path))
            {
                writer.WriteLine("");
                writer.WriteLine("      public byte[] DWM_fragment"+fCnt+"()");
                writer.WriteLine("      {"                            );
                writer.Write    ("          byte[] DWMBuffer = {"         );

                //Запишем ЦВЗ в виде константного байтового массива
                for (int i = p; i < p + LenSize; i++)
                {
                    for (int k = 0; k < BlockSize; k++)
                    {
                        writer.Write("0x");
                        writer.Write( DWMBuffer[i * BlockSize + k].ToString("X"));
                        if ((i < p + LenSize - 1) || (k < BlockSize - 1))
                        {
                            writer.Write(",");
                        }
                    }
                }                
                writer.WriteLine("};");

                // Запишем текст самой функции
                writer.Write("          byte[] DWMFragment = new byte["         );
                writer.Write(""+BlockSize*LenSize);     // Размер массива фрагмента ЦВЗ
                writer.WriteLine("];"                                           );
                writer.Write("          for (int i = 0; i < "                   );
                writer.Write("" + BlockSize * LenSize); // Размер массива фрагмента ЦВЗ                
                writer.WriteLine("; i++)"                                       );
                writer.WriteLine("          {"                                  );
                writer.WriteLine("              DWMFragment[i] = DWMBuffer[i];" );
                writer.WriteLine("          }"                                  );
                writer.WriteLine("          return DWMFragment;"                );
                writer.WriteLine("      }"                                      );
                writer.Close();
            }
        }

        /// <summary>
        /// Генерирует весь файл исходного кода с функциями построения ЦВЗ
        /// </summary>
        public void WriteAllSources()
        {
            MakeRunLen();
            using (StreamWriter writer = new StreamWriter(Path))
            {
                writer.WriteLine("namespace DWM");
                writer.WriteLine("{");
                writer.WriteLine("    public class Watermark");
                writer.WriteLine("    {");                
                writer.Close();
            }

            int i = 0, fCnt=1;
            do {                
                WriteOneFunction(i,fCnt++);
                i = GetNextRunLen(i);
            } while (i != -1);

            using (StreamWriter writer = File.AppendText(Path))
            {
                writer.WriteLine("    }");
                writer.WriteLine("}");
                writer.Close();
            }            
        }
    }
}
