# GMem
A memory management tool for C# which is

 1. Very lightweight - GMem has only 7 tiny functions and 2 objects
 2. Generic - You don't deal with data conversions and you don't have to call different functions for every data type.
 3. Object oriented - So that what makes your life easier doesn't make your code look bad.

# How to use ?

    // Get an instance of GMemProcess.
    GMemProcess gameProc = new GMemProcess("processname", "ModuleName");
    /* ptrObject object holds;
     calculated address which is (modulebase + pointer address) + offsets
     processHandle which is the handle pointer of your process
     You can create a ptrObject like below
     */
    ptrObject obj = gameProc.create_ptr_object(0xFFFFFFFF, int[]{ 0xFF, 0x10 });
    /*
    Reads and writes are so simple. Functions are generic so you don't need to deal with conversions. 
    GMem supports int,float,double and byte data types but more will be added real soon.
    */
    // Read
    int intvalue = gameProc.read<int>(obj);
    // Write
    bool iswritten = gameProc.write<int>(obj, 50);
    
# How to test ?
Tests for this solution were done on steam version of Torchlight 2. There are further information on test documents.

#Stage - Work in Progress

 - Support for more data types will be implemented

