using NBitcoin;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using Nethereum.Web3;
using Nethereum.Web3.Accounts;
using Nethereum.Util;
using Nethereum.Hex.HexConvertors.Extensions;
using Nethereum.HdWallet;

namespace bitcoin_wallet_check_csharp
{
    internal class Program
    {
        private const string WET_ENTIES_FILE_NAME = "wet.txt";
        private static readonly object outFileLock = new();
        private static bool IsRunning = true;
        private static bool Silent;
        private static long Total = 0;
        private static long Wet = 0;
        private static long speed = 0;
        private static long speedAD = 0;
        private static HashSet<string> _addressDb;
        private static string? win;
        static DateTime t1, t2;
        private static long cur;
        private static string PATH, PATH1;
        private static int words;
        private static int derivation;
        private static int mode, sil;
        private static string? BIP39_Passphrase;

        private static void Main(string[] args)
        {
            if (args.Length < 1)
            {
                Console.WriteLine("Not enough args!");
            }
            else
            {
                string filePath = args[0];
                win = Environment.MachineName;
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

                if (win == "XOPMC")
                {
                    while (true)
                        Console.WriteLine("Ты чертила): @XopMC." + Environment.MachineName);
                }
                else
                {
                    int processorCount = Environment.ProcessorCount;
                    Console.WriteLine("Loading database addresses... | Загрузка базы адресов\n");
                    Console.WriteLine("Choose mode: 1 - BTC, 2 - ETH, 3 - BTC + ETH| Выберите режим:  1 - BTC, 2 - ETH, 3 - BTC + ETH ");
                    try
                    {
                        mode = Convert.ToInt32(Console.ReadLine());
                    }
                    catch (FormatException)
                    {
                        Console.WriteLine("Wrong mode! Starting with BTC mode | Некорректный режим! Запускаю режим BTC");
                        mode = 1;
                    }
                    if ((mode != 1) && (mode != 2) && (mode != 3))
                    {
                        Console.WriteLine("Wrong mode! Starting with BTC mode | Некорректный режим! Запускаю режим BTC");
                        mode = 1;
                    }
                    Console.WriteLine("Input 1 - for normal mode/ 2 - for debug mode | Введите 1 - для Нормального режима/ 2 - для полного отображения: ");
                    try 
                    {
                        sil = Convert.ToInt32(Console.ReadLine());

                    }
                    catch (FormatException)
                    {
                        Console.WriteLine("\nF*ck! wrong number... starting with normal mode | Бля, написано же 1 или 2... выберу сам - выбран нормальный режим..");
                        sil = 1;
                    }

                    if ((sil != 1) && (sil != 2))
                    {
                        Console.WriteLine("\nF*ck! wrong number... starting with normal mode | Бля, написано же 1 или 2... выберу сам - выбран нормальный режим..");
                        sil = 1;
                    }

                    if (sil == 1)
                    {
                        Silent = true;
                    }
                    else if (sil == 2)
                    {
                        Silent = false;
                    }
                    Console.WriteLine("\nMax threads | Доступно потоков: {0} ", processorCount);
                    Console.WriteLine("\nHow many threads should use? | Сколько потоков задействуем? :");
                    int num = Convert.ToInt32(Console.ReadLine());
                    if (num > processorCount)
                    {
                        num = processorCount;
                    }
                    
                    Console.WriteLine("\nStarting {0} threads... | Запускаю {0} поток(а)ов...\n", num);
                    Console.WriteLine("Loaded addresses from {0}  | Загружены адреса из {0} \n", filePath);
                    _addressDb = LoadDatabase(filePath);
                    Console.WriteLine("Input required amount of words for mnemonic ( 12, 15, 18, 21, 24) | Введите количество слов для мнемоники ( 12, 15, 18, 21, 24): ");
                    words = Convert.ToInt32(Console.ReadLine());
                    if ((words != 12) && (words != 15) && (words != 18) && (words != 21) && (words != 24))
                    {
                        Console.WriteLine("\nWrong words count! Starting with 12 words | Введено неверное количество слов, включаю стандартный режим по 12 словам.");
                        words = 12;
                    }
                    if (mode == 1)
                    {
                        Console.WriteLine("\nInput Derivation PATH without m/, (for default 44'/0'/0'/0/0 - press Enter) | Введите путь деривации без m/, (для стандартного 44'/0'/0'/0/0 - Нажмите Enter):");
                        PATH = Console.ReadLine();
                        if (PATH == "")
                        {
                            PATH = "44'/0'/0'/0/0";
                        }
                    }
                    if (mode == 2)
                    {
                        Console.WriteLine("\nInput Derivation PATH without m/, (for default 44'/60'/0'/0/0  - press Enter) | Введите путь деривации без m/, (пля стандартного 44'/60'/0'/0/0  - Нажмите Enter):");
                        PATH = Console.ReadLine();
                        if (PATH == "")
                        {
                            PATH = "44'/60'/0'/0/0";
                        }

                    }
                    if (mode == 3)
                    {
                        Console.WriteLine("\nInput Derivation PATH without m/, (for default 44'/0'/0'/0/0 - press Enter) - For BTC | Введите путь деривации без m/, (для стандартного 44'/0'/0'/0/0 - Нажмите Enter) - Для BTC:");
                        PATH = Console.ReadLine();
                        if (PATH == "")
                        {
                            PATH = "44'/0'/0'/0/0";
                        }
                        Console.WriteLine("\nInput Derivation PATH without m/, (for default 44'/60'/0'/0/0  - press Enter) - For ETH | Введите путь деривации без m/, (пля стандартного 44'/60'/0'/0/0  - Нажмите Enter) - Для ETH:");
                        PATH1 = Console.ReadLine();
                        if (PATH1 == "")
                        {
                            PATH1 = "44'/60'/0'/0/0";
                        }

                    }

                    Console.WriteLine("Input BIP39 Passphrase (if not required, press Enter) | Введите BIP39 Passphrase (Если не нужно, нажмите Enter):");
                    BIP39_Passphrase = Console.ReadLine();
                    Console.WriteLine("\nInput Derivation deep (for example - 5) | Введите глубину деривации (например - 5)");
                    derivation = Convert.ToInt32(Console.ReadLine());
                    Console.WriteLine("\nStarting porgramm with {0} words, with (m/{1}) derivation PATH | Запускаю режим работы по {0} словам, с путём деривации (m/{1}) ", words, PATH);
                    List<Thread> threadList = new();
                    for (int index = 0; index < num; ++index)
                    {
                        Thread thread = new(new ThreadStart(WorkerThread));
                        threadList.Add(thread);
                    }
                    IsRunning = true;
                    foreach (Thread thread in threadList)
                        thread.Start();
                    Console.WriteLine("\nThreads are started... | Потоки запущены...");
                    Console.WriteLine("\nCTRL+C to interrupt the process | CTRL+C для остановки процесса");
                    if (mode == 1)
                        if (Silent == true)
                        {
                            while (IsRunning)
                            {
                                //Console.Write("\r" + new string(' ', Console.WindowWidth - 1) + "\r");
                                Console.Write("\rTotal: {0} | Found: {1} | Speed: {2} Keys/s  | Speed: {3} Addresses/s | Elapsed: {4}", Total, Wet, speed, speedAD, cur);
                                Thread.Sleep(1000);
                            }
                        }
                    else if ((mode == 2) || (mode == 3))
                        if (Silent == true)
                        {
                            while (IsRunning)
                            {
                                //Console.Write("\r" + new string(' ', Console.WindowWidth - 1) + "\r");
                                Console.Write("\rTotal: {0} | Found: {1} | Speed: {2} Keys/s | Elapsed: {3}", Total, Wet, speed, cur);
                                Thread.Sleep(1000);
                            }
                        }
                    Console.CancelKeyPress += new ConsoleCancelEventHandler(MyHandler);

                    foreach (Thread thread in threadList)
                        thread.Join();
                }
            }
        }

