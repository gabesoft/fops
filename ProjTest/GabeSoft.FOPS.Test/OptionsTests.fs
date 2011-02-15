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

let equal message a b =
   sprintf "%s %s" message (prepareOutput (a, b)) |> toSpec
   a = b

//let file arg (o:Options) = equal "file" o.File arg
//let basePath arg (o:Options) = equal "basePath" o.BaseSrc arg
//let jobId arg (o:Options) = equal "jobId" o.JobId arg
//let source arg (o:Options) = equal "source" o.From arg
//let destination arg (o:Options) = equal "destination" o.To arg
//let overwrite (o:Options) = printMethod ""; o.Overwrite 
//let hlinks (o:Options) = printMethod ""; o.Hlinks
//let invalid (o:Options) = printMethod ""; not o.Valid
//
//[<Scenario>]
//let ``Given a jobs file and a base path results in valid options`` () =
//   let args = [ "-x"; "jobs.xml"; "-b"; @"C:\Temp"; "-j"; "123" ] 
//   Given args
//   |> When creatingOptions
//   |> It should have (file args.[1])
//   |> It should have (basePath args.[3])
//   |> It should have (jobId args.[5])
//   |> Verify
//
//[<Scenario>]
//let ``Given a from and to path results in valid options`` () =
//   let args = [ "-f"; @"C:\Source\*.txt"; "-t"; @"C:\Destination"; "-o"; "-h" ]
//   Given args
//   |> When creatingOptions
//   |> It should have (source args.[1])
//   |> It should have (destination args.[3])
//   |> It should have overwrite
//   |> It should have hlinks
//   |> Verify
//
//[<Scenario>]
//let ``Given a jobs file and a source or destination results in invalid options`` () =
//   let args = ["--file=jobs.xml"; "--to=Temp" ]
//   Given args
//   |> When creatingOptions
//   |> It should be invalid
//   |> Verify
//
//[<Scenario>]
//let ``Not given a source file requires both a source and a destination`` () =
//   let args = ["--from=Destination"; "-o"; "-h"]
//   Given args
//   |> When creatingOptions
//   |> It should be invalid
//   |> Verify