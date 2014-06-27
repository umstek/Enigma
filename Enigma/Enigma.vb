''' <summary>
''' A digital representation (half-mathematical half-physical modal ;) ) 
''' of the popular German Enigma Machine with multiple improvements and extensions.
''' However, can support all the original Enigma machines.
''' </summary>
''' <typeparam name="E">Type of the Enigma machine. (Char, Byte etc)</typeparam>
''' <remarks></remarks>
<Serializable>
Public Class Enigma(Of E)

#Region "Instance Variables"

    Private ReadOnly alphabet As List(Of E)

    Private ReadOnly plugs As Plugboard

    Private ReadOnly entry As EntryWheel

    Private ReadOnly rotorCount As Integer
    Private ReadOnly availRotors As List(Of Rotor)
    <NonSerialized> Private rotors As List(Of Rotor)

    Private ReadOnly thinRotorCount As Integer
    Private ReadOnly availThinRotors As List(Of ThinRotor)
    <NonSerialized> Private thinRotors As List(Of ThinRotor)

    Private ReadOnly availReflectors As List(Of Reflector)
    Private ReadOnly rotatingReflector As Boolean
    <NonSerialized> Private cReflector As Reflector

#End Region

#Region "Constructing and persistance"

    ''' <summary>
    ''' Creates a new instance of an Enigma machine according to given specifications.
    ''' </summary>
    ''' <param name="alphabet">Enigma alphabet. </param>
    ''' <param name="plugsCount">Plugboard swaps count. </param>
    ''' <param name="entryWheelSubstitutes">Mapping of symbols from plugboard to first rotor. 
    ''' (Indeces) </param>
    ''' <param name="availableRotors">List of rotors to be used with the machine. 
    ''' (Substitutes, Notches) </param>
    ''' <param name="rotorCount">How many rotor slots in the machine. </param>
    ''' <param name="availableThinRotors">List of thin (static) rotors to be used. 
    ''' (Substitutes) </param>
    ''' <param name="thinRotorCount">How many thin rotor slots in the machine. </param>
    ''' <param name="availableReflectors">List of available reflectors. 
    ''' (Substitutes: first half originals, second half swaps vice versa.) </param>
    ''' <param name="rotatingReflector">Whether the reflector rotates as a rotor. 
    ''' (Only when there are no thin rotors.)</param>
    ''' <remarks></remarks>
    Public Sub New(alphabet As List(Of E),
                   plugsCount As Integer,
                   entryWheelSubstitutes As List(Of Integer),
                   availableRotors As List(Of Tuple(Of List(Of Integer), List(Of Integer))),
                   rotorCount As Integer,
                   availableThinRotors As List(Of Tuple(Of List(Of Integer))),
                   thinRotorCount As Integer,
                   availableReflectors As List(Of Tuple(Of List(Of Integer))),
                   rotatingReflector As Boolean)

        Me.alphabet = alphabet

        If plugsCount * 2 > alphabet.Count Then
            Throw New EnigmaException("Cannot have more plugs than half the alphabet length.", "plugsCount")
        End If
        Me.plugs = New Plugboard(plugsCount)

        If entryWheelSubstitutes.Count <> alphabet.Count Then
            Throw New EnigmaException("Substitute alphabet length mismatch.", "entryWheelSubstitutes")
        End If
        Me.entry = New EntryWheel(entryWheelSubstitutes)

        ' Checks an integer list for repeated numbers and out-of-range indeces. 
        Dim isValid = Function(lst As List(Of Integer)) (
                           Aggregate i In lst Distinct
                           Where i < alphabet.Count
                           Into Count()
                           ) = alphabet.Count

        ' Only valid rotors are selected!
        Me.availRotors = Aggregate rot In availableRotors
                         Where isValid(rot.Item1)
                         Select New Rotor(rot.Item1, rot.Item2) Into ToList()
        If availRotors.Count < rotorCount Then
            Throw New EnigmaException("Cannot have empty rotor slots.", "rotorCount")
        End If
        Me.rotorCount = rotorCount

        ' Only valid thin rotors are selected. 
        Me.availThinRotors = Aggregate thin In availableThinRotors
                             Where isValid(thin.Item1)
                             Select New ThinRotor(thin.Item1) Into ToList()
        If availRotors.Count < thinRotorCount Then
            Throw New EnigmaException("Cannot have empty thin rotor slots.", "thinRotorCount")
        End If
        Me.thinRotorCount = thinRotorCount

        ' Only valid reflectors are selected. 
        Me.availReflectors = Aggregate refl In availableReflectors
                             Where isValid(refl.Item1)
                             Select New Reflector(refl.Item1) Into ToList()
        If availReflectors.Count = 0 Then
            Throw New EnigmaException("Cannot create Enigma without Reflectors.", "availableReflectors")
        End If
        Me.rotatingReflector = rotatingReflector

    End Sub ' New

    ''' <summary>
    ''' Creates Enigma machine using a given plot. 
    ''' </summary>
    ''' <param name="plot">A plot describing the machine. </param>
    ''' <remarks></remarks>
    Public Sub New(plot As Plot(Of E))
        Me.New(plot.Alphabet,
               plot.Plugboard.CableCount,
               (Aggregate sym In plot.EntryWheel.Substitutes
                Select plot.Alphabet.IndexOf(sym) Into ToList()),
               (Aggregate rotor In plot.Rotors
                Select New Tuple(Of List(Of Integer), List(Of Integer))((Aggregate sym In rotor.Substitutes
                                                                         Select plot.Alphabet.IndexOf(sym) Into ToList()),
                                                                        (Aggregate sym In rotor.Notches
                                                                         Select plot.Alphabet.IndexOf(sym) Into ToList()))
                                                                 Into ToList()),
               plot.RotorCount,
               (Aggregate trotor In plot.ThinRotors
                Select New Tuple(Of List(Of Integer))((Aggregate sym In trotor.Substitutes
                                                       Select plot.Alphabet.IndexOf(sym) Into ToList()))
                                               Into ToList()),
               plot.ThinRotorCount,
               (Aggregate ref In plot.Reflectors
                Select New Tuple(Of List(Of Integer))((From sym In ref.SwapsA
                                                       Select plot.Alphabet.IndexOf(sym)).
                                                      Concat(
                                                       From sym In ref.SwapsB
                                                       Select plot.Alphabet.IndexOf(sym)).
                                                      Concat(
                                                       If(ref.ExtraLetter Is Nothing OrElse
                                                          plot.Alphabet.IndexOf(ref.ExtraLetter) = -1,
                                                          New Integer() {},
                                                          {plot.Alphabet.IndexOf(ref.ExtraLetter)})).ToList)
                                               Into ToList()),
               plot.RotatingReflector)

    End Sub ' New (plot)

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetPlot() As Plot(Of E)
        Return New Plot(Of E) With
               {.Alphabet = Me.alphabet,
                .Plugboard = Me.plugs.GetPlot,
                .EntryWheel = Me.entry.GetPlot(alphabet),
                .RotorCount = Me.rotorCount,
                .Rotors = (Aggregate rotor In availRotors
                          Select rotor.GetPlot(alphabet) Into ToList()),
                .ThinRotorCount = Me.thinRotorCount,
                .ThinRotors = (Aggregate trotor In availThinRotors
                               Select trotor.GetPlot(alphabet) Into ToList()),
                .RotatingReflector = Me.rotatingReflector,
                .Reflectors = (Aggregate ref In availReflectors
                               Select ref.GetPlot(alphabet) Into ToList())}
    End Function ' GetPlot

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetConfig() As Configuration(Of E)
        ' Rotors and ThinRotors lists need to be reversed, 
        ' Because the order is RTL in the machine but, LTR for outsiders. 
        Dim tmpRotors As New List(Of Rotor)(Me.rotors)
        tmpRotors.Reverse()
        Dim tmpThinRotors As New List(Of ThinRotor)(Me.thinRotors)
        tmpThinRotors.Reverse()
        Return New Configuration(Of E) With
               {.Alphabet = Me.alphabet,
                .Plugboard = Me.plugs.GetConfig(alphabet),
                .Rotors = (Aggregate rotor In tmpRotors
                           Select (Function() As Configuration(Of E).RotorCfg
                                       Dim cfg = rotor.GetConfig(alphabet)
                                       cfg.Index = Me.availRotors.IndexOf(rotor) + 1
                                       Return cfg
                                   End Function).Invoke Into ToList()),
                .ThinRotors = (Aggregate trotor In tmpThinRotors
                               Select (Function() As Configuration(Of E).ThinRotorCfg
                                           Dim cfg = trotor.GetConfig(alphabet)
                                           cfg.Index = Me.availThinRotors.IndexOf(trotor) + 1
                                           Return cfg
                                       End Function).Invoke Into ToList()),
                .Reflector = (Function() As Configuration(Of E).ReflectorCfg
                                  Dim cfg = cReflector.GetConfig(alphabet)
                                  cfg.Index = Me.availReflectors.IndexOf(cReflector) + 1
                                  Return cfg
                              End Function).Invoke}

    End Function ' GetConfig

