Module Main

    Sub Main()
        Console.Title = "Enigma"
        Dim e1 = New Enigma(Of Char)(CharDefaults.EnigmaI_1938dec)
        e1.Prepare(CharDefaults.EnigmaI_1938dec_cfg)

        Dim str1 = Console.ReadLine
        Console.WriteLine(New String(e1.Parse(str1.ToList).ToArray))
        Console.ReadKey()
    End Sub

End Module
