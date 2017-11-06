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


        //abcdefghijklmnopqrstuvwxyz
        private static List<char> usedEnglish = new List<char>()
        {
            'a',   'b',   'c',   'd',   'e',   'f',   'g',   'h',   'i',   'j',   'k',  'l', 'm', 'n', 'o', 'p' ,  'q' ,  'r',   's',  't',  'u','v',   'w',   'x',   'y',  'z'
        };
        
        /*
         G 6
         R 17
         N 13
         A 0
         F 5
         B 1
         V 21
        */

        /*
         E 4
         A 0
         R 17
         I 8
         O 14
         T 19
         N 13
        */

        private static int CharToIndex(char c)
        {
            return usedEnglish.IndexOf(c);
        }

        private static char IndextoChar(int i)
        {
            return usedEnglish[i];
        }

        private static HashSet<char> usedLetters_set = new HashSet<char>(usedEnglish);

        static List<CharInfo> GetCharInfos(string textBlock, bool verbose = false)
        {
            Dictionary<char, int> charCount = new Dictionary<char, int>();

            foreach (var letter in usedLetters_set)
            {
                charCount[letter] = 0;
            }

            foreach (var character in textBlock)
                if (charCount.ContainsKey(character))
                {
                    charCount[character]++;
                }
                else
                {
                    throw new Exception("That's impossible");
                }

            List<CharInfo> charInfos =
                charCount
                .Select(pair => new CharInfo()
                {
                    Character = pair.Key,
                    Count = pair.Value
                })
                    .Where(pair => usedLetters_set.Contains(pair.Character))
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
            string encryptedText = File.ReadAllText("encrypted.txt", Encoding.Default);

            string encryptedTextEng = File.ReadAllText("encryptedEng.txt", Encoding.Default);


            // Get example text statistics

            /*
            string exampleText = File.ReadAllText(encryptedTextEng, Encoding.Default);

            var exampleDistribution = GetCharInfos(exampleText, verbose: true);
            
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            Console.WriteLine();
            */

            // Decrypt
            
            // Find r - period


            encryptedText = encryptedTextEng;

            for (int r = 2; r < 40; r++)
            {
                Console.WriteLine("r = {0}     Dr = {1}", r, Dr(encryptedText, r));
            }

            for (int r = 2; r < 50; r++)
            {
                Console.WriteLine("r = {0}     Eng Dr = {1}", r, Dr(encryptedTextEng, r));
            }

            // r = 17

            int m = 26;

            int rActual = 15;

            // Y blocks built

            int mostFreqNum = 3;

            List<char> actualMostFreqRus = new List<char>(new char[] { 'о', 'е', 'а' });

            List<char> actualMostFreq = new List<char>(new char[] { 'e', 'a', 'r', 'i' });

            // Build the Y blocks
            string[] Yblocks = new string[rActual];

            string[,] YblocksShifted = new string[rActual, actualMostFreq.Count];

            for (int i = 0; i < encryptedText.Length; i++)
            {
                Yblocks[i % rActual] += encryptedText[i];
            }

            

            // calculate frequencies for each block
            for (int blockI = 0; blockI < rActual; blockI++)
            {
                for (int freqInd = 0; freqInd < actualMostFreq.Count; freqInd++)
                {
                    var blockDistrib = GetCharInfos(Yblocks[blockI]);

                    var orderNumberInTermsOfOriginalTableForMostFrequentCharacter =
                        CharToIndex(blockDistrib[0 + freqInd].Character);

                    // freqInd = 0
                    var actualMostFrequent = actualMostFreq[0];

                    var originalTextIndex = CharToIndex(actualMostFrequent);

                    int CeasarShift = (orderNumberInTermsOfOriginalTableForMostFrequentCharacter - originalTextIndex) % m;  // 13 or -13

                    string s = string.Empty;

                    for (int i = 0; i < Yblocks[blockI].Length; i++)
                    {
                        // replace with character shifted by CeasarShift

                        int alphaBeticalOrder = CharToIndex(Yblocks[blockI][i]);

                        int shiftedAlphabeticalOrder = (alphaBeticalOrder - CeasarShift) % m;

                        if (shiftedAlphabeticalOrder < 0)
                        {
                            shiftedAlphabeticalOrder += m;
                        } 

                        char replacementCharacter = IndextoChar(shiftedAlphabeticalOrder);

                        s += replacementCharacter;
                    }

                    YblocksShifted[blockI, freqInd] = s;

                    Console.WriteLine("Block №{0}: {1}, order current: {2}, order orig: {3}", blockI, CeasarShift, orderNumberInTermsOfOriginalTableForMostFrequentCharacter, originalTextIndex);
                }
            }

            // first col to row
            for (int columnInd = 0; columnInd < 5; columnInd++)
            for (int blInd = 0; blInd < rActual; blInd++)
            {
                for (int freqInd = 0; freqInd < actualMostFreq.Count; freqInd++)
                {
                    Console.Write("{0} | ", YblocksShifted[blInd, freqInd][columnInd]);
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
