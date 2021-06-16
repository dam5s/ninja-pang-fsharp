[<AutoOpen>]
module Effect

open Microsoft.Xna.Framework.Audio

type Effect<'animation> =
    | NoEffect
    | SetAnimation of 'animation
    | PlaySound of SoundEffect
    | Both of 'animation * SoundEffect

[<RequireQualifiedAccess>]
module Effect =

    let play (sound: SoundEffect) =
        sound.Play() |> ignore

    let execute<'anim when 'anim : comparison> (a: AnimationSet<'anim>) (e: Effect<'anim>) =
        match e with
        | NoEffect -> ()
        | SetAnimation anim -> a.Play anim
        | PlaySound s -> play s
        | Both (anim, s) -> a.Play anim; play s
