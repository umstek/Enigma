''' <summary>
''' Encapsulates static methods for writing Enigma machines and/or 
''' configurations into files and reading back from files. 
''' </summary>
''' <remarks></remarks>
Public Class JSONSerializer

    ''' <summary>
    ''' Serializes an Enigma machine or configuration to JSON text. 
    ''' </summary>
    ''' <typeparam name="T">Type of the given object. </typeparam>
    ''' <param name="content">Object to be written. </param>
    ''' <remarks></remarks>
    Public Shared Function Serialize(Of T)(content As T) As String
        Dim strWriter As New IO.StringWriter
        Dim jsonWriter As New Newtonsoft.Json.JsonTextWriter(strWriter)
        Dim jsonSerialize As New Newtonsoft.Json.JsonSerializer With
            {.Formatting = Newtonsoft.Json.Formatting.Indented}

        jsonSerialize.Serialize(jsonWriter, content)

        Return strWriter.ToString
    End Function ' Serialize

    ''' <summary>
    ''' Desirializes an Enigma machine or configuration from JSON text. 
    ''' </summary>
    ''' <typeparam name="T">The type of the object to be generated. </typeparam>
    ''' <param name="json">JSON text. </param>
    ''' <returns>Generated plot or configuration. </returns>
    ''' <remarks></remarks>
    Public Shared Function Deserialize(Of T)(json As String) As T
        Dim strReader As New IO.StringReader(json)
        Dim jsonReader As New Newtonsoft.Json.JsonTextReader(strReader)
        Dim jsonSerialize As New Newtonsoft.Json.JsonSerializer With
            {.Formatting = Newtonsoft.Json.Formatting.Indented}

        Dim obj = jsonSerialize.Deserialize(jsonReader, GetType(T))

        Return CType(obj, T)
    End Function ' Deserialize

End Class ' JSONSerializer
