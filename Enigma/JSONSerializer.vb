''' <summary>
''' Encapsulates static methods for writing Enigma machines and/or 
''' configurations into files and reading back from files. 
''' </summary>
''' <remarks></remarks>
Public Class JSONSerializer

    ''' <summary>
    ''' Serializes an Enigma machine or configuration directly to a file. 
    ''' </summary>
    ''' <typeparam name="T">Type of the given object. </typeparam>
    ''' <param name="file">Absolute or relative file path. </param>
    ''' <param name="content">Object to be written. </param>
    ''' <remarks></remarks>
    Public Shared Sub Serialize(Of T)(file As String, content As T)
        Dim strWriter As New IO.StringWriter
        Dim jsonWriter As New Newtonsoft.Json.JsonTextWriter(strWriter)
        Dim jsonSerialize As New Newtonsoft.Json.JsonSerializer With
            {.Formatting = Newtonsoft.Json.Formatting.Indented}

        jsonSerialize.Serialize(jsonWriter, content)

        System.IO.File.WriteAllText(file, strWriter.ToString)
    End Sub ' Serialize

    ''' <summary>
    ''' Desirializes an Enigma machine or configuration directly from a file. 
    ''' </summary>
    ''' <typeparam name="T">The type of the object to be generated. </typeparam>
    ''' <param name="file">File to read data from. </param>
    ''' <returns>Generated plot or configuration. </returns>
    ''' <remarks></remarks>
    Public Shared Function Deserialize(Of T)(file As String) As T
        Dim strReader As New IO.StringReader(System.IO.File.ReadAllText(file))
        Dim jsonReader As New Newtonsoft.Json.JsonTextReader(strReader)
        Dim jsonSerialize As New Newtonsoft.Json.JsonSerializer With
            {.Formatting = Newtonsoft.Json.Formatting.Indented}

        Dim obj = jsonSerialize.Deserialize(jsonReader, GetType(T))

        Return CType(obj, T)
    End Function ' Deserialize

End Class ' JSONSerializer