        private static void MyHandler(object sender, ConsoleCancelEventArgs e)
        {
            Console.WriteLine("\nInterupting...| Остановка...");
            e.Cancel = true;
            Console.WriteLine("\n\nDeveloped by @XopMC for t.me/brythbit\n\n\n");
            Console.WriteLine("Press any key to exit...");
            Console.ReadLine();
            IsRunning = false;
            Console.CancelKeyPress -= new ConsoleCancelEventHandler(MyHandler);

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

        private static void WorkerThread()
        {
            t1 = DateTime.Now;
            Thread.Sleep(100);
            //KeyPath keyPath = new(PATH);
            if (mode == 1)
            {
                while (true)
                {
                    int a = 0;
                    Mnemonic? mnemonic = null;
                    if (words == 12)
                    {
                        mnemonic = new(Wordlist.English, WordCount.Twelve);
                    }
                    else if (words == 15)
                    {
                        mnemonic = new(Wordlist.English, WordCount.Fifteen);
                    }
                    else if (words == 18)
                    {
                        mnemonic = new(Wordlist.English, WordCount.Eighteen);
                    }
                    else if (words == 21)
                    {
                        mnemonic = new(Wordlist.English, WordCount.TwentyOne);
                    }
                    else if (words == 24)
                    {
                        mnemonic = new(Wordlist.English, WordCount.TwentyFour);
                    }
                    
                    //Mnemonic mnemonic = new("angle bonus rich melody cotton lyrics skate stuff fragile guard fresh snake glance join artefact slender sting craft decorate time absent magic index entry");
                    while (a <= derivation)
                    {
                        string DerPath = PATH.Remove(PATH.Length - 1) + a;
                        NBitcoin.KeyPath keyPath = new(DerPath);
                        ExtKey extKey = mnemonic.DeriveExtKey(BIP39_Passphrase).Derive(keyPath);

                        string address = extKey.Neuter().PubKey.GetAddress(ScriptPubKeyType.Legacy, Network.Main).ToString();
                        string address1 = extKey.Neuter().PubKey.GetAddress(ScriptPubKeyType.Segwit, Network.Main).ToString();
                        string address2 = extKey.Neuter().PubKey.GetAddress(ScriptPubKeyType.SegwitP2SH, Network.Main).ToString();
                        string address3 = extKey.Neuter().PubKey.GetAddress(ScriptPubKeyType.TaprootBIP86, Network.Main).ToString();
                        bool flag = HasBalance(address);
                        bool flag1 = HasBalance(address1);
                        bool flag2 = HasBalance(address2);
                        bool flag3 = HasBalance(address3);

                        Interlocked.Increment(ref Total);
                        t2 = DateTime.Now;
                        cur = ((t2.Ticks) - (t1.Ticks)) / 10000000;
                        if (cur != 0)
                        {
                            speed = Total / cur;
                            speedAD = speed * 4;
                        }
                        else
                        {
                            Thread.Sleep(1000);
                            t2 = DateTime.Now;
                            cur = ((t2.Ticks) - (t1.Ticks)) / 10000000;
                            speed = Total / cur;
                            speedAD = speed * 4;
                        }
                        if ((flag != false) || (flag1 != false) || (flag2 != false) || (flag3 != false))
                        {
                            bool flag0 = true;
                            byte[] pvk = extKey.PrivateKey.GetBitcoinSecret(Network.Main).ToBytes();
                            string hex = BitConverter.ToString(pvk).Replace("-", string.Empty);
                            hex = hex.Remove(hex.Length - 2);
                            Console.Write("\nAddress:\n|Legacy: {0} \n|Segwit: {1} \n|P2SH: {2} \n|BIP86: {3} \n|Mnemonic phrase: {4} \n|Path: {5} \n|PVK: {6}\n Total: {7} | Found: {8} | Speed: {9} Keys/s | Speed: {10} Addresses/s |\n", address, address1, address2, address3, mnemonic, keyPath, hex, Total, Wet, speed, speedAD);
                            string contents = string.Format("|Addresses: {0}  |Balance: {8}\n|{5}  |Balance: {9}\n|{6}  |Balance: {10}\n|{7}  |Balance: {11} \n|Has balance: {1} \n|Mnemonic phrase: {2} \n|Private: {3} \n|PATH: {4}\n\n", address, flag0, mnemonic.ToString(), hex, DerPath.ToString(), address1, address2, address3, flag, flag1, flag2, flag3);
                            object outFileLock = Program.outFileLock;
                            bool lockTaken = false;
                            try
                            {
                                Monitor.Enter(outFileLock, ref lockTaken);
                                File.AppendAllText("wet.txt", contents);
                            }
                            finally
                            {
                                if (lockTaken)
                                    Monitor.Exit(outFileLock);
                            }
                            Interlocked.Increment(ref Wet);
                        }
                        else
                        {
                            if (Silent == false)
                            {
                                byte[] pvk = extKey.PrivateKey.GetBitcoinSecret(Network.Main).ToBytes();
                                string hex = BitConverter.ToString(pvk).Replace("-", string.Empty);
                                hex = hex.Remove(hex.Length - 2);
                                Console.Write("\nAddress:\n|Legacy: {0} \n|Segwit: {1} \n|P2SH: {2} \n|BIP86: {3} \n|Mnemonic phrase: {4} \n|Path: {5} \n|PVK: {6}\n Total: {7} | Found: {8} | Speed: {9} Keys/s | Speed: {10} Addresses/s |\n", address, address1, address2, address3, mnemonic, keyPath, hex, Total, Wet, speed, speedAD);
                            }
                        }
                        a++;
                    }
                }
            }
            else if (mode ==2)
            {
                while (true)
                {
                    int a = 0;
                    Mnemonic? mnemonic = null;
                    if (words == 12)
                    {
                        mnemonic = new(Wordlist.English, WordCount.Twelve);
                    }
                    else if (words == 15)
                    {
                        mnemonic = new(Wordlist.English, WordCount.Fifteen);
                    }
                    else if (words == 18)
                    {
                        mnemonic = new(Wordlist.English, WordCount.Eighteen);
                    }
                    else if (words == 21)
                    {
                        mnemonic = new(Wordlist.English, WordCount.TwentyOne);
                    }
                    else if (words == 24)
                    {
                        mnemonic = new(Wordlist.English, WordCount.TwentyFour);
                    }
                    
                    //Mnemonic mnemonic = new("angle bonus rich melody cotton lyrics skate stuff fragile guard fresh snake glance join artefact slender sting craft decorate time absent magic index entry");
                    while (a <= derivation)
                    {
                        string DerPath = PATH.Remove(PATH.Length - 1) + a;
                        //NBitcoin.KeyPath keyPath = new(DerPath);
                        //.KeyPath keyPath = new(DerPath)
                        //ExtKey extKey = mnemonic.DeriveExtKey().Derive(keyPath);
                        var wallet2 = new Wallet(mnemonic.ToString(), BIP39_Passphrase, PATH.Remove(PATH.Length - 1));
                        var wallet1 = wallet2.GetAccount(a);

                        string address0 = wallet1.Address;
                        bool flag0 = HasBalance(address0);

                        Interlocked.Increment(ref Total);
                        t2 = DateTime.Now;
                        cur = ((t2.Ticks) - (t1.Ticks)) / 10000000;
                        if (cur != 0)
                        {
                            speed = Total / cur;
                        }
                        else
                        {
                            Thread.Sleep(1000);
                            t2 = DateTime.Now;
                            cur = ((t2.Ticks) - (t1.Ticks)) / 10000000;
                            speed = Total / cur;
                        }
                        if (flag0 != false)
                        {
                            byte[] str = wallet2.GetPrivateKey(a);
                            string HEX = BitConverter.ToString(str).Replace("-", string.Empty);
                            string contents = string.Format("|Addresses: {0} \n|Has balance: {1} \n|Mnemonic phrase: {2} \n|Private: {3} \n|PATH: {4}\n\n", (object)address0, (object)flag0.ToString(), (object)mnemonic.ToString(), (object)HEX, (object)DerPath);
                            Console.Write("\nAddress:{0}\n|Mnemonic phrase: {1} \n|Path: {2} \n|PVK: {3}\n Total: {4} | Found: {5} | Speed: {6} Keys/s |\n", address0, mnemonic, DerPath, HEX, Total, Wet, speed);
                            object outFileLock = Program.outFileLock;
                            bool lockTaken = false;
                            try
                            {
                                Monitor.Enter(outFileLock, ref lockTaken);
                                File.AppendAllText("wet.txt", contents);
                            }
                            finally
                            {
                                if (lockTaken)
                                    Monitor.Exit(outFileLock);
                            }
                            Interlocked.Increment(ref Wet);
                        }
                        else
                        {
                            if (Silent == false)
                            {
                                byte[] pvk = wallet2.GetPrivateKey(a);
                                string hex = BitConverter.ToString(pvk).Replace("-", string.Empty);
                                Console.Write("\nAddress:{0}\n|Mnemonic phrase: {1} \n|Path: {2} \n|PVK: {3}\n Total: {4} | Found: {5} | Speed: {6} Keys/s |\n", address0, mnemonic, DerPath, hex, Total, Wet, speed);
                            }
                        }
                        a++;
                    }
                }
            }
            else if (mode == 3)
            {
                while (true)
                {
                    int a = 0;
                    Mnemonic? mnemonic = null;
                    if (words == 12)
                    {
                        mnemonic = new(Wordlist.English, WordCount.Twelve);
                    }
                    else if (words == 15)
                    {
                        mnemonic = new(Wordlist.English, WordCount.Fifteen);
                    }
                    else if (words == 18)
                    {
                        mnemonic = new(Wordlist.English, WordCount.Eighteen);
                    }
                    else if (words == 21)
                    {
                        mnemonic = new(Wordlist.English, WordCount.TwentyOne);
                    }
                    else if (words == 24)
                    {
                        mnemonic = new(Wordlist.English, WordCount.TwentyFour);
                    }
                    
                    //Mnemonic mnemonic = new("angle bonus rich melody cotton lyrics skate stuff fragile guard fresh snake glance join artefact slender sting craft decorate time absent magic index entry");
                    while (a <= derivation)
                    {
                        string DerPath = PATH.Remove(PATH.Length - 1) + a;
                        string EthPath = PATH1.Remove(PATH1.Length - 1) + a;
                        //string DerPath = "m/44'/0'/0'/0/0";
                        NBitcoin.KeyPath keyPath = new(DerPath);
                        ExtKey extKey = mnemonic.DeriveExtKey(BIP39_Passphrase).Derive(keyPath);
                        var wallet2 = new Wallet(mnemonic.ToString(), BIP39_Passphrase, PATH1.Remove(PATH1.Length - 1));
                        var wallet1 = wallet2.GetAccount(a);

                        string address0 = wallet1.Address;
                        string address = extKey.Neuter().PubKey.GetAddress(ScriptPubKeyType.Legacy, Network.Main).ToString();
                        string address1 = extKey.Neuter().PubKey.GetAddress(ScriptPubKeyType.Segwit, Network.Main).ToString();
                        string address2 = extKey.Neuter().PubKey.GetAddress(ScriptPubKeyType.SegwitP2SH, Network.Main).ToString();
                        string address3 = extKey.Neuter().PubKey.GetAddress(ScriptPubKeyType.TaprootBIP86, Network.Main).ToString();
                        bool flag0 = HasBalance(address0);
                        bool flag = HasBalance(address);
                        bool flag1 = HasBalance(address1);
                        bool flag2 = HasBalance(address2);
                        bool flag3 = HasBalance(address3);

                        Interlocked.Increment(ref Total);
                        t2 = DateTime.Now;
                        cur = ((t2.Ticks) - (t1.Ticks)) / 10000000;
                        if (cur != 0)
                        {
                            speed = Total / cur;
                            speedAD = speed * 5;
                        }
                        else
                        {
                            Thread.Sleep(1000);
                            t2 = DateTime.Now;
                            cur = ((t2.Ticks) - (t1.Ticks)) / 10000000;
                            speed = Total / cur;
                            speedAD = speed * 5;
                        }
                        if ((flag != false) || (flag1 != false) || (flag2 != false) || (flag3 != false) || (flag0 != false))
                        {
                            bool FLAG = true;
                            byte[] pvk1 = extKey.PrivateKey.GetBitcoinSecret(Network.Main).ToBytes();
                            string str = BitConverter.ToString(pvk1).Replace("-", string.Empty);
                            str = str.Remove(str.Length - 2);

                            byte[] pvk = wallet2.GetPrivateKey(a);
                            string hex = BitConverter.ToString(pvk).Replace("-", string.Empty);
                            Console.Write("\nAddress:\n|Legacy: {0} \n|Segwit: {1} \n|P2SH: {2} \n|BIP86: {3} \n|ETHEREUM: {11} \n|Mnemonic phrase: {4} \n|Path BTC: {5} \n|Path ETH: {13}\n|PVK BTC: {6} \n|PVK ETH: {12}\n Total: {7} | Found: {8} | Speed: {9} Keys/s | Speed: {10} Addresses/s |\n", address, address1, address2, address3, mnemonic, keyPath, str, Total, Wet, speed, speedAD, address0, hex, EthPath);
                            string contents = string.Format("|Addresses: {0}  |Balance: {8}\n|{5}  |Balance: {9}\n|{6}  |Balance: {10}\n|{7}  |Balance: {11} \n|ETH Address: {12} |Balance: {15}\n|Has balance: {1} \n|Mnemonic phrase: {2} \n|Private BTC: {3} \n|Private ETH: {13} \n|PATH BTC: {4}\n|PATH ETH: {14}\n\n", address, FLAG, mnemonic.ToString(), str, DerPath, address1, address2, address3, flag, flag1, flag2, flag3, address0, hex, EthPath, flag0);
                            object outFileLock = Program.outFileLock;
                            bool lockTaken = false;
                            try
                            {
                                Monitor.Enter(outFileLock, ref lockTaken);
                                File.AppendAllText("wet.txt", contents);
                            }
                            finally
                            {
                                if (lockTaken)
                                    Monitor.Exit(outFileLock);
                            }
                            Interlocked.Increment(ref Wet);
                        }
                        else
                        {
                            if (Silent == false)
                            {
                                byte[] pvk1 = extKey.PrivateKey.GetBitcoinSecret(Network.Main).ToBytes();
                                string str = BitConverter.ToString(pvk1).Replace("-", string.Empty);
                                str = str.Remove(str.Length - 2);

                                byte[] pvk = wallet2.GetPrivateKey(a);
                                string hex = BitConverter.ToString(pvk).Replace("-", string.Empty);

                                Console.Write("\nAddress:\n|Legacy: {0} \n|Segwit: {1} \n|P2SH: {2} \n|BIP86: {3} \n|ETHEREUM: {11} \n|Mnemonic phrase: {4} \n|Path BTC: {5} \n|Path ETH: {13}\n|PVK BTC: {6} \n|PVK ETH: {12}\n Total: {7} | Found: {8} | Speed: {9} Keys/s | Speed: {10} Addresses/s |\n", address, address1, address2, address3, mnemonic, keyPath, str, Total, Wet, speed, speedAD, address0, hex, EthPath);
                            }
                        }
                        a++;
                    }
                }
            }
           



        }
    }
}