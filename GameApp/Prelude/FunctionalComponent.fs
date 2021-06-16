[<AutoOpen>]
module FunctionalComponent

open Microsoft.Xna.Framework
open Microsoft.Xna.Framework.Graphics

type FunctionalEvent<'event> =
    | Event of 'event
    | OnUpdate of GameTime

type Init<'state> = unit -> 'state
type Update<'state, 'event, 'anim> = FunctionalEvent<'event> -> 'state -> 'state * Effect<'anim>
type Draw<'state, 'anim when 'anim : comparison> = 'state -> AnimationSet<'anim> -> SpriteBatch -> unit

module UpdateComposition =
    let combineEffects e1 e2 =
        match e1, e2 with
        | NoEffect, a -> a
        | a, NoEffect -> a
        | a, b -> Multiple (a, b)

    let compose
        (f: 'state -> GameTime -> 'state * Effect<'a>)
        (g: 'state -> GameTime -> 'state * Effect<'a>) (s: 'state) (t: GameTime) =
        let state1, effect1 = f s t
        let state2, effect2 = g state1 t
        state2, combineEffects effect1 effect2

    module Operators =
        let (>->) = compose

type FunctionalComponent<'s, 'e, 'a when 'a : comparison>(init: Init<'s>,
                                                          update: Update<'s, 'e, 'a>,
                                                          draw: Draw<'s, 'a>,
                                                          anims: unit -> AnimationSet<'a>) =

    let animations = anims ()
    let mutable state = init ()

    member this.Dispatch event =
        let newState, effect = update (Event event) state
        state <- newState
        Effect.execute animations effect

    member this.Update (t: GameTime) =
        let newState, effect = update (OnUpdate t) state
        state <- newState
        Effect.execute animations effect
        animations.Update t

    member this.Draw (sb: SpriteBatch) =
        draw state animations sb
