using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using utils;

namespace humanoidhunt
{
    class Task01
    {
        public static void Run()
        {
            // C# 8-bit = sbyte
            // Each byte gives byte-index from start to next
            // If byte-index goes over the given line, it is considered invalid
            // After valid bytes, convert next invalid byte to char

            // https://www.digital-detective.net/understanding-big-and-little-endian-byte-order/

            Console.WriteLine("Reading password from stream...");
            var input = FileSystem.GetInput("input-tattoo");
            var password = "";

            foreach (var line in input)
            {
                var byteStream = line;
                //byteStream = ConvertToSmallEndian8Bit(line);
                //byteStream = ConvertToSmallEndianFullLine(line);
                //byteStream = ConvertToSmallEndian16Bit(line);
                //byteStream = ConvertToSmallEndian32Bit(line);

                var streamByteCount = byteStream.Length / 8;
                int firstValidByte = 0;

                // Find first valid byte
                for (int i = 0; i < streamByteCount; i++)
                {
                    var singleByte = byteStream.Substring(i * 8, 8);
                    var toInt = Convert.ToInt32(singleByte, 2);
                    if (toInt >= streamByteCount)
                    {
                        // Invalid
                        continue;
                    }

                    firstValidByte = toInt;
                    break;
                }

                // Keep going till first invalid
                int firstInvalid = 0;
                int nextLocation = firstValidByte;
                while (true)
                {
                    var singleByte = byteStream.Substring(nextLocation * 8, 8);
                    nextLocation = Convert.ToInt32(singleByte, 2);
                    if (nextLocation >= streamByteCount)
                    {
                        // Invalid
                        firstInvalid = nextLocation;
                        break;
                    }

                }

                if (firstInvalid > 127) throw new ArgumentException();
                //var tempByte = (byte)firstInvalid;
                //var temp = Convert.ToChar(tempByte);
                var passwordChar = Convert.ToChar(firstInvalid);
                password += passwordChar.ToString();
                Console.WriteLine(passwordChar);
            }

            Console.WriteLine(password);
        }

        /// <summary>
        /// Reverse each 8-bit part of string
        /// </summary>
        /// <param name="byteString"></param>
        /// <returns></returns>
        static string ConvertToSmallEndian8Bit(string byteString)
        {
            string bigEndian = "";
            var streamByteCount = byteString.Length / 8;

            for (int i = 0; i < streamByteCount; i++)
            {
                var singleByte = byteString.Substring(i * 8, 8);

                bigEndian += ReverseString(singleByte);
            }

            return bigEndian;
        }

        /// <summary>
        /// Reverse all bytes
        /// </summary>
        /// <param name="byteString"></param>
        /// <returns></returns>
        static string ConvertToSmallEndianFullLine(string byteString)
        {
            string smallEndian = "";
            var streamByteCount = byteString.Length / 8;

            for (int i = streamByteCount - 1; i >= 0; i--)
            {
                var singleByte = byteString.Substring(i * 8, 8);

                smallEndian += singleByte;
            }

            return smallEndian;
        }

        /// <summary>
        /// Reverse all bytes in 16-bit format
        /// </summary>
        /// <param name="byteString"></param>
        /// <returns></returns>
        static string ConvertToSmallEndian16Bit(string byteString)
        {
            string smallEndian = "";
            var streamByteCount = byteString.Length / 8;
            var swapByteCount = streamByteCount / 2;

            for (int i = 0; i < swapByteCount; i++)
            {
                var firstByte = byteString.Substring(i * 16, 8);
                var secondByte = byteString.Substring(i * 16 + 8, 8);

                smallEndian += secondByte + firstByte;
            }

            return smallEndian;
        }

        /// <summary>
        /// Reverse all bytes in 32-bit format
        /// </summary>
        /// <param name="byteString"></param>
        /// <returns></returns>
        static string ConvertToSmallEndian32Bit(string byteString)
        {
            string smallEndian = "";
            var streamByteCount = byteString.Length / 8;
            var swapByteCount = streamByteCount / 4;

            for (int i = 0; i < swapByteCount; i++)
            {
                var firstByte = byteString.Substring(i * 16, 8);
                var secondByte = byteString.Substring(i * 16 + 8, 8);
                var thirdByte = byteString.Substring(i * 16 + 16, 8);
                var fourthByte = byteString.Substring(i * 16 + 32, 8);

                smallEndian += fourthByte + thirdByte + secondByte + firstByte;
            }

            return smallEndian;
        }

        static string ReverseString(string input)
        {
            var result = "";
            for (int i = input.Length - 1; i >= 0; i--)
            {
                result += input[i];
            }

            return result;
        }

        // https://jamesmccaffrey.wordpress.com/2013/12/29/converting-a-big-endian-integer-to-low-endian-using-c/
        static int ReverseBytes(int val)
        {
            byte[] intAsBytes = BitConverter.GetBytes(val);
            Array.Reverse(intAsBytes);
            return BitConverter.ToInt32(intAsBytes, 0);
        }

        static int BigEndianToValue(string byteString)
        {
            var littleEndian = BigEndianToLittleEndian32Bit(byteString);
            var sum = 0;
            for (int i = 0; i < 32; i++)
            {
                var next = int.Parse(littleEndian[i].ToString());
                sum += next * (int)Math.Pow(2, i);
            }

            return sum;
        }

        /// <summary>
        /// Reverse the 4 bytes
        /// 1        2        3        4
        /// 00000000 10000000 00010000 00000001
        /// 00000001 00010000 10000000 00000000
        /// 4        3        2        1 
        /// </summary>
        /// <param name="byteString"></param>
        /// <returns></returns>
        static string BigEndianToLittleEndian32Bit(string byteString)
        {
            if (byteString.Length != 32) throw new ArgumentException();

            // To little endian
            var littleEndian = "";
            for (int i = 0; i < 4; i++)
            {
                var bitString = byteString.Substring(i * 8, 8);
                littleEndian = littleEndian.Insert(0, bitString);
            }

            return littleEndian;
        }
    }
}
