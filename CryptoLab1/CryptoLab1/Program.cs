using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoLab1
{
    struct CharInfo
    {
        public char Character;
        public int Count;

        public int OrderNumber;
    }

    class Program
    {
        static void Main(string[] args)
        {
            string allText = File.ReadAllText("Arthur.txt", Encoding.Default);

            Dictionary<char, int> charCount = new Dictionary<char, int>();

            foreach (var character in allText)
                if (charCount.ContainsKey(character))
                {
                    charCount[character]++;
                }
                else
                {
                    charCount[character] = 1;
                }

            List<CharInfo> charInfos =
                charCount.Select(pair => new CharInfo()
                    {
                        Character = pair.Key,
                        Count = pair.Value
                    })
                    .OrderByDescending(info => info.Count)
                    .ToList();

            int order = 0;

            foreach (var charInfo in charInfos)
            {
                Console.WriteLine("Char: {0}   Count: {1},   Order: {2}", charInfo.Character, charInfo.Count, order++);
            }

            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.WriteLine(allText);

            Console.ReadKey();
        }
    }
}
