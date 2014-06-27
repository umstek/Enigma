''' <summary>
''' 
''' </summary>
''' <remarks></remarks>
Public Class CharDefaults
    Private Shared Property Alphabet As List(Of Char) = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToList

    Private Shared Property RotorI As New Plot(Of Char).RotorPlot With
        {.Substitutes = "EKMFLGDQVZNTOWYHXUSPAIBRCJ".ToList,
         .Notches = "R".ToList}
    Private Shared Property RotorII As New Plot(Of Char).RotorPlot With
        {.Substitutes = "AJDKSIRUXBLHWTMCQGZNPYFVOE".ToList,
         .Notches = "F".ToList}
    Private Shared Property RotorIII As New Plot(Of Char).RotorPlot With
        {.Substitutes = "BDFHJLCPRTXVZNYEIWGAKMUSQO".ToList,
         .Notches = "W".ToList}
    Private Shared Property RotorIV As New Plot(Of Char).RotorPlot With
        {.Substitutes = "ESOVPZJAYQUIRHXLNFTGKDCMWB".ToList,
         .Notches = "K".ToList}
    Private Shared Property RotorV As New Plot(Of Char).RotorPlot With
        {.Substitutes = "VZBRGITYUPSDNHLXAWMJQOFECK".ToList,
         .Notches = "A".ToList}

    Private Shared Property RotorVI As New Plot(Of Char).RotorPlot With
        {.Substitutes = "JPGVOUMFYQBENHZRDKASXLICTW".ToList,
         .Notches = "AN".ToList}
    Private Shared Property RotorVII As New Plot(Of Char).RotorPlot With
        {.Substitutes = "NZJHGRCXMYSWBOUFAIVLPEKQDT".ToList,
         .Notches = "AN".ToList}
    Private Shared Property RotorVIII As New Plot(Of Char).RotorPlot With
        {.Substitutes = "FKQHTLXOCBJSPDZRAMEWNIUYGV".ToList,
         .Notches = "AN".ToList}

    Private Shared Property RotorBeta As New Plot(Of Char).ThinRotorPlot With
        {.Substitutes = "LEYJVCNIXWPBQMDRTAKZGFUHOS".ToList}
    Private Shared Property RotorGamma As New Plot(Of Char).ThinRotorPlot With
        {.Substitutes = "FSOKANUERHMBTIYCWLQPZXVGJD".ToList}

    Private Shared Property ReflectorB As New Plot(Of Char).ReflectorPlot With
        {.SwapsA = "ABCDEFGIJKMTV".ToList, .SwapsB = "YRUHQSLPXNOZW".ToList, .ExtraLetter = Nothing}
    Private Shared Property ReflectorC As New Plot(Of Char).ReflectorPlot With
        {.SwapsA = "ABCDEGHKLMNQS".ToList, .SwapsB = "FVPJIOYRZXWTU".ToList, .ExtraLetter = Nothing}

    Private Shared Property ReflectorBThin As New Plot(Of Char).ReflectorPlot With
        {.SwapsA = "ABCDFGHILMRST".ToList, .SwapsB = "ENKQUYWJOPXZV".ToList, .ExtraLetter = Nothing}
    Private Shared Property ReflectorCThin As New Plot(Of Char).ReflectorPlot With
        {.SwapsA = "ABCEFGHILPQSU".ToList, .SwapsB = "RDOJNTKVMWZXY".ToList, .ExtraLetter = Nothing}

    Public Shared Property EnigmaI_1938dec As New Plot(Of Char) With
        {.Alphabet = Alphabet,
         .Plugboard = New Plot(Of Char).PlugboardPlot With {.CableCount = 10},
         .EntryWheel = New Plot(Of Char).EntryWheelPlot With {.Substitutes = Alphabet},
         .RotorCount = 3,
         .Rotors = New List(Of Plot(Of Char).RotorPlot) From {RotorI, RotorII, RotorIII, RotorIV, RotorV},
         .ThinRotorCount = 0,
         .ThinRotors = New List(Of Plot(Of Char).ThinRotorPlot) From {},
         .RotatingReflector = False,
         .Reflectors = New List(Of Plot(Of Char).ReflectorPlot) From {ReflectorB}}

    Public Shared Property EnigmaI_1938dec_cfg As New Configuration(Of Char) With
        {.Alphabet = Alphabet,
         .Plugboard = New Configuration(Of Char).PlugboardCfg With
                      {.SwapsA = "ABCDEFGHIJ".ToList, .SwapsB = "QRSTUVWXYZ".ToList},
         .Rotors = New List(Of Configuration(Of Char).RotorCfg) From
         {New Configuration(Of Char).RotorCfg With {.Index = 1, .Display = "A"c, .RingSetting = "A"c},
          New Configuration(Of Char).RotorCfg With {.Index = 2, .Display = "A"c, .RingSetting = "A"c},
          New Configuration(Of Char).RotorCfg With {.Index = 3, .Display = "A"c, .RingSetting = "A"c}},
         .ThinRotors = New List(Of Configuration(Of Char).ThinRotorCfg),
         .Reflector = New Configuration(Of Char).ReflectorCfg With {.Index = 1, .Display = "A"c, .RingSetting = "A"c}}

End Class
