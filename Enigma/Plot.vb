Imports UMSTeK.Enigma.Util

''' <summary>
''' Represents a plan or a plot of an Enigma construction. 
''' We (I; oops) use this for the sake of persistence and GUI. 
''' </summary>
''' <typeparam name="TE">Type of the machine. </typeparam>
''' <remarks></remarks>
<Serializable>
Public Class Plot(Of TE)

    Public Property Alphabet As List(Of TE)

    Public Property Plugboard As PlugboardPlot

    Public Property EntryWheel As EntryWheelPlot

    Public Property RotorCount As Integer
    Public Property Rotors As List(Of RotorPlot)

    Public Property ThinRotorCount As Integer
    Public Property ThinRotors As List(Of ThinRotorPlot)

    Public Property RotatingReflector As Boolean
    Public Property Reflectors As List(Of ReflectorPlot)

    ''' <summary>
    ''' Represents a plot of an Enigma plugboard. 
    ''' </summary>
    ''' <remarks></remarks>
    Public Class PlugboardPlot
        Public Property CableCount As Integer

        Public Shared Operator =(l As PlugboardPlot, r As PlugboardPlot) As Boolean
            Return l.CableCount = r.CableCount
        End Operator

        Public Shared Operator <>(l As PlugboardPlot, r As PlugboardPlot) As Boolean
            Return Not l = r
        End Operator

        Public Overrides Function Equals(obj As Object) As Boolean
            If Not TypeOf obj Is PlugboardPlot Then Return False

            Return Me = CType(obj, PlugboardPlot)
        End Function

    End Class

    ''' <summary>
    ''' Represents a plot of an Enigma EntryWheel
    ''' </summary>
    ''' <remarks></remarks>
    Public Class EntryWheelPlot
        Public Property Substitutes As List(Of TE)

        Public Shared Operator =(l As EntryWheelPlot, r As EntryWheelPlot) As Boolean
            Return ListsAreEqual(l.Substitutes, r.Substitutes)
        End Operator

        Public Shared Operator <>(l As EntryWheelPlot, r As EntryWheelPlot) As Boolean
            Return Not l = r
        End Operator

        Public Overrides Function Equals(obj As Object) As Boolean
            If Not TypeOf obj Is EntryWheelPlot Then Return False

            Return Me = CType(obj, EntryWheelPlot)
        End Function

    End Class

    ''' <summary>
    ''' Represents a plot of an Enigma ThinRotor
    ''' </summary>
    ''' <remarks></remarks>
    Public Class ThinRotorPlot
        Public Property Substitutes As List(Of TE)

        Public Shared Operator =(l As ThinRotorPlot, r As ThinRotorPlot) As Boolean
            Return ListsAreEqual(l.Substitutes, r.Substitutes)
        End Operator

        Public Shared Operator <>(l As ThinRotorPlot, r As ThinRotorPlot) As Boolean
            Return Not l = r
        End Operator

        Public Overrides Function Equals(obj As Object) As Boolean
            If Not TypeOf obj Is ThinRotorPlot Then Return False
            If TypeOf obj Is RotorPlot Then Return False

            Return Me = CType(obj, ThinRotorPlot)
        End Function

    End Class

    ''' <summary>
    ''' Represents a plot of an Enigma Rotor
    ''' </summary>
    ''' <remarks></remarks>
    Public Class RotorPlot
        Inherits ThinRotorPlot
        Public Property Notches As List(Of TE)

        Public Overloads Shared Operator =(l As RotorPlot, r As RotorPlot) As Boolean
            Return ListsAreEqual(l.Notches, r.Notches) AndAlso
                ListsAreEqual(l.Substitutes, r.Substitutes)
        End Operator

        Public Overloads Shared Operator <>(l As RotorPlot, r As RotorPlot) As Boolean
            Return Not l = r
        End Operator

        Public Overrides Function Equals(obj As Object) As Boolean
            If Not TypeOf obj Is RotorPlot Then Return False

            Return Me = CType(obj, RotorPlot)
        End Function

    End Class

    ''' <summary>
    ''' Represents a plot of an Enigma Reflector. 
    ''' </summary>
    ''' <remarks></remarks>
    Public Class ReflectorPlot
        Public Property SwapsA As List(Of TE)
        Public Property SwapsB As List(Of TE)
        Public Property ExtraLetter As TE

        Public Overloads Shared Operator =(l As ReflectorPlot, r As ReflectorPlot) As Boolean
            If Not (ListsAreEqual(l.SwapsA, r.SwapsA) AndAlso
                ListsAreEqual(l.SwapsB, r.SwapsB)) Then Return False

            If Not l.ExtraLetter.Equals(r.ExtraLetter) Then Return False

            Return True
        End Operator

        Public Overloads Shared Operator <>(l As ReflectorPlot, r As ReflectorPlot) As Boolean
            Return Not l = r
        End Operator

        Public Overrides Function Equals(obj As Object) As Boolean
            If Not TypeOf obj Is ReflectorPlot Then Return False

            Return Me = CType(obj, ReflectorPlot)
        End Function

    End Class

    Public Shared Operator =(l As Plot(Of TE), r As Plot(Of TE)) As Boolean

        If Not (ListsAreEqual(l.Alphabet, r.Alphabet) AndAlso
                ListsAreEqual(l.Rotors, r.Rotors) AndAlso
                ListsAreEqual(l.ThinRotors, r.ThinRotors) AndAlso
                ListsAreEqual(l.Reflectors, r.Reflectors)) Then Return False

        If Not (l.EntryWheel = r.EntryWheel AndAlso
                l.Plugboard = r.Plugboard AndAlso
                l.RotatingReflector = r.RotatingReflector AndAlso
                l.RotorCount = r.RotorCount AndAlso
                l.ThinRotorCount = r.ThinRotorCount) Then Return False

        Return True
    End Operator

    Public Shared Operator <>(l As Plot(Of TE), r As Plot(Of TE)) As Boolean
        Return Not l = r
    End Operator

    Public Overrides Function Equals(obj As Object) As Boolean
        If Not TypeOf obj Is Plot(Of TE) Then Return False

        Return Me = CType(obj, Plot(Of TE))
    End Function

End Class
