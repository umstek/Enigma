Imports Enigma.Util

''' <summary>
''' Represents a setting or a certain key configuration of Enigma machine. 
''' </summary>
''' <typeparam name="E">Type of the machine. </typeparam>
''' <remarks></remarks>
Public Class Configuration(Of E)

    Public Property Alphabet As List(Of E)

    Public Property Plugboard As PlugboardCfg

    Public Property Rotors As List(Of RotorCfg)

    Public Property ThinRotors As List(Of ThinRotorCfg)

    Public Property Reflector As ReflectorCfg

    Public Class PlugboardCfg
        Public Property SwapsA As List(Of E)
        Public Property SwapsB As List(Of E)

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

    Public Class ThinRotorCfg
        Public Property Index As Integer
        Public Property RingSetting As E
        Public Property Display As E

        Public Shared Operator =(l As ThinRotorCfg, r As ThinRotorCfg) As Boolean
            Return l.Index = r.Index AndAlso l.RingSetting.Equals(r.RingSetting)
            ' We don't simply mind the display, 
            ' because it's the password - but we store it...!
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

    Public Class RotorCfg
        Inherits ThinRotorCfg

        Public Overrides Function Equals(obj As Object) As Boolean
            If Not TypeOf obj Is RotorCfg Then Return False

            Return Me = CType(obj, RotorCfg)
        End Function ' Equals

    End Class ' RotorCfg

    Public Class ReflectorCfg
        Inherits ThinRotorCfg

        Public Overrides Function Equals(obj As Object) As Boolean
            If Not TypeOf obj Is ReflectorCfg Then Return False

            Return Me = CType(obj, ReflectorCfg)
        End Function ' Equals

    End Class ' ReflectorCfg

    Public Shared Operator =(l As Configuration(Of E), r As Configuration(Of E)) As Boolean
        If Not (ListsAreEqual(l.Alphabet, r.Alphabet) AndAlso
               l.Plugboard = r.Plugboard AndAlso
               ListsAreEqual(l.Rotors, r.Rotors) AndAlso
               ListsAreEqual(l.ThinRotors, r.ThinRotors) AndAlso
               l.Reflector = r.Reflector) Then Return False

        Return True
    End Operator ' =

    Public Shared Operator <>(l As Configuration(Of E), r As Configuration(Of E)) As Boolean
        Return Not l = r
    End Operator ' <>

    Public Overrides Function Equals(obj As Object) As Boolean
        If Not TypeOf obj Is Configuration(Of E) Then Return False

        Return Me = CType(obj, Configuration(Of E))
    End Function ' Equals

End Class
