using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoLab1
{
    

class CharInfo
    {
        public char Character;

        public int Count;
        public double FractionPercent;

        public int OrderNumber;
    }



    class Program
    {
        private static char[] russianLetters =
        {

            'а', 'б', 'в', 'г', 'д', 'е', 'ё', 'ж', 'з', 'и', 'й', 'к', 'л', 'м', 'н', 'о', 'п', 'р', 'с', 'т', 'у', 'ф',
            'х', 'ц', 'ч', 'ш', 'щ', 'ъ', 'ы', 'ь', 'э', 'ю', 'я'
        };

        private static List<char> usedRussianLetters = new List<char>{
            'а', 'б', 'в', 'г', 'д', 'е', 'ж', 'з', 'и', 'й', 'к', 'л', 'м', 'н', 'о', 'п', 'р', 'с', 'т', 'у', 'ф',
            'х', 'ц', 'ч', 'ш', 'щ', 'ы', 'ь', 'э', 'ю', 'я', 'ъ'
        };
        

        private static int CharToIndex(char c)
        {
            return usedRussianLetters.IndexOf(c);
        }

        private static char IndextoChar(int i)
        {
            return usedRussianLetters[i];
        }

        private static HashSet<char> usedRussianLetters_set = new HashSet<char>(usedRussianLetters);

        static List<CharInfo> GetCharInfos(string textBlock, bool verbose = false)
        {
            Dictionary<char, int> charCount = new Dictionary<char, int>();

            foreach (var character in textBlock)
                if (charCount.ContainsKey(character))
                {
                    charCount[character]++;
                }
                else
                {
                    charCount[character] = 1;
                }

            List<CharInfo> charInfos =
                charCount
                .Select(pair => new CharInfo()
                {
                    Character = pair.Key,
                    Count = pair.Value
                })
                    .Where(pair => usedRussianLetters_set.Contains(pair.Character))
                    .OrderByDescending(info => info.Count)
                    .ToList();

            double sum = charInfos.Sum(e => e.Count);

            for (int order = 0; order < charInfos.Count; order++)
            {
                charInfos[order].OrderNumber = order;
                charInfos[order].FractionPercent = 100 * charInfos[order].Count/sum;

                if (verbose)
                {
                    Console.WriteLine("Char: {0}   Count: {1},   Percentage: {2}   Order: {3}",
                        charInfos[order].Character,
                        charInfos[order].Count,
                        charInfos[order].FractionPercent,
                        order);
                }
            }

            return charInfos;
        }

        static void Main(string[] args)
        {
            // Get example text statistics

            string exampleText = File.ReadAllText("Arthur.txt", Encoding.Default);

            var exampleDistribution = GetCharInfos(exampleText, verbose: true);
            
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            Console.WriteLine();

            // Decrypt
            
            // Find r - period

            string encryptedText = File.ReadAllText("encrypted.txt", Encoding.Default);

            for (int r = 2; r < 40; r++)
            {
                Console.WriteLine("r = {0}     Dr = {1}", r, Dr(encryptedText, r));
            }

            for (int r = 2; r < 40; r++)
            {
                Console.WriteLine("r = {0}     Drr = {1}", r, Drr(encryptedText, r));
            }

            // r = 17

            int m = 32;

            int rActual = 17;

            // Build the Y blocks
            string[] Yblocks = new string[rActual];

            string[,] YblocksShifted = new string[rActual, 3];

            for (int i = 0; i < encryptedText.Length; i++)
            {
                Yblocks[i% rActual] += encryptedText[i];
            }

            // Y blocks built

            int mostFreqNum = 3;

            List<char> actualMostFreq = new List<char>(new char[] { 'о', 'е', 'а' });

            // calculate frequencies for each block
            for (int blockI = 0; blockI < rActual; blockI++)
            {
                for (int freqInd = 0; freqInd < actualMostFreq.Count; freqInd++)
                {
                    var blockDistrib = GetCharInfos(Yblocks[blockI]);

                    var orderNumberInTermsOfOriginalTableForMostFrequentCharacter =
                        CharToIndex(blockDistrib[0].Character);

                    var actualMostFrequent = actualMostFreq[freqInd];

                    var originalTextIndex = CharToIndex(actualMostFrequent);

                    int CeasarShift = (orderNumberInTermsOfOriginalTableForMostFrequentCharacter - originalTextIndex) % m;

                    string s = string.Empty;

                    for (int i = 0; i < Yblocks[blockI].Length; i++)
                    {
                        // replace with character shifted by CeasarShift

                        int alphaBeticalOrder = CharToIndex(Yblocks[blockI][i]);

                        int shiftedAlphabeticalOrder = (m + alphaBeticalOrder - CeasarShift) % m;

                        char replacementCharacter = IndextoChar(shiftedAlphabeticalOrder);

                        s += replacementCharacter;
                    }

                    YblocksShifted[blockI, freqInd] = s;

                    // Console.WriteLine("[{0}]", s.Substring(0, 6));
                }
            }

            // first col to row
            for (int blInd = 0; blInd < rActual; blInd++)
            {
                for (int freqInd = 0; freqInd < actualMostFreq.Count; freqInd++)
                {
                    Console.Write("{0} | ", YblocksShifted[blInd, freqInd][0]);
                }

                Console.WriteLine();
            }

            Console.ReadKey();
        }

        static int Dr(string encryptedText, int r)
        {
            int dr = 0;

            int n = encryptedText.Length;

            for (int i = 0; i < n - r; i++)
            {
                dr += (encryptedText[i] == encryptedText[i + r]) ? 1 : 0;
            }

            return dr;
        }

        static int Drr(string encryptedText, int r)
        {
            int dr = 0;

            int n = encryptedText.Length;

            for (int i = 0; i < n - r - 1; i++)
            {
                dr += (encryptedText.Substring(i, 2) == encryptedText.Substring(i + r, 2)) ? 1 : 0;
            }

            return dr;
        }
    }
}