#End Region

#Region "Preperation and preprocessing"

    ''' <summary>
    ''' Prepare the machine for encryption/decryption by providing required information. 
    ''' (i.e. Ring settings, initial key etc.)
    ''' </summary>
    ''' <param name="plugboardSwaps">Swapped pairs of the plugboard. 
    ''' (Letters in the 1st half of the list are swapped with those of 2nd half vice versa.)</param>
    ''' <param name="selectedRotors">(0 based) indeces of rotors to be selected. 
    ''' Will be arranged in enigma machine in LTR order. </param>
    ''' <param name="selectedThinRotors">(0 based) indeces of thin (static) rotors to be selected. 
    ''' Will be arranged in enigma machine in LTR order after rotors. </param>
    ''' <param name="selectedReflector">(0 based) index of reflector to be used. </param>
    ''' <remarks></remarks>
    Public Sub Prepare(plugboardSwaps As List(Of Integer),
                       selectedRotors As List(Of Tuple(Of Integer, Integer, Integer)),
                       selectedThinRotors As List(Of Tuple(Of Integer, Integer, Integer)),
                       selectedReflector As Tuple(Of Integer, Integer, Integer))

        Me.plugs.Prepare(plugboardSwaps)

        If selectedRotors.Count <> rotorCount Then
            Throw New EnigmaException("Invalid rotor count", "selectedRotors")
        End If
        Try
            selectedRotors.Reverse() ' User input is left to right, but machine execution is right to left. 
            Me.rotors = Aggregate t3rotor In selectedRotors
                        Select (Function()
                                    Dim rot = CType(availRotors(t3rotor.Item1).Clone, Rotor)
                                    rot.Prepare(t3rotor.Item2, t3rotor.Item3)
                                    Return rot
                                End Function).Invoke Into ToList()
        Catch ex As IndexOutOfRangeException
            Throw New EnigmaException("Wrong rotor index(es)", "selectedRotors")
        Catch ex As Exception
            Throw ex
        End Try

        If selectedThinRotors.Count <> thinRotorCount Then
            Throw New EnigmaException("Invalid thin rotor count", "selectedThinRotors")
        End If
        Try
            selectedThinRotors.Reverse() ' User input is left to right, but machine execution is right to left. 
            Me.thinRotors = Aggregate t3tRotor In selectedThinRotors
                            Select (Function()
                                        Dim rot = CType(availThinRotors(t3tRotor.Item1).Clone, ThinRotor)
                                        rot.Prepare(t3tRotor.Item2, t3tRotor.Item3)
                                        Return rot
                                    End Function).Invoke Into ToList()
        Catch ex As IndexOutOfRangeException
            Throw New EnigmaException("Wrong thin rotor index(es)", "selectedThinRotors")
        Catch ex As Exception
            Throw ex
        End Try

        If selectedReflector.Item1 >= availReflectors.Count Then
            Throw New EnigmaException("Wrong reflector index", "selectedReflector")
        End If
        Me.cReflector = (Function()
                             Dim rot = CType(availReflectors(selectedReflector.Item1).Clone, Reflector)
                             rot.Prepare(selectedReflector.Item2, selectedReflector.Item3)
                             Return rot
                         End Function).Invoke

    End Sub ' Prepare

    Public Sub Prepare(config As Configuration(Of E))
        Me.Prepare((From sym In config.Plugboard.SwapsA
                    Select alphabet.IndexOf(sym)).
                   Concat(
                    From sym In config.Plugboard.SwapsB
                    Select alphabet.IndexOf(sym)).ToList,
                   (Aggregate rotor In config.Rotors
                    Select New Tuple(Of Integer, Integer, Integer)(rotor.Index - 1,
                                                                   alphabet.IndexOf(rotor.RingSetting),
                                                                   alphabet.IndexOf(rotor.Display))
                                                               Into ToList()),
                   (Aggregate trotor In config.ThinRotors
                    Select New Tuple(Of Integer, Integer, Integer)(trotor.Index - 1,
                                                                   alphabet.IndexOf(trotor.RingSetting),
                                                                   alphabet.IndexOf(trotor.Display))
                                                               Into ToList()),
                   (New Tuple(Of Integer, Integer, Integer)(config.Reflector.Index - 1,
                                                            alphabet.IndexOf(config.Reflector.RingSetting),
                                                            alphabet.IndexOf(config.Reflector.Display))))
    End Sub ' Prepare (config)

