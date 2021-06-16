[<AutoOpen>]
module Effect

open Microsoft.Xna.Framework.Audio

type Effect<'anim> =
    | NoEffect
    | SetAnimation of 'anim
    | PlaySound of SoundEffect
    | Multiple of Effect<'anim> * Effect<'anim>

[<RequireQualifiedAccess>]
module Effect =

    let play (sound: SoundEffect) =
        sound.Play() |> ignore

    let rec execute<'anim when 'anim : comparison> (a: AnimationSet<'anim>) (e: Effect<'anim>) =
        match e with
        | NoEffect -> ()
        | SetAnimation anim -> a.Play anim
        | PlaySound s -> play s
        | Multiple (e1, e2) -> execute a e1; execute a e2
