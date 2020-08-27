using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DWM
{    
    /// <summary> 
    /// Класс для работы с буфером
    /// </summary>
    public class Buffer
    {
        private byte [] buffer;
        private long Size;
        private long Current;

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="b">Байтовый буфер</param>
        /// <param name="size">Размер</param>
        public Buffer(byte [] b, long size) 
        {
            buffer = b;
            Size = size;
            Current = 0;
        }

        /// <summary>
        /// Считывает последующий байт из буфера
        /// </summary>
        /// <returns>Ситанный байт, возвращает 0, если буфер закончился</returns>
        public byte GetNextByte()
        {
            return (Current < Size) ? buffer[Current++]:(byte)0;            
        }

        /// <summary>
        /// Определяет все ли байты считаны из буфера
        /// </summary>
        /// <returns>Истина/Ложь</returns>
        public bool IsEnd()
        {           
            return (Current == Size);
        }
    }
}
