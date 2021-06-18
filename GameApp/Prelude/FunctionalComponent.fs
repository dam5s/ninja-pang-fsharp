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
    let private combineEffects e1 e2 =
        match e1, e2 with
        | NoEffect, a -> a
        | a, NoEffect -> a
        | a, b -> Multiple (a, b)

    let compose
        (f: GameTime -> 'state -> 'state * Effect<'a>)
        (g: GameTime -> 'state -> 'state * Effect<'a>)
        (t: GameTime) (s: 'state) =
        let state1, effect1 = f t s
        let state2, effect2 = g t state1
        state2, combineEffects effect1 effect2

    module Operators =
        let (>->) = compose

type KillableComponent() =
    let mutable alive = true

    member this.Kill () =
        alive <- false

    member this.Alive () =
        alive

type FunctionalComponent<'s, 'e, 'a when 'a : comparison>(init: Init<'s>,
                                                          update: Update<'s, 'e, 'a>,
                                                          draw: Draw<'s, 'a>,
                                                          anims: unit -> AnimationSet<'a>) =
    inherit KillableComponent()

    let animations = anims ()
    let mutable state = init ()

    member this.GetState () =
        state

    member this.Dispatch event =
        if this.Alive()
        then
            let newState, effect = update (Event event) state
            state <- newState
            Effect.execute animations effect

    member this.Update (t: GameTime) =
        if this.Alive()
        then
            let newState, effect = update (OnUpdate t) state
            state <- newState
            Effect.execute animations effect
            animations.Update t

    member this.Draw (sb: SpriteBatch) =
        if this.Alive()
        then
            draw state animations sb
