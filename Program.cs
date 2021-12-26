using NBitcoin;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

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
        private static HashSet<string> _addressDb;
        private static string? win;
        static DateTime t1, t2;
        private static long cur;
        private static int words;

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
                Console.WriteLine("\n\nDeveloped by @XopMC for t.me/brythbit\n\n");
                if (win == "XOPMC")
                {
                    while (true)
                        Console.WriteLine("Ты чертила): @XopMC." + Environment.MachineName);
                }
                else
                {
                    int processorCount = Environment.ProcessorCount;
                    Console.WriteLine("Loading database addresses... | Загрузка базы адресов\n");
                    Console.WriteLine("Input 1 - for normal mode/ 2 - for debug mode | Введите 1 - для Нормального режима/ 2 - для полного отображения: ");
                    int sil = Convert.ToInt32(Console.ReadLine());
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
                    Console.WriteLine("\nMax threads | Доступно потоков: {0} ", (object)processorCount);
                    Console.WriteLine("\nHow many threads should use? | Сколько потоков задействуем? :");
                    int num = Convert.ToInt32(Console.ReadLine());
                    if (num > processorCount)
                        num = processorCount;
                    Console.WriteLine("\nStarting {0} threads... | Запускаю {1} поток(а)ов...\n", (object)num, (object)num);
                    Console.WriteLine("Loaded addresses from {0}  | Загружены адреса из {1} \n", (object)filePath, (object)filePath);
                    _addressDb = LoadDatabase(filePath);
                    Console.WriteLine("Input required amount of words for mnemonic ( 12, 15, 18, 21, 24) | Введите количество слов для мнемоники ( 12, 15, 18, 21, 24): ");
                    words = Convert.ToInt32(Console.ReadLine());
                    if ((words != 12) && (words != 15) && (words != 18) && (words != 21) && (words != 24))
                    {
                        Console.WriteLine("\nWrong words count! Starting with 12 words | Введено неверное количество слов, включаю стандартный режим по 12 словам.");
                        words = 12;
                    }
                    Console.WriteLine("\nStarting porgramm with {0} words | Запускаю режим работы по {1} словам", (object)words, (object)words);
                    List<Thread> threadList = new();
                    for (int index = 0; index < num; ++index)
                    {
                        Thread thread = new(new ThreadStart(WorkerThread));
                        threadList.Add(thread);
                    }
                    IsRunning = true;
                    foreach (Thread thread in threadList)
                        thread.Start();
                    Console.WriteLine("\nThreads are started... | Потоки запущены...", (object)processorCount);
                    Console.CancelKeyPress += new ConsoleCancelEventHandler(MyHandler);
                    Console.WriteLine("\nCTRL+C to interrupt the process | CTRL+C для остановки процесса");
                    if (Silent == true)
                    {
                        while (IsRunning)
                        {
                            //Console.Write("\r" + new string(' ', Console.WindowWidth - 1) + "\r");
                            Console.Write("\rTotal: {0} | Found: {1} | Speed: {2} Keys/s | Elapsed: {3}", (object)Total, (object)Wet, (object)speed, (object)cur);
                            Thread.Sleep(1000);
                        }
                    }
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
            Thread.Sleep(1000);
            KeyPath keyPath = new("44'/0'/0'/0/0");
            while (IsRunning)
            {
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
                //Mnemonic mnemonic = "angle bonus rich melody cotton lyrics skate stuff fragile guard fresh snake glance join artefact slender sting craft decorate time absent magic index entry";
                ExtKey extKey = mnemonic.DeriveExtKey().Derive(keyPath);
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
                speed = Total / cur;
                if ((flag != false) || (flag1 != false) || (flag2 != false) || (flag3 != false))
                {
                    string str = extKey.PrivateKey.GetBitcoinSecret(Network.Main).ToString();
                    string contents = string.Format("Address: {0} | Has balance: {1} | Mnemonic phrase: {2} | Private: {3}\n\n", (object)address, (object)flag, (object)mnemonic.ToString(), (object)str);
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
                        Console.Write("\nAddress:\n|Legacy: {0} \n|Segwit: {1} \n|P2SH: {2} \n|BIP86: {3} \n| Mnemonic phrase: {4} \n| Path: {5} \n| PVK: {6}\n Total: {7} | Found: {8} | Speed: {9} Keys/s", (object)address, (object)address1, (object)address2, (object)address3, (object)mnemonic, (object)keyPath, (object)hex, (object)Total, (object)Wet, (object)speed);
                    }
                    else if (Silent == true)
                    {
                        //Console.Write("\r" + new string(' ', Console.WindowWidth - 1) + "\r");
                        //Console.Write("\rTotal: {0} | Wet: {1} | Speed: {2} Keys/s | Elapsed: {3}", (object)Total, (object)Wet, (object)speed, (object)cur);
                        //Console.WriteLine("speed");

                    }
                }


            }
        }
    }
}