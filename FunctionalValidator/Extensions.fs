namespace FunctionalValidator

open System
open System.Runtime.CompilerServices

type CSharp =
    static member ToFSharp(act: Func<'T, 'R>) = act.Invoke
    static member ToFSharp(act: Action<'T>) = act.Invoke

[<Extension>]
module ExtensionMethods =
    [<Extension>]
    let Match (result, onOk: Func<'TOk, 'R>, onError: Func<'TError, 'R>) =
        match result with
        | Ok yay -> (CSharp.ToFSharp onOk) yay
        | Error boo -> (CSharp.ToFSharp onError) boo

    [<Extension>]
        let Do (result, onOk: Action<'TOk>, onError: Action<'TError>) =
            match result with
            | Ok yay -> (CSharp.ToFSharp onOk) yay
            | Error boo -> (CSharp.ToFSharp onError) boo
