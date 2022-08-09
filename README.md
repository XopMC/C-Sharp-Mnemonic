# C#-Mnemonic  
![image](https://user-images.githubusercontent.com/89750173/183668123-4ae73d1b-d8d5-470a-845a-d24668ac3782.png)


Mnemonic Bruteforce C#  

# Eng  
## 08/09/2022 UPDATE - Ver. 7.0.2  
✅ Added 38 new cryptocurrencies  
✅ Implemented incremental mnemonics search (entropy) in order (`-entropy hex`)  
✅ Added mode (`-m 100`) for simple generation of valid mnemonics (works both in random mode, with the words `-w 12`, and in the `-entropy hex` brute force mode)  
✅ Implemented the ability to save the passed stage in the mode of incremental entropy enumeration (sorting words in order) - once a minute the last entropy value will be saved to the file SAVED_ENTROPY.txt  
✅ Added the ability to change the step for incremental entropy enumeration (`-step number`)  
✅ Added loading derivation paths from file (`-PATH filename.txt`)  
✅ By popular demand, returned the BTC + ETH mode (`-m 0`)  

I plan to add:  
❌ bloomfilter is delayed indefinitely, to reduce RAM consumption - use in conjunction with brainlfayer.  
❌ New currencies (like Monero, Tron, etc)  
❌ Addresses on bc1q.  


## Description of launch arguments:  
`-h` `-help` -- Display help  
`-debug` -- Display debug mode (lower speed)  
`-d number` -- Derivation depth  
`-i path to file` -- Path to file with database  
`-hard` -- To operate on hardened addresses (adds a ' to derivation paths)  
`-m number` --  mode  0 - BTC+ETH, 1 - BTC, 2 - ETH, 3 - BTC (HASH160), 4 - Public keys, 5 - Private keys (generator for brainflayer), 100 - Valid Mnemonic Generator . Modes (6 - 43) for other cryptocurrencies.  
`-entropy hex` -- incremental entropy search, in order from specified hex number (Length - 8 , 16, 24 ,32 ,40 ,48 ,56 ,64) HEX characters  
`-step number` -- Specify step to add in `-entropy` mode  
`-n number` -- Number of keys for incremental search (PrivateKeys) (+- value to the keys obtained from the mnemonic)  
`-k number` -- Step for incremental search (what number will be added to the private key in incremental search mode)  
`-t number` -- Number of threads  
`-P "path"` -- Derivation paths if not specified will be 44'/0'/0'/0/0 - for BTC, 44'/60'/0'/0/0 - for ETH  
`-PATH file.txt` -- load derivation paths from file  
`-PLUS` Use a set of built-in most popular derivation paths (there are many)  
`-w number of words` -- 3, 6, 9, 12, 15, 18, 21, 24 - Number of words for the mnemonic (Default 12)  
`-lang language` (EN, CT, CS, KO, JA, IT, FR, SP) | Languages ​​for mnemonics -
English, ChineseTraditional, ChineseSimplified, Korean, Japanese, Italian, French, Spanish  



## List of additional modes and currencies:  
`-m 6` -- Bitcoin Cash  
`-m 7` -- Bitcoin Diamond  
`-m 8` -- Bitcoin SV  
`-m 9` -- ILCoin  
`-m 10` -- Tether   
`-m 11` -- CryptoVerificationCoin  
`-m 12` -- Litecoin Cash  
`-m 13` -- Zcash  
`-m 14` -- DigiByte  
`-m 15` -- Dogecoin  
`-m 16` -- PIVX  
`-m 17` -- Verge  
`-m 18` -- Horizen  
`-m 19` -- Einsteinium  
`-m 20` -- Groestlcoin  
`-m 21` -- Bitcoin Gold  
`-m 22` -- Litecoin  
`-m 23` -- MonaCoin  
`-m 24` -- ImageCoin  
`-m 25` -- NavCoin  
`-m 26` -- Neblio  
`-m 27` -- Axe  
`-m 28` -- Peercoin  
`-m 29` -- Particle  
`-m 30` -- Qtum  
`-m 31` -- Komodo  
`-m 32` -- Ravencoin  
`-m 33` -- Reddcoin  
`-m 34` -- SafeInsure  
`-m 35` -- SmartCash  
`-m 36` -- Stratis  
`-m 37` -- Syscoin  
`-m 38` -- Vertcoin  
`-m 39` -- Viacoin  
`-m 40` -- BeetleCoin  
`-m 41` -- Dash  
`-m 42` -- Xenios  
`-m 43` -- Zcoin  


## Usage  
Windows: Need to install .Net runtime: https://dotnet.microsoft.com/en-us/download/dotnet/thank-you/runtime-6.0.1-windows-x64-installer  
Ubuntu 22.04: `wget https://packages.microsoft.com/config/ubuntu/22.04/packages-microsoft-prod.deb -O packages-microsoft-prod.deb`  
`sudo dpkg -i packages-microsoft-prod.deb`  
`vrm packages-microsoft-prod.deb`  
`sudo apt-get update`  
`sudo apt-get install -y apt-transport-https`  
`sudo apt-get update`  
`sudo apt-get install -y aspnetcore-runtime-6.0`  
OSX(macOS):  
x64: download dotnet - https://dotnet.microsoft.com/en-us/download/dotnet/thank-you/runtime-6.0.8-macos-x64-installer  
arm64 (M1 processors): download dotnet - https://dotnet.microsoft.com/en-us/download/dotnet/thank-you/runtime-6.0.8-macos-arm64-installer  
## IF ERROR occurs ("Operation not permitted"), read the article here - https://github.com/XopMC/C-Sharp-Mnemonic/blob/main/ERROR_OSX.md  

Run the program via CMD!  

# Run example:  
`C#-Mnemonic.exe -m 1 -i btc.txt -t 4 -d 5 -w 12 -lang FR -n 1000 -k 2 -debug`  

Where:  
`-m 1` - Start BTC mode  
`-i btc.txt` - Load database from file btc.txt  
`-t 4` - Run a 4-thread program  
`-d 5` - The program will traverse each specified (or standard) derivation path in depth by 5  
`-w 12` - The program will generate a mnemonic of 12 words  
`-lang FR` - The mnemonic will be in French  
`-n 1000 -k 2` - The program will add (and subtract) the number 2 to the private keys obtained from the mnemonic 1000 times  
`-debug` - Show how addresses and mnemonics are generated  


# Description of some modes:  
## Incremental Private key search (`-n` and `-k`)  
When adding these parameters, after receiving the private key, it will be added (+) and subtracted (-) the number that you specified in the `-k` parameter  
For example:  
When running `-n 1000 -k 2`  
![image](https://user-images.githubusercontent.com/89750173/183724205-7d059bdc-7bff-48f8-80b4-c07bc549a00f.png)  
We will see how the program added +2 and subtracted -2 ​​from the original private key, and checked the received addresses  
And This procedure will be repeated 1000 times (as specified by `-n`)!!!  
Also, the line with the private key shows what number was added to the original private key `N = +00000002` for example.  

## Entropy iteration (`-entropy` and `-step`):  
When using the `-entropy` argument, you must specify strings in HEX format after it.  
EXAMPLE:  
`-entropy 00000000` - 8 HEX (32 bits) characters, iteration will start at 3 words, with the words `abandon abandon ability`  
`-entropy 0000000000000000` - 16 HEX (64 bits) characters, 6 words each, from the words `abandon abandon abandon abandon abandon able`  
`-entropy 000000000000000000000000` - 24 HEX (96 bit) characters, 9 words from words `abandon abandon abandon abandon abandon abandon abandon abandon`  
`-entropy 0000000000000000000000000000000` - 32 HEX (128 bit) characters, 12 words each, from the words `abandon abandon abandon abandon abandon abandon abandon abandon abandon about`  
........  and so on  
`-entropy 00000000000000000000000000000000000000000000000000000000000000` - 64 HEX (256 bits) characters, 24-word search, from the words  `abandon abandon abandon abandon abandon abandon abandon abandon abandon abandon abandon abandon abandon abandon abandon abandon abandon abandon abandon abandon abandon abandon abandon art`    


THE COMMANDS ABOVE ARE FOR EXAMPLE ONLY!!!  
In general, thanks to this parameter, you can iterate in order starting from any combination.  

For example, I have the phrase `churn right music thank orient easily fix robust option web brisk click`  
In order to find out its entropy, I need to go to the site https://iancoleman.io/bip39/  
In the `BIP39 Mnemonic` field, insert a mnemonic, and click on the `Show entropy details` checkbox  
To the point, entropy will seem to me (circled in the screenshot)  
![image](https://user-images.githubusercontent.com/89750173/183734861-efa3cf7a-5581-48c8-8cf2-d29b247a87cf.png)  

Next, I copy this entropy and paste it into the `-entropy 28d73a47efe9c88b1605da9bbf187115` parameter  
As we can see, the entropy is 32 characters long (128 bits), which means the search will be 12 words long  
![image](https://user-images.githubusercontent.com/89750173/183735595-deb24c6a-c475-4c81-ab24-74ae036f0a4b.png)  
In the program, we see that one is added to the current entropy and the last word is changed.  

`-step` OPTION:  
In the screenshot above, you can see that one is added to the current entropy.  
This parameter allows you to specify what number will be added to the entropy.  
Accordingly, if I tell the program `-entropy 28d73a47efe9c88b1605da9bbf187115 -step 2` - it will "jump" through 1 word ...  
![image](https://user-images.githubusercontent.com/89750173/183736662-43ac7cb5-7cf6-4d13-a3a1-c430f9627249.png)  
(It is important to note that the `-step` - parameter must be a decimal number.  
For example, if you want to add the number `f` to the entropy - you need to specify `-step 15`...  
# THE PROGRAM SAVE THE VALUE OF THE LAST PASSED ENTROPY TO THE FILE `SAVED_ENTROPY.txt` ONCE A MINUTE!!!  

## `-m 5` and `-m 100` modes:  
## In `-m 5` mode, the program simply displays the generated private keys, they can either be compiled into a file or passed to brainlfayer.  
![image](https://user-images.githubusercontent.com/89750173/183741610-4717bc4d-009c-419f-ae65-373d862a05ed.png)  

PARAMETERS `-n -k` and `-entropy -step` ALSO WORK IN THIS MODE  
Example: `C#-Mnemonic.exe -m 5 -n 1000 -k 1`  
![image](https://user-images.githubusercontent.com/89750173/183742150-f2e8a49d-4a3d-4fd5-aa06-5588f7fc0049.png)  

EXAMPLES:  
Saving to PrivateKeys.txt: `C#-Mnemonic.exe -m 5 -n 1000 -k 1 >> PrivateKeys.txt`  
Pass to brainflayer: `C#-Mnemonic.exe -m 5 -n 1000 -k 1 | brainflayer.exe -v -t priv -x ....`  

## In `-m 100` mode, the program simply generates a valid mnemonic, it can either be compiled into a file or passed to brainlfayer.  
![image](https://user-images.githubusercontent.com/89750173/183743041-f157d211-580e-479b-a2d0-6f0023cffc6c.png)  

The `-entropy` parameter also works in this mode:  
Example: `C#-Mnemonic.exe -m 100 -entropy 00000000` - generates VALID mnemonics in order  
![image](https://user-images.githubusercontent.com/89750173/183743346-352f3ae7-5ee0-40f7-b5af-5ccc5daf1c9e.png)  
(The `-n` and `-k` options do not work in this mode, since there are no private keys)  
EXAMPLES:  
Save to file Mnemonic.txt: `C#-Mnemonic.exe -m 100 -w 12 >> Mnemonic.txt`  
Pass to brainflayer: `C#-Mnemonic.exe -m 100 -w 12 | brainflayer.exe -v ....`  

# Also os added the ability to run the program by simply holding the address database with the mouse and transferring it to the program  
In this case, the program runs in BTC+ETH mode, with minimal settings.  
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
3)OSX:  
    1.x64  
    2.arm64  


When the program finds a wallet, it will be displayed on the screen (if Debug mode is not enabled) and in the FOUND.txt file  

I would love suggestions and feedback!  

Donation: bc1qlwcaxwnp2ulndlppdc0wvkdz7ly2npfuz6ny0a - BTC  0xDE85c1Ef7874A1D94578f11332e8fa9A6a0eE853 - ETH   
--------

# Rus  
## 09.08.2022 ОБНОВЛЕНИЕ  - Ver. 7.0.2  
✅ Добавлены 38 новых криптовалют  
✅ Реализован перебор мнемоники (энтропии) по порядку (`-entropy hex`)  
✅ Добавлен режим (`-m 100`) для простой генерации валидной мнемоники (работает как и в рандомном режиме, с указанием слов `-w 12`, так и в режиме перебора по порядку `-entropy hex`)   
✅ Реализована возможность сохранения пройденного этапа в режиме инкрементального перебора энтропии (перебора слов по порядку) - раз в минуту последнее значение энтропии будет сохраняться в файл SAVED_ENTROPY.txt   
✅ Добавлена возможность менять шаг для инкрементального перебора энтропии (`-step число`)  
✅ Добавлена загрузка путей деривации из файла (`-PATH имя_файла.txt`)   
✅ По многочисленным просьбам вернул режим BTC+ETH (`-m 0`)

Планирую добавить:   
❌ bloomfilter откладывается на неопределённое время, для уменьшения потребления ОЗУ - используйте совместно с brainlfayer.  
❌ Новые валюты (например Монеро, Трон, и т.д)   
❌ Адреса на bc1q.  




## Описание аргументов запуска:  
`-h`  `-help` --  Показывает справку  
`-debug` -- Режим дебаг отображения (ниже скорость)  
`-d число` -- Глубина деривации  
`-i путь к файлу` -- Путь к файлу с базой  
`-hard` -- Для работы по  hardened адресам (добавляет значок ' к путям деривации)  
`-m число` -- Режим работы 0 - BTC+ETH, 1 - BTC, 2 - ETH, 3 - BTC (HASH160), 4 - Публичные ключи , 5 - Приватные ключи (генератор для brainflayer),  100 - Valid Mnemonic Generator. Режимы (6 - 43) для других криптовалют.  
`-entropy hex` -- Перебор энтропии по порядку от указанного hex числа (Длина - 8 , 16, 24 ,32 ,40 ,48 ,56 ,64) HEX символов  
`-step число` -- Указать шаг для прибавления в режиме `-entropy`  
`-n число` -- Кол-во ключей для инкрементального поиска (+- значение к полученным из мнемоники ключам)  
`-k число` -- Шаг для инкрементального поиска (какое число будет прибавляться к приватному ключу в режиме инкрементального поиска)  
`-t число` -- Количество потоков  
`-P "путь"` -- Пути деривации, если не указан будет 44'/0'/0'/0/0 - для BTC, 44'/60'/0'/0/0 - для ETH  
`-PATH файл.txt` -- загрузка путей деривации из файла  
`-PLUS` Использовать набор вшитых самых популярных путей деривации (их много)  
`-w кол-во слов` -- 3, 6, 9, 12, 15, 18, 21, 24 - Количество слов для мнемоники (По стандарту 12)  
`-lang язык` (EN, CT, CS, KO, JA, IT, FR, SP)  | Языки для мнемоники -  
English, ChineseTraditional, ChineseSimplified, Korean , Japanese, Italian, French, Spanish  



## Список дополнительных режимов и валют:  
`-m 6` -- Bitcoin Cash  
`-m 7` -- Bitcoin Diamond  
`-m 8` -- Bitcoin SV  
`-m 9` -- ILCoin  
`-m 10` -- Tether  
`-m 11` -- CryptoVerificationCoin  
`-m 12` -- Litecoin Cash  
`-m 13` -- Zcash  
`-m 14` -- DigiByte  
`-m 15` -- Dogecoin  
`-m 16` -- PIVX  
`-m 17` -- Verge  
`-m 18` -- Horizen  
`-m 19` -- Einsteinium  
`-m 20` -- Groestlcoin  
`-m 21` -- Bitcoin Gold  
`-m 22` -- Litecoin  
`-m 23` -- MonaCoin  
`-m 24` -- ImageCoin  
`-m 25` -- NavCoin  
`-m 26` -- Neblio  
`-m 27` -- Axe  
`-m 28` -- Peercoin  
`-m 29` -- Particl  
`-m 30` -- Qtum  
`-m 31` -- Komodo  
`-m 32` -- Ravencoin  
`-m 33` -- Reddcoin  
`-m 34` -- SafeInsure  
`-m 35` -- SmartCash  
`-m 36` -- Stratis  
`-m 37` -- Syscoin  
`-m 38` -- Vertcoin  
`-m 39` -- Viacoin  
`-m 40` -- Beetle Coin  
`-m 41` -- Dash  
`-m 42` -- Xenios  
`-m 43` -- Zcoin  


## Использование 
Windows: Нужно установить .Net runtime: https://dotnet.microsoft.com/en-us/download/dotnet/thank-you/runtime-6.0.1-windows-x64-installer  
Ubuntu 22.04: `wget https://packages.microsoft.com/config/ubuntu/22.04/packages-microsoft-prod.deb -O packages-microsoft-prod.deb`  
`sudo dpkg -i packages-microsoft-prod.deb`  
`vrm packages-microsoft-prod.deb`
`sudo apt-get update`  
`sudo apt-get install -y apt-transport-https`  
`sudo apt-get update`  
`sudo apt-get install -y aspnetcore-runtime-6.0`  
OSX(macOS):  
x64: качаем dotnet - https://dotnet.microsoft.com/en-us/download/dotnet/thank-you/runtime-6.0.8-macos-x64-installer   
arm64 (процессоры М1): качаем dotnet - https://dotnet.microsoft.com/en-us/download/dotnet/thank-you/runtime-6.0.8-macos-arm64-installer  
## ЕСЛИ ВОЗНИКАЕТ ОШИБКА ("Operation not permitted"), читаем статью тут - https://github.com/XopMC/C-Sharp-Mnemonic/blob/main/ERROR_OSX.md  

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


# Описание некоторых режимов:    
## Инкрементальный поиск по приватным ключам (`-n` и `-k`)  
При добавлении данных параметров, после получения приватного ключа, к нему будет прибавлено (+) и вычтено (-) число, которое вы указали в параметре `-k`  
Например:   
При запуске `-n 1000 -k 2`  
![image](https://user-images.githubusercontent.com/89750173/183724205-7d059bdc-7bff-48f8-80b4-c07bc549a00f.png)  
Мы увидим как программа прибавила +2 и вычла -2 от исходного приватного ключа, и проверила полученные адреса  
И Данная процедура будет повторена 1000 раз (как указано в `-n`)!!!   
Так-же, в строке с приватным ключем показывается какое число было прибавлено к исходному приватному ключу `N = +00000002` например.  

## Перебор энтропии (`-entropy` и `-step`):  
При использовании аргумента `-entropy` после него необходимо указывать строчки в HEX формате.  
ПРИМЕР:  
`-entropy 00000000` - 8 HEX (32 бита) символов, перебор начнётся по 3 словам, со слов `abandon abandon ability`  
`-entropy 0000000000000000` - 16 HEX (64 бита) символов, по 6 словам, со слов `abandon abandon abandon abandon abandon able`  
`-entropy 000000000000000000000000` - 24 HEX (96 бит) символов, по 9 словам, со слов `abandon abandon abandon abandon abandon abandon abandon abandon abandon`  
`-entropy 00000000000000000000000000000000` - 32 HEX (128 бит) символов, по 12 словам, со слов `abandon abandon abandon abandon abandon abandon abandon abandon abandon abandon abandon about`  
........  и так далее   
`-entropy 0000000000000000000000000000000000000000000000000000000000000000` - 64 HEX (256 бит) символов, перебор по 24 словам, со слов `abandon abandon abandon abandon abandon abandon abandon abandon abandon abandon abandon abandon abandon abandon abandon abandon abandon abandon abandon abandon abandon abandon abandon art`  

КОМАНДЫ ВЫШЕ ПРИВЕДЕНЫ ТОЛЬКО ДЛЯ ПРИМЕРА!!!  
Вообще, благодаря данному параметру, вы можете перебирать по порядку начиная с любой комбинации.  

Например, у меня есть фраза `churn right music thank orient easily fix robust option web brisk click`  
Для того, чтобы узнать её энтропию, мне нужно перейти на сайт https://iancoleman.io/bip39/  
В поле `BIP39 Mnemonic` вставить мнемонику, и нажать на галочку `Show entropy details`  
Навреху у меня покажется энтропия (обвёл на скриншоте)  
![image](https://user-images.githubusercontent.com/89750173/183734861-efa3cf7a-5581-48c8-8cf2-d29b247a87cf.png)  

Далее я копирую эту энтропию, и вставляю в параметр `-entropy 28d73a47efe9c88b1605da9bbf187115`  
Как мы видим, энтропия длинной в 32 символа (128 бит), значит перебор будет по 12 словам  
![image](https://user-images.githubusercontent.com/89750173/183735595-deb24c6a-c475-4c81-ab24-74ae036f0a4b.png)  
В программе видим, что к текущей энтропии прибавляется единица и последнее слово меняется.  

ПАРАМЕТР `-step`:  
На скриншоте выше, видно что к текущей энтропии прибавляется единица.  
Данный параметр, позволяет указать, какое число будет прибавляться к энтропии.  
Соответственно, если я укажу программе `-entropy 28d73a47efe9c88b1605da9bbf187115 -step 2` -- она будет "прыгать" через 1 слово...   
![image](https://user-images.githubusercontent.com/89750173/183736662-43ac7cb5-7cf6-4d13-a3a1-c430f9627249.png)  
(Важно учесть, что в параметре `-step` - необходимо указывать десятичное число.  
Например, если вы хотите добавлять к энтропии число `f` - вам нужно указать `-step 15`...   
# ПРОГРАММА РАЗ В МИНУТУ СОХРАНАЕТ ЗНАЧЕНИЕ ПОСЛЕДНЕЙ ПРОЙДЁННОЙ ЭНТРОПИИ В ФАЙЛ `SAVED_ENTROPY.txt`!!!  

## Режимы `-m 5` и `-m 100`:  
## В режиме `-m 5` программа просто выводит на экран сгенерированные приватные ключи, их можно либо собирать в файл, либо передавать в brainlfayer.  
![image](https://user-images.githubusercontent.com/89750173/183741610-4717bc4d-009c-419f-ae65-373d862a05ed.png)   

ТАК-ЖЕ В ДАННОМ РЕЖИМЕ РАБОТАЮТ ПАРАМЕТРЫ `-n -k` и `-entropy -step`  
Пример: `C#-Mnemonic.exe -m 5 -n 1000 -k 1`  
![image](https://user-images.githubusercontent.com/89750173/183742150-f2e8a49d-4a3d-4fd5-aa06-5588f7fc0049.png)  

ПРИМЕРЫ:  
Сохранение в файл PrivateKeys.txt: `C#-Mnemonic.exe -m 5 -n 1000 -k 1 >> PrivateKeys.txt`   
Передача в brainflayer: `C#-Mnemonic.exe -m 5 -n 1000 -k 1 | brainflayer.exe -v -t priv -x ....`   

## В режиме `-m 100` программа просто генерирует валидную мнемонику, её можно либо собирать в файл, либо передавать в brainlfayer.  
![image](https://user-images.githubusercontent.com/89750173/183743041-f157d211-580e-479b-a2d0-6f0023cffc6c.png)  

Так-же в данной режиме рабоет параметр `-entropy`:  
Пример: `C#-Mnemonic.exe -m 100 -entropy 00000000` - генерирует ВАЛИДНУЮ мнемонику по порядку  
![image](https://user-images.githubusercontent.com/89750173/183743346-352f3ae7-5ee0-40f7-b5af-5ccc5daf1c9e.png)  
(Параметры `-n` и `-k` в данном режиме не работают, так как нет приватных ключей)  
ПРИМЕРЫ:  
Сохранение в файл Mnemonic.txt: `C#-Mnemonic.exe -m 100 -w 12 >> Mnemonic.txt`   
Передача в brainflayer: `C#-Mnemonic.exe -m 100 -w 12 | brainflayer.exe -v ....`   

# Так-же оставил возможность запустить программу, просто зажав мышкой базу адресов и перенеся её на программу  
В таком случае, программа запускается в режиме BTC+ETH, с минимальными настройками.  
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
3)OSX-x64:  
        1. x64   
        2. ARM64 (для процессоров М1)     


Когда программа найдет кошелек, он будет выводиться на экран (если не включен режим Debug) и в файл FOUND.txt   

Буду рад предложениям и обратной связи!

Donation: bc1qlwcaxwnp2ulndlppdc0wvkdz7ly2npfuz6ny0a - BTC  0xDE85c1Ef7874A1D94578f11332e8fa9A6a0eE853 - ETH  
--------