#End Region

#Region "Processing, parsing"

    ''' <summary>
    ''' Process a single value and geth the result.
    ''' </summary>
    ''' <param name="value">The symbol to be encrypted/decrypted. </param>
    ''' <returns>Processed symbol. </returns>
    ''' <remarks></remarks>
    Public Function Parse(value As E) As E
        ' Unknown value, return itself unchanged.
        If Not alphabet.Contains(value) Then Return value

        ' Get the index of the given symbol.
        Dim index = alphabet.IndexOf(value)

        ' Plugboard parse.
        index = plugs.Parse(index)

        ' Entrywheel parse.
        index = entry.Parse(index)

        ' First rotor always turns. 
        rotors(0).Turn()
        ' Parse with all rotors and turn them as necessary.
        index = rotors(0).Parse(index)
        For i = 1 To rotorCount - 1 Step 1
            If rotors(i - 1).Kicks Then rotors(i).Turn()
            index = rotors(i).Parse(index)
        Next

        'Thin rotors don't turn. Just parse.
        For i = 0 To thinRotorCount - 1 Step 1
            index = thinRotors(i).Parse(index)
        Next

        ' Parse with reflector. 
        ' Rotating reflector is supported when there are no thin rotors.
        If rotatingReflector AndAlso
            thinRotorCount = 0 AndAlso
            rotors.Last.Kicks Then cReflector.Turn()
        index = cReflector.Parse(index)

        ' Backward parse with thin rotors.
        For i = thinRotorCount - 1 To 0 Step -1
            index = thinRotors(i).Reparse(index)
        Next

        ' Backword parse with rotors.
        For i = rotorCount - 1 To 0 Step -1
            index = rotors(i).Reparse(index)
        Next

        ' Backward parse with entrywheel.
        index = entry.Reparse(index)

        ' Backward parse is same as the forward parse with plugboard.
        index = plugs.Parse(index)

        ' Return corresponding symbol.
        Return alphabet(index)
    End Function ' Parse (Symbol)

    ''' <summary>
    ''' Process a collection one by one and obtain the output as necessary. 
    ''' </summary>
    ''' <param name="values">Symbols to be processed. </param>
    ''' <returns>Processed symbols. </returns>
    ''' <remarks></remarks>
    Public Iterator Function Parse(values As IEnumerable(Of E)) As IEnumerable(Of E)
        For Each value In values
            Yield Parse(value)
        Next
    End Function ' Parse (Iterator)

    ''' <summary>
    ''' Parse a chunk of symbols at once. 
    ''' </summary>
    ''' <param name="values">A list of symbols to be processed. </param>
    ''' <returns>Processed list of symbols. </returns>
    ''' <remarks></remarks>
    Public Function Parse(values As List(Of E)) As List(Of E)
        Dim result = New List(Of E)

        For Each value In values
            result.Add(Parse(value))
        Next

        Return result
    End Function ' Parse (Chunk)

