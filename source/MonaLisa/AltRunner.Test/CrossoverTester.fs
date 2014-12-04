module AltRunner.Test


open AltRunner
open Xunit
open FsUnit.Xunit

[<Fact>]
let ``Adder add 5 and 3 should return 8.`` () = 
    5 + 3 |> should equal 8