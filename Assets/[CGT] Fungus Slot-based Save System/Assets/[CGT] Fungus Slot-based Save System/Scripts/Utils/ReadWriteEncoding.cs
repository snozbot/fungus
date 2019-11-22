using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;

namespace CGTUnity.Fungus.SaveSystem
{
    public enum ReadWriteEncoding
    {
        ASCII, 
        BigEndianUnicode,
        Default,
        UTF7,
        UTF8,
        UTF32,
        Unicode

    }

    public static class ReadWriteEncodingExtensions
    {
        public static Encoding ToTextEncoding(this ReadWriteEncoding encoding)
        {
            switch (encoding)
            {
                case ReadWriteEncoding.ASCII:
                    return Encoding.ASCII;

                case ReadWriteEncoding.BigEndianUnicode:
                    return Encoding.BigEndianUnicode;

                case ReadWriteEncoding.Default:
                    return Encoding.Default;

                case ReadWriteEncoding.UTF32:
                    return Encoding.UTF32;

                case ReadWriteEncoding.UTF7:
                    return Encoding.UTF7;

                case ReadWriteEncoding.UTF8:
                    return Encoding.UTF8;

                case ReadWriteEncoding.Unicode:
                    return Encoding.Unicode;
                    
                default:
                    throw new System.ArgumentException("ReadWriteEncoding " + encoding + " not accounted for.");
            }
        }
    }

}