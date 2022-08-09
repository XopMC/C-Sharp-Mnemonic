using Cryptography.ECDSA;
using Epoche;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace Generator_Mnemonics
{
    public enum WordListLanguage
    {
        ChineseSimplified,
        ChineseTraditional,
        English,
        French,
        Italian,
        Japanese,
        Korean,
        Spanish
    }
    class Program
    {
        private static string[] _english;
        private static string[] lang;
        private static string[] _chineseSimplified;
        private static string[] _chineseTraditional;
        private static string[] _french;
        private static string[] _italian;
        private static string[] _japanese;
        private static string[] _korean;
        private static string[] _spanish;
        private const string InvalidMnemonic = "Invalid mnemonic";
        private const string InvalidEntropy = "Invalid entropy";
        private const string InvalidChecksum = "Invalid mnemonic checksum";
        private const string salt = "mnemonic";
        private const string bitcoinSeed = "Bitcoin seed";
        private static string PATH1, filePath, language;
        private static string prefix_legacy = "00";
        private static string prefix_segwit = "05";
        private static string hardened = "";
        private static List<string> PATH = new List<string>();
        private static readonly BigInteger order = BigInteger.Parse("115792089237316195423570985008687907852837564279074904382605163141518161494337");
        private static long Total, Found = 0;
        private static BigInteger IncrementalSearch, Entropy = 0;
        private static BigInteger Step, EntropyStep = 1;
        private static bool Silent = true;
        private static int processorCount = Environment.ProcessorCount;
        private static int num = 1;
        private static int words = 12;
        private static long derivation = 0;
        private static int mode = 1;
        private static WordListLanguage Language_list;
        private static bool IsRunning = true;
        private static HashSet<string> _addressDb;
        private static double cur, speed, Constant;
        private static Stopwatch sw = new Stopwatch();
        private static readonly object outFileLock = new();
        private static bool IncrementalEntropy = false;
        private static string EntropyLength;


        //static DateTime t1, t2;

        static void Main(string[] args)
        {
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                Constant = 1000000000;
            }
            else
            {
                Constant = 10000000;
            }

            if (args.Length < 1)
            {
                Console.OutputEncoding = Encoding.UTF8;
                Console.WriteLine("Not enough args! | Нет параметров запуска, отдыхай и учи матчасть!\n");
                Console.ReadLine();
                Environment.Exit(0);
            }
            else
            {
                //PATH = "/44'/0";
                //PATH1 = "";
                if ((args.Length == 1) && (args[0] != "-help") && (args[0] != "-h"))
                {
                    filePath = args[0];
                    Console.OutputEncoding = Encoding.UTF8;
                    mode = 0;
                    words = 12;
                    PATH.Add("44'/0'/0'/0");
                    PATH.Add("44'/60'/0'/0");
                    prefix_legacy = "00";
                    prefix_segwit = "05";
                    PATH1 = "";
                    derivation = 10;
                    language = "EN";
                    Console.WriteLine("Input number of cores | Введи кол-во ядер");
                    num = Convert.ToInt32(Console.ReadLine());
                    if (num > processorCount)
                    {
                        num = processorCount;
                    }
                    Console.WriteLine("Input '1' for Silent mode or '2' for Debug mode | Введи '1' для Тихого режима или '2' для режима Дебаг");
                    var re = Convert.ToInt32(Console.ReadLine());
                    if (re == 2)
                    { Silent = false; }
                    else if (re == 1)
                    { Silent = true; }
                    else
                    {
                        Console.OutputEncoding = Encoding.UTF8;
                        Console.WriteLine("Wrong number! Starting Silent mode | Некорректный номер, запуская Тихий режим! ");
                        Silent = true;
                    }
                }
                for (int i = 0; i < args.Length; i++)
                {
                    if ((args[i] == "-help") || (args[i] == "-h"))
                    {
                        Console.WriteLine("\n\nDeveloped by @XopMC for https://t.me/brythbit \n\n");
                        Console.WriteLine(" ___________________________________________________________________");
                        Console.WriteLine("|###################################################################|");
                        Console.WriteLine("|#                                                                 #|");
                        Console.WriteLine("|#  ##       ##                    ##         ##      ########     #|");
                        Console.WriteLine("|#   ##     ##                     ###      ## ##    ###    ###    #|");
                        Console.WriteLine("|#    ##   ##                      ## ##  ##   ##   ##             #|");
                        Console.WriteLine("|#     ## ##                       ##   ##     ##   ##             #|");
                        Console.WriteLine("|#      ###         ###    #####   ##          ##   ##             #|");
                        Console.WriteLine("|#     ## ##      ##   ##  ##  ##  ##          ##   ##             #|");
                        Console.WriteLine("|#    ##   ##     ##   ##  #####   ##          ##    ##            #|");
                        Console.WriteLine("|#   ##     ##    ##   ##  ##      ##          ##     ###   ###    #|");
                        Console.WriteLine("|#  ##       ##     ###    ##      ##          ##      ######      #|");
                        Console.WriteLine("|#                                                                 #|");
                        Console.WriteLine("|###################################################################|");
                        Console.WriteLine("\nUsage: | Использование:\n-h \t-help\t\tShow this help | Показывает справку");
                        //Console.WriteLine("-bip39 \tpassword\t\tBIP39 Passphrase | Кодовая фраза BIP39");
                        Console.WriteLine("-debug\tDebug mode | Режим полного отображения");
                        Console.WriteLine("-d \tnumber\t derivation deep | Глубина деривации");
                        Console.WriteLine("-i \tfile\t filename with addresses | Имя файла с адресами");
                        Console.WriteLine("-hard\tto hardened addresses | Для hardened адресов");
                        Console.WriteLine("-m \tmode\t 0 - BTC+ETH,  1 - BTC, 2 - ETH, 3 - HASH160, 4 - Public Keys (for any Cryptocurrency), 5 - Private keys (for brainflayer), 100 - Valid Mnemonic Generator ");
                        Console.WriteLine("-entropy \tHEX\t Input (8 , 16, 24 ,32 ,40 ,48 ,56 ,64) hex symbols for entoropy incremental search \n Введите (8 , 16, 24 ,32 ,40 ,48 ,56 ,64) HEX символов для перебора энтропии по порядку.");
                        Console.WriteLine("-step \tnumber\t (Only for -entropy) Step for incremental entropy search | (Только для -entropy) Шаг для перебора энтропии по порядку");
                        Console.WriteLine("Additional modes | Дополнительные режимы: \n 6 - Bitcoin Cash\n 7 - Bitcoin Diamond\n 8 - Bitcoin SV\n 9 - ILCoin\n 10 - Tether");
                        Console.WriteLine(" 11 - CryptoVerificationCoin\n 12 - Litecoin Cash\n 13 - Zcash\n 14 - DigiByte\n 15 - Dogecoin\n 16 - PIVX\n 17 - Verge\n 18 - Horizen ");
                        Console.WriteLine(" 19 - Einsteinium\n 20 - Groestlcoin\n 21 - Bitcoin Gold\n 22 - Litecoin\n 23 - MonaCoin\n 24 - ImageCoin\n 25 - NavCoin\n 26 - Neblio ");
                        Console.WriteLine(" 27 - Axe\n 28 - Peercoin\n 29 - Particl\n 30 - Qtum\n 31 - Komodo\n 32 - Ravencoin\n 33 - Reddcoin\n 34 - SafeInsure\n 35 - SmartCash ");
                        Console.WriteLine(" 36 - Stratis\n 37 - Syscoin\n 38 - Vertcoin\n 39 - Viacoin\n 40 - Beetle Coin\n 41 - Dash\n 42 - Xenios\n 43 - Zcoin.");
                        Console.WriteLine("-n \tnumber\t amount of keys for incremental search (+ and -) | Кол-во ключей для инкрементального поиска (+ и -)");
                        Console.WriteLine("-k \tnumber\t step for -n (for incremental search) | Шаг для инкрементального поиска");
                        Console.WriteLine("-t \tthreads\t threads number | количество потоков");
                        Console.WriteLine("-P \tPATH BTC\t Derivation PATH, If not specified -  44'/0'/0'/0/0 | Путь деривации, если не указан будет -  44'/0'/0'/0/0");
                        Console.WriteLine("\tPATH ETH\t Derivation PATH, If not specified -  44'/60'/0'/0/0 | Путь деривации, если не указан будет -  44'/60'/0'/0/0");
                        Console.WriteLine("-PATH \tfile\t path to file with derivations | Путь к файлу с деривациями");
                        Console.WriteLine("-PLUS\tUsing included most popular Derivation path's | Использовать набор вшитых самых популярных путей деривации");
                        Console.WriteLine("-w \tnumber\t 3,6,9,12,15,18,21,24 - number of words for mnemonics | Количество слов для мнемоники");
                        Console.WriteLine("-lang \tLanguage\tEN, CT, CS, KO, JA, IT, FR, SP - mnemonic language \t English, ChineseTraditional, ChineseSimplified, Korean , Japanese, Italian, French, Spanish | Языки для мнемоники");
                        Console.WriteLine("\n\nDeveloped by @XopMC for https://t.me/brythbit \n\n");
                        Console.ReadLine();
                        Environment.Exit(0);

                        //Legacy address: | SegWit base58:
                        //00 — Bitcoin | 05 — Bitcoin     1
                        // 00 — Bitcoin Cash | 05 —  Bitcoin Cash    6  m/44'/145'/0'/0
                        // 00 — Bitcoin Diamond | 05 —  Bitcoin Diamond    7  
                        // 00 — Bitcoin SV | 05 — Bitcoin SV    8
                        // 00 — ILCoin | 05 — ILCoin    9
                        // 00 — Tether | 05 — Tether    10
                        // 1C — CryptoVerificationCoin | ---11
                        // 1C — Litecoin Cash | 32 — Litecoin Cash     12
                        // 1CB8 — Zcash | 1CBD — Zcash     13
                        // 1E — DigiByte | 3F — DigiByte     14
                        // 1E — Dogecoin | 16 — Dogecoin     15
                        // 1E — PIVX | ---16
                        // 1E — Verge | ---17
                        // 2089 — Horizen | ---18
                        // 21 — Einsteinium | 37 — Einsteinium      19
                        // 24 — Groestlcoin | 05 - Groestlcoin      20
                        // 26 — Bitcoin Gold | 17 — Bitcoin Gold       21
                        // 30 — Litecoin | 32 — Litecoin       22
                        // 32 — MonaCoin | ---23
                        // 33 — ImageCoin | ---24
                        // 35 — NavCoin | ---25
                        // 35 — Neblio | ---26
                        // 37 — Axe | 10 — Axe     27
                        // 37 — Peercoin | 75 — Peercoin    28
                        // 38 — Particl | 3C — Particl    29
                        // 3A — Qtum | 32 — Qtum    30
                        // 3C — Komodo | ---31
                        // 3C — Ravencoin | ---32
                        // 3D — Reddcoin | ---33
                        // 3F — SafeInsure | ---34
                        // 3F — SmartCash | 12 — SmartCash   35
                        // 3F — Stratis | 7D — Stratis    36
                        // 3F — Syscoin | ---37
                        // 47 — Vertcoin | ---38
                        // 47 — Viacoin | 21 — Viacoin    39
                        // 4B — Beetle Coin | 40
                        // 4C — Dash | 10 — Dash    41
                        // 4C — Xenios | ---42
                        // 52 — Zcoin | 07 — Zcoin     43

                    }
                    if (args[i] == "-i")
                    {
                        if (i + 1 >= args.Length)
                        {
                            Console.WriteLine("Missing base | Нет базы, отдыхай");
                            break;
                        }
                        else
                        {
                            filePath = args[i + 1];
                        }
                    }
                    if (args[i] == "-debug")
                    {
                        Silent = false;

                    }
                    if (args[i] == "-d")
                    {
                        derivation = Convert.ToInt64(args[i + 1]);
                        if (derivation > 2147483648)
                        {
                            derivation = 2147483648;
                            Console.WriteLine("Cannot be more than 2147483648");
                        }
                    }
                    if (args[i] == "-hard")
                    {
                        hardened = "'";
                    }
                    if (args[i] == "-entropy")
                    {
                        if ((args[i+1].Length != 8) && (args[i + 1].Length != 16) && (args[i + 1].Length != 24) && (args[i + 1].Length != 32) && (args[i + 1].Length != 40) && (args[i + 1].Length != 48) && (args[i + 1].Length != 56) && (args[i + 1].Length != 64))
                        {
                            IncrementalEntropy = false;
                            Console.WriteLine("Entered hex are incorrect length (need 8 , 16, 24 ,32 ,40 ,48 ,56 ,64 hex symbols) | Введённый HEX имеет некорректную длинну (нужно 8 , 16, 24 ,32 ,40 ,48 ,56 ,64 hex символов)");
                            Console.WriteLine("Press Any key to start Random search | Нажми любую клавишу для рандомного поиска");
                            Console.ReadKey();
                        }
                        else
                        {
                            try
                            {
                                EntropyLength = "X" + args[i + 1].Length;
                                Entropy = BigInteger.Parse("0" + args[i + 1], System.Globalization.NumberStyles.AllowHexSpecifier);
                                IncrementalEntropy = true;
                            }
                            catch
                            {
                                IncrementalEntropy = false;
                                Console.WriteLine("Entered string are not in HEX format!!! | Введена строка не в HEX формате!!!");
                                Console.WriteLine("Press Any key to start Random search | Нажми любую клавишу для рандомного поиска");
                                Console.ReadKey();
                            }
                        }
                           
                    }
                    if (args[i] == "-m")
                    {
                        mode = Convert.ToInt32(args[i + 1]);
                        //if ((mode != 1) && (mode != 2) && (mode != 3) && (mode != 4) && (mode != 5) && (mode != 0))
                        //{
                        //    Console.WriteLine("Wrong mode! Starting with BTC mode | Некорректный режим! Запускаю режим BTC");
                        //    mode = 1;
                        //}
                    }
                    if (args[i] == "-t")
                    {
                        num = Convert.ToInt32(args[i + 1]);
                        if (num > processorCount)
                        {
                            num = processorCount;
                        }
                    }
                    if (args[i] == "-n")
                    {
                        IncrementalSearch = BigInteger.Parse(args[i + 1]);
                    }
                    if (args[i] == "-k")
                    {
                        Step = BigInteger.Parse(args[i + 1]);
                    }
                    if (args[i] == "-P")
                    {
                        PATH.Add(args[i + 1]);
                    }
                    if (args[i] == "-PATH")
                    {
                        foreach (string readLine in File.ReadLines(args[i + 1]))
                        {
                            string[] strArray = readLine.Split('\t');
                            if (strArray[0] == "")
                            {
                                continue;
                            }
                            if (strArray[0][^1..] == "/")
                            {
                                PATH.Add(strArray[0][..^1]);
                            }    
                            else
                            {
                                PATH.Add(strArray[0]);
                            }
                            
                        }
                    }
                    if (args[i] == "-PLUS")
                    {
                        PATH.Add("0'/0'");
                        PATH.Add("0'/0");
                        PATH.Add("0/0'");
                        PATH.Add("0/0");
                        PATH.Add("44'/0'/0'");
                        PATH.Add("83696968'/39'/0'/12'");
                        PATH.Add("83696968'/39'/0'/18'");
                        PATH.Add("83696968'/39'/0'/24'");
                        PATH.Add("83696968'/39'/1'/12'");
                        PATH.Add("83696968'/39'/1'/18'");
                        PATH.Add("83696968'/39'/1'/24'");
                        PATH.Add("83696968'/39'/2'/12'");
                        PATH.Add("83696968'/39'/2'/18'");
                        PATH.Add("83696968'/39'/2'/24'");
                        PATH.Add("83696968'/39'/4'/12'");
                        PATH.Add("83696968'/39'/4'/18'");
                        PATH.Add("83696968'/39'/4'/24'");
                        PATH.Add("83696968'/39'/5'/12'");
                        PATH.Add("83696968'/39'/5'/18'");
                        PATH.Add("83696968'/39'/5'/24'");
                        PATH.Add("0'");
                        PATH.Add("44'/0'/0'/0");
                        PATH.Add("44'/0'/0'/1");
                        PATH.Add("44'/0'/1'/0");
                        PATH.Add("44'/0'/1'/1");
                        PATH.Add("49'/0'/0'/0");
                        PATH.Add("49'/0'/0'/1");
                        PATH.Add("49'/0'/1'/0");
                        PATH.Add("49'/0'/1'/1");
                        PATH.Add("84'/0'/0'/0");
                        PATH.Add("84'/0'/0'/1");
                        PATH.Add("84'/0'/1'/0");
                        PATH.Add("84'/0'/1'/1");
                        PATH.Add("83696968'/0'");
                        PATH.Add("0");
                        //PATH.Add("");
                        PATH.Add("44'/0'");
                        PATH.Add("48'/0'");
                        PATH.Add("49'/0'");
                        PATH.Add("84'/0'");
                        PATH.Add("44'/0'/0'");
                        PATH.Add("48'/0'/0'/0");
                        PATH.Add("48'/0'/0'/1");
                    }
                    /*if (args[i] == "-PE")
                    {
                        PATH = args[i + 1];
                    }*/
                    /*if (args[i] == "-bip39")
                    {
                        BIP39_Passphrase = args[i + 1];
                    }*/
                    if (args[i] == "-step")
                    {
                        EntropyStep = BigInteger.Parse(args[i + 1]);
                    }
                    if (args[i] == "-XopMC")
                    {
                        int b = 0;
                        while (b != 500000)
                        {
                            Console.WriteLine("LOL\t\t you found the bug: @XopMC.");
                            b++;
                        }
                    }
                    if (args[i] == "-w")
                    {
                        words = Convert.ToInt32(args[i + 1]);

                    }
                    if (args[i] == "-lang")
                    {
                        language = args[i + 1];

                    }


                }

                switch (words)
                {
                    case 3:
                        words = 32;
                        break;
                    case 6:
                        words = 64;
                        break;
                    case 9:
                        words = 96;
                        break;
                    case 12:
                        words = 128;
                        break;
                    case 15:
                        words = 160;
                        break;
                    case 18:
                        words = 192;
                        break;
                    case 21:
                        words = 224;
                        break;
                    case 24:
                        words = 256;
                        break;
                    default:
                        words = 128;
                        Console.WriteLine("Wrong words number... starting with 12 words");
                        break;
                }

                switch (mode)
                {
                    case 1:
                    case 3:
                    case 4:
                    case 5:
                        if (!PATH.Any())
                        {
                            PATH.Add("44'/0'/0'/0");
                        }
                        break;
                    case 2:
                        if (!PATH.Any())
                        {
                            PATH.Add("44'/60'/0'/0");
                        }
                        break;
                    case 0:
                        if (!PATH.Any())
                        {
                            PATH.Add("44'/0'/0'/0");
                            PATH.Add("44'/60'/0'/0");
                        }
                        break;

                    case 6:
                        if (!PATH.Any())
                        {
                            PATH.Add("44'/145'/0'/0");
                            PATH.Add("49'/145'/0'/0");
                        }
                        break;
                    case 7:
                        if (!PATH.Any())
                        {
                            PATH.Add("44'/999'/0'/0");
                            PATH.Add("49'/999'/0'/0");
                        }
                        break;
                    case 8:
                        if (!PATH.Any())
                        {
                            PATH.Add("44'/236'/0'/0");
                            PATH.Add("49'/236'/0'/0");
                        }
                        break;

                    case 9:
                        if (!PATH.Any())
                        {
                            PATH.Add("44'/578'/0'/0");
                            PATH.Add("49'/578'/0'/0");
                        }
                        break;

                    case 10:
                        if (!PATH.Any())
                        {
                            PATH.Add("44'/0'/0'/0");
                        }
                        break;
                    case 11:
                        if (!PATH.Any())
                        {
                            PATH.Add("44'/0'/0'/0");
                        }
                        prefix_legacy = "1C";
                        prefix_segwit = "";
                        break;
                    case 12:
                        if (!PATH.Any())
                        {
                            PATH.Add("44'/192'/0'/0");
                            PATH.Add("49'/192'/0'/0");
                        }
                        prefix_legacy = "1C";
                        prefix_segwit = "32";
                        break;
                    case 13:
                        if (!PATH.Any())
                        {
                            PATH.Add("44'/133'/0'/0");
                            PATH.Add("49'/133'/0'/0");
                        }
                        prefix_legacy = "1CB8";
                        prefix_segwit = "1CBD";
                        break;
                    case 14:
                        if (!PATH.Any())
                        {
                            PATH.Add("44'/20'/0'/0");
                            PATH.Add("49'/20'/0'/0");
                        }
                        prefix_legacy = "1E";
                        prefix_segwit = "3F";
                        break;
                    case 15:
                        if (!PATH.Any())
                        {
                            PATH.Add("44'/3'/0'/0");
                            PATH.Add("49'/3'/0'/0");
                        }
                        prefix_legacy = "1E";
                        prefix_segwit = "16";
                        break;
                    case 16:
                        if (!PATH.Any())
                        {
                            PATH.Add("44'/119'/0'/0");
                        }
                        prefix_legacy = "1E";
                        prefix_segwit = "";
                        break;
                    case 17:
                        if (!PATH.Any())
                        {
                            PATH.Add("44'/77'/0'/0");
                        }
                        prefix_legacy = "1E";
                        prefix_segwit = "";
                        break;
                    case 18:
                        if (!PATH.Any())
                        {
                            PATH.Add("44'/0'/0'/0");
                        }
                        prefix_legacy = "2089";
                        prefix_segwit = "";
                        break;
                    case 19:
                        if (!PATH.Any())
                        {
                            PATH.Add("44'/41'/0'/0");
                            PATH.Add("49'/41'/0'/0");
                        }
                        prefix_legacy = "21";
                        prefix_segwit = "37";
                        break;
                    case 20:
                        if (!PATH.Any())
                        {
                            PATH.Add("44'/17'/0'/0");
                            PATH.Add("49'/17'/0'/0");
                        }
                        prefix_legacy = "24";
                        prefix_segwit = "05";
                        break;
                    case 21:
                        if (!PATH.Any())
                        {
                            PATH.Add("44'/156'/0'/0");
                            PATH.Add("49'/156'/0'/0");
                        }
                        prefix_legacy = "26";
                        prefix_segwit = "17";
                        break;
                    case 22:
                        if (!PATH.Any())
                        {
                            PATH.Add("44'/2'/0'/0");
                            PATH.Add("49'/2'/0'/0");
                        }
                        prefix_legacy = "30";
                        prefix_segwit = "32";
                        break;
                    case 23:
                        if (!PATH.Any())
                        {
                            PATH.Add("44'/22'/0'/0");
                            //PATH.Add("49'/2'/0'/0");
                        }
                        prefix_legacy = "32";
                        prefix_segwit = "";
                        break;
                    case 24:
                        if (!PATH.Any())
                        {
                            PATH.Add("44'/388'/0'/0");
                            //PATH.Add("49'/2'/0'/0");
                        }
                        prefix_legacy = "33";
                        prefix_segwit = "";
                        break;
                    case 25:
                        if (!PATH.Any())
                        {
                            PATH.Add("44'/130'/0'/0");
                            //PATH.Add("49'/2'/0'/0");
                        }
                        prefix_legacy = "35";
                        prefix_segwit = "";
                        break;
                    case 26:
                        if (!PATH.Any())
                        {
                            PATH.Add("44'/146'/0'/0");
                            //PATH.Add("49'/2'/0'/0");
                        }
                        prefix_legacy = "35";
                        prefix_segwit = "";
                        break;
                    case 27:
                        if (!PATH.Any())
                        {
                            PATH.Add("44'/4242'/0'/0");
                            PATH.Add("49'/4242'/0'/0");
                        }
                        prefix_legacy = "37";
                        prefix_segwit = "10";
                        break;
                    case 28:
                        if (!PATH.Any())
                        {
                            PATH.Add("44'/6'/0'/0");
                            PATH.Add("49'/6'/0'/0");
                        }
                        prefix_legacy = "37";
                        prefix_segwit = "75";
                        break;
                    case 29:
                        if (!PATH.Any())
                        {
                            PATH.Add("44'/44'/0'/0");
                            PATH.Add("49'/44'/0'/0");
                        }
                        prefix_legacy = "38";
                        prefix_segwit = "3C";
                        break;
                    case 30:
                        if (!PATH.Any())
                        {
                            PATH.Add("44'/2301'/0'/0");
                            PATH.Add("49'/2301'/0'/0");
                        }
                        prefix_legacy = "3A";
                        prefix_segwit = "32";
                        break;
                    case 31:
                        if (!PATH.Any())
                        {
                            PATH.Add("44'/141'/0'/0");
                            //PATH.Add("49'/2'/0'/0");
                        }
                        prefix_legacy = "3C";
                        prefix_segwit = "";
                        break;
                    case 32:
                        if (!PATH.Any())
                        {
                            PATH.Add("44'/175'/0'/0");
                            //PATH.Add("49'/2'/0'/0");
                        }
                        prefix_legacy = "3C";
                        prefix_segwit = "";
                        break;
                    case 33:
                        if (!PATH.Any())
                        {
                            PATH.Add("44'/4'/0'/0");
                            //PATH.Add("49'/2'/0'/0");
                        }
                        prefix_legacy = "3D";
                        prefix_segwit = "";
                        break;
                    case 34:
                        if (!PATH.Any())
                        {
                            PATH.Add("44'/'/0'/0");
                            //PATH.Add("49'/2'/0'/0");
                        }
                        prefix_legacy = "3F";
                        prefix_segwit = "";
                        break;
                    case 35:
                        if (!PATH.Any())
                        {
                            PATH.Add("44'/224'/0'/0");
                            PATH.Add("49'/224'/0'/0");
                        }
                        prefix_legacy = "3F";
                        prefix_segwit = "12";
                        break;
                    case 36:
                        if (!PATH.Any())
                        {
                            PATH.Add("44'/105'/0'/0");
                            PATH.Add("49'/105'/0'/0");
                        }
                        prefix_legacy = "3F";
                        prefix_segwit = "7D";
                        break;
                    case 37:
                        if (!PATH.Any())
                        {
                            PATH.Add("44'/57'/0'/0");
                            //PATH.Add("49'/2'/0'/0");
                        }
                        prefix_legacy = "3F";
                        prefix_segwit = "";
                        break;
                    case 38:
                        if (!PATH.Any())
                        {
                            PATH.Add("44'/28'/0'/0");
                            //PATH.Add("49'/2'/0'/0");
                        }
                        prefix_legacy = "47";
                        prefix_segwit = "";
                        break;
                    case 39:
                        if (!PATH.Any())
                        {
                            PATH.Add("44'/14'/0'/0");
                            PATH.Add("49'/14'/0'/0");
                        }
                        prefix_legacy = "47";
                        prefix_segwit = "21";
                        break;
                    case 40:
                        if (!PATH.Any())
                        {
                            PATH.Add("44'/800'/0'/0");
                            //PATH.Add("49'/2'/0'/0");
                        }
                        prefix_legacy = "4B";
                        prefix_segwit = "";
                        break;
                    case 41:
                        if (!PATH.Any())
                        {
                            PATH.Add("44'/5'/0'/0");
                            PATH.Add("49'/5'/0'/0");
                        }
                        prefix_legacy = "4C";
                        prefix_segwit = "10";
                        break;
                    case 42:
                        if (!PATH.Any())
                        {
                            PATH.Add("44'/0'/0'/0");
                            //PATH.Add("49'/2'/0'/0");
                        }
                        prefix_legacy = "4C";
                        prefix_segwit = "";
                        break;
                    case 43:
                        if (!PATH.Any())
                        {
                            PATH.Add("44'/136'/0'/0");
                            PATH.Add("49'/136'/0'/0");
                        }
                        prefix_legacy = "52";
                        prefix_segwit = "07";
                        break;


                }


                if ((mode != 5) && (mode != 100))
                {
                    Console.WriteLine("Loading addresses from {0}  | Загружаю адреса из {0} \n", filePath);
                    _addressDb = LoadDatabase(filePath);
                    Console.WriteLine("\nStarting {0} threads... | Запускаю {0} поток(а)ов...\n", num);
                    Console.WriteLine("Start!");
                }
                List<Thread> threadList = new();
                for (int index = 0; index < num; ++index)
                {
                    Thread thread;
                    if (mode == 1)
                    {
                        thread = new(new ThreadStart(WorkerThread));
                        threadList.Add(thread);
                    }
                    else if (mode == 2)
                    {
                        thread = new(new ThreadStart(WorkerThread2));
                        threadList.Add(thread);
                    }
                    else if (mode == 3)
                    {
                        thread = new(new ThreadStart(WorkerThread3));
                        threadList.Add(thread);
                    }
                    else if (mode == 4)
                    {
                        thread = new(new ThreadStart(WorkerThread4));
                        threadList.Add(thread);
                    }
                    else if (mode == 5)
                    {
                        thread = new(new ThreadStart(WorkerThread5));
                        threadList.Add(thread);
                    }
                    else if (mode == 0)
                    {
                        thread = new(new ThreadStart(WorkerThread0));
                        threadList.Add(thread);
                    }
                    else if (mode == 100)
                    {
                        thread = new(new ThreadStart(WorkerThreadMnemonicGen));
                        threadList.Add(thread);
                    }
                    else
                    {
                        thread = new(new ThreadStart(WorkerThread));
                        threadList.Add(thread);
                    }
                    //Thread thread = new(new ThreadStart(WorkerThread));

                }
                IsRunning = true;
                foreach (Thread thread in threadList)
                    thread.Start();
                //list path = []

                //Console.ReadKey();
                var seconds = 0;
                if ((Silent == true) && (!(mode == 5)) && (!(mode == 100)))
                {
                    while (IsRunning)
                    {
                        if ((seconds == 60) && IncrementalEntropy)
                        {
                            File.WriteAllText("SAVED_ENTROPY.txt", "Saved last entropy: " + Entropy.ToString(EntropyLength));
                            seconds = 0;
                        }
                        double Elapsed_MS = sw.ElapsedTicks;
                        cur = Elapsed_MS / Constant;
                        cur = Math.Round(cur);
                        speed = Total / (Elapsed_MS / Constant);
                        speed = Math.Round(speed);
                        //Console.Write("\r" + new string(' ', Console.WindowWidth - 1) + "\r");
                        Console.Write("\r| Total: {2} | Elapsed: {1} | Found: {3} | Speed: {0} Keys/s ", speed, cur, Total, Found);
                        Thread.Sleep(1000);
                        seconds++;
                    }
                }
                else if ((Silent == false) && IncrementalEntropy)
                {
                    while (IsRunning)
                    {
                        Thread.Sleep(1000);
                        seconds++;
                        if ((seconds == 60))
                        {
                            File.WriteAllText("SAVED_ENTROPY.txt", "Saved last entropy: " + Entropy.ToString(EntropyLength));
                            seconds = 0;
                        }
                    }
                }
                foreach (Thread thread in threadList)
                    thread.Join();


            }
        }

        private static void WorkerThread0()
        {
            Console.OutputEncoding = Encoding.UTF8;
            //int processorCount = Environment.ProcessorCount;
            //long Total = 0;
            InitializationWordList();
            //var lang = _english;
            //var Language_list = WordListLanguage.ChineseTraditional;
            switch (language)
            {
                case "EN":
                    {
                        lang = _english;
                        Language_list = WordListLanguage.English;
                        break;
                    }
                case "CS":
                    {
                        lang = _chineseSimplified;
                        Language_list = WordListLanguage.ChineseSimplified;
                        break;
                    }
                case "CT":
                    {
                        lang = _chineseTraditional;
                        Language_list = WordListLanguage.ChineseTraditional;
                        break;
                    }
                case "FR":
                    {
                        lang = _french;
                        Language_list = WordListLanguage.French;
                        break;
                    }
                case "IT":
                    {
                        lang = _italian;
                        Language_list = WordListLanguage.Italian;
                        break;
                    }
                case "JA":
                    {
                        lang = _japanese;
                        Language_list = WordListLanguage.Japanese;
                        break;
                    }
                case "KO":
                    {
                        lang = _korean;
                        Language_list = WordListLanguage.Korean;
                        break;
                    }
                case "SP":
                    {
                        lang = _spanish;
                        Language_list = WordListLanguage.Spanish;
                        break;
                    }
                default:
                    {
                        lang = _english;
                        Language_list = WordListLanguage.English;
                        break;
                    }
            }
            sw.Start();
            while (true)
            {
                //Шаг 1. Получаем энтропию.
                byte[] seedBytes;
                if (IncrementalEntropy)
                {
                    seedBytes = StringToByteArray(Entropy.ToString(EntropyLength));
                    Entropy = Entropy + EntropyStep;
                }
                else
                {
                    seedBytes = GenerateMnemonicBytes(words);
                }

                //var lang = _english;
                //var Language_list = WordListLanguage.ChineseTraditional;
                //var seed = EntropyToMnemonic(seedBytes, _english, WordListLanguage.English);
                var seed = EntropyToMnemonic(seedBytes, lang, Language_list);
                //var seed = "edge shift acquire essence sniff ankle ten prevent december drama churn feel shed ring pair curve biology ability equal cherry yellow blush abuse drift";
                //Console.WriteLine($"Seed: {seed}");
                var saltByte = Encoding.UTF8.GetBytes(salt);
                var masterSecret = Encoding.UTF8.GetBytes(bitcoinSeed);
                var BIP39SeedByte = new Rfc2898DeriveBytes(seed, saltByte, 2048, HashAlgorithmName.SHA512).GetBytes(64);
                //var BIP39Seed = BytesToHexString(BIP39SeedByte);
                var masterPrivateKey = new byte[32]; // Master private key
                var masterChainCode = new byte[32]; // Master chain code
                //Console.WriteLine($"BIP39Seed: {BIP39Seed}");
                var hmac = new HMACSHA512(masterSecret);
                var i = hmac.ComputeHash(BIP39SeedByte);
                Buffer.BlockCopy(i, 0, masterPrivateKey, 0, 32);
                Buffer.BlockCopy(i, 32, masterChainCode, 0, 32);
                //Console.WriteLine($"Master Private Key: {BytesToHexString(masterPrivateKey)}");
                //Console.WriteLine($"Master Chain Code: {BytesToHexString(masterChainCode)}");
                foreach (var path in PATH)
                {
                    int a = 0;
                    while (a <= derivation)
                    {
                        string DER_PATH = path + '/' + a + hardened;
                        //Console.WriteLine(DER_PATH);
                        byte[] PrivateKey = GetChildKey(masterPrivateKey, masterChainCode, DER_PATH);
                        var range = BytesToHexString(PrivateKey).Length;
                        if (range != 64)
                        {
                            while (range != 64)
                            {
                                //Console.WriteLine("D!");
                                string PVK64 = BytesToHexString(PrivateKey);
                                //Console.WriteLine(PVK64);
                                PrivateKey = StringToByteArray("00" + PVK64);
                                //Console.WriteLine("Проверка ключа ппосле добвки нулей " + BytesToHexString(PrivateKey));
                                range++;
                                range++;
                            }
                        }
                        if (IncrementalSearch != 0)
                        {
                            long n = 0;
                            BigInteger PrivateDEC = BigInteger.Parse("0" + BytesToHexString(PrivateKey), System.Globalization.NumberStyles.AllowHexSpecifier);
                            while (n <= IncrementalSearch)
                            {
                                //Считаем Приватник + N число
                                var PrivateDEC1 = PrivateDEC - (n * Step);
                                var PrivateDEC0 = PrivateDEC + (n * Step);
                                //Console.WriteLine(PrivateDEC.ToString("X32"));
                                var PrivateHEX1 = PrivateDEC1.ToString("X32");
                                var PrivateHEX0 = PrivateDEC0.ToString("X32");

                                //Console.WriteLine(PrivateDEC0.ToString("X32"));
                                //Console.WriteLine(PrivateDEC1.ToString("X32"));
                                range = PrivateDEC.ToString("X32").Length;
                                if (range < 64)
                                {
                                    while (range < 64)
                                    {
                                        PrivateHEX0 = '0' + PrivateHEX0;
                                        PrivateHEX1 = '0' + PrivateHEX1;
                                        range++;
                                    }
                                }
                                else if (range > 64)
                                {
                                    PrivateHEX0 = PrivateHEX0[^64..];
                                    PrivateHEX1 = PrivateHEX1[^64..];
                                }
                                var PrivateKeyCHILD = StringToByteArray(PrivateHEX0);
                                var Public_key_compressed = Secp256K1Manager.GetPublicKey(PrivateKeyCHILD, true);
                                var Public_key_uncompressed = Secp256K1Manager.GetPublicKey(PrivateKeyCHILD, false);
                                var address_compressed = GetAddress(Public_key_compressed);
                                var address_uncompressed = GetAddress(Public_key_uncompressed);
                                //var address_compressed = GetHash160(Public_key_compressed);
                                //var address_uncompressed = GetHash160(Public_key_uncompressed);
                                
                                var eth_adr = GetEthAddress(Public_key_uncompressed);
                                Interlocked.Increment(ref Total);

                                string address_segwit;
                                bool flag1;
                                address_segwit = GetSegWit_base58(Public_key_compressed);
                                flag1 = HasBalance(address_segwit);
                                
                                

                                bool flag = HasBalance(address_compressed);
                                bool flag0 = HasBalance(address_uncompressed);
                                
                                bool flag2 = HasBalance(eth_adr);

                                if ((flag != false) || (flag0 != false) || (flag1 != false) || (flag2 != false))
                                {
                                    Console.WriteLine(address_compressed + "\n" + address_uncompressed + "\n" + address_segwit + "\n0x" + eth_adr + "\nmnemonic: " + seed + "\nPrivate Key: " + BytesToHexString(PrivateKeyCHILD) + "   N = +" + (n*Step).ToString("X8") + "\nEntropy: " + BytesToHexString(seedBytes) + "\nDer.PATH: " + DER_PATH + "\nTotal: " + Total + " Found: " + Found + " Speed: " + speed + '\n');
                                    string contents = string.Format($" \n\nAddresses: \n{address_compressed}  Balance: {flag}  \n{address_uncompressed}  Balance: {flag0}  \n{address_segwit}  :  Balance: {flag1} \n0x{eth_adr}  :  Balance: {flag2} \nMnemonic phrase: {seed} \nPrivate Key: {BytesToHexString(PrivateKeyCHILD)} \nEntropy: {BytesToHexString(seedBytes)} \nDerivation PATH: {DER_PATH}");
                                    object outFileLock = Program.outFileLock;
                                    bool lockTaken = false;
                                    try
                                    {
                                        Monitor.Enter(outFileLock, ref lockTaken);
                                        File.AppendAllText("FOUND.txt", contents);
                                    }
                                    finally
                                    {
                                        if (lockTaken)
                                            Monitor.Exit(outFileLock);
                                    }
                                    Interlocked.Increment(ref Found);

                                }
                                else
                                {
                                    if (Silent == false)
                                    {
                                        double Elapsed_MS = sw.ElapsedTicks;
                                        cur = Elapsed_MS / Constant;
                                        cur = Math.Round(cur);
                                        speed = Total / (Elapsed_MS / Constant);
                                        speed = Math.Round(speed);
                                        Console.WriteLine(address_compressed + "\n" + address_uncompressed + "\n" + address_segwit + "\n0x" + eth_adr + "\nmnemonic: " + seed + "\nPrivate Key: " + BytesToHexString(PrivateKeyCHILD) + "   N = +" + (n*Step).ToString("X8") + "\nEntropy: " + BytesToHexString(seedBytes) + "\nDer.PATH: " + DER_PATH + "\nTotal: " + Total + " Found: " + Found + " Speed: " + speed + '\n');

                                    }
                                }

                                //Считаем Приватник - N число
                                PrivateKeyCHILD = StringToByteArray(PrivateHEX1);
                                Public_key_compressed = Secp256K1Manager.GetPublicKey(PrivateKeyCHILD, true);
                                Public_key_uncompressed = Secp256K1Manager.GetPublicKey(PrivateKeyCHILD, false);
                                address_compressed = GetAddress(Public_key_compressed);
                                address_uncompressed = GetAddress(Public_key_uncompressed);
                                //var address_compressed = GetHash160(Public_key_compressed);
                                //var address_uncompressed = GetHash160(Public_key_uncompressed);
                                //address_segwit = GetSegWit_base58(Public_key_compressed);
                                eth_adr = GetEthAddress(Public_key_uncompressed);
                                Interlocked.Increment(ref Total);

                               
                                address_segwit = GetSegWit_base58(Public_key_compressed);
                                flag1 = HasBalance(address_segwit);
                                

                                flag = HasBalance(address_compressed);
                                flag0 = HasBalance(address_uncompressed);
                                //flag1 = HasBalance(address_segwit);
                                flag2 = HasBalance(eth_adr);

                                if ((flag != false) || (flag0 != false) || (flag1 != false) || (flag2 != false))
                                {
                                    Console.WriteLine(address_compressed + "\n" + address_uncompressed + "\n" + address_segwit + "\n0x" + eth_adr + "\nmnemonic: " + seed + "\nPrivate Key: " + BytesToHexString(PrivateKeyCHILD) + "   N = -" + (n*Step).ToString("X8") + "\nEntropy: " + BytesToHexString(seedBytes) + "\nDer.PATH: " + DER_PATH + "\nTotal: " + Total + " Found: " + Found + " Speed: " + speed + '\n');
                                    string contents = string.Format($" \n\nAddresses: \n{address_compressed}  Balance: {flag}  \n{address_uncompressed}  Balance: {flag0}  \n{address_segwit}  :  Balance: {flag1} \n0x{eth_adr}  :  Balance: {flag2} \nMnemonic phrase: {seed} \nPrivate Key: {BytesToHexString(PrivateKeyCHILD)} \nEntropy: {BytesToHexString(seedBytes)} \nDerivation PATH: {DER_PATH}");
                                    object outFileLock = Program.outFileLock;
                                    bool lockTaken = false;
                                    try
                                    {
                                        Monitor.Enter(outFileLock, ref lockTaken);
                                        File.AppendAllText("FOUND.txt", contents);
                                    }
                                    finally
                                    {
                                        if (lockTaken)
                                            Monitor.Exit(outFileLock);
                                    }
                                    Interlocked.Increment(ref Found);

                                }
                                else
                                {
                                    if (Silent == false)
                                    {
                                        double Elapsed_MS = sw.ElapsedTicks;
                                        cur = Elapsed_MS / Constant;
                                        cur = Math.Round(cur);
                                        speed = Total / (Elapsed_MS / Constant);
                                        speed = Math.Round(speed);
                                        Console.WriteLine(address_compressed + "\n" + address_uncompressed + "\n" + address_segwit + "\n0x" + eth_adr + "\nmnemonic: " + seed + "\nPrivate Key: " + BytesToHexString(PrivateKeyCHILD) + "   N = -" + (n*Step).ToString("X8") + "\nEntropy: " + BytesToHexString(seedBytes) + "\nDer.PATH: " + DER_PATH + "\nTotal: " + Total + " Found: " + Found + " Speed: " + speed + '\n');

                                    }
                                }
                                n++;
                            }
                        }
                        else
                        {
                            var Public_key_compressed = Secp256K1Manager.GetPublicKey(PrivateKey, true);
                            var Public_key_uncompressed = Secp256K1Manager.GetPublicKey(PrivateKey, false);



                            var address_compressed = GetAddress(Public_key_compressed);
                            var address_uncompressed = GetAddress(Public_key_uncompressed);
                            //var address_compressed = GetHash160(Public_key_compressed);
                            //var address_uncompressed = GetHash160(Public_key_uncompressed);
                            //var address_segwit = GetSegWit_base58(Public_key_compressed);
                            var eth_adr = GetEthAddress(Public_key_uncompressed);
                            Interlocked.Increment(ref Total);

                            string address_segwit;
                            bool flag1;
                            address_segwit = GetSegWit_base58(Public_key_compressed);
                            flag1 = HasBalance(address_segwit);
                            
                            bool flag = HasBalance(address_compressed);
                            bool flag0 = HasBalance(address_uncompressed);
                            //bool flag1 = HasBalance(address_segwit);
                            bool flag2 = HasBalance(eth_adr);

                            if ((flag != false) || (flag0 != false) || (flag1 != false) || (flag2 != false))
                            {
                                Console.WriteLine(address_compressed + "\n" + address_uncompressed + "\n" + address_segwit + "\n0x" + eth_adr + "\nmnemonic: " + seed + "\nPrivate Key: " + BytesToHexString(PrivateKey) + "\nEntropy: " + BytesToHexString(seedBytes) + "\nDer.PATH: " + DER_PATH + "\nTotal: " + Total + " Found: " + Found + " Speed: " + speed + '\n');
                                string contents = string.Format($" \n\nAddresses: \n{address_compressed}  Balance: {flag}  \n{address_uncompressed}  Balance: {flag0}  \n{address_segwit}  :  Balance: {flag1} \n0x{eth_adr}  :  Balance: {flag2} \nMnemonic phrase: {seed} \nPrivate Key: {BytesToHexString(PrivateKey)} \nEntropy: {BytesToHexString(seedBytes)} \nDerivation PATH: {DER_PATH}");
                                object outFileLock = Program.outFileLock;
                                bool lockTaken = false;
                                try
                                {
                                    Monitor.Enter(outFileLock, ref lockTaken);
                                    File.AppendAllText("FOUND.txt", contents);
                                }
                                finally
                                {
                                    if (lockTaken)
                                        Monitor.Exit(outFileLock);
                                }
                                Interlocked.Increment(ref Found);

                            }
                            else
                            {
                                if (Silent == false)
                                {
                                    double Elapsed_MS = sw.ElapsedTicks;
                                    cur = Elapsed_MS / Constant;
                                    cur = Math.Round(cur);
                                    speed = Total / (Elapsed_MS / Constant);
                                    speed = Math.Round(speed);
                                    Console.WriteLine(address_compressed + "\n" + address_uncompressed + "\n" + address_segwit + "\n0x" + eth_adr + "\nmnemonic: " + seed + "\nPrivate Key: " + BytesToHexString(PrivateKey) + "\nEntropy: " + BytesToHexString(seedBytes) + "\nDer.PATH: " + DER_PATH + "\nTotal: " + Total + " Found: " + Found + " Speed: " + speed + '\n');

                                }
                            }
                        }
                        a++;
                    }
                }
            }
        }


        private static void WorkerThread()
        {
            Console.OutputEncoding = Encoding.UTF8;
            //int processorCount = Environment.ProcessorCount;
            //long Total = 0;
            InitializationWordList();
            //var lang = _english;
            //var Language_list = WordListLanguage.ChineseTraditional;
            switch (language)
            {
                case "EN":
                    {
                        lang = _english;
                        Language_list = WordListLanguage.English;
                        break;
                    }
                case "CS":
                    {
                        lang = _chineseSimplified;
                        Language_list = WordListLanguage.ChineseSimplified;
                        break;
                    }
                case "CT":
                    {
                        lang = _chineseTraditional;
                        Language_list = WordListLanguage.ChineseTraditional;
                        break;
                    }
                case "FR":
                    {
                        lang = _french;
                        Language_list = WordListLanguage.French;
                        break;
                    }
                case "IT":
                    {
                        lang = _italian;
                        Language_list = WordListLanguage.Italian;
                        break;
                    }
                case "JA":
                    {
                        lang = _japanese;
                        Language_list = WordListLanguage.Japanese;
                        break;
                    }
                case "KO":
                    {
                        lang = _korean;
                        Language_list = WordListLanguage.Korean;
                        break;
                    }
                case "SP":
                    {
                        lang = _spanish;
                        Language_list = WordListLanguage.Spanish;
                        break;
                    }
                default:
                    {
                        lang = _english;
                        Language_list = WordListLanguage.English;
                        break;
                    }
            }
            sw.Start();
            while (true)
            {
                //Шаг 1. Получаем энтропию.
                //var seedBytes = GenerateMnemonicBytes(words);
                byte[] seedBytes;
                if (IncrementalEntropy)
                {
                    //Console.WriteLine("Entropy: " + Entropy.ToString("X8"));
                    seedBytes = StringToByteArray(Entropy.ToString(EntropyLength));
                    
                    Entropy = Entropy + EntropyStep;
                }
                else
                {
                    seedBytes = GenerateMnemonicBytes(words);
                }
                //var lang = _english;
                //var Language_list = WordListLanguage.ChineseTraditional;
                //var seed = EntropyToMnemonic(seedBytes, _english, WordListLanguage.English);
                var seed = EntropyToMnemonic(seedBytes, lang, Language_list);
                //var seed = "edge shift acquire essence sniff ankle ten prevent december drama churn feel shed ring pair curve biology ability equal cherry yellow blush abuse drift";
                //Console.WriteLine($"Seed: {seed}");
                var saltByte = Encoding.UTF8.GetBytes(salt);
                var masterSecret = Encoding.UTF8.GetBytes(bitcoinSeed);
                var BIP39SeedByte = new Rfc2898DeriveBytes(seed, saltByte, 2048, HashAlgorithmName.SHA512).GetBytes(64);
                //var BIP39Seed = BytesToHexString(BIP39SeedByte);
                var masterPrivateKey = new byte[32]; // Master private key
                var masterChainCode = new byte[32]; // Master chain code
                //Console.WriteLine($"BIP39Seed: {BIP39Seed}");
                var hmac = new HMACSHA512(masterSecret);
                var i = hmac.ComputeHash(BIP39SeedByte);
                Buffer.BlockCopy(i, 0, masterPrivateKey, 0, 32);
                Buffer.BlockCopy(i, 32, masterChainCode, 0, 32);
                //Console.WriteLine($"Master Private Key: {BytesToHexString(masterPrivateKey)}");
                //Console.WriteLine($"Master Chain Code: {BytesToHexString(masterChainCode)}");
                foreach (var path in PATH)
                {
                    int a = 0;
                    while (a <= derivation)
                    {
                        string DER_PATH = path + '/' + a + hardened;
                        //Console.WriteLine(DER_PATH);
                        byte[] PrivateKey = GetChildKey(masterPrivateKey, masterChainCode, DER_PATH);
                        var range = BytesToHexString(PrivateKey).Length;
                        if (range != 64)
                        {
                            while (range != 64)
                            {
                                //Console.WriteLine("D!");
                                string PVK64 = BytesToHexString(PrivateKey);
                                //Console.WriteLine(PVK64);
                                PrivateKey = StringToByteArray("00" + PVK64);
                                //Console.WriteLine("Проверка ключа ппосле добвки нулей " + BytesToHexString(PrivateKey));
                                range++;
                                range++;
                            }
                        }
                        if (IncrementalSearch != 0)
                        {
                            long n = 0;
                            BigInteger PrivateDEC = BigInteger.Parse("0" + BytesToHexString(PrivateKey), System.Globalization.NumberStyles.AllowHexSpecifier);
                            while (n <= IncrementalSearch)
                            {
                                //Считаем Приватник + N число
                                var PrivateDEC1 = PrivateDEC - (n * Step);
                                var PrivateDEC0 = PrivateDEC + (n * Step);
                                //Console.WriteLine(PrivateDEC.ToString("X32"));
                                var PrivateHEX1 = PrivateDEC1.ToString("X32");
                                var PrivateHEX0 = PrivateDEC0.ToString("X32");
                                
                                //Console.WriteLine(PrivateDEC0.ToString("X32"));
                                //Console.WriteLine(PrivateDEC1.ToString("X32"));
                                range = PrivateDEC.ToString("X32").Length;
                                if (range < 64)
                                {
                                    while (range < 64)
                                    {
                                        PrivateHEX0 = '0' + PrivateHEX0;
                                        PrivateHEX1 = '0' + PrivateHEX1;
                                        range++;
                                    }
                                }
                                else if (range > 64)
                                {
                                    PrivateHEX0 = PrivateHEX0[^64..];
                                    PrivateHEX1 = PrivateHEX1[^64..];
                                }
                                var PrivateKeyCHILD = StringToByteArray(PrivateHEX0);
                                var Public_key_compressed = Secp256K1Manager.GetPublicKey(PrivateKeyCHILD, true);
                                var Public_key_uncompressed = Secp256K1Manager.GetPublicKey(PrivateKeyCHILD, false);
                                var address_compressed = GetAddress(Public_key_compressed);
                                var address_uncompressed = GetAddress(Public_key_uncompressed);
                                //var address_compressed = GetHash160(Public_key_compressed);
                                //var address_uncompressed = GetHash160(Public_key_uncompressed);
                                
                                //var eth_adr = GetEthAddress(Public_key_uncompressed);
                                Interlocked.Increment(ref Total);
                                string address_segwit;
                                bool flag1;
                                if (prefix_segwit == "")
                                {
                                    address_segwit = "NULL";
                                    flag1 = false;
                                }
                                else
                                {
                                    address_segwit = GetSegWit_base58(Public_key_compressed);
                                    flag1 = HasBalance(address_segwit);
                                }
                                bool flag = HasBalance(address_compressed);
                                bool flag0 = HasBalance(address_uncompressed);
                                

                                if ((flag != false) || (flag0 != false) || (flag1 != false))
                                {
                                    Console.WriteLine(address_compressed + "\n" + address_uncompressed + "\n" + address_segwit + "\nmnemonic: " + seed + "\nPrivate Key: " + BytesToHexString(PrivateKeyCHILD) + "   N = +" + (n*Step).ToString("X8") + "\nEntropy: " + BytesToHexString(seedBytes) + "\nDer.PATH: " + DER_PATH + "\nTotal: " + Total + " Found: " + Found + " Speed: " + speed + '\n');
                                    string contents = string.Format($" \n\nAddresses: \n{address_compressed}  Balance: {flag}  \n{address_uncompressed}  Balance: {flag0}  \n{address_segwit}  :  Balance: {flag1} \nMnemonic phrase: {seed} \nPrivate Key: {BytesToHexString(PrivateKeyCHILD)} \nEntropy: {BytesToHexString(seedBytes)}  \nDerivation PATH: {DER_PATH}");
                                    object outFileLock = Program.outFileLock;
                                    bool lockTaken = false;
                                    try
                                    {
                                        Monitor.Enter(outFileLock, ref lockTaken);
                                        File.AppendAllText("FOUND.txt", contents);
                                    }
                                    finally
                                    {
                                        if (lockTaken)
                                            Monitor.Exit(outFileLock);
                                    }
                                    Interlocked.Increment(ref Found);

                                }
                                else
                                {
                                    if (Silent == false)
                                    {
                                        double Elapsed_MS = sw.ElapsedTicks;
                                        cur = Elapsed_MS / Constant;
                                        cur = Math.Round(cur);
                                        speed = Total / (Elapsed_MS / Constant);
                                        speed = Math.Round(speed);
                                        Console.WriteLine(address_compressed + "\n" + address_uncompressed + "\n" + address_segwit + "\nmnemonic: " + seed + "\nPrivate Key: " + BytesToHexString(PrivateKeyCHILD) + "   N = +" + (n*Step).ToString("X8") + "\nEntropy: " + BytesToHexString(seedBytes) + "\nDer.PATH: " + DER_PATH + "\nTotal: " + Total + " Found: " + Found + " Speed: " + speed + '\n');

                                    }
                                }

                                //Считаем Приватник - N число
                                PrivateKeyCHILD = StringToByteArray(PrivateHEX1);
                                Public_key_compressed = Secp256K1Manager.GetPublicKey(PrivateKeyCHILD, true);
                                Public_key_uncompressed = Secp256K1Manager.GetPublicKey(PrivateKeyCHILD, false);
                                address_compressed = GetAddress(Public_key_compressed);
                                address_uncompressed = GetAddress(Public_key_uncompressed);
                                //var address_compressed = GetHash160(Public_key_compressed);
                                //var address_uncompressed = GetHash160(Public_key_uncompressed);
                                //address_segwit = GetSegWit_base58(Public_key_compressed);
                                //var eth_adr = GetEthAddress(Public_key_uncompressed);
                                Interlocked.Increment(ref Total);
                                if (prefix_segwit == "")
                                {
                                    address_segwit = "NULL";
                                    flag1 = false;
                                }
                                else
                                {
                                    address_segwit = GetSegWit_base58(Public_key_compressed);
                                    flag1 = HasBalance(address_segwit);
                                }

                                flag = HasBalance(address_compressed);
                                flag0 = HasBalance(address_uncompressed);
                                //flag1 = HasBalance(address_segwit);

                                if ((flag != false) || (flag0 != false) || (flag1 != false))
                                {
                                    Console.WriteLine(address_compressed + "\n" + address_uncompressed + "\n" + address_segwit + "\nmnemonic: " + seed + "\nPrivate Key: " + BytesToHexString(PrivateKeyCHILD) + "   N = -" + (n*Step).ToString("X8") + "\nEntropy: " + BytesToHexString(seedBytes) + "\nDer.PATH: " + DER_PATH + "\nTotal: " + Total + " Found: " + Found + " Speed: " + speed + '\n');
                                    string contents = string.Format($" \n\nAddresses: \n{address_compressed}  Balance: {flag}  \n{address_uncompressed}  Balance: {flag0}  \n{address_segwit}  :  Balance: {flag1} \nMnemonic phrase: {seed} \nPrivate Key: {BytesToHexString(PrivateKeyCHILD)}  \nEntropy: {BytesToHexString(seedBytes)} \nDerivation PATH: {DER_PATH}");
                                    object outFileLock = Program.outFileLock;
                                    bool lockTaken = false;
                                    try
                                    {
                                        Monitor.Enter(outFileLock, ref lockTaken);
                                        File.AppendAllText("FOUND.txt", contents);
                                    }
                                    finally
                                    {
                                        if (lockTaken)
                                            Monitor.Exit(outFileLock);
                                    }
                                    Interlocked.Increment(ref Found);

                                }
                                else
                                {
                                    if (Silent == false)
                                    {
                                        double Elapsed_MS = sw.ElapsedTicks;
                                        cur = Elapsed_MS / Constant;
                                        cur = Math.Round(cur);
                                        speed = Total / (Elapsed_MS / Constant);
                                        speed = Math.Round(speed);
                                        Console.WriteLine(address_compressed + "\n" + address_uncompressed + "\n" + address_segwit + "\nmnemonic: " + seed + "\nPrivate Key: " + BytesToHexString(PrivateKeyCHILD) + "   N = -" + (n*Step).ToString("X8") + "\nEntropy: " + BytesToHexString(seedBytes) + "\nDer.PATH: " + DER_PATH + "\nTotal: " + Total + " Found: " + Found + " Speed: " + speed + '\n');

                                    }
                                }
                                n++;
                            }
                        }
                        else
                        {
                            var Public_key_compressed = Secp256K1Manager.GetPublicKey(PrivateKey, true);
                            var Public_key_uncompressed = Secp256K1Manager.GetPublicKey(PrivateKey, false);



                            var address_compressed = GetAddress(Public_key_compressed);
                            var address_uncompressed = GetAddress(Public_key_uncompressed);
                            //var address_compressed = GetHash160(Public_key_compressed);
                            //var address_uncompressed = GetHash160(Public_key_uncompressed);
                            //var address_segwit = GetSegWit_base58(Public_key_compressed);
                            //var eth_adr = GetEthAddress(Public_key_uncompressed);
                            Interlocked.Increment(ref Total);
                            string address_segwit;
                            bool flag1;
                            if (prefix_segwit == "")
                            {
                                address_segwit = "NULL";
                                flag1 = false;
                            }
                            else
                            {
                                address_segwit = GetSegWit_base58(Public_key_compressed);
                                flag1 = HasBalance(address_segwit);
                            }

                            bool flag = HasBalance(address_compressed);
                            bool flag0 = HasBalance(address_uncompressed);
                            //bool flag1 = HasBalance(address_segwit);

                            if ((flag != false) || (flag0 != false) || (flag1 != false))
                            {
                                Console.WriteLine(address_compressed + "\n" + address_uncompressed + "\n" + address_segwit + "\nmnemonic: " + seed + "\nPrivate Key: " + BytesToHexString(PrivateKey) + "   N: " + "\nEntropy: " + BytesToHexString(seedBytes) + "\nDer.PATH: " + DER_PATH + "\nTotal: " + Total + " Found: " + Found + " Speed: " + speed + '\n');
                                string contents = string.Format($" \n\nAddresses: \n{address_compressed}  Balance: {flag}  \n{address_uncompressed}  Balance: {flag0}  \n{address_segwit}  :  Balance: {flag1} \nMnemonic phrase: {seed} \nPrivate Key: {BytesToHexString(PrivateKey)}  \nEntropy: {BytesToHexString(seedBytes)} \nDerivation PATH: {DER_PATH}");
                                object outFileLock = Program.outFileLock;
                                bool lockTaken = false;
                                try
                                {
                                    Monitor.Enter(outFileLock, ref lockTaken);
                                    File.AppendAllText("FOUND.txt", contents);
                                }
                                finally
                                {
                                    if (lockTaken)
                                        Monitor.Exit(outFileLock);
                                }
                                Interlocked.Increment(ref Found);

                            }
                            else
                            {
                                if (Silent == false)
                                {
                                    double Elapsed_MS = sw.ElapsedTicks;
                                    cur = Elapsed_MS / Constant;
                                    cur = Math.Round(cur);
                                    speed = Total / (Elapsed_MS / Constant);
                                    speed = Math.Round(speed);
                                    Console.WriteLine(address_compressed + "\n" + address_uncompressed + "\n" + address_segwit + "\nmnemonic: " + seed + "\nPrivate Key: " + BytesToHexString(PrivateKey) + "\nEntropy: " + BytesToHexString(seedBytes) + "\nDer.PATH: " + DER_PATH + "\nTotal: " + Total + " Found: " + Found + " Speed: " + speed + '\n');

                                }
                            }
                        }
                        a++;
                    }
                }
            }
        }

        private static void WorkerThread2()
        {
            Console.OutputEncoding = Encoding.UTF8;
            //int processorCount = Environment.ProcessorCount;
            //long Total = 0;
            InitializationWordList();
            //var lang = _english;
            //var Language_list = WordListLanguage.ChineseTraditional;
            switch (language)
            {
                case "EN":
                    {
                        lang = _english;
                        Language_list = WordListLanguage.English;
                        break;
                    }
                case "CS":
                    {
                        lang = _chineseSimplified;
                        Language_list = WordListLanguage.ChineseSimplified;
                        break;
                    }
                case "CT":
                    {
                        lang = _chineseTraditional;
                        Language_list = WordListLanguage.ChineseTraditional;
                        break;
                    }
                case "FR":
                    {
                        lang = _french;
                        Language_list = WordListLanguage.French;
                        break;
                    }
                case "IT":
                    {
                        lang = _italian;
                        Language_list = WordListLanguage.Italian;
                        break;
                    }
                case "JA":
                    {
                        lang = _japanese;
                        Language_list = WordListLanguage.Japanese;
                        break;
                    }
                case "KO":
                    {
                        lang = _korean;
                        Language_list = WordListLanguage.Korean;
                        break;
                    }
                case "SP":
                    {
                        lang = _spanish;
                        Language_list = WordListLanguage.Spanish;
                        break;
                    }
                default:
                    {
                        lang = _english;
                        Language_list = WordListLanguage.English;
                        break;
                    }
            }
            sw.Start();
            while (true)
            {


                //Шаг 1. Получаем энтропию.
                //var seedBytes = GenerateMnemonicBytes(words);
                byte[] seedBytes;
                if (IncrementalEntropy)
                {
                    seedBytes = StringToByteArray(Entropy.ToString(EntropyLength));
                    Entropy = Entropy + EntropyStep;
                }
                else
                {
                    seedBytes = GenerateMnemonicBytes(words);
                }
                //var lang = _english;
                //var Language_list = WordListLanguage.ChineseTraditional;
                //var seed = EntropyToMnemonic(seedBytes, _english, WordListLanguage.English);
                var seed = EntropyToMnemonic(seedBytes, lang, Language_list);
                //var seed = "edge shift acquire essence sniff ankle ten prevent december drama churn feel shed ring pair curve biology ability equal cherry yellow blush abuse drift";
                //Console.WriteLine($"Seed: {seed}");
                var saltByte = Encoding.UTF8.GetBytes(salt);
                var masterSecret = Encoding.UTF8.GetBytes(bitcoinSeed);
                var BIP39SeedByte = new Rfc2898DeriveBytes(seed, saltByte, 2048, HashAlgorithmName.SHA512).GetBytes(64);
                //var BIP39Seed = BytesToHexString(BIP39SeedByte);
                var masterPrivateKey = new byte[32]; // Master private key
                var masterChainCode = new byte[32]; // Master chain code
                //Console.WriteLine($"BIP39Seed: {BIP39Seed}");
                var hmac = new HMACSHA512(masterSecret);
                var i = hmac.ComputeHash(BIP39SeedByte);
                Buffer.BlockCopy(i, 0, masterPrivateKey, 0, 32);
                Buffer.BlockCopy(i, 32, masterChainCode, 0, 32);
                //Console.WriteLine($"Master Private Key: {BytesToHexString(masterPrivateKey)}");
                //Console.WriteLine($"Master Chain Code: {BytesToHexString(masterChainCode)}");
                foreach (var path in PATH)
                {
                    int a = 0;
                    while (a <= derivation)
                    {
                        string DER_PATH = path + '/' + a + hardened;
                        //Console.WriteLine(DER_PATH);
                        byte[] PrivateKey = GetChildKey(masterPrivateKey, masterChainCode, DER_PATH);
                        var range = BytesToHexString(PrivateKey).Length;
                        if (range != 64)
                        {
                            while (range != 64)
                            {
                                //Console.WriteLine("D!");
                                string PVK64 = BytesToHexString(PrivateKey);
                                //Console.WriteLine(PVK64);
                                PrivateKey = StringToByteArray("00" + PVK64);
                                //Console.WriteLine("Проверка ключа ппосле добвки нулей " + BytesToHexString(PrivateKey));
                                range++;
                                range++;
                            }
                        }
                        if (IncrementalSearch != 0)
                        {
                            long n = 0;
                            BigInteger PrivateDEC = BigInteger.Parse("0" + BytesToHexString(PrivateKey), System.Globalization.NumberStyles.AllowHexSpecifier);
                            while (n <= IncrementalSearch)
                            {
                                //Считаем Приватник + N число
                                var PrivateDEC1 = PrivateDEC - (n * Step);
                                var PrivateDEC0 = PrivateDEC + (n * Step);
                                //Console.WriteLine(PrivateDEC.ToString("X32"));
                                var PrivateHEX1 = PrivateDEC1.ToString("X32");
                                var PrivateHEX0 = PrivateDEC0.ToString("X32");

                                //Console.WriteLine(PrivateDEC0.ToString("X32"));
                                //Console.WriteLine(PrivateDEC1.ToString("X32"));
                                range = PrivateDEC.ToString("X32").Length;
                                if (range < 64)
                                {
                                    while (range < 64)
                                    {
                                        PrivateHEX0 = '0' + PrivateHEX0;
                                        PrivateHEX1 = '0' + PrivateHEX1;
                                        range++;
                                    }
                                }
                                else if (range > 64)
                                {
                                    PrivateHEX0 = PrivateHEX0[^64..];
                                    PrivateHEX1 = PrivateHEX1[^64..];
                                }
                                var PrivateKeyCHILD = StringToByteArray(PrivateHEX0);
                                
                                var Public_key_uncompressed = Secp256K1Manager.GetPublicKey(PrivateKeyCHILD, false);
                        
                                var eth_adr = GetEthAddress(Public_key_uncompressed);
                                Interlocked.Increment(ref Total);
                                //var Public_key_compressed = Secp256K1Manager.GetPublicKey(PrivateKey, true);
                                bool flag = HasBalance(eth_adr);
                                bool flag0 = HasBalance("0x" + eth_adr);

                                if ((flag != false) || (flag0 != false))
                                {
                                    Console.WriteLine(eth_adr + "\nmnemonic: " + seed + "\nPrivate Key: " + BytesToHexString(PrivateKeyCHILD) + "   N = +" + (n*Step).ToString("X8") + "\nEntropy: " + BytesToHexString(seedBytes) + "\nDer.PATH: " + DER_PATH + "\nTotal: " + Total + " Found: " + Found + " Speed: " + speed + '\n');
                                    string contents = string.Format($" \n\nAddress: 0x{eth_adr} \nPrivate Key: {BytesToHexString(PrivateKeyCHILD)}  \nEntropy: {BytesToHexString(seedBytes)}  \nMnemonic phrase: {seed} \n Derivation PATH: {DER_PATH}");
                                    object outFileLock = Program.outFileLock;
                                    bool lockTaken = false;
                                    try
                                    {
                                        Monitor.Enter(outFileLock, ref lockTaken);
                                        File.AppendAllText("FOUND.txt", contents);
                                    }
                                    finally
                                    {
                                        if (lockTaken)
                                            Monitor.Exit(outFileLock);
                                    }
                                    Interlocked.Increment(ref Found);

                                }
                                else
                                {
                                    if (Silent == false)
                                    {
                                        double Elapsed_MS = sw.ElapsedTicks;
                                        cur = Elapsed_MS / Constant;
                                        cur = Math.Round(cur);
                                        speed = Total / (Elapsed_MS / Constant);
                                        speed = Math.Round(speed);
                                        Console.WriteLine("0x" + eth_adr + "\nmnemonic: " + seed + "\nPrivate Key: " + BytesToHexString(PrivateKeyCHILD) + "   N = +" + (n*Step).ToString("X8") + "\nEntropy: " + BytesToHexString(seedBytes) + "\nDer.PATH: " + DER_PATH + "\nTotal: " + Total + " Found: " + Found + " Speed: " + speed + '\n');

                                    }
                                }

                                PrivateKeyCHILD = StringToByteArray(PrivateHEX1);

                                Public_key_uncompressed = Secp256K1Manager.GetPublicKey(PrivateKeyCHILD, false);

                                eth_adr = GetEthAddress(Public_key_uncompressed);
                                Interlocked.Increment(ref Total);
                                //var Public_key_compressed = Secp256K1Manager.GetPublicKey(PrivateKey, true);
                                flag = HasBalance(eth_adr);
                                flag0 = HasBalance("0x" + eth_adr);

                                if ((flag != false) || (flag0 != false))
                                {
                                    Console.WriteLine(eth_adr + "\nmnemonic: " + seed + "\nPrivate Key: " + BytesToHexString(PrivateKeyCHILD) + "   N = -" + (n*Step).ToString("X8") + "\nEntropy: " + BytesToHexString(seedBytes) + "\nDer.PATH: " + DER_PATH + "\nTotal: " + Total + " Found: " + Found + " Speed: " + speed + '\n');
                                    string contents = string.Format($" \n\nAddress: 0x{eth_adr} \nPrivate Key: {BytesToHexString(PrivateKeyCHILD)}  \nEntropy: {BytesToHexString(seedBytes)}  \nMnemonic phrase: {seed} \n Derivation PATH: {DER_PATH}");
                                    object outFileLock = Program.outFileLock;
                                    bool lockTaken = false;
                                    try
                                    {
                                        Monitor.Enter(outFileLock, ref lockTaken);
                                        File.AppendAllText("FOUND.txt", contents);
                                    }
                                    finally
                                    {
                                        if (lockTaken)
                                            Monitor.Exit(outFileLock);
                                    }
                                    Interlocked.Increment(ref Found);

                                }
                                else
                                {
                                    if (Silent == false)
                                    {
                                        double Elapsed_MS = sw.ElapsedTicks;
                                        cur = Elapsed_MS / Constant;
                                        cur = Math.Round(cur);
                                        speed = Total / (Elapsed_MS / Constant);
                                        speed = Math.Round(speed);
                                        Console.WriteLine("0x" + eth_adr + "\nmnemonic: " + seed + "\nPrivate Key: " + BytesToHexString(PrivateKeyCHILD) + "   N = -" + (n*Step).ToString("X8") + "\nEntropy: " + BytesToHexString(seedBytes) + "\nDer.PATH: " + DER_PATH + "\nTotal: " + Total + " Found: " + Found + " Speed: " + speed + '\n');

                                    }
                                }

                                n++;

                            }
                        }
                        else
                        {
                            var Public_key_uncompressed = Secp256K1Manager.GetPublicKey(PrivateKey, false);
                            //var address_compressed = GetAddress(Public_key_compressed);
                            //var address_uncompressed = GetAddress(Public_key_uncompressed);
                            //var address_compressed = GetHash160(Public_key_compressed);
                            //var address_uncompressed = GetHash160(Public_key_uncompressed);
                            //var address_segwit = GetSegWit_base58(Public_key_compressed);
                            var eth_adr = GetEthAddress(Public_key_uncompressed);
                            Interlocked.Increment(ref Total);
                            bool flag = HasBalance(eth_adr);
                            bool flag0 = HasBalance("0x" + eth_adr);

                            if ((flag != false) || (flag0 != false))
                            {
                                Console.WriteLine(eth_adr + "\nmnemonic: " + seed + "\nPrivate Key: " + BytesToHexString(PrivateKey) + "\nEntropy: " + BytesToHexString(seedBytes) + "\nDer.PATH: " + DER_PATH + "\nTotal: " + Total + " Found: " + Found + " Speed: " + speed + '\n');
                                string contents = string.Format($" \n\nAddress: 0x{eth_adr} \nPrivate Key: {BytesToHexString(PrivateKey)}  \nEntropy: {BytesToHexString(seedBytes)}  \nMnemonic phrase: {seed} \n Derivation PATH: {DER_PATH}");
                                object outFileLock = Program.outFileLock;
                                bool lockTaken = false;
                                try
                                {
                                    Monitor.Enter(outFileLock, ref lockTaken);
                                    File.AppendAllText("FOUND.txt", contents);
                                }
                                finally
                                {
                                    if (lockTaken)
                                        Monitor.Exit(outFileLock);
                                }
                                Interlocked.Increment(ref Found);

                            }
                            else
                            {
                                if (Silent == false)
                                {
                                    double Elapsed_MS = sw.ElapsedTicks;
                                    cur = Elapsed_MS / Constant;
                                    cur = Math.Round(cur);
                                    speed = Total / (Elapsed_MS / Constant);
                                    speed = Math.Round(speed);
                                    Console.WriteLine("0x" + eth_adr + "\nmnemonic: " + seed + "\nPrivate Key: " + BytesToHexString(PrivateKey) + "\nEntropy: " + BytesToHexString(seedBytes) + "\nDer.PATH: " + DER_PATH + "\nTotal: " + Total + " Found: " + Found + " Speed: " + speed + '\n');

                                }
                            }
                        }
                        //Console.WriteLine(Total);
                        a++;
                    }
                }
            }
        }

        private static void WorkerThread3()
        {
            Console.OutputEncoding = Encoding.UTF8;
            //int processorCount = Environment.ProcessorCount;
            //long Total = 0;
            InitializationWordList();
            //var lang = _english;
            //var Language_list = WordListLanguage.ChineseTraditional;
            switch (language)
            {
                case "EN":
                    {
                        lang = _english;
                        Language_list = WordListLanguage.English;
                        break;
                    }
                case "CS":
                    {
                        lang = _chineseSimplified;
                        Language_list = WordListLanguage.ChineseSimplified;
                        break;
                    }
                case "CT":
                    {
                        lang = _chineseTraditional;
                        Language_list = WordListLanguage.ChineseTraditional;
                        break;
                    }
                case "FR":
                    {
                        lang = _french;
                        Language_list = WordListLanguage.French;
                        break;
                    }
                case "IT":
                    {
                        lang = _italian;
                        Language_list = WordListLanguage.Italian;
                        break;
                    }
                case "JA":
                    {
                        lang = _japanese;
                        Language_list = WordListLanguage.Japanese;
                        break;
                    }
                case "KO":
                    {
                        lang = _korean;
                        Language_list = WordListLanguage.Korean;
                        break;
                    }
                case "SP":
                    {
                        lang = _spanish;
                        Language_list = WordListLanguage.Spanish;
                        break;
                    }
                default:
                    {
                        lang = _english;
                        Language_list = WordListLanguage.English;
                        break;
                    }
            }
            sw.Start();
            while (true)
            {
                //Шаг 1. Получаем энтропию.
                //var seedBytes = GenerateMnemonicBytes(words);
                byte[] seedBytes;
                if (IncrementalEntropy)
                {
                    seedBytes = StringToByteArray(Entropy.ToString(EntropyLength));
                    Entropy = Entropy + EntropyStep;
                }
                else
                {
                    seedBytes = GenerateMnemonicBytes(words);
                }
                //var lang = _english;
                //var Language_list = WordListLanguage.ChineseTraditional;
                //var seed = EntropyToMnemonic(seedBytes, _english, WordListLanguage.English);
                var seed = EntropyToMnemonic(seedBytes, lang, Language_list);
                //var seed = "edge shift acquire essence sniff ankle ten prevent december drama churn feel shed ring pair curve biology ability equal cherry yellow blush abuse drift";
                //Console.WriteLine($"Seed: {seed}");
                var saltByte = Encoding.UTF8.GetBytes(salt);
                var masterSecret = Encoding.UTF8.GetBytes(bitcoinSeed);
                var BIP39SeedByte = new Rfc2898DeriveBytes(seed, saltByte, 2048, HashAlgorithmName.SHA512).GetBytes(64);
                //var BIP39Seed = BytesToHexString(BIP39SeedByte);
                var masterPrivateKey = new byte[32]; // Master private key
                var masterChainCode = new byte[32]; // Master chain code
                //Console.WriteLine($"BIP39Seed: {BIP39Seed}");
                var hmac = new HMACSHA512(masterSecret);
                var i = hmac.ComputeHash(BIP39SeedByte);
                Buffer.BlockCopy(i, 0, masterPrivateKey, 0, 32);
                Buffer.BlockCopy(i, 32, masterChainCode, 0, 32);
                //Console.WriteLine($"Master Private Key: {BytesToHexString(masterPrivateKey)}");
                //Console.WriteLine($"Master Chain Code: {BytesToHexString(masterChainCode)}");
                foreach (var path in PATH)
                {
                    int a = 0;
                    while (a <= derivation)
                    {
                        string DER_PATH = path + '/' + a + hardened;
                        //Console.WriteLine(DER_PATH);
                        byte[] PrivateKey = GetChildKey(masterPrivateKey, masterChainCode, DER_PATH);
                        var range = BytesToHexString(PrivateKey).Length;
                        if (range != 64)
                        {
                            while (range != 64)
                            {
                                //Console.WriteLine("D!");
                                string PVK64 = BytesToHexString(PrivateKey);
                                //Console.WriteLine(PVK64);
                                PrivateKey = StringToByteArray("00" + PVK64);
                                //Console.WriteLine("Проверка ключа ппосле добвки нулей " + BytesToHexString(PrivateKey));
                                range++;
                                range++;
                            }
                        }
                        if (IncrementalSearch != 0)
                        {
                            long n = 0;
                            BigInteger PrivateDEC = BigInteger.Parse("0" + BytesToHexString(PrivateKey), System.Globalization.NumberStyles.AllowHexSpecifier);
                            while (n <= IncrementalSearch)
                            {
                                //Считаем Приватник + N число
                                var PrivateDEC1 = PrivateDEC - (n * Step);
                                var PrivateDEC0 = PrivateDEC + (n * Step);
                                //Console.WriteLine(PrivateDEC.ToString("X32"));
                                var PrivateHEX1 = PrivateDEC1.ToString("X32");
                                var PrivateHEX0 = PrivateDEC0.ToString("X32");

                                //Console.WriteLine(PrivateDEC0.ToString("X32"));
                                //Console.WriteLine(PrivateDEC1.ToString("X32"));
                                range = PrivateDEC.ToString("X32").Length;
                                if (range < 64)
                                {
                                    while (range < 64)
                                    {
                                        PrivateHEX0 = '0' + PrivateHEX0;
                                        PrivateHEX1 = '0' + PrivateHEX1;
                                        range++;
                                    }
                                }
                                else if (range > 64)
                                {
                                    PrivateHEX0 = PrivateHEX0[^64..];
                                    PrivateHEX1 = PrivateHEX1[^64..];
                                }
                                var PrivateKeyCHILD = StringToByteArray(PrivateHEX0);
                                var Public_key_compressed = Secp256K1Manager.GetPublicKey(PrivateKeyCHILD, true);
                                var Public_key_uncompressed = Secp256K1Manager.GetPublicKey(PrivateKeyCHILD, false);
                                var hash_compressed = GetHash160(Public_key_compressed);
                                var hash_uncompressed = GetHash160(Public_key_uncompressed);
                                //var address_segwit = GetSegWit_base58(Public_key_compressed);
                                //var eth_adr = GetEthAddress(Public_key_uncompressed);
                                Interlocked.Increment(ref Total);
                                bool flag = HasBalance(hash_compressed);
                                bool flag1 = HasBalance(hash_uncompressed);
                                if ((flag != false) || (flag1 != false))
                                {
                                    Console.WriteLine(hash_compressed + "\n" + hash_uncompressed + "\nmnemonic: " + seed + "\nPrivate Key: " + BytesToHexString(PrivateKeyCHILD) + "   N = +" + (n*Step).ToString("X8") + "\nEntropy: " + BytesToHexString(seedBytes) + "\nDer.PATH: " + DER_PATH + "\nTotal: " + Total + " Found: " + Found + " Speed: " + speed + '\n');
                                    string contents = string.Format($" \n\nHashes: \n{hash_compressed}  Balance: {flag}  \n{hash_uncompressed} Balance: {flag1} \nPrivate Key: {BytesToHexString(PrivateKeyCHILD)}  \nEntropy: {BytesToHexString(seedBytes)}   \nMnemonic phrase: {seed}  \n Derivation PATH: {DER_PATH}");
                                    object outFileLock = Program.outFileLock;
                                    bool lockTaken = false;
                                    try
                                    {
                                        Monitor.Enter(outFileLock, ref lockTaken);
                                        File.AppendAllText("FOUND.txt", contents);
                                    }
                                    finally
                                    {
                                        if (lockTaken)
                                            Monitor.Exit(outFileLock);
                                    }
                                    Interlocked.Increment(ref Found);

                                }
                                else
                                {
                                    if (Silent == false)
                                    {
                                        double Elapsed_MS = sw.ElapsedTicks;
                                        cur = Elapsed_MS / Constant;
                                        cur = Math.Round(cur);
                                        speed = Total / (Elapsed_MS / Constant);
                                        speed = Math.Round(speed);
                                        Console.WriteLine(hash_compressed + "\n" + hash_uncompressed + "\nmnemonic: " + seed + "\nPrivate Key: " + BytesToHexString(PrivateKeyCHILD) + "   N = +" + (n*Step).ToString("X8") + "\nEntropy: " + BytesToHexString(seedBytes) + "\nDer.PATH: " + DER_PATH + "\nTotal: " + Total + " Found: " + Found + " Speed: " + speed + '\n');

                                    }
                                }

                                PrivateKeyCHILD = StringToByteArray(PrivateHEX1);
                                Public_key_compressed = Secp256K1Manager.GetPublicKey(PrivateKeyCHILD, true);
                                Public_key_uncompressed = Secp256K1Manager.GetPublicKey(PrivateKeyCHILD, false);
                                hash_compressed = GetHash160(Public_key_compressed);
                                hash_uncompressed = GetHash160(Public_key_uncompressed);
                                //var address_segwit = GetSegWit_base58(Public_key_compressed);
                                //var eth_adr = GetEthAddress(Public_key_uncompressed);
                                Interlocked.Increment(ref Total);
                                flag = HasBalance(hash_compressed);
                                flag1 = HasBalance(hash_uncompressed);
                                if ((flag != false) || (flag1 != false))
                                {
                                    Console.WriteLine(hash_compressed + "\n" + hash_uncompressed + "\nmnemonic: " + seed + "\nPrivate Key: " + BytesToHexString(PrivateKeyCHILD) + "   N = -" + (n*Step).ToString("X8") + "\nEntropy: " + BytesToHexString(seedBytes) + "\nDer.PATH: " + DER_PATH + "\nTotal: " + Total + " Found: " + Found + " Speed: " + speed + '\n');
                                    string contents = string.Format($" \n\nHashes: \n{hash_compressed}  Balance: {flag}  \n{hash_uncompressed} Balance: {flag1} \nPrivate Key: {BytesToHexString(PrivateKeyCHILD)}  \nEntropy: {BytesToHexString(seedBytes)}   \nMnemonic phrase: {seed}  \n Derivation PATH: {DER_PATH}");
                                    object outFileLock = Program.outFileLock;
                                    bool lockTaken = false;
                                    try
                                    {
                                        Monitor.Enter(outFileLock, ref lockTaken);
                                        File.AppendAllText("FOUND.txt", contents);
                                    }
                                    finally
                                    {
                                        if (lockTaken)
                                            Monitor.Exit(outFileLock);
                                    }
                                    Interlocked.Increment(ref Found);

                                }
                                else
                                {
                                    if (Silent == false)
                                    {
                                        double Elapsed_MS = sw.ElapsedTicks;
                                        cur = Elapsed_MS / Constant;
                                        cur = Math.Round(cur);
                                        speed = Total / (Elapsed_MS / Constant);
                                        speed = Math.Round(speed);
                                        Console.WriteLine(hash_compressed + "\n" + hash_uncompressed + "\nmnemonic: " + seed + "\nPrivate Key: " + BytesToHexString(PrivateKeyCHILD) + "   N = -" + (n*Step).ToString("X8") + "\nEntropy: " + BytesToHexString(seedBytes) + "\nDer.PATH: " + DER_PATH + "\nTotal: " + Total + " Found: " + Found + " Speed: " + speed + '\n');

                                    }
                                }
                                n++;
                            }
                        }
                        else
                        {
                            var Public_key_compressed = Secp256K1Manager.GetPublicKey(PrivateKey, true);
                            var Public_key_uncompressed = Secp256K1Manager.GetPublicKey(PrivateKey, false);



                            //var address_compressed = GetAddress(Public_key_compressed);
                            //var address_uncompressed = GetAddress(Public_key_uncompressed);
                            var hash_compressed = GetHash160(Public_key_compressed);
                            var hash_uncompressed = GetHash160(Public_key_uncompressed);
                            //var address_segwit = GetSegWit_base58(Public_key_compressed);
                            //var eth_adr = GetEthAddress(Public_key_uncompressed);
                            Interlocked.Increment(ref Total);
                            bool flag = HasBalance(hash_compressed);
                            bool flag1 = HasBalance(hash_uncompressed);
                            if ((flag != false) || (flag1 != false))
                            {
                                Console.WriteLine(hash_compressed + "\n" + hash_uncompressed + "\nmnemonic: " + seed + "\nPrivate Key: " + BytesToHexString(PrivateKey) +  "\nEntropy: " + BytesToHexString(seedBytes) + "\nDer.PATH: " + DER_PATH + "\nTotal: " + Total + " Found: " + Found + " Speed: " + speed + '\n');
                                string contents = string.Format($" \n\nHashes: \n{hash_compressed}  Balance: {flag}  \n{hash_uncompressed} Balance: {flag1} \nPrivate Key: {BytesToHexString(PrivateKey)}  \nEntropy: {BytesToHexString(seedBytes)}   \nMnemonic phrase: {seed}  \n Derivation PATH: {DER_PATH}");
                                object outFileLock = Program.outFileLock;
                                bool lockTaken = false;
                                try
                                {
                                    Monitor.Enter(outFileLock, ref lockTaken);
                                    File.AppendAllText("FOUND.txt", contents);
                                }
                                finally
                                {
                                    if (lockTaken)
                                        Monitor.Exit(outFileLock);
                                }
                                Interlocked.Increment(ref Found);

                            }
                            else
                            {
                                if (Silent == false)
                                {
                                    double Elapsed_MS = sw.ElapsedTicks;
                                    cur = Elapsed_MS / Constant;
                                    cur = Math.Round(cur);
                                    speed = Total / (Elapsed_MS / Constant);
                                    speed = Math.Round(speed);
                                    Console.WriteLine(hash_compressed + "\n" + hash_uncompressed + "\nmnemonic: " + seed + "\nPrivate Key: " + BytesToHexString(PrivateKey) + "\nEntropy: " + BytesToHexString(seedBytes) + "\nDer.PATH: " + DER_PATH + "\nTotal: " + Total + " Found: " + Found + " Speed: " + speed + '\n');

                                }
                            }
                        }
                        a++;
                    }
                }
            }
        }

        private static void WorkerThread4()
        {
            Console.OutputEncoding = Encoding.UTF8;
            //int processorCount = Environment.ProcessorCount;
            //long Total = 0;
            InitializationWordList();
            //var lang = _english;
            //var Language_list = WordListLanguage.ChineseTraditional;
            switch (language)
            {
                case "EN":
                    {
                        lang = _english;
                        Language_list = WordListLanguage.English;
                        break;
                    }
                case "CS":
                    {
                        lang = _chineseSimplified;
                        Language_list = WordListLanguage.ChineseSimplified;
                        break;
                    }
                case "CT":
                    {
                        lang = _chineseTraditional;
                        Language_list = WordListLanguage.ChineseTraditional;
                        break;
                    }
                case "FR":
                    {
                        lang = _french;
                        Language_list = WordListLanguage.French;
                        break;
                    }
                case "IT":
                    {
                        lang = _italian;
                        Language_list = WordListLanguage.Italian;
                        break;
                    }
                case "JA":
                    {
                        lang = _japanese;
                        Language_list = WordListLanguage.Japanese;
                        break;
                    }
                case "KO":
                    {
                        lang = _korean;
                        Language_list = WordListLanguage.Korean;
                        break;
                    }
                case "SP":
                    {
                        lang = _spanish;
                        Language_list = WordListLanguage.Spanish;
                        break;
                    }
                default:
                    {
                        lang = _english;
                        Language_list = WordListLanguage.English;
                        break;
                    }
            }
            sw.Start();
            while (true)
            {
                //Шаг 1. Получаем энтропию.
                //var seedBytes = GenerateMnemonicBytes(words);
                byte[] seedBytes;
                if (IncrementalEntropy)
                {
                    seedBytes = StringToByteArray(Entropy.ToString(EntropyLength));
                    Entropy = Entropy + EntropyStep;
                }
                else
                {
                    seedBytes = GenerateMnemonicBytes(words);
                }
                //var lang = _english;
                //var Language_list = WordListLanguage.ChineseTraditional;
                //var seed = EntropyToMnemonic(seedBytes, _english, WordListLanguage.English);
                var seed = EntropyToMnemonic(seedBytes, lang, Language_list);
                //var seed = "edge shift acquire essence sniff ankle ten prevent december drama churn feel shed ring pair curve biology ability equal cherry yellow blush abuse drift";
                //Console.WriteLine($"Seed: {seed}");
                var saltByte = Encoding.UTF8.GetBytes(salt);
                var masterSecret = Encoding.UTF8.GetBytes(bitcoinSeed);
                var BIP39SeedByte = new Rfc2898DeriveBytes(seed, saltByte, 2048, HashAlgorithmName.SHA512).GetBytes(64);
                //var BIP39Seed = BytesToHexString(BIP39SeedByte);
                var masterPrivateKey = new byte[32]; // Master private key
                var masterChainCode = new byte[32]; // Master chain code
                //Console.WriteLine($"BIP39Seed: {BIP39Seed}");
                var hmac = new HMACSHA512(masterSecret);
                var i = hmac.ComputeHash(BIP39SeedByte);
                Buffer.BlockCopy(i, 0, masterPrivateKey, 0, 32);
                Buffer.BlockCopy(i, 32, masterChainCode, 0, 32);
                //Console.WriteLine($"Master Private Key: {BytesToHexString(masterPrivateKey)}");
                //Console.WriteLine($"Master Chain Code: {BytesToHexString(masterChainCode)}");
                foreach (var path in PATH)
                {
                    int a = 0;
                    while (a <= derivation)
                    {
                        string DER_PATH = path + '/' + a + hardened;
                        //Console.WriteLine(DER_PATH);
                        byte[] PrivateKey = GetChildKey(masterPrivateKey, masterChainCode, DER_PATH);
                        var range = BytesToHexString(PrivateKey).Length;
                        if (range != 64)
                        {
                            while (range != 64)
                            {
                                //Console.WriteLine("D!");
                                string PVK64 = BytesToHexString(PrivateKey);
                                //Console.WriteLine(PVK64);
                                PrivateKey = StringToByteArray("00" + PVK64);
                                //Console.WriteLine("Проверка ключа ппосле добвки нулей " + BytesToHexString(PrivateKey));
                                range++;
                                range++;
                            }
                        }
                        if (IncrementalSearch != 0)
                        {
                            long n = 0;
                            BigInteger PrivateDEC = BigInteger.Parse("0" + BytesToHexString(PrivateKey), System.Globalization.NumberStyles.AllowHexSpecifier);
                            while (n <= IncrementalSearch)
                            {
                                //Считаем Приватник + N число
                                var PrivateDEC1 = PrivateDEC - (n * Step);
                                var PrivateDEC0 = PrivateDEC + (n * Step);
                                //Console.WriteLine(PrivateDEC.ToString("X32"));
                                var PrivateHEX1 = PrivateDEC1.ToString("X32");
                                var PrivateHEX0 = PrivateDEC0.ToString("X32");

                                //Console.WriteLine(PrivateDEC0.ToString("X32"));
                                //Console.WriteLine(PrivateDEC1.ToString("X32"));
                                range = PrivateDEC.ToString("X32").Length;
                                if (range < 64)
                                {
                                    while (range < 64)
                                    {
                                        PrivateHEX0 = '0' + PrivateHEX0;
                                        PrivateHEX1 = '0' + PrivateHEX1;
                                        range++;
                                    }
                                }
                                else if (range > 64)
                                {
                                    PrivateHEX0 = PrivateHEX0[^64..];
                                    PrivateHEX1 = PrivateHEX1[^64..];
                                }
                                var PrivateKeyCHILD = StringToByteArray(PrivateHEX0);
                                var Public_key_compressed = Secp256K1Manager.GetPublicKey(PrivateKeyCHILD, true);
                                var Public_key_uncompressed = Secp256K1Manager.GetPublicKey(PrivateKeyCHILD, false);
                                bool flag = HasBalance(BytesToHexString(Public_key_compressed));
                                bool flag1 = HasBalance(BytesToHexString(Public_key_uncompressed));
                                //var address_segwit = GetSegWit_base58(Public_key_compressed);
                                //var eth_adr = GetEthAddress(Public_key_uncompressed);
                                Interlocked.Increment(ref Total);
                                if ((flag != false) || (flag1 != false))
                                {
                                    Console.WriteLine(BytesToHexString(Public_key_compressed) + "\n" + BytesToHexString(Public_key_uncompressed) + "\nmnemonic: " + seed + "\nPrivate Key: " + BytesToHexString(PrivateKeyCHILD) + "   N = +" + (n*Step).ToString("X8") + "\nEntropy: " + BytesToHexString(seedBytes) + "\nDer.PATH: " + DER_PATH + "\nTotal: " + Total + " Found: " + Found + " Speed: " + speed + '\n');
                                    string contents = string.Format($" \n\nPublic Keys: \n{BytesToHexString(Public_key_compressed)}  Balance: {flag}  \n{BytesToHexString(Public_key_uncompressed)} Balance: {flag1} \nPrivate Key: {BytesToHexString(PrivateKeyCHILD)} \nEntropy: {BytesToHexString(seedBytes)}   \nMnemonic phrase: {seed} \nDerivation PATH: {DER_PATH}");
                                    object outFileLock = Program.outFileLock;
                                    bool lockTaken = false;
                                    try
                                    {
                                        Monitor.Enter(outFileLock, ref lockTaken);
                                        File.AppendAllText("FOUND.txt", contents);
                                    }
                                    finally
                                    {
                                        if (lockTaken)
                                            Monitor.Exit(outFileLock);
                                    }
                                    Interlocked.Increment(ref Found);

                                }
                                else
                                {
                                    if (Silent == false)
                                    {
                                        double Elapsed_MS = sw.ElapsedTicks;
                                        cur = Elapsed_MS / Constant;
                                        cur = Math.Round(cur);
                                        speed = Total / (Elapsed_MS / Constant);
                                        speed = Math.Round(speed);
                                        Console.WriteLine(BytesToHexString(Public_key_compressed) + "\n" + BytesToHexString(Public_key_uncompressed) + "\nmnemonic: " + seed + "\nPrivate Key: " + BytesToHexString(PrivateKeyCHILD) + "   N = +" + (n*Step).ToString("X8") + "\nEntropy: " + BytesToHexString(seedBytes) + "\nDer.PATH: " + DER_PATH + "\nTotal: " + Total + " Found: " + Found + " Speed: " + speed + '\n');

                                    }
                                }

                                PrivateKeyCHILD = StringToByteArray(PrivateHEX1);
                                Public_key_compressed = Secp256K1Manager.GetPublicKey(PrivateKeyCHILD, true);
                                Public_key_uncompressed = Secp256K1Manager.GetPublicKey(PrivateKeyCHILD, false);
                                flag = HasBalance(BytesToHexString(Public_key_compressed));
                                flag1 = HasBalance(BytesToHexString(Public_key_uncompressed));
                                //var address_segwit = GetSegWit_base58(Public_key_compressed);
                                //var eth_adr = GetEthAddress(Public_key_uncompressed);
                                Interlocked.Increment(ref Total);
                                if ((flag != false) || (flag1 != false))
                                {
                                    Console.WriteLine(BytesToHexString(Public_key_compressed) + "\n" + BytesToHexString(Public_key_uncompressed) + "\nmnemonic: " + seed + "\nPrivate Key: " + BytesToHexString(PrivateKeyCHILD) + "   N = -" + (n*Step).ToString("X8") + "\nEntropy: " + BytesToHexString(seedBytes) + "\nDer.PATH: " + DER_PATH + "\nTotal: " + Total + " Found: " + Found + " Speed: " + speed + '\n');
                                    string contents = string.Format($" \n\nPublic Keys: \n{BytesToHexString(Public_key_compressed)}  Balance: {flag}  \n{BytesToHexString(Public_key_uncompressed)} Balance: {flag1} \nPrivate Key: {BytesToHexString(PrivateKeyCHILD)} \nEntropy: {BytesToHexString(seedBytes)}   \nMnemonic phrase: {seed} \nDerivation PATH: {DER_PATH}");
                                    object outFileLock = Program.outFileLock;
                                    bool lockTaken = false;
                                    try
                                    {
                                        Monitor.Enter(outFileLock, ref lockTaken);
                                        File.AppendAllText("FOUND.txt", contents);
                                    }
                                    finally
                                    {
                                        if (lockTaken)
                                            Monitor.Exit(outFileLock);
                                    }
                                    Interlocked.Increment(ref Found);

                                }
                                else
                                {
                                    if (Silent == false)
                                    {
                                        double Elapsed_MS = sw.ElapsedTicks;
                                        cur = Elapsed_MS / Constant;
                                        cur = Math.Round(cur);
                                        speed = Total / (Elapsed_MS / Constant);
                                        speed = Math.Round(speed);
                                        Console.WriteLine(BytesToHexString(Public_key_compressed) + "\n" + BytesToHexString(Public_key_uncompressed) + "\nmnemonic: " + seed + "\nPrivate Key: " + BytesToHexString(PrivateKeyCHILD) + "   N = -" + (n*Step).ToString("X8") + "\nEntropy: " + BytesToHexString(seedBytes) + "\nDer.PATH: " + DER_PATH + "\nTotal: " + Total + " Found: " + Found + " Speed: " + speed + '\n');

                                    }
                                }
                                n++;

                            }
                        }
                        else
                        {
                            var Public_key_compressed = Secp256K1Manager.GetPublicKey(PrivateKey, true);
                            var Public_key_uncompressed = Secp256K1Manager.GetPublicKey(PrivateKey, false);



                            //var address_compressed = GetAddress(Public_key_compressed);
                            //var address_uncompressed = GetAddress(Public_key_uncompressed);
                            bool flag = HasBalance(BytesToHexString(Public_key_compressed));
                            bool flag1 = HasBalance(BytesToHexString(Public_key_uncompressed));
                            //var address_segwit = GetSegWit_base58(Public_key_compressed);
                            //var eth_adr = GetEthAddress(Public_key_uncompressed);
                            Interlocked.Increment(ref Total);
                            if ((flag != false) || (flag1 != false))
                            {
                                Console.WriteLine(BytesToHexString(Public_key_compressed) + "\n" + BytesToHexString(Public_key_uncompressed) + "\nmnemonic: " + seed + "\nPrivate Key: " + BytesToHexString(PrivateKey) + "\nEntropy: " + BytesToHexString(seedBytes) + "\nDer.PATH: " + DER_PATH + "\nTotal: " + Total + " Found: " + Found + " Speed: " + speed + '\n');
                                string contents = string.Format($" \n\nPublic Keys: \n{BytesToHexString(Public_key_compressed)}  Balance: {flag}  \n{BytesToHexString(Public_key_uncompressed)} Balance: {flag1} \nPrivate Key: {BytesToHexString(PrivateKey)} \nEntropy: {BytesToHexString(seedBytes)}   \nMnemonic phrase: {seed} \nDerivation PATH: {DER_PATH}");
                                object outFileLock = Program.outFileLock;
                                bool lockTaken = false;
                                try
                                {
                                    Monitor.Enter(outFileLock, ref lockTaken);
                                    File.AppendAllText("FOUND.txt", contents);
                                }
                                finally
                                {
                                    if (lockTaken)
                                        Monitor.Exit(outFileLock);
                                }
                                Interlocked.Increment(ref Found);

                            }
                            else
                            {
                                if (Silent == false)
                                {
                                    double Elapsed_MS = sw.ElapsedTicks;
                                    cur = Elapsed_MS / Constant;
                                    cur = Math.Round(cur);
                                    speed = Total / (Elapsed_MS / Constant);
                                    speed = Math.Round(speed);
                                    Console.WriteLine(BytesToHexString(Public_key_compressed) + "\n" + BytesToHexString(Public_key_uncompressed) + "\nmnemonic: " + seed + "\nPrivate Key: " + BytesToHexString(PrivateKey) + "\nEntropy: " + BytesToHexString(seedBytes) + "\nDer.PATH: " + DER_PATH + "\nTotal: " + Total + " Found: " + Found + " Speed: " + speed + '\n');

                                }
                            }
                        }
                        a++;
                    }
                }
            }
        }

        private static void WorkerThread5()
        {
            Console.OutputEncoding = Encoding.UTF8;
            //int processorCount = Environment.ProcessorCount;
            //long Total = 0;
            InitializationWordList();
            //var lang = _english;
            //var Language_list = WordListLanguage.ChineseTraditional;
            switch (language)
            {
                case "EN":
                    {
                        lang = _english;
                        Language_list = WordListLanguage.English;
                        break;
                    }
                case "CS":
                    {
                        lang = _chineseSimplified;
                        Language_list = WordListLanguage.ChineseSimplified;
                        break;
                    }
                case "CT":
                    {
                        lang = _chineseTraditional;
                        Language_list = WordListLanguage.ChineseTraditional;
                        break;
                    }
                case "FR":
                    {
                        lang = _french;
                        Language_list = WordListLanguage.French;
                        break;
                    }
                case "IT":
                    {
                        lang = _italian;
                        Language_list = WordListLanguage.Italian;
                        break;
                    }
                case "JA":
                    {
                        lang = _japanese;
                        Language_list = WordListLanguage.Japanese;
                        break;
                    }
                case "KO":
                    {
                        lang = _korean;
                        Language_list = WordListLanguage.Korean;
                        break;
                    }
                case "SP":
                    {
                        lang = _spanish;
                        Language_list = WordListLanguage.Spanish;
                        break;
                    }
                default:
                    {
                        lang = _english;
                        Language_list = WordListLanguage.English;
                        break;
                    }
            }
            sw.Start();
            while (true)
            {
                //Шаг 1. Получаем энтропию.
                //var seedBytes = GenerateMnemonicBytes(words);
                byte[] seedBytes;
                if (IncrementalEntropy)
                {
                    seedBytes = StringToByteArray(Entropy.ToString(EntropyLength));
                    Entropy = Entropy + EntropyStep;
                }
                else
                {
                    seedBytes = GenerateMnemonicBytes(words);
                }
                //var lang = _english;
                //var Language_list = WordListLanguage.ChineseTraditional;
                //var seed = EntropyToMnemonic(seedBytes, _english, WordListLanguage.English);
                var seed = EntropyToMnemonic(seedBytes, lang, Language_list);
                //var seed = "edge shift acquire essence sniff ankle ten prevent december drama churn feel shed ring pair curve biology ability equal cherry yellow blush abuse drift";
                //Console.WriteLine($"Seed: {seed}");
                var saltByte = Encoding.UTF8.GetBytes(salt);
                var masterSecret = Encoding.UTF8.GetBytes(bitcoinSeed);
                var BIP39SeedByte = new Rfc2898DeriveBytes(seed, saltByte, 2048, HashAlgorithmName.SHA512).GetBytes(64);
                //var BIP39Seed = BytesToHexString(BIP39SeedByte);
                var masterPrivateKey = new byte[32]; // Master private key
                var masterChainCode = new byte[32]; // Master chain code
                //Console.WriteLine($"BIP39Seed: {BIP39Seed}");
                var hmac = new HMACSHA512(masterSecret);
                var i = hmac.ComputeHash(BIP39SeedByte);
                Buffer.BlockCopy(i, 0, masterPrivateKey, 0, 32);
                Buffer.BlockCopy(i, 32, masterChainCode, 0, 32);
                //Console.WriteLine($"Master Private Key: {BytesToHexString(masterPrivateKey)}");
                //Console.WriteLine($"Master Chain Code: {BytesToHexString(masterChainCode)}");
                foreach (var path in PATH)
                {
                    int a = 0;
                    while (a <= derivation)
                    {
                        string DER_PATH = path + '/' + a + hardened;
                        //Console.WriteLine(DER_PATH);
                        byte[] PrivateKey = GetChildKey(masterPrivateKey, masterChainCode, DER_PATH);
                        var range = BytesToHexString(PrivateKey).Length;
                        if (range != 64)
                        {
                            while (range != 64)
                            {
                                //Console.WriteLine("D!");
                                string PVK64 = BytesToHexString(PrivateKey);
                                //Console.WriteLine(PVK64);
                                PrivateKey = StringToByteArray("00" + PVK64);
                                //Console.WriteLine("Проверка ключа ппосле добвки нулей " + BytesToHexString(PrivateKey));
                                range++;
                                range++;
                            }
                        }
                        if (IncrementalSearch != 0)
                        {
                            long n = 0;
                            BigInteger PrivateDEC = BigInteger.Parse("0" + BytesToHexString(PrivateKey), System.Globalization.NumberStyles.AllowHexSpecifier);
                            while (n <= IncrementalSearch)
                            {
                                //Считаем Приватник + - N число
                                var PrivateDEC1 = PrivateDEC - (n * Step);
                                var PrivateDEC0 = PrivateDEC + (n * Step);
                                //Console.WriteLine(PrivateDEC.ToString("X32"));
                                var PrivateHEX1 = PrivateDEC1.ToString("X32");
                                var PrivateHEX0 = PrivateDEC0.ToString("X32");

                                //Console.WriteLine(PrivateDEC0.ToString("X32"));
                                //Console.WriteLine(PrivateDEC1.ToString("X32"));
                                range = PrivateDEC.ToString("X32").Length;
                                if (range < 64)
                                {
                                    while (range < 64)
                                    {
                                        PrivateHEX0 = '0' + PrivateHEX0;
                                        PrivateHEX1 = '0' + PrivateHEX1;
                                        range++;
                                    }
                                }
                                else if (range > 64)
                                {
                                    PrivateHEX0 = PrivateHEX0[^64..];
                                    PrivateHEX1 = PrivateHEX1[^64..];
                                }
                                Console.WriteLine(PrivateHEX0);
                                Console.WriteLine(PrivateHEX1);
                                n++;
                            }
                        }
                        else
                        {
                            Console.WriteLine(BytesToHexString(PrivateKey));
                        }
                        a++;
                    }
                }
            }
        }


        private static void WorkerThreadMnemonicGen()
        {
            Console.OutputEncoding = Encoding.UTF8;
            //int processorCount = Environment.ProcessorCount;
            //long Total = 0;
            InitializationWordList();
            //var lang = _english;
            //var Language_list = WordListLanguage.ChineseTraditional;
            switch (language)
            {
                case "EN":
                    {
                        lang = _english;
                        Language_list = WordListLanguage.English;
                        break;
                    }
                case "CS":
                    {
                        lang = _chineseSimplified;
                        Language_list = WordListLanguage.ChineseSimplified;
                        break;
                    }
                case "CT":
                    {
                        lang = _chineseTraditional;
                        Language_list = WordListLanguage.ChineseTraditional;
                        break;
                    }
                case "FR":
                    {
                        lang = _french;
                        Language_list = WordListLanguage.French;
                        break;
                    }
                case "IT":
                    {
                        lang = _italian;
                        Language_list = WordListLanguage.Italian;
                        break;
                    }
                case "JA":
                    {
                        lang = _japanese;
                        Language_list = WordListLanguage.Japanese;
                        break;
                    }
                case "KO":
                    {
                        lang = _korean;
                        Language_list = WordListLanguage.Korean;
                        break;
                    }
                case "SP":
                    {
                        lang = _spanish;
                        Language_list = WordListLanguage.Spanish;
                        break;
                    }
                default:
                    {
                        lang = _english;
                        Language_list = WordListLanguage.English;
                        break;
                    }
            }
            sw.Start();
            while (true)
            {
                //Шаг 1. Получаем энтропию.
                //var seedBytes = GenerateMnemonicBytes(words);
                byte[] seedBytes;
                if (IncrementalEntropy)
                {
                    seedBytes = StringToByteArray(Entropy.ToString(EntropyLength));
                    Entropy = Entropy + EntropyStep;
                }
                else
                {
                    seedBytes = GenerateMnemonicBytes(words);
                }
                //var lang = _english;
                //var Language_list = WordListLanguage.ChineseTraditional;
                //var seed = EntropyToMnemonic(seedBytes, _english, WordListLanguage.English);
                var seed = EntropyToMnemonic(seedBytes, lang, Language_list);
                Console.WriteLine(seed);
                //var seed = "edge shift acquire essence sniff ankle ten prevent december drama churn feel shed ring pair curve biology ability equal cherry yellow blush abuse drift";
                //Console.WriteLine($"Seed: {seed}");
                //var saltByte = Encoding.UTF8.GetBytes(salt);
                //var masterSecret = Encoding.UTF8.GetBytes(bitcoinSeed);
                //var BIP39SeedByte = new Rfc2898DeriveBytes(seed, saltByte, 2048, HashAlgorithmName.SHA512).GetBytes(64);
                ////var BIP39Seed = BytesToHexString(BIP39SeedByte);
                //var masterPrivateKey = new byte[32]; // Master private key
                //var masterChainCode = new byte[32]; // Master chain code
                ////Console.WriteLine($"BIP39Seed: {BIP39Seed}");
                //var hmac = new HMACSHA512(masterSecret);
                //var i = hmac.ComputeHash(BIP39SeedByte);
                //Buffer.BlockCopy(i, 0, masterPrivateKey, 0, 32);
                //Buffer.BlockCopy(i, 32, masterChainCode, 0, 32);
                ////Console.WriteLine($"Master Private Key: {BytesToHexString(masterPrivateKey)}");
                ////Console.WriteLine($"Master Chain Code: {BytesToHexString(masterChainCode)}");
                //foreach (var path in PATH)
                //{
                //    int a = 0;
                //    while (a <= derivation)
                //    {
                //        string DER_PATH = path + '/' + a + hardened;
                //        //Console.WriteLine(DER_PATH);
                //        byte[] PrivateKey = GetChildKey(masterPrivateKey, masterChainCode, DER_PATH);
                //        var range = BytesToHexString(PrivateKey).Length;
                //        if (range != 64)
                //        {
                //            while (range != 64)
                //            {
                //                //Console.WriteLine("D!");
                //                string PVK64 = BytesToHexString(PrivateKey);
                //                //Console.WriteLine(PVK64);
                //                PrivateKey = StringToByteArray("00" + PVK64);
                //                //Console.WriteLine("Проверка ключа ппосле добвки нулей " + BytesToHexString(PrivateKey));
                //                range++;
                //                range++;
                //            }
                //        }
                //        if (IncrementalSearch != 0)
                //        {
                //            long n = 0;
                //            BigInteger PrivateDEC = BigInteger.Parse("0" + BytesToHexString(PrivateKey), System.Globalization.NumberStyles.AllowHexSpecifier);
                //            while (n <= IncrementalSearch)
                //            {
                //                //Считаем Приватник + - N число
                //                var PrivateDEC1 = PrivateDEC - (n * Step);
                //                var PrivateDEC0 = PrivateDEC + (n * Step);
                //                //Console.WriteLine(PrivateDEC.ToString("X32"));
                //                var PrivateHEX1 = PrivateDEC1.ToString("X32");
                //                var PrivateHEX0 = PrivateDEC0.ToString("X32");

                //                //Console.WriteLine(PrivateDEC0.ToString("X32"));
                //                //Console.WriteLine(PrivateDEC1.ToString("X32"));
                //                range = PrivateDEC.ToString("X32").Length;
                //                if (range < 64)
                //                {
                //                    while (range < 64)
                //                    {
                //                        PrivateHEX0 = '0' + PrivateHEX0;
                //                        PrivateHEX1 = '0' + PrivateHEX1;
                //                        range++;
                //                    }
                //                }
                //                else if (range > 64)
                //                {
                //                    PrivateHEX0 = PrivateHEX0[^64..];
                //                    PrivateHEX1 = PrivateHEX1[^64..];
                //                }
                //                Console.WriteLine(PrivateHEX0);
                //                Console.WriteLine(PrivateHEX1);
                //                n++;
                //            }
                //        }
                //        else
                //        {
                //            Console.WriteLine(BytesToHexString(PrivateKey));
                //        }
                //        a++;
                //    }
                //}
            }
        }


        private static HashSet<string> LoadDatabase(string filePath)
        {
            HashSet<string> stringSet = new();
            foreach (string readLine in File.ReadLines(filePath))
            {
                string[] strArray = readLine.Split('\t');
                stringSet.Add(strArray[0]);
            }
            return stringSet;
        }

        private static bool HasBalance(string address) => _addressDb.Contains(address);

        private static void InitializationWordList()
        {
            _english = GetWordList(WordListLanguage.English);
            _chineseSimplified = GetWordList(WordListLanguage.ChineseSimplified);
            _chineseTraditional = GetWordList(WordListLanguage.ChineseTraditional);
            _french = GetWordList(WordListLanguage.French);
            _italian = GetWordList(WordListLanguage.Italian);
            _japanese = GetWordList(WordListLanguage.Japanese);
            _korean = GetWordList(WordListLanguage.Korean);
            _spanish = GetWordList(WordListLanguage.Spanish);
        }

        private static byte[] GetChildKey(byte[] Private, byte[] Chain, string PATH)
        {
            string[] Derivation = PATH.Split('/');

            byte[] masterPrivateKey = Private;
            byte[] masterChainCode = Chain;
            var DER = new List<long>();
            //Console.WriteLine(Derivation[0]);
            foreach (string sub in Derivation)
            {
                if (sub.Contains("'"))
                {
                    //Console.WriteLine(sub);
                    var ser = Int32.Parse(sub[..^1]);
                    DER.Add(0x80000000 + ser);
                }
                else
                {
                    //Console.WriteLine(sub);
                    DER.Add(Int32.Parse(sub));
                }
            }
            string keys;
            byte[] key, D;

            byte[] PrivateKey = new byte[32]; //private key
            byte[] ChainCode = new byte[32];
            //for (var n = 0; n < DER.Count; n++)
            foreach (var n in DER)
            //var n;
            //Parallel.ForEach(DER, n)
            {

                var k = masterChainCode;
                string d;
                if ((n & 0x80000000) != 0)
                {
                    byte[] b = ASCIIEncoding.ASCII.GetBytes("\x00");
                    keys = BytesToHexString(b) + BytesToHexString(masterPrivateKey);
                    key = StringToByteArray(keys);
                }
                else
                {
                    int range = BytesToHexString(masterPrivateKey).Length;
                    //Console.WriteLine("Проверка ключа перед ошибкой " + BytesToHexString(masterPrivateKey) + " Длинна: " + range);

                    if (range != 64)
                    {
                        while (range != 64)
                        {
                            //Console.WriteLine("DAA!");
                            string PVK64 = BytesToHexString(masterPrivateKey);
                            //Console.WriteLine(PVK64);
                            masterPrivateKey = StringToByteArray("00" + PVK64);
                            //Console.WriteLine("Проверка ключа ппосле добвки нулей " + BytesToHexString(masterPrivateKey));
                            range++;
                            range++;
                        }
                    }
                    key = Cryptography.ECDSA.Secp256K1Manager.GetPublicKey(masterPrivateKey, true);
                }
                d = BytesToHexString(key) + (n*Step).ToString("X8");
                D = StringToByteArray(d);
                while (true)
                {
                    var HMAC = new HMACSHA512(k);
                    var h = HMAC.ComputeHash(D);

                    Buffer.BlockCopy(h, 0, PrivateKey, 0, 32);
                    Buffer.BlockCopy(h, 32, ChainCode, 0, 32);
                    BigInteger a = BigInteger.Parse("0" + BytesToHexString(PrivateKey), System.Globalization.NumberStyles.AllowHexSpecifier);
                    BigInteger b = BigInteger.Parse("0" + BytesToHexString(masterPrivateKey), System.Globalization.NumberStyles.AllowHexSpecifier);
                    BigInteger key1 = (a + b) % order;
                    if ((key1.ToString("X32").Length > 64))
                    {
                        var key_string = key1.ToString("X32")[^64..];
                        key1 = BigInteger.Parse(key_string, System.Globalization.NumberStyles.AllowHexSpecifier);
                    }
                    if ((a < order) && (key1 != 0))
                    {
                        key = key1.ToByteArray(false, true);
                        masterPrivateKey = key;
                        masterChainCode = ChainCode;
                        break;
                    }
                    byte[] b2 = ASCIIEncoding.ASCII.GetBytes("\x01");
                    var dd = BytesToHexString(b2) + BytesToHexString(ChainCode) + (n*Step).ToString("X8");
                    D = StringToByteArray(dd);

                }
            }
            return masterPrivateKey;
        }

        private static string BytesToBinary(byte[] hash)
        {
            return string.Join("", hash.Select(h => LeftPad(Convert.ToString(h, 2), "0", 8)));
        }

        private static string LeftPad(string str, string padString, int length)
        {
            while (str.Length < length)
            {
                str = padString + str;
            }

            return str;
        }

        private static string DeriveChecksumBits(byte[] checksum)
        {
            var ent = checksum.Length * 8;
            var cs = ent / 32;

            var sha256Provider = new SHA256CryptoServiceProvider();
            var hash = sha256Provider.ComputeHash(checksum);
            var result = BytesToBinary(hash);
            return result.Substring(0, cs);
        }

        private static string[] GetWordList(WordListLanguage wordList)
        {
            var wordLists = new Dictionary<string, string>
            {
                {WordListLanguage.ChineseSimplified.ToString(), "chinese_simplified"},
                {WordListLanguage.ChineseTraditional.ToString(), "chinese_traditional"},
                {WordListLanguage.English.ToString(), "english"},
                {WordListLanguage.French.ToString(), "french"},
                {WordListLanguage.Italian.ToString(), "italian"},
                {WordListLanguage.Japanese.ToString(), "japanese"},
                {WordListLanguage.Korean.ToString(), "korean"},
                {WordListLanguage.Spanish.ToString(), "spanish"}
            };

            var wordListFile = wordLists[wordList.ToString()];

            var assembly = Assembly.GetAssembly(typeof(WordListLanguage));
            var wordListFileStream = assembly.GetManifestResourceStream($"{typeof(WordListLanguage).Namespace}.Words.{wordListFile}.txt");

            var words = new List<string>();
            using (var reader = new StreamReader(wordListFileStream ?? throw new InvalidOperationException($"could not load word list for {wordList}")))
            {
                while (reader.Peek() >= 0)
                {
                    words.Add(reader.ReadLine());
                }
            }

            var wordListResults = words.ToArray();
            return wordListResults;
        }

        public static string EntropyToMnemonic(byte[] entropyBytes, string[] wordList, WordListLanguage wordListType)
        {
            CheckValidEntropy(entropyBytes);

            var entropyBits = BytesToBinary(entropyBytes);
            var checksumBits = DeriveChecksumBits(entropyBytes);

            var bits = entropyBits + checksumBits;

            var chunks = Regex.Matches(bits, "(.{1,11})")
                .OfType<Match>()
                .Select(m => m.Groups[0].Value)
                .ToArray();

            var words = chunks.Select(binary =>
            {
                var index = Convert.ToInt32(binary, 2);
                return wordList[index];
            });

            var joinedText = string.Join((wordListType == WordListLanguage.Japanese ? "\u3000" : " "), words);

            return joinedText;
        }

        private static void CheckValidEntropy(byte[] entropyBytes)
        {
            if (entropyBytes == null)
                throw new ArgumentNullException(nameof(entropyBytes));

            if (entropyBytes.Length < 4)
                throw new ArgumentException(InvalidEntropy);

            if (entropyBytes.Length > 32)
                throw new ArgumentException(InvalidEntropy);

            if (entropyBytes.Length % 4 != 0)
                throw new ArgumentException(InvalidEntropy);
        }

        public static byte[] GenerateMnemonicBytes(int strength)
        {
            if (strength % 32 != 0)
                throw new NotSupportedException(InvalidEntropy);

            var rngCryptoServiceProvider = new RNGCryptoServiceProvider();

            var buffer = new byte[strength / 8];
            rngCryptoServiceProvider.GetBytes(buffer);

            return buffer;
        }

        public static string GetHash160(byte[] public_key)
        {

            var SHA256 = Sha256Manager.GetHash(public_key);
            var hash160 = BytesToHexString(Ripemd160Manager.GetHash(SHA256));

            return hash160;

        }

        public static string GetAddress(byte[] public_key)
        {

            var SHA256 = Sha256Manager.GetHash(public_key);
            var hash160 = Ripemd160Manager.GetHash(SHA256);
            //Console.WriteLine("hash160: " + BytesToHexString(hash160));
            var add_prefix = StringToByteArray(prefix_legacy + BytesToHexString(hash160));
            SHA256 = Sha256Manager.GetHash(add_prefix);
            SHA256 = Sha256Manager.GetHash(SHA256);
            var checksum = StringToByteArray(BytesToHexString(add_prefix) + BytesToHexString(SHA256)[..8]);
            var address = Base58.Encode(checksum);
            //Console.WriteLine("address: " + comressed_address);

            return address;

        }
        public static string GetEthAddress(byte[] public_key)
        {

            var pub = StringToByteArray(BytesToHexString(public_key)[^128..]);
            var keccak = Keccak256.ComputeHash(pub);
            var address = BytesToHexString(keccak)[^40..];
            return address;
        }
        public static string GetSegWit_base58(byte[] public_key)
        {
            var SHA256 = Sha256Manager.GetHash(public_key);
            var hash160 = Ripemd160Manager.GetHash(SHA256);
            var add_prefix = StringToByteArray("0014" + BytesToHexString(hash160));
            SHA256 = Sha256Manager.GetHash(add_prefix);
            hash160 = Ripemd160Manager.GetHash(SHA256);
            add_prefix = StringToByteArray(prefix_segwit + BytesToHexString(hash160));
            SHA256 = Sha256Manager.GetHash(add_prefix);
            SHA256 = Sha256Manager.GetHash(SHA256);
            var checksum = StringToByteArray(BytesToHexString(add_prefix) + BytesToHexString(SHA256)[..8]);
            var address = Base58.Encode(checksum);

            return address;

        }
        // <summary>
        /// Turns a byte array into a Hex encoded string
        /// </summary>
        /// <param name="bytes">The bytes to encode to hex</param>
        /// <returns>The hex encoded representation of the bytes</returns>
        public static string BytesToHexString(byte[] bytes, bool upperCase = false)
        {
            if (upperCase)
            {
                return string.Concat(bytes.Select(byteb => byteb.ToString("X2")).ToArray());
            }
            else
            {
                return string.Concat(bytes.Select(byteb => byteb.ToString("x2")).ToArray());
            }
        }

        public static byte[] StringToByteArray(string hex)
        {
            return Enumerable.Range(0, hex.Length)
                             .Where(x => x % 2 == 0)
                             .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                             .ToArray();
        }
    }
}
