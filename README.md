# GMem
A memory management tool for C# which is

 1. Very lightweight - GMem has only a few tiny functions and 2 objects
 2. Generic - You don't deal with data conversions and you don't have to call different functions for every data type.
 3. Object oriented - So that what makes your life easier doesn't make your code look bad.
 4. Extendable - I tried to keep the code as simple as i can so that you can extend it for your own work.

# How to use ?
```csharp
    // Get an instance of GMemProcess.
    GMemProcess gameProc = new GMemProcess("processname", "ModuleName");
    /* ptrObject object holds;
     calculated address which is (modulebase + pointer address) + offsets or direct address if its not a pointer.
     processHandle which is the handle pointer of your process
     You can create a ptrObject like below
     */
    ptrObject obj = gameProc.create_ptr_object(0xFFFFFFFF, int[]{ 0xFF, 0x10 });
    /*
    Reads and writes are so simple.
    GMem supports almost all types that you might need.
    */
    // Read
    int intvalue = gameProc.read<int>(obj);
    // Write
	int valueToWrite = 50;
    bool iswritten = gameProc.write<int>(obj, valueToWrite);
	// If you want to read string or byte[] you can use the read function overload
	int stringlength = 10;
	string text = gameProc.read<string>(obj, stringlength);
	// or
	int byteLength = 10;
	byte[] dataBuffer = gameProc.read<byte[]>(obj, 10);
``` 
# How to test ?
Tests for this solution were done on steam version of Torchlight 2. There are further information on test documents.

#Stage - Work in Progress

 - Implementations are done. Code refactoring and cleaning in progress.

