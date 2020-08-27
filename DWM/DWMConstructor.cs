using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DWM
{
    /// <summary>
    /// Класс построения Динамического цифрового водяного знака, представляемого в виде отдельных массивов с сигнатурами
    /// </summary>
    /// <remarks> 
    /// ДВЦ представлен в виде отдельных Блоков в начале и конце блока размещаются сигнатуры, в середине часть полезного содержимого ЦВЗ
    /// </remarks>
    public class DWMConstructor
    {
        private long DWMSize, ContentSize;          //Размеры ЦВЗ и полезного содержимого
        private long BlockSize, UsefullBlockSize;   //Размеры Блока ЦВЗ и полезной части блока (без сигнатур)
        private long SignSize;                      //Размер одной сигнатуры
        private byte[] Sign;                        //Массив с самой первой сигнатурой
        private String FileSign;                    //Файл с самой первой сигнатурой
        private String ContentFilePath;             //Файл с полезным содержимым ЦВЗ
        private byte[] DWMBuffer, FileBuffer;       //Буферы для ЦВЗ и файла полезного контента
        private Buffer B;                           //Класс обработки буфера   
        private Random rnd = new Random();
        private int DWMSubfunctions;                //Число функций в исходном коде, строящих ЦВЗ

        /// <summary>
        /// Рассчитывает размер всего ЦВЗ с учетом размеров сигнатур и полезного содержимого
        /// </summary>
        /// <returns>Размер в байтах</returns>
        private long GetDWMSize()
        {
            UsefullBlockSize = BlockSize - 2 * SignSize;
            if (UsefullBlockSize <= 0) { throw new Exception("Полезное место в блоке слишком маленькое или сигнатура занимает весь блок"); }            

            if (ContentSize % UsefullBlockSize != 0)
            {
                DWMSize = (int)(BlockSize*(ContentSize / UsefullBlockSize + 1));
            }
            else
            {
                DWMSize = (int)(BlockSize * ContentSize / UsefullBlockSize);
            }
            return DWMSize;
        }

        /// <summary>
        /// Вычисляет размер файла
        /// </summary>
        /// <param name="FilePath">Имя файла</param>
        /// <returns>Размер в байтах, возвращает 0, если файл не найден</returns>
        private long GetFileSize(string FilePath)
        {
            long t=0;
            if (File.Exists(FilePath))
            {
                FileInfo f = new FileInfo(FilePath);
                t = f.Length;                
            }
            return t;            
        }

        /// <summary>
        /// Читает файл в буфер
        /// </summary>
        /// <param name="b">Буфер</param>
        /// <param name="file">Имя файла</param>
        private void ReadFile(out byte [] b,String file)
        {
            b = new byte [GetFileSize(file)];
            b = File.ReadAllBytes(file);
        }

        /// <summary>
        /// Записывает сигнатуру в указанную позицию
        /// </summary>
        /// <remarks>
        /// Первая сигнатура берется из файла, последующие генерируются случайным образом
        /// </remarks>
        /// <param name="p">Позиция в буфере</param>
        /// <param name="buf">Буфер</param>
        private void PutSign(long p,ref byte[] buf)
        {
            if (p!=0)
            {
                
                for (long i = p; i < p + SignSize; i++)
                {
                    buf[i] = (byte)(rnd.Next() % 256);
                    if (i+SignSize < DWMSize)
                    {
                        buf[i + SignSize] = buf[i];
                    }
                }                
            }
            else
            {
                for (long i = 0; i < SignSize; i++)
                {
                    buf[i] = Sign[i];
                }
            }
        }

        /// <summary>
        /// Читается первичная сигнатура из файла в буфер
        /// </summary>
        /// <remarks>
        /// Формат файла - двоичный
        /// </remarks>
        /// <param name="SignFile">Имя файла с сигнатурой</param>
        public void ReadSign(String SignFile)
        {
            ReadFile(out Sign,SignFile);
            SetSignSize(GetFileSize(SignFile));
        }

        /// <summary>
        /// Производится проверка наличия необходимых файлов
        /// </summary>
        public void Check()
        {
            if (!File.Exists(ContentFilePath))
            {
                throw new Exception("Не определен файл с полезным содержимым");
            }

            if (!File.Exists(FileSign))
            {
                throw new Exception("Не определен файл с сигнатурой");
            }
        }

        /// <summary>
        /// Формируется ЦВЗ в виде длинного массива, содержащего последовательность блоков.  
        /// </summary>
        public void GenerateDWM()
        {
            ContentSize = GetFileSize(ContentFilePath);     //Вычисляется размер файла с полезным содержимым
            ReadFile(out FileBuffer, ContentFilePath);      //Читается файл с полезным содержимым (например, паспортные данные Автора)            
            ReadSign(FileSign);                             //Читается файл с первично сигнатурой

            DWMSize = GetDWMSize();
            DWMBuffer = new byte[DWMSize];
            B = new Buffer(FileBuffer, ContentSize);

            long i = 0;
            PutSign(i, ref DWMBuffer);
            i += SignSize;

            while (!B.IsEnd())                              //Пока не закончится буфер с полезным содержимым
            {
                for (long j =0; j < UsefullBlockSize; j++,i++)
                {
                    DWMBuffer[i] = B.GetNextByte();         //Пишем полезное содержимое в ЦВЗ
                }
                PutSign(i, ref DWMBuffer);                  //Пишем сигнатуры ЦВЗ
                i += 2*SignSize;
            }
        }

        /// <summary>
        /// Генерирует файл исходного кода (*.cs), строящего ЦВЗ.
        /// </summary>
        /// <param name="SourceFilePath">Имя файла</param>
        public void SaveDWM(String SourceFilePath)
        {
            Sources Src = new Sources(SourceFilePath, DWMBuffer, DWMSize, BlockSize, DWMSubfunctions);            
            Src.WriteAllSources();
        }

        /// <summary>
        /// Задается размер блока
        /// </summary>
        /// <param name="size">Размер в байтах</param>
        public void SetBlockSize(long size)
        {
            BlockSize = size;
        }

        /// <summary>
        /// Задается размер сигнатуры
        /// </summary>
        /// <param name="size">Размер в байтах</param>
        public void SetSignSize(long size)
        {
            SignSize = size;
        }

        /// <summary>
        /// Задается имя файла с сигнатурой
        /// </summary>
        /// <param name="name">Имя файла</param>
        public void SetFileSign(String name)
        {
            FileSign = name;
        }

        /// <summary>
        /// Задается имя файла с полезным содержимым
        /// </summary>
        /// <param name="name">Имя файла</param>
        public void SetContentFilePath(String name)
        {
            ContentFilePath = name;
        }        

        /// <summary>
        /// Задается число функций в исходном коде, строящих ЦВЗ
        /// </summary>
        /// <param name="i"></param>
        public void SetDWMSubfunctions(int i)
        {
            DWMSubfunctions = i;
        }
    }
}
