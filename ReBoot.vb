Module ReBoot
    Enum CommandType
        Help
        Quit
        Save
        Look
        Walk
    End Enum
    Enum Direction
        Around
        Up
        Down
        Left
        Right
    End Enum
    Structure Command
        Dim Type As CommandType
        Dim Direction As Direction
        Dim Times As Integer
    End Structure
    Enum TerrarianType
        None
        Ground
        Wall
    End Enum
    Structure Entity
        Dim Glyph As Char
    End Structure
    Structure Cell
        Dim Terrarian As TerrarianType
        Dim Entity As Entity
    End Structure
    Sub ProcessText(ParamArray strings As String())
        Console.WriteLine("╠════════════════════════════════════════════════════════════════►")
        For i = 0 To strings.Length - 1
            Console.WriteLine($"║ {strings(i)}")
        Next
    End Sub
    Sub ProcessMap(cameraX As Integer, cameraY As Integer, map As Cell(,))
        Dim screen(8, 8) As Char
        Console.WriteLine("╠════════════════════════════════════════════════════════════════►")
        Console.WriteLine("║ ┌─────────┐")
        For y = cameraY To cameraY + 8
            For x = cameraX To cameraX + 8

            Next
        Next
        For y = cameraY To cameraY + 8
            Console.Write("║ │")
            For x = cameraX To cameraX + 8
                If x < 0 OrElse y < 0 OrElse x > map.GetUpperBound(1) OrElse y > map.GetUpperBound(0) Then
                    If (x + y) Mod 2 = 0 Then
                        Console.Write("┼")
                    Else
                        Console.Write("╬")
                    End If
                    Continue For
                End If
                If map(y, x).Entity.Glyph <> Chr(0) Then
                    Console.Write(map(y, x).Entity.Glyph)
                    Continue For
                End If
                Select Case map(y, x).Terrarian
                    Case TerrarianType.None
                        Console.Write(" ")
                    Case TerrarianType.Ground
                        Console.Write(".")
                    Case TerrarianType.Wall
                        Console.Write("█")
                    Case Else
                        Console.Write("?")
                End Select
            Next
            Console.WriteLine("│")
        Next
        Console.WriteLine("║ └─────────┘")
    End Sub
    Function ProcessStringInput(title As String) As String
        Console.WriteLine($"╠════════════════════════════════════════════════════════════════►")
        Console.WriteLine($"║ {title}:")
        Do
            Console.Write("╟─► ")
            Dim input As String = Console.ReadLine().Trim()
            If input = "" Then
                Continue Do
            End If
            Return input
        Loop
        Return Console.ReadLine()
    End Function
    Function ProcessDialog(title As String, ParamArray entries As String()) As Integer
        Console.WriteLine($"╠════════════════════════════════════════════════════════════════►")
        Console.WriteLine($"║ {title}:")
        For i = 0 To entries.Length - 1
            Console.WriteLine($"║ {i + 1}) {entries(i)}")
        Next
        Do
            Console.Write("╟─► ")
            Dim choose As Integer
            Dim input As String = Console.ReadLine().Trim().ToLower()
            If input = "" Then
                Return 1
            End If
            If Not Integer.TryParse(input, choose) Then
                choose = Array.IndexOf(Array.ConvertAll(entries, Function(str) str.ToLower()), input) + 1
            End If
            If choose > entries.Length OrElse choose < 1 Then
                Console.WriteLine("║ Unrecognized opinion")
                Continue Do
            End If
            Return choose
        Loop
    End Function
    Function ParseCommand(toParse As String) As Command
        Dim command As Command
        command.Times = 1
        If toParse.Trim() = "" Then
            Return command
        End If
        Dim words() As String = toParse.Trim().Split()
        Select Case words(0).ToLower()
            Case "quit"
                command.Type = CommandType.Quit
            Case "save"
                command.Type = CommandType.Save
            Case "look"
                command.Type = CommandType.Look
            Case "walk"
                command.Type = CommandType.Walk
            Case Else
                command.Type = CommandType.Help
        End Select
        If words.Length < 2 Then
            Return command
        End If
        Select Case words(1).ToLower()
            Case "around"
                command.Direction = Direction.Around
            Case "up"
                command.Direction = Direction.Up
            Case "down"
                command.Direction = Direction.Down
            Case "left"
                command.Direction = Direction.Left
            Case "right"
                command.Direction = Direction.Right
            Case Else
                command.Type = CommandType.Help
        End Select
        If words.Length < 3 Then
            Return command
        End If
        If Not Integer.TryParse(words(2), command.Times) OrElse command.Times < 1 Then
            command.Times = 1
        End If
        Return command
    End Function
    Sub Main()
        Console.WriteLine("╔════════════════════════════════════════════════════════════════►")
        Console.WriteLine("║ Welcome to the world of Miretylis")
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
                    Dim saveGame As XElement =
                        <save>
                            <character>
                                <name><%= ProcessStringInput("Name") %></name>
                                <position>
                                    <x>3</x>
                                    <y>3</y>
                                </position>
                            </character>
                        </save>
                    saveGame.Save("save.xml")
                    ProcessText("Character is created")
                    Continue Do
                Case 2
                    Dim playerX As Integer = Integer.Parse(savedGame.<character>.<position>.<x>.Value)
                    Dim playerY As Integer = Integer.Parse(savedGame.<character>.<position>.<y>.Value)
                    Dim map(10, 10) As Cell
                    map(1, 1).Terrarian = TerrarianType.Wall
                    map(playerY, playerX).Entity.Glyph = "@"c
                    Do
                        Dim command As Command = ParseCommand(ProcessStringInput("Command"))
                        Select Case command.Type
                            Case CommandType.Help
                                ProcessText("┌─────────┬────────────────────────────────────────────┐",
                                            "│ Command │ Action                                     │",
                                            "├─────────┼────────────────────────────────────────────┤",
                                            "│ help    │ Display the help                           │",
                                            "│ quit    │ Quit to the main menu (save will be lost)  │",
                                            "│ save    │ Save the game                              │",
                                            "│ look    │ Display the local map                      │",
                                            "│ walk    │ Step towards direction or pass if in place │",
                                            "└─────────┴────────────────────────────────────────────┘",
                                            "Directions are [around](default), [up], [down], [left], and [right]")
                            Case CommandType.Quit
                                ProcessText("Quiting to main menu...")
                                Exit Do
                            Case CommandType.Save
                                ProcessText("Saving the game...")
                                savedGame.Save("save.xml")
                            Case CommandType.Look
                                Select Case command.Direction
                                    Case Direction.Around
                                        ProcessMap(playerX - 4, playerY - 4, map)
                                    Case Direction.Up
                                        ProcessMap(playerX - 4, playerY - 8, map)
                                    Case Direction.Down
                                        ProcessMap(playerX - 4, playerY, map)
                                    Case Direction.Left
                                        ProcessMap(playerX - 8, playerY - 4, map)
                                    Case Direction.Right
                                        ProcessMap(playerX, playerY - 4, map)
                                End Select
                            Case CommandType.Walk
                                Select Case command.Direction
                                    Case Direction.Around
                                        For i = 1 To command.Times
                                            ProcessText("Do nothing")
                                        Next
                                    Case Direction.Up
                                        For i = 1 To command.Times
                                            If playerY < 1 OrElse map(playerY - 1, playerX).Terrarian = TerrarianType.Wall Then
                                                ProcessText("Blocked...")
                                                Exit For
                                            Else
                                                map(playerY, playerX).Entity.Glyph = Chr(0)
                                                playerY -= 1
                                                map(playerY, playerX).Entity.Glyph = "@"c
                                                savedGame.<character>.<position>.<y>.Value = playerY.ToString()
                                                ProcessText("You Stept up")
                                            End If
                                        Next
                                    Case Direction.Down
                                        For i = 1 To command.Times
                                            If playerY > map.GetUpperBound(0) - 1 OrElse map(playerY + 1, playerX).Terrarian = TerrarianType.Wall Then
                                                ProcessText("Blocked...")
                                                Exit For
                                            Else
                                                map(playerY, playerX).Entity.Glyph = Chr(0)
                                                playerY += 1
                                                map(playerY, playerX).Entity.Glyph = "@"c
                                                savedGame.<character>.<position>.<y>.Value = playerY.ToString()
                                                ProcessText("You Step down")
                                            End If
                                        Next
                                    Case Direction.Left
                                        For i = 1 To command.Times
                                            If playerX < 1 OrElse map(playerY, playerX - 1).Terrarian = TerrarianType.Wall Then
                                                ProcessText("Blocked...")
                                                Exit For
                                            Else
                                                map(playerY, playerX).Entity.Glyph = Chr(0)
                                                playerX -= 1
                                                map(playerY, playerX).Entity.Glyph = "@"c
                                                savedGame.<character>.<position>.<x>.Value = playerX.ToString()
                                                ProcessText("You Step left")
                                            End If
                                        Next
                                    Case Direction.Right
                                        For i = 1 To command.Times
                                            If playerX > map.GetUpperBound(1) - 1 OrElse map(playerY, playerX + 1).Terrarian = TerrarianType.Wall Then
                                                ProcessText("Blocked...")
                                                Exit For
                                            Else
                                                map(playerY, playerX).Entity.Glyph = Chr(0)
                                                playerX += 1
                                                map(playerY, playerX).Entity.Glyph = "@"c
                                                savedGame.<character>.<position>.<x>.Value = playerX.ToString()
                                                ProcessText("You Step right")
                                            End If
                                        Next
                                End Select
                        End Select
                    Loop
                Case 3
                    ProcessText("ReBoot, Text-based technofantasy roguelike.",
                                "Version 0.1.0 'The Beauty of words'", ' next 'The Tricky Dungeon'
                                "─═─═─═─═─═─═─═─═─═─═─═─═─═─═─═─",
                                "Interaction:",
                                "• Command is an instruction in the format :`Base` `Direction` `Times`:",
                                "• Enter dialogue option or its number to choose it",
                                "• Commands and dialogue opinions are case-insesitive",
                                "Story:",
                                "• At the beginning there were humans, demons, and angels.",
                                "• Demons and angels were in the holy war, and the battleground was a human world, the Gaia.",
                                "• Neither demons nor angels couldn't win in this war of madness.",
                                "• Much blood were letted, much lives were faded...",
                                "• But there were an angel and a demon which were frazzled up with this war.",
                                "• They came to the Gaia and with humans founded a brand new kingdom.",
                                "• The Great kingdom Lezetill, where noone would got any fear.",
                                "• But there were another plague, alongside with the holy war.",
                                "• Magical beasts, all full of power and madness, the 'Death beasts'...")
                Case 4
                    ProcessText("See you later")
                    Console.Write("╚════════════════════════════════════════════════════════════════►")
                    Console.ReadKey()
                    Exit Sub
            End Select
        Loop
    End Sub
End Module
