Module ReBoot
    Sub ProcessText(ParamArray strings As String())
        Console.WriteLine("+--------------------------------")
        For i = 0 To strings.Length - 1
            Console.WriteLine($"| {strings(i)}")
        Next
    End Sub
    Sub ProcessMap(playerX As Integer, playerY As Integer)
        Console.WriteLine("+--------------------------------")
        Console.WriteLine("| +-----+")
        Console.WriteLine("| |.....|")
        Console.WriteLine("| |.....|")
        Console.WriteLine("| |.....|")
        Console.WriteLine("| |.....|")
        Console.WriteLine("| |.....|")
        Console.WriteLine("| +-----+")
        Console.CursorLeft += 2 + playerX
        Console.CursorTop -= 7 - playerY
        Console.Write("@")
        Console.CursorLeft -= 2 + playerX + 1
        Console.CursorTop += 7 - playerY
    End Sub
    Function ProcessStringInput(title As String) As String
        Console.WriteLine("+--------------------------------")
        Console.WriteLine($"| {title}:")
        Do
            Console.Write("| >>> ")
            Dim input As String = Console.ReadLine().Trim()
            If input = "" Then
                Continue Do
            End If
            Return input
        Loop
        Return Console.ReadLine()
    End Function
    Function ProcessDialog(title As String, ParamArray entries As String()) As Integer
        Console.WriteLine("+--------------------------------")
        Console.WriteLine($"| {title}:")
        For i = 0 To entries.Length - 1
            Console.WriteLine($"| {i + 1}) {entries(i)}")
        Next
        Do
            Console.Write("| >>> ")
            Dim choose As Integer
            Dim input As String = Console.ReadLine().Trim().ToLower()
            If input = "" Then
                Return 1
            End If
            If Not Integer.TryParse(input, choose) Then
                choose = Array.IndexOf(Array.ConvertAll(entries, Function(str) str.ToLower()), input) + 1
            End If
            If choose > entries.Length OrElse choose < 1 Then
                Console.WriteLine("| Unrecognized opinion")
                Continue Do
            End If
            Return choose
        Loop
    End Function
    Sub Main()
        ProcessText("Welcome to the world of Miretylis")
        Do
            Dim savedGame As XElement
            Try
                savedGame = XElement.Load("save.xml")
            Catch ex As Exception
                savedGame =
                    <save>
                        <character>
                            <name>Dominic Lexertux</name>
                            <position>
                                <x>3</x>
                                <y>3</y>
                            </position>
                        </character>
                    </save>
                savedGame.Save("save.xml")
            End Try
            Select Case ProcessDialog("Menu", "Create Character", $"Continue as [{savedGame.<character>.<name>.Value}]", "Intro", "Quit")
                Case 1
                    Dim character As XElement =
                        <save>
                            <character>
                                <name><%= ProcessStringInput("Name") %></name>
                                <position>
                                    <x>3</x>
                                    <y>3</y>
                                </position>
                            </character>
                        </save>
                    character.Save("save.xml")
                    ProcessText("Character is created")
                    Continue Do
                Case 2
                    Dim playerX As Integer = 3
                    Dim playerY As Integer = 3
                    Do
                        Select Case ProcessStringInput("Command [type help for help]").ToLower()
                            Case "help"
                                ProcessText("Command      | Result",
                                            "-------------+--------",
                                            "help         | Display this help",
                                            "map          | Display local map",
                                            "quit         | Quit to main menu",
                                            "[move] up    | Step up",
                                            "[move] down  | Step down",
                                            "[move] left  | Step left",
                                            "[move] right | Step right")
                                Continue Do
                            Case "map"
                                ProcessMap(playerX, playerY)
                                Continue Do
                            Case "move up", "up"
                                If playerY - 1 < 1 Then
                                    ProcessText("Blocked...")
                                Else
                                    playerY -= 1
                                    ProcessText("You step up")
                                End If
                            Case "move down", "down"
                                If playerY + 1 > 5 Then
                                    ProcessText("Blocked...")
                                Else
                                    playerY += 1
                                    ProcessText("You step down")
                                End If
                            Case "move left", "left"
                                If playerX - 1 < 1 Then
                                    ProcessText("Blocked...")
                                Else
                                    playerX -= 1
                                    ProcessText("You step left")
                                End If
                            Case "move right", "right"
                                If playerX + 1 > 5 Then
                                    ProcessText("Blocked...")
                                Else
                                    playerX += 1
                                    ProcessText("You step right")
                                End If
                            Case "quit"
                                ProcessText("Quiting to main menu...")
                                Exit Do
                            Case Else
                                ProcessText("Unknown command")
                                Continue Do
                        End Select
                        Console.WriteLine("| Time runs")
                    Loop
                Case 3
                    ProcessText("ReBoot, Text-based technofantasy roguelike.",
                                "Version 0.0.1 'The Cretion art'",
                                "-------------------------------",
                                "Interaction:",
                                "  Enter comand to execute it",
                                "  Enter dialogue option or its number to choose it",
                                "  Commands and dialogue opinions are case-insesitive",
                                "Story:",
                                "  At the beginning there were humans, demons, and angels.",
                                "  Demons and angels were in the holy war, and the battleground was a human world, the Gaia.",
                                "  Neither demons nor angels couldn't win in this war of madness.",
                                "  Much blood were letted, much lives were faded...",
                                "  But there were an angel and a demon which were frazzled up with this war.",
                                "  They came to the Gaia and with humans founded a brand new kingdom.",
                                "  The Great kingdom Lezetill, where noone would got any fear.",
                                "  But there were another plague, alongside with the holy war.",
                                "  Magical beasts full of power and madness, the 'Death beasts'...")
                Case 4
                    ProcessText("See you later")
                    Console.Write("+--------------------------------")
                    Console.ReadKey()
                    Exit Sub
            End Select
        Loop
    End Sub
End Module
