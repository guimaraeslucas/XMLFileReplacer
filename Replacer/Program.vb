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

            'Try to open and read file
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
                        'Le os itens do template inicial
                        sTemplateName = oSingleTemplateNode.SelectSingleNode("name").InnerText
                        sTemplateFileName = oSingleTemplateNode.SelectSingleNode("mainfile").InnerText
                        sWorkingDir = oSingleTemplateNode.SelectSingleNode("outputdir").InnerText
                        'Informa que vai ler
                        DisplayAction("Reading Template: " & sTemplateFileName)
                        sStoreFile = System.IO.File.ReadAllText(sTemplateFileName, Text.Encoding.UTF8)
                        'Onde iremos salvar
                        DisplayAction("Output to: " & sWorkingDir & sTemplateName)
                        'Para cada tag file, abra o arquivo, leia e substitua no template
                        For Each oFileNode As XmlNode In oSingleTemplateNode.SelectNodes("merger/files/file")
                            DisplayInfo("Working on tag/file: " & oFileNode.SelectSingleNode("tag").InnerText & " - " & oFileNode.SelectSingleNode("link").InnerText)
                            'Arquivo a abrir
                            sStoreInclude = System.IO.File.ReadAllText(oFileNode.SelectSingleNode("link").InnerText, Text.Encoding.UTF8)
                            'Substituir tag
                            sStoreFile = sStoreFile.Replace(oFileNode.SelectSingleNode("tag").InnerText, sStoreInclude)
                        Next
                        'Escreve o arquivo final definido no node, se o diretório não existir cria antes de escrever
                        If IO.Directory.Exists(sWorkingDir) = False Then
                            IO.Directory.CreateDirectory(sWorkingDir)
                        End If
                        IO.File.WriteAllText(sWorkingDir & sTemplateName, sStoreFile)
                        DisplayInfo("Merge complete: " & sWorkingDir & sTemplateName)
                    Next
                End If
                'Presumimos que tudo foi OK 
                DisplaySuccess("All done.")

            Catch ex As Exception
                ' Could Not open Or process file
                DisplayError(ex.Message)
            End Try

        Else
            'Oops, config file not set
            DisplayError("Config file is not set. Quitting app...")

        End If

        'Encerra o programa
        If bDebug = True Then
            Console.ForegroundColor = ConsoleColor.DarkGray
            Console.WriteLine("-- DEBUG: Press any key to continue... --")
            Console.ReadKey()
        End If

    End Sub

    Sub DisplayError(sError As String)
        Console.Beep(1050, 200)
        Console.ForegroundColor = ConsoleColor.White
        Console.BackgroundColor = ConsoleColor.Red
        Console.Write("ERROR:")
        Console.WriteLine(sError)
        Console.BackgroundColor = CurrentColor

        DefaultColor()
    End Sub

    Sub DisplayInfo(sMessage As String)
        Console.ForegroundColor = ConsoleColor.Cyan
        Console.WriteLine("INFO: " & sMessage)
        DefaultColor()
    End Sub

    Sub DisplayAction(sMessage As String)
        Console.ForegroundColor = ConsoleColor.DarkYellow
        Console.WriteLine("+ " & sMessage)
        DefaultColor()
    End Sub

    Sub DisplaySuccess(sMessage As String)
        Console.ForegroundColor = ConsoleColor.Green
        Console.WriteLine("SUCCESS: " & sMessage)
        DefaultColor()
    End Sub

    Sub DefaultColor()
        Console.BackgroundColor = ConsoleColor.Black
        Console.ForegroundColor = ConsoleColor.Gray
    End Sub

End Module
'<?xml version="1.0" encoding="utf-8"?>
'<!--INTELIMERGER CONFIGURATION FILE-->
'<intellimerger>
'<fileinfo>
'<desc> Junta todos os arquivos Do PED como um só</desc>
'<author> LucasGuimarães</author>
'<copyright>(C)2017 - LucasGuimaraes.com</copyright>
'<packname> http : //lgvirtual.com/lgped</packname>
'</fileinfo>
'<templates>
'<template>
'<name> index.htm</name>
'<mainfile> C : \Users\lucas\Desktop\Receitas\ped\index.htm</mainfile>
'<outputdir> C : \Users\lucas\Desktop\Receitas\ped\rendered\</outputdir>
'<merger>
'<files>
'<file>
'<tag><![CDATA[/*miligram*/]]></tag>
'<link> C : \Users\lucas\Desktop\Receitas\ped\includes\miligram.txt</link>
'</file>
'</files>
'</merger>
'</template>
'</templates>
'</intellimerger>
