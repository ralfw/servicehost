module Main exposing (..)

import Html exposing (Html, button, div, text, program, input)
import Html.Events exposing (onClick)
import Http
import Json.Decode exposing (..)


-- MODEL


type alias Model =
    { a : String, b : String, sum : Int }


init : ( Model, Cmd Msg )
init =
    ( Model "0" "0" 0, Cmd.none )



-- MESSAGES

type FormField = A | B

type Msg
    = AddRequested
    | ValueChanged FormField String
    | ResultReceived (Result Http.Error Int)


-- VIEW


view : Model -> Html Msg
view model =
    div []
        [
          text "A", input [Html.Events.onInput (ValueChanged A)] [], Html.br [] [],
          text "B", input [Html.Events.onInput (ValueChanged B)] [], Html.br [] [],
          button [ onClick AddRequested ] [ text "Add" ]
        , text "Result: ", text (toString model.sum) 
        ]



-- UPDATE


update : Msg -> Model -> ( Model, Cmd Msg )
update msg model =
    let
        toInt s = Result.withDefault 0 (String.toInt s)
    in
        case msg of
            ValueChanged A newValue ->
                ( {model | a = newValue}, Cmd.none )
            ValueChanged B newValue ->
                ( {model | b = newValue}, Cmd.none )

            AddRequested ->
                ( {model | sum = 0 }, sendAddRequest model.a model.b )

            ResultReceived (Ok result) ->
                ( {model | sum = result}, Cmd.none )
            ResultReceived (Err error) ->
                ( {model | sum = -1 }, Cmd.none )


sendAddRequest a b =
    let
        url = "http://localhost:1234/add?A=" ++ a ++ "&B=" ++ b
        decodeSum = field "Sum" int
        request = Http.get url decodeSum
    in
        Http.send ResultReceived request



-- SUBSCRIPTIONS


subscriptions : Model -> Sub Msg
subscriptions model =
    Sub.none



-- MAIN


main : Program Never Model Msg
main =
    program
        { init = init
        , view = view
        , update = update
        , subscriptions = subscriptions
        }