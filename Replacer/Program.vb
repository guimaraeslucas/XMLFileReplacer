Imports System
Imports System.Xml

Module Program
    Dim bDebug As Boolean = False
    Dim sFileToProcess As String = ""
    Dim sStoreFile As String
    Dim sStoreInclude As String
    Dim CurrentColor = Console.BackgroundColor

    Dim oOutputNode As XmlNodeList

    Sub Main()
        'Config console
        Console.CursorSize = 100
        Console.CursorVisible = True
        DefaultColor()
        Console.Clear()

        'Start App      
        Console.ForegroundColor = ConsoleColor.White
        Console.BackgroundColor = ConsoleColor.DarkBlue
        Console.WriteLine(System.Reflection.Assembly.GetExecutingAssembly().GetName().Name & " [version: " & System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString & "]")
        DefaultColor()
        Console.WriteLine("---" & Environment.NewLine)

        'Display where we are running
        DisplayInfo("App running at " & Reflection.Assembly.GetExecutingAssembly().Location)
        'Get command line arguments
        Dim sCommandLineArguments As String() = Environment.GetCommandLineArgs()

        'Config file set, try to open and read it
        If sCommandLineArguments.Length > 1 Then

            sFileToProcess = sCommandLineArguments(1)

            'Try to open and process file and xml
            Try
                DisplayAction("Trying to open file " & sFileToProcess)
                Dim xmlDoc As New XmlDocument()
                xmlDoc.Load(sFileToProcess)
                DisplayAction("File opened")
                DisplayAction("Getting info")
                Console.WriteLine("----------------")
                Dim oFileInfo As XmlNode = xmlDoc.DocumentElement.SelectSingleNode("fileinfo")
                DisplayInfo(oFileInfo.SelectSingleNode("desc").InnerText)
                DisplayInfo(oFileInfo.SelectSingleNode("author").InnerText)
                DisplayInfo(oFileInfo.SelectSingleNode("copyright").InnerText)
                DisplayInfo(oFileInfo.SelectSingleNode("packname").InnerText)
                Console.WriteLine("----------------")
                DisplayAction("Searching templates")
                Dim oTemplate As XmlNodeList = xmlDoc.DocumentElement.SelectNodes("templates/template")
                Dim sTemplateFileName, sTemplateName, sWorkingDir As String
                If oTemplate.Count = 0 Then
                    DisplayInfo("Template tag not found")
                Else
                    For Each oSingleTemplateNode As XmlNode In oTemplate
                        'Read the template
                        sTemplateName = oSingleTemplateNode.SelectSingleNode("name").InnerText
                        sTemplateFileName = oSingleTemplateNode.SelectSingleNode("mainfile").InnerText
                        sWorkingDir = oSingleTemplateNode.SelectSingleNode("outputdir").InnerText
                        'Display the template file name
                        DisplayAction("Reading Template: " & sTemplateFileName)
                        sStoreFile = System.IO.File.ReadAllText(sTemplateFileName, Text.Encoding.UTF8)
                        'Display the output file name
                        DisplayAction("Output to: " & sWorkingDir & sTemplateName)
                        'For each tag, open a file and replace it
                        For Each oFileNode As XmlNode In oSingleTemplateNode.SelectNodes("merger/files/file")
                            DisplayInfo("Working on tag/file: " & oFileNode.SelectSingleNode("tag").InnerText & " - " & oFileNode.SelectSingleNode("link").InnerText)
                            'Read text from file
                            sStoreInclude = System.IO.File.ReadAllText(oFileNode.SelectSingleNode("link").InnerText, Text.Encoding.UTF8)
                            'Replace the tag
                            sStoreFile = sStoreFile.Replace(oFileNode.SelectSingleNode("tag").InnerText, sStoreInclude)
                        Next
                        Try
                            For Each oFileNode As XmlNode In oSingleTemplateNode.SelectNodes("merger/strings/string")
                                DisplayInfo("Working on tag/text: " & oFileNode.SelectSingleNode("tag").InnerText & " - " & oFileNode.SelectSingleNode("link").InnerText)
                                'Read string from file
                                sStoreInclude = oFileNode.SelectSingleNode("text").InnerText
                                'Replace the tag
                                sStoreFile = sStoreFile.Replace(oFileNode.SelectSingleNode("tag").InnerText, sStoreInclude)
                            Next
                        Catch ex As Exception
                            DisplayInfo("String tag exception")
                        End Try
                        'Writes the file into directory, check if it exists first.
                        If IO.Directory.Exists(sWorkingDir) = False Then
                            IO.Directory.CreateDirectory(sWorkingDir)
                        End If
                        IO.File.WriteAllText(sWorkingDir & sTemplateName, sStoreFile)
                        DisplayInfo("Merge complete: " & sWorkingDir & sTemplateName)
                    Next
                End If
                'Done
                DisplaySuccess("All done.")

            Catch ex As Exception
                ' Couldn't open or process file
                DisplayError(ex.Message)
            End Try

        Else
            'Oops, config file not set
            DisplayError("Config file is not set. Quitting...")

        End If

        'Ends the program - for .NET 4.0 edition
        If bDebug = True Then
            Console.ForegroundColor = ConsoleColor.DarkGray
            Console.WriteLine("-- DEBUG: Press any key to continue... --")
            Console.ReadKey()
        End If

    End Sub

    ''' <summary>
    ''' Display an Error
    ''' </summary>
    ''' <param name="sError">String</param>
    Sub DisplayError(sError As String)
        Console.Beep(1050, 200)
        Console.ForegroundColor = ConsoleColor.White
        Console.BackgroundColor = ConsoleColor.Red
        Console.Write("!ERROR:")
        Console.WriteLine(sError)
        Console.BackgroundColor = CurrentColor

        DefaultColor()
    End Sub
    ''' <summary>
    ''' Display a message
    ''' </summary>
    ''' <param name="sMessage">String</param>
    Sub DisplayInfo(sMessage As String)
        Console.ForegroundColor = ConsoleColor.Cyan
        Console.WriteLine("*INFO: " & sMessage)
        DefaultColor()
    End Sub
    ''' <summary>
    ''' Display a program action
    ''' </summary>
    ''' <param name="sMessage"></param>
    Sub DisplayAction(sMessage As String)
        Console.ForegroundColor = ConsoleColor.DarkYellow
        Console.WriteLine("+ " & sMessage)
        DefaultColor()
    End Sub
    ''' <summary>
    ''' Display a success message
    ''' </summary>
    ''' <param name="sMessage"></param>
    Sub DisplaySuccess(sMessage As String)
        Console.ForegroundColor = ConsoleColor.Green
        Console.WriteLine("#SUCCESS: " & sMessage)
        DefaultColor()
    End Sub
    ''' <summary>
    ''' Resets the console color
    ''' </summary>
    Sub DefaultColor()
        Console.BackgroundColor = ConsoleColor.Black
        Console.ForegroundColor = ConsoleColor.Gray
    End Sub

End Module