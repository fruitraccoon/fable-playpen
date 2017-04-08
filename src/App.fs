module FablePlaypen

open Fable.Core
open Fable.Import
open Elmish

// MODEL
type State = {
    count: int
}

type Msg =
  | Increment
  | Decrement

let init () = ({ count = 0 }, Cmd.none)

// UPDATE
let update msg state =
  let { count = currentCount } = state
  match msg with
  | Increment -> ({ state with count = currentCount + 1 }, Cmd.none)
  | Decrement -> ({ state with count = currentCount - 1 }, Cmd.none)

// Subscriptions
let subscriptions state =
    Cmd.none

// rendering views with React
open Fable.Core.JsInterop
open Fable.Helpers.React.Props
module R = Fable.Helpers.React

let view state dispatch =
  let onClick msg =
    OnClick <| fun _ -> msg |> dispatch

  R.div []
    [ R.button [ onClick Decrement ] [ R.str "-" ]
      R.div [] [ R.str (string state.count) ]
      R.button [ onClick Increment ] [ R.str "+" ] ]

open Elmish.React
open Elmish.Debug

// App
Program.mkProgram init update view
|> Program.withSubscription subscriptions
|> Program.withConsoleTrace
|> Program.withReact "elmish-app"
|> Program.withDebugger
|> Program.run
