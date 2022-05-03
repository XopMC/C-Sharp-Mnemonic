# C#-Mnemonic  
![image](https://user-images.githubusercontent.com/89750173/166493263-476fefcd-dab1-4d1a-83ae-5861b9460891.png)


Mnemonic Bruteforce C#  

# Eng  
## 03.05.2022 BIG UPDATE - Ver. 3.0.0  
✅ Add Ethereum mode  
✅ Add ETH + BTC mode (common base is required)  
✅ Support BIP39 Passphrase  

Implemented:  
✅ Silent(normal) mode - minimal display output  
✅ Debug mode - shows the complete process of generating mnemonics + 4 addresses and a private key  
✅ checking 4 addresses : (Compressed, Segwit ( starting with 3), p2wpkh, p2wsh (starting with bc1)  
✅ choice amount of words to generate mnemonics (12, 15, 18, 21, 24)  
✅ checking custom derivation paths + variable derivation deep

Planning to add:  
❌ use bloomfilter - because .txt base takes a lot of RAM  
❌ add Uncompressed addresses check - failed yet  
❌ add other languages for mnemonic  

## Usage  
Need to install .Net runtime: https://dotnet.microsoft.com/en-us/download/dotnet/thank-you/runtime-6.0.1-windows-x64-installer  

You can run program from cmd!   

with your btc Base in argument   
*for examlpe:*  
`Mnemo-XopMC.exe bit.txt`  
Where `bit.txt` - your addresses base `Possible to use tabular .tsv bases!` 

# Another way to start - drug and drop base file into program icon

![Снимок1](https://user-images.githubusercontent.com/89750173/166121357-5ee60d2f-8f49-4de1-8fcc-084561a00ea9.PNG)

When the program finds the wallet, there will be output to the screen (if Debug mode is not enabled) and to the wet.txt file  

I would be glad to receive any feedback!  

Donation: bc1qlwcaxwnp2ulndlppdc0wvkdz7ly2npfuz6ny0a  
--------

# Rus  
## 03.05.2022 БОЛЬШОЕ ОБНОВЛЕНИЕ  - Ver. 3.0.0  
✅ Добавлен режим по Эфиру  
✅ Добавлен режим Эфир + Биткоин (нужна единая база)  
✅ Добавлена возможность использования BIP39 Passphrase  

Реализовано:  
✅ Тихий (нормальный) режим - минимальный вывод на дисплей  
✅ Режим отладки - показывает полный процесс генерации мнемоники + 4 адреса и приватный ключ  
✅ проверка 4 адресов: (сжатый, Segwit (начиная с 3), p2wpkh, p2wsh (начиная с bc1)  
✅ выбор количества слов для создания мнемоники (12, 15, 18, 21, 24)  
✅ Кастомные пути деривации + настраиваемая глубина пути

Планирую добавить:   
❌ использовать bloomfilter - потому что база .txt занимает много оперативной памяти  
❌ добавить проверку несжатых адресов - пока не удалось  
❌ добавить другие языки для мнемоники  

## Использование 
Нужно установить .Net runtime: https://dotnet.microsoft.com/en-us/download/dotnet/thank-you/runtime-6.0.1-windows-x64-installer  

Запускать программу можно через CMD!   
С вашей базой в аргументе    
*например:*  
`Mnemo-XopMC.exe bit.txt`  
Где `bit.txt` - Ваша база адресов   `Можно использовать табличные .tsv базы!`  

# Можно запустить программу, просто зажав мышкой базу адресов и перенеся её на программу
![Снимок1](https://user-images.githubusercontent.com/89750173/166121357-5ee60d2f-8f49-4de1-8fcc-084561a00ea9.PNG)

Когда программа найдет кошелек, он будет выводиться на экран (если не включен режим Debug) и в файл wet.txt   

Буду рад предложениям и обратной связи!

Donation: bc1qlwcaxwnp2ulndlppdc0wvkdz7ly2npfuz6ny0a  
--------


