Module ReBoot
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
        Console.WriteLine("+--------------------------------")
        For i = 0 To strings.Length - 1
            Console.WriteLine($"| {strings(i)}")
        Next
    End Sub
    Sub ProcessMap(cameraX As Integer, cameraY As Integer, map As Cell(,))
        Dim screen(8, 8) As Char
        Console.WriteLine("+--------------------------------")
        Console.WriteLine("| +---------+")
        For y = cameraY To cameraY + 8
            For x = cameraX To cameraX + 8

            Next
        Next
            For y = cameraY To cameraY + 8
            Console.Write("| |")
            For x = cameraX To cameraX + 8
                If x < 0 OrElse y < 0 OrElse x > map.GetUpperBound(1) OrElse y > map.GetUpperBound(0) Then
                    Console.Write("X")
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
                        Console.Write("#")
                    Case Else
                        Console.Write("?")
                End Select
            Next
            Console.WriteLine("|")
        Next
        Console.WriteLine("| +---------+")
    End Sub
    Function ProcessStringInput(title As String) As String
        Console.WriteLine($"+--------------------------------")
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
        Console.WriteLine($"+--------------------------------")
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
                    Dim map(3, 3) As Cell
                    map(1, 1).Terrarian = TerrarianType.Wall
                    map(playerY, playerX).Entity.Glyph = "@"c
                    Do
                        Select Case ProcessStringInput("Command [type help for help]").ToLower()
                            Case "help"
                                ProcessText("Command      | Result",
                                            "-------------+--------",
                                            "help         | Display this help",
                                            "quit         | Quit to main menu (game will be lost)",
                                            "save         | Save game",
                                            "look         | Display local map",
                                            "look up      | Display local map towards up",
                                            "look down    | Display local map towards down",
                                            "look left    | Display local map towards left",
                                            "look right   | Display local map towards right",
                                            "[move] up    | Step up",
                                            "[move] down  | Step down",
                                            "[move] left  | Step left",
                                            "[move] right | Step right")
                                Continue Do
                            Case "look"
                                ProcessMap(playerX - 4, playerY - 4, map)
                                Continue Do
                            Case "look up"
                                ProcessMap(playerX - 4, playerY - 8, map)
                                Continue Do
                            Case "look down"
                                ProcessMap(playerX - 4, playerY, map)
                                Continue Do
                            Case "look left"
                                ProcessMap(playerX - 8, playerY - 4, map)
                                Continue Do
                            Case "look right"
                                ProcessMap(playerX, playerY - 4, map)
                                Continue Do
                            Case "move up", "up"
                                If playerY < 1 OrElse map(playerY - 1, playerX).Terrarian = TerrarianType.Wall Then
                                    ProcessText("Blocked...")
                                    Continue Do
                                Else
                                    map(playerY, playerX).Entity.Glyph = Chr(0)
                                    playerY -= 1
                                    map(playerY, playerX).Entity.Glyph = "@"c
                                    savedGame.<character>.<position>.<y>.Value = playerY.ToString()
                                    ProcessText("You step up")
                                End If
                            Case "move down", "down"
                                If playerY > map.GetUpperBound(0) - 1 OrElse map(playerY + 1, playerX).Terrarian = TerrarianType.Wall Then
                                    ProcessText("Blocked...")
                                    Continue Do
                                Else
                                    map(playerY, playerX).Entity.Glyph = Chr(0)
                                    playerY += 1
                                    map(playerY, playerX).Entity.Glyph = "@"c
                                    savedGame.<character>.<position>.<y>.Value = playerY.ToString()
                                    ProcessText("You step down")
                                End If
                            Case "move left", "left"
                                If playerX < 1 OrElse map(playerY, playerX - 1).Terrarian = TerrarianType.Wall Then
                                    ProcessText("Blocked...")
                                    Continue Do
                                Else
                                    map(playerY, playerX).Entity.Glyph = Chr(0)
                                    playerX -= 1
                                    map(playerY, playerX).Entity.Glyph = "@"c
                                    savedGame.<character>.<position>.<x>.Value = playerX.ToString()
                                    ProcessText("You step left")
                                End If
                            Case "move right", "right"
                                If playerX > map.GetUpperBound(1) - 1 OrElse map(playerY, playerX + 1).Terrarian = TerrarianType.Wall Then
                                    ProcessText("Blocked...")
                                    Continue Do
                                Else
                                    map(playerY, playerX).Entity.Glyph = Chr(0)
                                    playerX += 1
                                    map(playerY, playerX).Entity.Glyph = "@"c
                                    savedGame.<character>.<position>.<x>.Value = playerX.ToString()
                                    ProcessText("You step right")
                                End If
                            Case "save"
                                ProcessText("Saving the game...")
                                savedGame.Save("save.xml")
                                Continue Do
                            Case "quit"
                                ProcessText("Quiting to main menu...")
                                Exit Do
                            Case "d"
                                ProcessText("Debug", $"X: {playerX}", $"Y: {playerY}")
                            Case Else
                                ProcessText("Unknown command")
                                Continue Do
                        End Select
                        Console.WriteLine("| Time runs")
                    Loop
                Case 3
                    ProcessText("ReBoot, Text-based technofantasy roguelike.",
                                "Version 0.0.3 'The Cretion art'", ' next 'The Second day'
                                "-------------------------------",
                                "Interaction:",
                                "  Enter comand to execute it",
                                "  Enter dialogue option or its number to choose it",
                                "  Commands and dialogue opinions are case-insesitive",
                                "  Type 'help' to get help",
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
