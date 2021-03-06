﻿'    DWSIM Universal Flash Algorithm
'    Copyright 2020 Daniel Wagner O. de Medeiros
'
'    This file is part of DWSIM.
'
'    DWSIM is free software: you can redistribute it and/or modify
'    it under the terms of the GNU General Public License as published by
'    the Free Software Foundation, either version 3 of the License, or
'    (at your option) any later version.
'
'    DWSIM is distributed in the hope that it will be useful,
'    but WITHOUT ANY WARRANTY; without even the implied warranty of
'    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
'    GNU General Public License for more details.
'
'    You should have received a copy of the GNU General Public License
'    along with DWSIM.  If not, see <http://www.gnu.org/licenses/>.

Imports System.Math
Imports DWSIM.Interfaces.Enums

Namespace PropertyPackages.Auxiliary.FlashAlgorithms

    Public Class UniversalFlash

        Inherits FlashAlgorithm

        Dim nl As New NestedLoops
        Dim nl3 As New NestedLoops3PV3
        Dim nls As New NestedLoopsSLE
        Dim nlsv As New NestedLoopsSVLLE

        Public Sub New()
            MyBase.New
            Order = 0
        End Sub

        Public Overrides ReadOnly Property AlgoType As Interfaces.Enums.FlashMethod
            Get
                Return Interfaces.Enums.FlashMethod.Universal
            End Get
        End Property

        Public Overrides ReadOnly Property Description As String
            Get
                Return "Universal Equilibrium Flash Algorithm"
            End Get
        End Property

        Public Overrides ReadOnly Property Name As String
            Get
                Return "Universal Flash"
            End Get
        End Property

        Public Overrides ReadOnly Property MobileCompatible As Boolean
            Get
                Return False
            End Get
        End Property

        Public Overrides Function Flash_PT(Vz() As Double, P As Double, T As Double, PP As PropertyPackage, Optional ReuseKI As Boolean = False, Optional PrevKi() As Double = Nothing) As Object

            Dim Flashtype As String = FlashSettings(FlashSetting.ForceEquilibriumCalculationType)

            Dim hres = PerformHeuristicsTest(Vz, T, P, PP)

            If Flashtype = "Default" Then

                'chech possible phases to decide on suitable flash algorithm
                If hres.SolidPhase Or PP.ForcedSolids.Count > 0 Then
                    If hres.LiquidPhaseSplit Then
                        Flashtype = "SVLLE"
                    Else
                        Flashtype = "SVLE"
                    End If
                Else
                    If hres.LiquidPhaseSplit Then
                        Flashtype = "VLLE"
                    Else
                        Flashtype = "VLE"
                    End If
                End If
            End If

            Dim result As Object = Nothing

            Select Case Flashtype
                Case "VLE"
                    Dim nl = New NestedLoops
                    nl.FlashSettings = FlashSettings
                    result = nl.Flash_PT(Vz, P, T, PP, ReuseKI, PrevKi)
                Case "VLLE"
                    If Not FlashSettings(FlashSetting.ImmiscibleWaterOption) = True Then
                        Dim nl = New NestedLoops3PV3
                        nl.FlashSettings = FlashSettings
                        result = nl.Flash_PT(Vz, P, T, PP, ReuseKI, PrevKi)
                    Else
                        Dim imm As New NestedLoopsImmiscible With {.FlashSettings = FlashSettings}
                        result = imm.Flash_PT(Vz, P, T, PP, ReuseKI, PrevKi)
                    End If
                Case "SVLE", "SVLLE"
                    Dim nl As New NestedLoopsSVLLE
                    nl.FlashSettings = FlashSettings
                    result = nl.Flash_PT(Vz, P, T, PP, ReuseKI, PrevKi)
            End Select

            Return result

        End Function

        Public Overrides Function Flash_PH(ByVal Vz As Double(), ByVal P As Double, ByVal H As Double, ByVal Tref As Double, ByVal PP As PropertyPackages.PropertyPackage, Optional ByVal ReuseKI As Boolean = False, Optional ByVal PrevKi As Double() = Nothing) As Object

            Dim Flashtype As String = FlashSettings(FlashSetting.ForceEquilibriumCalculationType)

            Dim hres = PerformHeuristicsTest(Vz, Tref, P, PP)

            If Flashtype = "Default" Then

                'check possible phases to decide on suitable flash algorithm
                If hres.SolidPhase Or PP.ForcedSolids.Count > 0 Then
                    If hres.LiquidPhaseSplit Then
                        Flashtype = "SVLLE"
                    Else
                        Flashtype = "SVLE"
                    End If
                Else
                    If hres.LiquidPhaseSplit Then
                        Flashtype = "VLLE"
                    Else
                        Flashtype = "VLE"
                    End If
                End If
            End If

            Dim result As Object = Nothing

            Select Case Flashtype
                Case "VLE", "SVLE", "SVLLE"
                    Dim nl = New NestedLoops
                    nl.FlashSettings = FlashSettings
                    nl.PTFlashFunction = AddressOf Flash_PT
                    result = nl.Flash_PH(Vz, P, H, Tref, PP, ReuseKI, PrevKi)

                Case "VLLE"
                    Dim nl = New NestedLoops3PV3
                    nl.FlashSettings = FlashSettings
                    result = nl.Flash_PH(Vz, P, H, Tref, PP, ReuseKI, PrevKi)
            End Select

            Return result


        End Function

        Public Overrides Function Flash_PS(ByVal Vz As Double(), ByVal P As Double, ByVal S As Double, ByVal Tref As Double, ByVal PP As PropertyPackages.PropertyPackage, Optional ByVal ReuseKI As Boolean = False, Optional ByVal PrevKi As Double() = Nothing) As Object

            Dim Flashtype As String = FlashSettings(FlashSetting.ForceEquilibriumCalculationType)

            Dim sres = PerformHeuristicsTest(Vz, Tref, P, PP)

            If Flashtype = "Default" Then

                'check possible phases to decide on suitable flash algorithm
                If sres.SolidPhase Or PP.ForcedSolids.Count > 0 Then
                    If sres.LiquidPhaseSplit Then
                        Flashtype = "SVLLE"
                    Else
                        Flashtype = "SVLE"
                    End If
                Else
                    If sres.LiquidPhaseSplit Then
                        Flashtype = "VLLE"
                    Else
                        Flashtype = "VLE"
                    End If
                End If
            End If

            Dim result As Object = Nothing

            Select Case Flashtype
                Case "VLE", "SVLE", "SVLLE"
                    Dim nl = New NestedLoops
                    nl.FlashSettings = FlashSettings
                    nl.PTFlashFunction = AddressOf Flash_PT
                    result = nl.Flash_PS(Vz, P, S, Tref, PP, ReuseKI, PrevKi)

                Case "VLLE"
                    Dim nl = New NestedLoops3PV3
                    nl.FlashSettings = FlashSettings
                    result = nl.Flash_PS(Vz, P, S, Tref, PP, ReuseKI, PrevKi)
            End Select

            Return result


        End Function

        Public Overrides Function Flash_PV(Vz() As Double, P As Double, V As Double, Tref As Double, PP As PropertyPackage, Optional ReuseKI As Boolean = False, Optional PrevKi() As Double = Nothing) As Object

            Dim Flashtype As String = FlashSettings(FlashSetting.ForceEquilibriumCalculationType)

            Dim hres = PerformHeuristicsTest(Vz, Tref, P, PP)

            If Flashtype = "Default" Then

                'chech possible phases to decide on suitabel flash algorithm
                If hres.SolidPhase Or PP.ForcedSolids.Count > 0 Then
                    If hres.LiquidPhaseSplit Then
                        Flashtype = "SVLLE"
                    Else
                        Flashtype = "SVLE"
                    End If
                Else
                    If hres.LiquidPhaseSplit Then
                        Flashtype = "VLLE"
                    Else
                        Flashtype = "VLE"
                    End If
                End If
            End If

            Dim result As Object = Nothing

            Select Case Flashtype
                Case "VLE"
                    nl.FlashSettings = FlashSettings
                    result = nl.Flash_PV(Vz, P, V, Tref, PP, ReuseKI, PrevKi)
                Case "VLLE"
                    nl3.FlashSettings = FlashSettings
                    result = nl3.Flash_PV(Vz, P, V, Tref, PP, ReuseKI, PrevKi)
                Case "SVLE"
                    nls.FlashSettings = FlashSettings
                    result = nls.Flash_PV(Vz, P, V, Tref, PP, ReuseKI, PrevKi)
                Case "SVLLE"
                    nlsv.FlashSettings = FlashSettings
                    result = nlsv.Flash_PV(Vz, P, V, Tref, PP, ReuseKI, PrevKi)
            End Select

            Return result

        End Function

        Public Overrides Function Flash_TV(Vz() As Double, T As Double, V As Double, Pref As Double, PP As PropertyPackage, Optional ReuseKI As Boolean = False, Optional PrevKi() As Double = Nothing) As Object

            Dim Flashtype As String = FlashSettings(FlashSetting.ForceEquilibriumCalculationType)

            Dim hres = PerformHeuristicsTest(Vz, T, Pref, PP)

            If Flashtype = "Default" Then

                'chech possible phases to decide on suitabel flash algorithm
                If hres.SolidPhase Or PP.ForcedSolids.Count > 0 Then
                    If hres.LiquidPhaseSplit Then
                        Flashtype = "SVLLE"
                    Else
                        Flashtype = "SVLE"
                    End If
                Else
                    If hres.LiquidPhaseSplit Then
                        Flashtype = "VLLE"
                    Else
                        Flashtype = "VLE"
                    End If
                End If
            End If

            Dim result As Object = Nothing

            Select Case Flashtype
                Case "VLE"
                    nl.FlashSettings = FlashSettings
                    result = nl.Flash_TV(Vz, T, V, Pref, PP, ReuseKI, PrevKi)
                Case "VLLE"
                    nl3.FlashSettings = FlashSettings
                    result = nl3.Flash_TV(Vz, T, V, Pref, PP, ReuseKI, PrevKi)
                Case "SVLE"
                    nls.FlashSettings = FlashSettings
                    result = nls.Flash_TV(Vz, T, V, Pref, PP, ReuseKI, PrevKi)
                Case "SVLLE"
                    nlsv.FlashSettings = FlashSettings
                    result = nlsv.Flash_TV(Vz, T, V, Pref, PP, ReuseKI, PrevKi)
            End Select

            Return result

        End Function

    End Class

End Namespace