#End Region

#Region "Machine sub-parts classes"

    ''' <summary>
    ''' Represents the plugboard in Enigma machine. 
    ''' </summary>
    ''' <remarks></remarks>
    <Serializable>
    Private Class Plugboard

        Private ReadOnly cableCount As Integer
        <NonSerialized> Private swaps As List(Of Integer)

        ''' <summary>
        ''' Creates a new instance of plugboard with letting the specified number of symbol pairs to be swapped.
        ''' </summary>
        ''' <param name="cableCount">The number of symbol pairs that ca be swapped. </param>
        ''' <remarks></remarks>
        Public Sub New(cableCount As Integer)
            Me.cableCount = cableCount
        End Sub ' New

        ''' <summary>
        ''' Prepares plugboard for accepting input by specifying the swapped symbols. 
        ''' </summary>
        ''' <param name="swaps">The list of symbols to be swapped. 
        ''' First half contains the originals and the second half contains the replacements vice versa.</param>
        ''' <remarks></remarks>
        Public Sub Prepare(swaps As List(Of Integer))
            If cableCount * 2 <> swaps.Count Then
                Throw New PlugboardException("Incorrect swaps count. ", "swaps")
            End If
            If swaps.Count > swaps.Distinct.Count Then
                Throw New PlugboardException("Incorrect swapping. ", "swaps")
            End If
            Me.swaps = swaps
        End Sub ' Prepare

        ''' <summary>
        ''' Processes an input and gives the output. 
        ''' </summary>
        ''' <param name="value">The value (index) to be parsed. </param>
        ''' <returns>The parsed value (index)</returns>
        ''' <remarks></remarks>
        Public Function Parse(value As Integer) As Integer
            ' If plugboard has the index (symbol) in the list, return the substitute.
            ' Else return itself.
            Return If(swaps.Contains(value),
                      swaps((swaps.IndexOf(value) + swaps.Count \ 2) Mod swaps.Count),
                      value)
        End Function ' Parse

        ''' <summary>
        ''' Gets a plot which represents the plugboard. 
        ''' </summary>
        ''' <returns>Plugboard plot. </returns>
        ''' <remarks></remarks>
        Public Function GetPlot() As Plot(Of E).PlugboardPlot
            Return New Plot(Of E).PlugboardPlot With {.CableCount = Me.cableCount}
        End Function ' GetPlot

        ''' <summary>
        ''' Gets currrent configuration of the plugboard. 
        ''' </summary>
        ''' <param name="alphabet">Symbols used in the machine. </param>
        ''' <returns>Plugboard configuration. </returns>
        ''' <remarks></remarks>
        Public Function GetConfig(alphabet As List(Of E)) As Configuration(Of E).PlugboardCfg
            Return New Configuration(Of E).PlugboardCfg With
                   {.SwapsA = (Aggregate index In Me.swaps
                               Take (Me.swaps.Count \ 2) Select alphabet(index) Into ToList()),
                    .SwapsB = (Aggregate index In Me.swaps
                               Skip (Me.swaps.Count \ 2) Select alphabet(index) Into ToList())}
        End Function ' GetConfig

    End Class ' Plugboard

    ''' <summary>
    ''' Represents the entry wheel (ETW) aka stator in Enigma machine. 
    ''' </summary>
    ''' <remarks></remarks>
    <Serializable>
    Private Class EntryWheel

        Private ReadOnly substitutes As List(Of Integer)

        ''' <summary>
        ''' Creates a new instance of stator using given sequence of indeces as substitutes. 
        ''' </summary>
        ''' <param name="substitutes">The substitute sequence of indexes. </param>
        ''' <remarks></remarks>
        Public Sub New(substitutes As List(Of Integer))
            If substitutes.Count > substitutes.Distinct.Count Then
                Throw New EntryWheelException("Repeated symbols in substitutes list. ", "substitutes")
            End If
            Me.substitutes = substitutes
        End Sub ' New

        ''' <summary>
        ''' Parses an input from the plugboard through the entry wheel and 
        ''' gives the output which is then sent to the first rotor.
        ''' </summary>
        ''' <param name="value"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function Parse(value As Integer) As Integer
            Return substitutes(value)
        End Function ' Parse

        ''' <summary>
        ''' Processes the (reparsed) output captured from the first rotor and directs it to the plugboard. 
        ''' </summary>
        ''' <param name="value">Value captured from the first rotor. </param>
        ''' <returns>Reparsed (index) value. </returns>
        ''' <remarks></remarks>
        Public Function Reparse(value As Integer) As Integer
            Return substitutes.IndexOf(value)
        End Function ' Reparse

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="alphabet"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetPlot(alphabet As List(Of E)) As Plot(Of E).EntryWheelPlot
            Return New Plot(Of E).EntryWheelPlot With
                   {.Substitutes = Aggregate index In Me.substitutes
                                   Select alphabet(index) Into ToList()}
        End Function ' GetPlot

    End Class ' EntryWheel

    ''' <summary>
    ''' Represents a (static) thin rotor which is used in Enigma machine. 
    ''' This is a rotor which does not have the ability to rotate automatically but 
    ''' can be rotated manually (i.e.: according to the given key) and has a ring setting.
    ''' </summary>
    ''' <remarks>Acts as the base class for both rotor and reflector. </remarks>
    <Serializable>
    Private Class ThinRotor
        Implements ICloneable

        Protected substitutes As List(Of Integer)
        <NonSerialized> Protected ringSetting As Integer
        <NonSerialized> Protected face As Integer

        Protected Sub New()
            ' Cloning support
        End Sub

        ''' <summary>
        ''' Creates a new instance of static rotor by using the given sequence of substitutes. 
        ''' </summary>
        ''' <param name="substitutes">A sequence of substitutes (indeces).</param>
        ''' <remarks></remarks>
        Public Sub New(substitutes As List(Of Integer))
            Me.substitutes = substitutes
        End Sub ' New

        ''' <summary>
        ''' Prepares the static rotor for accepting input by giving the initial setings. 
        ''' </summary>
        ''' <param name="ringSetting">The position in which the letter ring 
        ''' (which has the notches on it when considering a standard rotor) 
        ''' is positioned relative to the inner ring which contains the real wirings. </param>
        ''' <param name="display">The value which is displayed initially to the user. </param>
        ''' <remarks></remarks>
        Public Sub Prepare(ringSetting As Integer, display As Integer)
            If ringSetting >= substitutes.Count Then
                Throw New ThinRotorException("Invalid ring setting", "ringSetting")
            End If
            If display >= substitutes.Count Then
                Throw New ThinRotorException("Invalid display value", "display")
            End If
            Me.ringSetting = ringSetting
            Me.face = display
        End Sub ' Prepare

        ''' <summary>
        ''' Gets or sets the symbol (index of it) at the window.
        ''' </summary>
        ''' <value>Item at the window. </value>
        ''' <returns>Item at the window.</returns>
        ''' <remarks></remarks>
        Public Property Display As Integer
            Get
                Return face
            End Get
            Set(value As Integer)
                face = value
            End Set
        End Property ' Display

        Private ReadOnly Property Shift As Integer
            Get
                ' Current absolute offset = face - ringsetting.
                Return face - ringSetting
            End Get
        End Property

        ' __________________________________________________
        ' index(x) is the inverse of item(x) operation
        ' o = item(i + f - r) - f + r -- (1)
        ' o + f - r = item(i + f - r)
        ' index(o + f - r) = i + f - r
        ' index(o + f - r) - f + r = i -- (2)
        ' __________________________________________________

        ''' <summary>
        ''' Parses a(n) (index of) value and return the corresponding output. 
        ''' </summary>
        ''' <param name="value">Index to be parsed. </param>
        ''' <returns>The parsed index. </returns>
        ''' <remarks></remarks>
        Public Function Parse(value As Integer) As Integer
            ' Parse 1
            Return (substitutes.Item(
                    (value + Shift + substitutes.Count) Mod substitutes.Count
                    ) - Shift + substitutes.Count) Mod substitutes.Count
        End Function ' Parse

        ''' <summary>
        ''' Reparses a(n) (index of) value and return the corresponding output. 
        ''' </summary>
        ''' <param name="value">Index to be reparsed. </param>
        ''' <returns>The reparsed index. </returns>
        ''' <remarks>Reparsing is the reverse procedure of parsing. 
        ''' (i.e.: parsing = f(x) --> reparsing = g(x) where g(x) is the inverse function of f(x).</remarks>
        Public Function Reparse(value As Integer) As Integer
            ' Parse 2
            Return (substitutes.IndexOf(
                    (value + Shift + substitutes.Count) Mod substitutes.Count
                    ) - Shift + substitutes.Count) Mod substitutes.Count
        End Function ' Reparse

        Public Function Clone() As Object Implements ICloneable.Clone
            Dim doli = New ThinRotor
            doli.substitutes = New List(Of Integer)(Me.substitutes)

            Return doli
        End Function ' Clone

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="alphabet"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetPlot(alphabet As List(Of E)) As Plot(Of E).ThinRotorPlot
            Return New Plot(Of E).ThinRotorPlot With
                   {.Substitutes = Aggregate index In Me.substitutes
                                   Select alphabet(index) Into ToList()}
        End Function ' GetPlot

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="alphabet"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetConfig(alphabet As List(Of E)) As Configuration(Of E).ThinRotorCfg
            Return New Configuration(Of E).ThinRotorCfg With
                   {.Display = alphabet(Me.Display),
                    .RingSetting = alphabet(Me.ringSetting)}
        End Function ' GetConfig

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="obj"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overrides Function Equals(obj As Object) As Boolean
            If Not TypeOf obj Is ThinRotor Then Return False
            If TypeOf obj Is Rotor OrElse TypeOf obj Is Reflector Then Return False

            Return Util.ListsAreEqual(Me.substitutes, CType(obj, ThinRotor).substitutes)
        End Function

    End Class ' ThinRotor

    ''' <summary>
    ''' Represents a rotor which is used in Enigma machine. 
    ''' This is a normal rotor which rotates when triggered. 
    ''' </summary>
    ''' <remarks>This is derived from ThinRotor class. </remarks>
    <Serializable>
    Private Class Rotor
        Inherits ThinRotor
        Implements ICloneable

        Private notches As List(Of Integer)

        Private Sub New()
            ' Cloning support
        End Sub

        ''' <summary>
        ''' Creates a new instance of Rotor using the given sequence of substitutes and notches. 
        ''' </summary>
        ''' <param name="substitutes">A sequence of substitutes (indeces).</param>
        ''' <param name="notches">The trigger points on the alphabet ring 
        ''' which causes the next wheel (rotor) to rotate when encountered. </param>
        ''' <remarks></remarks>
        Public Sub New(substitutes As List(Of Integer), notches As List(Of Integer))
            MyBase.New(substitutes)
            If notches.Count < notches.Distinct.Count Then
                Throw New RotorException("Repeated symbols in notches", "notches")
            End If
            Me.notches = notches
        End Sub ' New

        ''' <summary>
        ''' Turns the wheel to display the next value at the window. 
        ''' </summary>
        ''' <remarks></remarks>
        Public Sub Turn()
            Me.face = (Me.face + 1) Mod substitutes.Count
        End Sub ' Turn

        ''' <summary>
        ''' Returns a value representing wheter this wheel wants the next wheel to rotate. 
        ''' </summary>
        ''' <value>Whether this wheel kicks next wheel to rotate.</value>
        ''' <returns>Whether this wheel kicks next wheel to rotate.</returns>
        ''' <remarks></remarks>
        Public ReadOnly Property Kicks As Boolean
            Get
                Return notches.Contains(face)
            End Get
        End Property ' Kicks

        Public Overloads Function Clone() As Object
            Dim doli = New Rotor
            doli.substitutes = New List(Of Integer)(Me.substitutes)
            doli.notches = New List(Of Integer)(Me.notches)

            Return doli
        End Function ' Clone

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="alphabet"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overloads Function GetPlot(alphabet As List(Of E)) As Plot(Of E).RotorPlot
            Return New Plot(Of E).RotorPlot With
                   {.Substitutes = (Aggregate index In Me.substitutes
                                   Select alphabet(index) Into ToList()),
                    .Notches = (Aggregate index In Me.notches
                                Select alphabet(index) Into ToList())}
        End Function ' GetPlot

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="alphabet"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overloads Function GetConfig(alphabet As List(Of E)) As Configuration(Of E).RotorCfg
            Return New Configuration(Of E).RotorCfg With
                   {.Display = alphabet(Me.Display),
                    .RingSetting = alphabet(Me.ringSetting)}
        End Function ' GetConfig

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="obj"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overrides Function Equals(obj As Object) As Boolean
            If Not TypeOf obj Is Rotor Then Return False

            Return Util.ListsAreEqual(Me.substitutes, CType(obj, Rotor).substitutes) AndAlso
                Util.ListsAreEqual(Me.notches, CType(obj, Rotor).notches)
        End Function

    End Class ' Rotor

    <Serializable>
    Private Class Reflector
        Inherits ThinRotor
        Implements ICloneable

        Private Sub New()
            ' Cloning support
        End Sub

        ''' <summary>
        ''' Creates a new instance of reflector which collects the output from last (thin?)rotor and
        ''' swaps them according to the given sequence of substitutes and
        ''' returns the output back to the(thin?)rotor.
        ''' </summary>
        ''' <param name="swaps">The first half of the list contains the originals and 
        ''' the second half, the substitutes vice versa. 
        ''' If the list is of odd element count then the last element is bound to itself!
        ''' </param>
        ''' <remarks>
        ''' WARNING: What happens when the list is of odd number of items is
        ''' contrary to the standard Enigma specification.</remarks>
        Public Sub New(swaps As List(Of Integer))

            MyBase.New((Function()
                            ' First half originals.
                            ' Second half replacements, vice versa.
                            ' If list is odd, the last is bound to itself.

                            Dim substitutes As New List(Of Integer)
                            Dim half = swaps.Count \ 2

                            Dim subs0 = swaps.GetRange(0, half)
                            Dim subs1 = swaps.GetRange(half, half)

                            For i = 0 To swaps.Count - 1
                                If subs0.Contains(i) Then
                                    substitutes.Add(subs1(subs0.IndexOf(i)))
                                ElseIf subs1.Contains(i) Then
                                    substitutes.Add(subs0(subs1.IndexOf(i)))
                                Else
                                    ' This element must be the last in the list.
                                    If swaps(swaps.Count - 1) <> i Then
                                        Throw New ReflectorException("Last item mismatched.", "swaps")
                                    End If
                                    substitutes.Add(i)
                                End If
                            Next

                            Return substitutes
                        End Function).Invoke)

        End Sub ' New

        ''' <summary>
        ''' Turns the reflector by one position. 
        ''' </summary>
        ''' <remarks></remarks>
        Public Sub Turn()
            Me.face = (Me.face + 1) Mod substitutes.Count
        End Sub ' Turn

        Public Overloads Function Clone() As Object
            Dim doli = New Reflector
            doli.substitutes = New List(Of Integer)(Me.substitutes)

            Return doli
        End Function ' Clone

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="alphabet"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overloads Function GetPlot(alphabet As List(Of E)) As Plot(Of E).ReflectorPlot
            Return New Plot(Of E).ReflectorPlot With
                   {.SwapsA = (Aggregate index In Me.substitutes
                               Take Me.substitutes.Count \ 2 Select alphabet(index) Into ToList()),
                    .SwapsB = (Aggregate index In Me.substitutes
                               Skip Me.substitutes.Count \ 2 Skip While
                                   ((Me.substitutes.Count Mod 2 > 0) AndAlso (index = Me.substitutes.Count - 1))
                               Select alphabet(index) Into ToList()),
                    .ExtraLetter = If((Me.substitutes.Count Mod 2 > 0), alphabet(Me.substitutes.Count - 1), Nothing)}
        End Function ' GetPlot

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="alphabet"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overloads Function GetConfig(alphabet As List(Of E)) As Configuration(Of E).ReflectorCfg
            Return New Configuration(Of E).ReflectorCfg With
                   {.Display = alphabet(Me.Display),
                    .RingSetting = alphabet(Me.ringSetting)}
        End Function ' GetConfig

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="obj"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overrides Function Equals(obj As Object) As Boolean
            If Not TypeOf obj Is Reflector Then Return False

            Return Util.ListsAreEqual(Me.substitutes, CType(obj, Reflector).substitutes)
        End Function

    End Class ' Reflector

#End Region

#Region "Exceptions"

    Public Class PlugboardException
        Inherits ArgumentException
        Public Sub New(message As String, paramName As String)
            MyBase.New(message, paramName)
        End Sub
    End Class

    Public Class EntryWheelException
        Inherits ArgumentException
        Public Sub New(message As String, paramName As String)
            MyBase.New(message, paramName)
        End Sub
    End Class

    Public Class RotorException
        Inherits ArgumentException
        Public Sub New(message As String, paramName As String)
            MyBase.New(message, paramName)
        End Sub
    End Class

    Public Class ThinRotorException
        Inherits ArgumentException
        Public Sub New(message As String, paramName As String)
            MyBase.New(message, paramName)
        End Sub
    End Class

    Public Class ReflectorException
        Inherits ArgumentException
        Public Sub New(message As String, paramName As String)
            MyBase.New(message, paramName)
        End Sub
    End Class

    Public Class EnigmaException
        Inherits ArgumentException
        Public Sub New(message As String, paramName As String)
            MyBase.New(message, paramName)
        End Sub
    End Class

#End Region

End Class
