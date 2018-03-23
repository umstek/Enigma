Imports UMSTeK.Enigma.Util

''' <summary>
''' Represents a setting or a certain key configuration of Enigma machine. 
''' </summary>
''' <typeparam name="TE">Type of the machine. </typeparam>
''' <remarks></remarks>
<Serializable>
Public Class Configuration(Of TE)

    Public Property Alphabet As List(Of TE)

    Public Property Plugboard As PlugboardCfg

    Public Property Rotors As List(Of RotorCfg)

    Public Property ThinRotors As List(Of ThinRotorCfg)

    Public Property Reflector As ReflectorCfg

    ''' <summary>
    ''' Represents the state of a Plugboard.
    ''' </summary>
    ''' <remarks></remarks>
    Public Class PlugboardCfg
        Public Property SwapsA As List(Of TE)
        Public Property SwapsB As List(Of TE)

        Public Shared Operator =(l As PlugboardCfg, r As PlugboardCfg) As Boolean
            Return ListsAreEqual(l.SwapsA, r.SwapsA) AndAlso
                ListsAreEqual(l.SwapsB, r.SwapsB)
        End Operator ' =

        Public Shared Operator <>(l As PlugboardCfg, r As PlugboardCfg) As Boolean
            Return Not l = r
        End Operator ' <>

        Public Overrides Function Equals(obj As Object) As Boolean
            If Not TypeOf obj Is PlugboardCfg Then Return False

            Return Me = CType(obj, PlugboardCfg)
        End Function ' Equals

    End Class ' PlugboardCfg

    ''' <summary>
    ''' Represents the state of a ThinRotor
    ''' </summary>
    ''' <remarks></remarks>
    Public Class ThinRotorCfg
        Public Property Index As Integer
        Public Property RingSetting As TE
        Public Property Display As TE

        Public Shared Operator =(l As ThinRotorCfg, r As ThinRotorCfg) As Boolean
            Return l.Index = r.Index AndAlso l.RingSetting.Equals(r.RingSetting)
            ' We don't simply mind the display 
            ' because it's the password; but we store it...!
        End Operator ' =

        Public Shared Operator <>(l As ThinRotorCfg, r As ThinRotorCfg) As Boolean
            Return Not l = r
        End Operator ' <>

        Public Overrides Function Equals(obj As Object) As Boolean
            If Not TypeOf obj Is ThinRotorCfg Then Return False
            If TypeOf obj Is RotorCfg OrElse TypeOf obj Is ReflectorCfg Then Return False

            Return Me = CType(obj, ThinRotorCfg)
        End Function ' Equals

    End Class ' ThinRotorCfg

    ''' <summary>
    ''' Represents the state of a Rotor
    ''' </summary>
    ''' <remarks></remarks>
    Public Class RotorCfg
        Inherits ThinRotorCfg

        Public Overrides Function Equals(obj As Object) As Boolean
            If Not TypeOf obj Is RotorCfg Then Return False

            Return Me = CType(obj, RotorCfg)
        End Function ' Equals

    End Class ' RotorCfg

    ''' <summary>
    ''' Represents the state of a Reflector. 
    ''' </summary>
    ''' <remarks></remarks>
    Public Class ReflectorCfg
        Inherits ThinRotorCfg

        Public Overrides Function Equals(obj As Object) As Boolean
            If Not TypeOf obj Is ReflectorCfg Then Return False

            Return Me = CType(obj, ReflectorCfg)
        End Function ' Equals

    End Class ' ReflectorCfg

    Public Shared Operator =(l As Configuration(Of TE), r As Configuration(Of TE)) As Boolean
        If Not (ListsAreEqual(l.Alphabet, r.Alphabet) AndAlso
               l.Plugboard = r.Plugboard AndAlso
               ListsAreEqual(l.Rotors, r.Rotors) AndAlso
               ListsAreEqual(l.ThinRotors, r.ThinRotors) AndAlso
               l.Reflector = r.Reflector) Then Return False

        Return True
    End Operator ' =

    Public Shared Operator <>(l As Configuration(Of TE), r As Configuration(Of TE)) As Boolean
        Return Not l = r
    End Operator ' <>

    Public Overrides Function Equals(obj As Object) As Boolean
        If Not TypeOf obj Is Configuration(Of TE) Then Return False

        Return Me = CType(obj, Configuration(Of TE))
    End Function ' Equals

End Class
