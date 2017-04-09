module FablePlaypen

open Fable.Core
open Fable.Import
open Elmish

// MODEL
type State = {
    count: int
    asyncInProgress: bool
    lastKey: string option
}

type Msg =
  | AsyncIncrement
  | Increment
  | Decrement
  | SetLastKey of string

let init () =
    ({ count = 0; asyncInProgress = false; lastKey = None }, Cmd.none)

// UPDATE
let delayedIncrement count =
    async {
        do! Async.Sleep (count * 1000)
        return Increment
    }

let update msg state =
    let { count = currentCount } = state
    match msg with
    | AsyncIncrement ->
        ({ state with asyncInProgress = true }, Cmd.ofAsync delayedIncrement currentCount id (fun _ -> Decrement))
    | Increment ->
        ({ state with count = currentCount + 1; asyncInProgress = false }, Cmd.none)
    | Decrement ->
        ({ state with count = currentCount - 1; asyncInProgress = false }, Cmd.none)
    | SetLastKey key ->
        ({ state with lastKey = Some key }, Cmd.none)


open Fable.Import.Browser

// Subscriptions
let keyboardSubscription dispatch =
    document.addEventListener_keyup(fun e -> dispatch <| (SetLastKey e.key); null)

let subscriptions state =
    Cmd.ofSub keyboardSubscription

// rendering views with React
open Fable.Core.JsInterop
open Fable.Helpers.React.Props
module R = Fable.Helpers.React

let progressStyles : ICSSProp list =
    [
        Position "fixed"
        Top 0
        Left 0
        Right 0
        TextAlign "center"
    ]

let view state dispatch =
    let onClick msg =
        OnClick <| fun _ -> msg |> dispatch

    R.div []
        [ R.button [ onClick Decrement; Disabled state.asyncInProgress ] [ R.str "-" ]
          R.div [] [ R.str (string state.count) ]
          R.button [ onClick AsyncIncrement; Disabled state.asyncInProgress ] [ R.str "+" ]
          R.div [ Style progressStyles ] [ (R.str <| if state.asyncInProgress then "Please wait ..." else "") ]
          R.div [] [ R.str (sprintf "Last Key: %s" (if state.lastKey.IsNone then "<none>" else state.lastKey.Value)) ]
        ]

open Elmish.React
open Elmish.Debug

// App
Program.mkProgram init update view
|> Program.withSubscription subscriptions
|> Program.withConsoleTrace
|> Program.withReact "elmish-app"
|> Program.withDebugger
|> Program.run
