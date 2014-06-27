''' <summary>
''' Encapsulates commonly used static methods. 
''' </summary>
''' <remarks></remarks>
Public Class Util

    ''' <summary>
    ''' Compares all items in a list for equility, hence determines whether lists are equal. 
    ''' </summary>
    ''' <typeparam name="T">Type of the lists. </typeparam>
    ''' <param name="l">Left list. </param>
    ''' <param name="r">Right list. </param>
    ''' <returns>Whether the lists are equal. </returns>
    ''' <remarks></remarks>
    Public Shared Function ListsAreEqual(Of T)(l As List(Of T), r As List(Of T)) As Boolean
        If l Is Nothing AndAlso r Is Nothing Then
            Return True ' Both = nothing 
        ElseIf l Is Nothing OrElse r Is Nothing Then
            Return False ' Only one item is nothing!
        End If

        If l.Count <> r.Count Then Return False

        For i = 0 To l.Count - 1
            If Not l(i).Equals(r(i)) Then Return False
        Next

        Return True
    End Function ' ListsAreEqual

End Class ' Util
