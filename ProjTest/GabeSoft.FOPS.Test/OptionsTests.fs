module OptionsTests

open System
open System.IO
open System.Diagnostics

open NaturalSpec
open NUnit.Framework

open GabeSoft.FOPS.Core

let creatingOptions args = 
   printMethod args
   new Options (args)

let expected f expected (opts:Options) = 
  let actual = f opts
  printMethod (expected, actual)
  expected = actual

[<Scenario>]
let File () =
  Given ["-f"; "jobs.xml"; "-b=src"; "-B:dst"; "-j"; "12" ]
  |> When creatingOptions
  |> It should have (fun o -> o.File = "jobs.xml")
  |> It should have (fun o -> o.BaseSrc = "src")
  |> It should have (fun o -> o.BaseDst = "dst")
  |> It should have (fun o -> o.JobId = "12")
  |> Verify

[<Scenario>]
let Delete () =
  Given ["-d"; "/a/b/c"]
  |> When creatingOptions
  |> It should have (fun o -> o.Yank)
  |> It should have (fun o -> o.Src = "/a/b/c")
  |> Verify

[<Scenario>]
let ``Delete dir`` () =
  Given ["-D"; "/a/b/c"]
  |> When creatingOptions
  |> It should have (fun o -> o.YankDir)
  |> It should have (fun o -> o.Src = "/a/b/c")
  |> Verify

[<Scenario>]
let Copy () =
  Given ["-c"; @"C:\a\src"; @"C:\b\dst"; "-F"]
  |> When creatingOptions
  |> It should have (fun o -> o.Copy)
  |> It should have (expected (fun o -> o.Src) @"C:\a\src")
  |> It should have (expected (fun o -> o.Dst) @"C:\b\dst")
  |> It should have (fun o -> o.Force)
  |> Verify

[<Scenario>]
let Link () =
  Given ["-l"; @"C:\a\src"; @"C:\b\dst"; "-F"]
  |> When creatingOptions
  |> It should have (fun o -> o.Link)
  |> It should have (expected (fun o -> o.Src) @"C:\a\src")
  |> It should have (expected (fun o -> o.Dst) @"C:\b\dst")
  |> It should have (fun o -> o.Force)
  |> Verify