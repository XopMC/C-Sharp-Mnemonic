# C#-Mnemonic  
![image](https://user-images.githubusercontent.com/89750173/182707704-b249e235-e77f-41b0-884f-23cd07b1f06f.png)


Mnemonic Bruteforce C#  

# Eng  
## 04.08.2022 UPDATE - Ver. 6.0.0 
✅ Completely rewritten all the code, redone the mnemonic logic  
✅ More than 2x speed increase (x100 in some modes)  
✅ Implemented control via arguments (`C#-Mnemonic.exe -h` to view help)  
✅ Added several modes (BTC, ETH, BTC(hash160), Public Keys, Private keys (for brainflayer))  
✅ Added the ability to use multiple derivation paths (`-P path -P path2 -P path3` and so on)  
✅ Implemented the possibility of incremental search (+- value to the keys obtained from the mnemonic)  

I plan to add:  
❌ use bloomfilter - because the base .txt takes up a lot of RAM 

Description of launch arguments:  
`-h` `-help` -- Display help  
`-debug` -- Display debug mode (lower speed)  
`-d number` -- Derivation depth  
`-i path to file` -- Path to file with database  
`-hard` -- To operate on hardened addresses (adds a ' to derivation paths)  
`-m number` -- Mode 1 - BTC, 2 - ETH, 3 - BTC (HASH160), 4 - Public keys, 5 - Private keys (generator for brainflayer)  
`-n number` -- Number of keys for incremental search (+- value to the keys obtained from the mnemonic)  
`-k number` -- Step for incremental search (what number will be added to the private key in incremental search mode)  
`-t number` -- Number of threads  
`-P path` -- Derivation paths if not specified 44'/0'/0'/0/0 - for BTC, 44'/60'/0'/0/0 - for ETH  
`-PLUS` Use a set of built-in most popular derivation paths (there are many)  
`-w number of words` -- 12,15,18,21,24 - Number of words for the mnemonic (Default 12)  
`-lang language` (EN, CT, CS, KO, JA, IT, FR, SP) | Languages for mnemonics -  
English, ChineseTraditional, ChineseSimplified, Korean, Japanese, Italian, French, Spanish  


## Usage  
Need to install .Net runtime: https://dotnet.microsoft.com/en-us/download/dotnet/thank-you/runtime-6.0.1-windows-x64-installer  

Run the program via CMD!  

# Run example:  
`C#-Mnemonic.exe -m 1 -i btc.txt -t 4 -d 5 -w 12 -lang FR -n 1000 -k 2 -debug`  

Where:  
`-m 1` - Start BTC mode  
`-i btc.txt` - Load database from file btc.txt  
`-t 4` - Run program with  4-thread  
`-d 5` - The program will traverse each specified (or standard) derivation path in depth by 5  
`-w 12` - The program will generate a mnemonic of 12 words  
`-lang FR` - The mnemonic will be in French  
`-n 1000 -k 2` - The program will add (and subtract) the number 2 to the private keys obtained from the mnemonic 1000 times  
`-debug` - Show how addresses and mnemonics are generated  


# Also left the opportunity to run the program by simply holding the address base with the mouse and transferring it to the program  
![image](https://user-images.githubusercontent.com/89750173/182706224-b76d9c57-a681-4ab9-a661-cc56cab6c65b.png)  

# Giant speed improvement in -m 5 (Generator for brainflayer)  
![image](https://user-images.githubusercontent.com/89750173/182708065-ac9aef70-3c25-494c-aad1-8474a16cc20e.png)


# In [GeneratorMnemonics/bin/Release](https://github.com/XopMC/C-Sharp-Mnemonic/tree/main/GeneratorMnemonics/bin/Release)  folder contains assemblies for the most popular operating systems:  
1)WINDOWS:  
    1.x64  
    2.x86  
    3.ARM  
    4.ARM64  
2)LINUX:  
    1.x64  
    2.ARM  
    3.ARM64  
3)OSX-x64  


When the program finds a wallet, it will be displayed on the screen (if Debug mode is not enabled) and in the FOUND.txt file  

I would love suggestions and feedback!  

Donation: bc1qlwcaxwnp2ulndlppdc0wvkdz7ly2npfuz6ny0a - BTC  0xDE85c1Ef7874A1D94578f11332e8fa9A6a0eE853 - ETH   
--------

# Rus  
## 04.08.2022 ОБНОВЛЕНИЕ  - Ver. 6.0.0  
✅ Полностью переписан весь код, переделана логика мнемоники   
✅ Увеличение скорости более чем в 2 раза (x100 в некоторых режимах)  
✅ Реализовано управление через аргументы (`C#-Mnemonic.exe -h` для просмотра справки)  
✅ Добавлены несколько режимов (BTC, ETH, BTC(hash160), Public Keys, Private keys (для brainflayer))  
✅ Добавлена возможность использования нескольких путей деривации (`-P путь -P путь2 -P путь3` и тд)  
✅ Реализована возможность инкрементального поиска (+- значение к полученным из мнемоники ключам)  

Планирую добавить:   
❌ использовать bloomfilter - потому что база .txt занимает много оперативной памяти   

Описание аргументов запуска:  
`-h`  `-help` --  Показывает справку  
`-debug` -- Режим дебаг отображения (ниже скорость)  
`-d число` -- Глубина деривации  
`-i путь к файлу` -- Путь к файлу с базой  
`-hard` -- Для работы по  hardened адресам (добавляет значок ' к путям деривации)  
`-m число` -- Режим работы 1 - BTC, 2 - ETH, 3 - BTC (HASH160), 4 - Публичные ключи , 5 - Приватные ключи (генератор для brainflayer)  
`-n число` -- Кол-во ключей для инкрементального поиска (+- значение к полученным из мнемоники ключам)  
`-k число` -- Шаг для инкрементального поиска (какое число будет прибавляться к приватному ключу в режиме инкрементального поиска)  
`-t число` -- Количество потоков  
`-P путь` -- Пути деривации, если не указан будет 44'/0'/0'/0/0 - для BTC, 44'/60'/0'/0/0 - для ETH  
`-PLUS` Использовать набор вшитых самых популярных путей деривации (их много)  
`-w кол-во слов` -- 12,15,18,21,24 - Количество слов для мнемоники (По стандарту 12)  
`-lang язык` (EN, CT, CS, KO, JA, IT, FR, SP)  | Языки для мнемоники -  
English, ChineseTraditional, ChineseSimplified, Korean , Japanese, Italian, French, Spanish  


## Использование 
Нужно установить .Net runtime: https://dotnet.microsoft.com/en-us/download/dotnet/thank-you/runtime-6.0.1-windows-x64-installer  

Запускать программу через CMD!  

# Пример запуска:  
`C#-Mnemonic.exe -m 1 -i btc.txt -t 4 -d 5 -w 12 -lang FR -n 1000 -k 2 -debug`  

Где:  
`-m 1` - Запуск режима работы по BTC  
`-i btc.txt` - Загрузит базу из файла btc.txt  
`-t 4` - Запустит программу на 4 потока  
`-d 5` - Программа будет проходить каждый указанный (или стандартный) путь деривации в глубину на 5  
`-w 12` - Программа будет генерировать мнемонику из 12 слов  
`-lang FR` - Мнемоника будет на французком языке  
`-n 1000 -k 2` - К полученным из мнемоники приватным ключам, программа 1000 раз прибавит (и отнимет) число 2   
`-debug` - Покажет как генерируются адреса и мнемоники  


# Так-же оставил возможность запустить программу, просто зажав мышкой базу адресов и перенеся её на программу  
![image](https://user-images.githubusercontent.com/89750173/182706224-b76d9c57-a681-4ab9-a661-cc56cab6c65b.png)  

# Огромная прибавка к скорости в режиме -m 5 (Генератор для brainflayer)  
![image](https://user-images.githubusercontent.com/89750173/182708065-ac9aef70-3c25-494c-aad1-8474a16cc20e.png)

# В папке [GeneratorMnemonics/bin/Release](https://github.com/XopMC/C-Sharp-Mnemonic/tree/main/GeneratorMnemonics/bin/Release) находятся сборки для самых популярных ОС:  
1)WINDOWS:  
        1. x64  
        2. x86  
        3. ARM  
        4. ARM64  
2)LINUX: 
        1. x64  
        2. ARM  
        3. ARM64 
3)OSX-x64  


Когда программа найдет кошелек, он будет выводиться на экран (если не включен режим Debug) и в файл FOUND.txt   

Буду рад предложениям и обратной связи!

Donation: bc1qlwcaxwnp2ulndlppdc0wvkdz7ly2npfuz6ny0a - BTC  0xDE85c1Ef7874A1D94578f11332e8fa9A6a0eE853 - ETH  
--------


