# C-SHARP--Mnemonic  
![с#](https://user-images.githubusercontent.com/89750173/147411895-fea08187-8114-4f49-a70c-dbb2ca0a0daf.PNG)

Bitcoin Mnemonic Bruteforce C#  

# Eng  

Implemented:  
✅ Silent(normal) mode - minimal display output  
✅ Debug mode - shows the complete process of generating mnemonics + 4 addresses and a private key  
✅ checking 4 addresses : (Compressed, Segwit ( starting with 3), p2wpkh, p2wsh (starting with bc1)  
✅ choice amount of words to generate mnemonics (12, 15, 18, 21, 24)  

Planning to add:  
❌ check multiple derivation paths - only standard bip44 yet  
❌ use bloomfilter - because .txt base takes a lot of RAM  
❌ add Uncompressed addresses check - failed yet  
❌ add other languages for mnemonic  

## Usage  
Need to install .Net runtime: https://dotnet.microsoft.com/en-us/download/dotnet/thank-you/runtime-6.0.1-windows-x64-installer  

run program from cmd!   

with your btc Base in argument   
*for examlpe:*  
`Mnemo-XopMC.exe bit.txt`  
Where `bit.txt` - your addresses base `Possible to use tabular .tsv bases!` 

When the program finds the wallet, there will be output to the screen (if Debug mode is not enabled) and to the wet.txt file  

I would be glad to receive any feedback!  

Donation: bc1qlwcaxwnp2ulndlppdc0wvkdz7ly2npfuz6ny0a  
--------

# Rus  
Реализовано:  
✅ Тихий (нормальный) режим - минимальный вывод на дисплей  
✅ Режим отладки - показывает полный процесс генерации мнемоники + 4 адреса и приватный ключ  
✅ проверка 4 адресов: (сжатый, Segwit (начиная с 3), p2wpkh, p2wsh (начиная с bc1)  
✅ выбор количества слов для создания мнемоники (12, 15, 18, 21, 24)   

Планирую добавить:  
❌ проверить несколько путей деривации - пока только стандартный bip44  
❌ использовать bloomfilter - потому что база .txt занимает много оперативной памяти  
❌ добавить проверку несжатых адресов - пока не удалось  
❌ добавить другие языки для мнемоники  

## Использование 
Нужно установить .Net runtime: https://dotnet.microsoft.com/en-us/download/dotnet/thank-you/runtime-6.0.1-windows-x64-installer  

Запускать програссу через CMD!   

С вашей базой в аргументе    
*например:*  
`Mnemo-XopMC.exe bit.txt`  
Где `bit.txt` - Ваша база адресов   `Можно использовать табличные .tsv базы!`  

Когда программа найдет кошелек, он будет выводиться на экран (если не включен режим Debug) и в файл wet.txt   

Буду рад предложениям и обратной связи!

Donation: bc1qlwcaxwnp2ulndlppdc0wvkdz7ly2npfuz6ny0a  
--------


