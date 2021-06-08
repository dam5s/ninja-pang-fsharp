open Game

[<EntryPoint>]
let main _ =
    use game = new GameLoop()
    game.Run()
    0
