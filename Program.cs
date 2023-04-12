using System.Numerics;
String Symbols = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ()[]{},<>!%*/+-=^ \r\n.'\"?$|@#:;";
start:
Console.Clear();
Console.WriteLine(Symbols.Length);
Console.WriteLine(Symbols.Substring(0,16));
Console.WriteLine(Symbols.Substring(15, 16));
Console.WriteLine(Symbols.Substring(31,16));
Console.WriteLine(Symbols.Substring(63, 16));
Console.WriteLine(Symbols.Substring(79));

WriteHeader();
Console.WriteLine("Encode:E\n\rDecode:D");
string input = Console.ReadKey().KeyChar.ToString().ToLower();
switch (input)
{
    case "e":
        Encode();
        goto start;
        break;
    case "d":
        Decode();
        goto start;
        break;
}


void Encode()
{
    Console.Clear();
    if (!File.Exists("uncompressed.txt"))
    {
        File.Create("uncompressed.txt");
    }
    string str = File.ReadAllText("uncompressed.txt");

    #region"Convert to binary string"
        string binary = "";
        foreach (char c in str)
        {
            byte number = (byte)Symbols.IndexOf(c);
        if ((Symbols.IndexOf(c)) < 0) throw new Exception($"{c}is not able to be compressed");
            string binarySubSection = Convert.ToString(number, 2);
            while (binarySubSection.Length < 7)
            {
                binarySubSection = "0" + binarySubSection;
            }
            binary += binarySubSection;
        }
        while (binary.Length % 8 != 0)
        {
            binary += "0";
        }
    #endregion

    #region"Split into 8bit sections
    List<string> binarySections = new List<string>();
    while (binary.Length >= 8)
    {
        string section = binary.Substring(0, 8);
        binary = binary.Substring(8);
        binarySections.Add(section);
    }
    #endregion

    #region"Place into byte arr"
    byte[] bytes = new byte[binarySections.Count];
    for (int i = 0; i < binarySections.Count; i++)
    {
        bytes[i] = Convert.ToByte(binarySections[i], 2);
    }
    #endregion

    #region"Write bytes to a file"
        using (var stream = File.Open("compressed.cmp", FileMode.Create))
        {
            BinaryWriter binaryWriter = new BinaryWriter(stream, System.Text.Encoding.UTF8, false);
            binaryWriter.Write(bytes);
        stream.Dispose();
        }
    #endregion
    Console.WriteLine("done");
    Console.ReadKey();
}

void Decode()
{
    Console.Clear();
    byte[] bytes = File.ReadAllBytes("compressed.cmp");
    
    #region"ConvertToStringRepresentation"
    string binaryString = "";
    foreach (byte b in bytes)
    {
        string section = Convert.ToString(b, 2);
        while (section.Length < 8) { section = "0" + section; }
        binaryString += section;
    }
    if (binaryString.Length < 8)
    {
        while(binaryString.Length < 8) { binaryString = "0" + binaryString; }
    }
    #endregion

    #region"7BitStringSections"
    List<string> binarySections = new List<string>();
    while(binaryString.Length > 7)
    {
        string section = binaryString.Substring(0, 7);
        binarySections.Add(section);
        binaryString = binaryString.Substring(7);
    }
    #endregion

    #region"ByteList"
    List<byte> charBytes = new List<byte>();
    foreach (string str in binarySections)
    {
        charBytes.Add(Convert.ToByte(str,2));
    }
    #endregion

    #region"ConvertFromByteListToString"
    string Output = "";
    foreach (byte b in charBytes)
    {
        Output += Symbols[b];
    }
    #endregion

    File.WriteAllText("uncompressed.txt", Output);
    Console.WriteLine("done");
    Console.ReadKey();
}

void WriteHeader()
{
    Console.WriteLine("Text file Encoder/Decoder");
    Console.WriteLine("----------------------------");
}
