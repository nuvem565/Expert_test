﻿module TypeDefinitions
open System.Text

// TYPES DEFINITIONS
type Source = 
    | NotSet
    | SetByUser
    | SetByExternalProgram
    | SetByDatabaseQuery

type ProbabilityMode =
    | YesNo
    | ZeroTen
    | HundredAverage
    | HundredIndependent
    | HundredDependent
    | IncrDecr
    | Custom
    | Fuzzy

type Derivation =
    | AllRulesUsed
    | ChosenByUser
    | NonRedundant
    //| FirstRuleOnly


type BasicAttribute =
    | Subject of string option
    | Author of string option
    | StartingText of string option
    | EndingText of string option
    | ExternalProgram of (string * string list * float list) option
    | Threshold of float // threshold value for the choice to be displayed at the end of the run. If greater then display. Optional. Zero default
    | Probability of ProbabilityMode // modes of probability confidence values system
    | Confidence of float // optional. Only for prob. system 5 - custom formula. Set initial confidence value for choice which can be other than zero in this mode
    | DisplayRules of bool // whether the rules are displayed when the expert  system is run. Deafult - no (false)
    | Derivation of Derivation //  A(ll rules used - default) | F(irst rule only) | N(on-redundant rules)
    | FuzzyThreshold of float 
    | LoopOver of string option // If true, all values in qualifiers will be reset to 0 after one run of the program. Default false. Should be true in fuzzy control.
    | CSV of char []
    | OrCurrent of bool // specify if current (in this iteration) value of fuzzy qualifier has to be calculate with new by fuzzy sum operation (fuzzy or)

type BasicInfo = 
    {
        Subject: BasicAttribute
        Author: BasicAttribute
        StartingText: BasicAttribute
        EndingText: BasicAttribute
        ExternalProgram: BasicAttribute
        DisplayThreshold: BasicAttribute
        Probability: BasicAttribute
        InitChoiceConfidence: BasicAttribute
        DisplayRules: BasicAttribute
        Derivation: BasicAttribute
        FuzzyThreshold: BasicAttribute
        LoopOverVariable: BasicAttribute
        CSVDelimiter: BasicAttribute
        OrCurrentValue: BasicAttribute
    }


type QualifierBody = 
    | FromRule of string 
    | FromProgram of program:string * question:string

type QualifierValue =
    | Undefined
    | Possibilities of string list

type Qualifier = 
    { 
        Number: int32
        Question: QualifierBody //body
        mutable Value: QualifierValue
        Enumerations: (string * (((float * float) list) option)) list
        Name: string option
        Fuzzify: string option // variable to fuzzify
        Defuzzify: string option // variable to set by defuzzification
    }
    member __.unwrapQuestion = 
        match __.Question with
        | FromRule r -> r
        | FromProgram(question = q) -> q
    member __.unwrapProgram =
        match __.Question with
        | FromProgram(program = p) -> p
        | _ -> failwith "Expected command of running program (in order to set qualifier)"
    member __.unwrapEnums = fst(List.unzip __.Enumerations)
    member __.printfEnums = 
        for i = 1 to __.Enumerations.Length do
            printf "\t%i) %s\r\n" i (__.unwrapEnums.Item (i - 1))
    member __.sprintfEnums =
        let sb = new StringBuilder()
        sb.Append("\r\n") |> ignore
        for i = 1 to __.Enumerations.Length do
            sb.AppendFormat ("\t{0}) {1}\r\n", i, __.unwrapEnums.Item(i-1)) |> ignore
        sb.ToString()
    member __.unwrapName = 
        match __.Name with
        | Some name -> name
        | None -> failwith "Expected qualifier name (which is unnecessary), that should be used as variable so inside square brackets. For example:  SAVE(...,...,...,[QUALIFIER1])"

type Qualifiers = Qualifiers of ResizeArray<Qualifier>
