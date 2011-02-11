﻿module JobsParserTests

open System
open System.IO
open System.Diagnostics

open NaturalSpec
open NUnit.Framework

open GabeSoft.FOPS.Core

let paths = 
   [ "Files\jobs1.xml"; "Files\jobs2.xml"; "Files\jobs3.xml"; "Files\jobs4.xml" ] 
   |> Seq.map (fun p -> Path.GetFullPath(p))

let file2 = paths |> Seq.skip 1 |> Seq.head
let file3 = paths |> Seq.skip 2 |> Seq.head
let file4 = paths |> Seq.skip 3 |> Seq.head

let checking_existence paths =
   printMethod ""
   paths |> Seq.map (fun p -> File.Exists(p))

let all_found seq =
   printMethod (Seq.toList seq)
   Seq.forall (fun p -> p) seq

let parsing file =
   printMethod file
   JobsParser.parseFile file

let expected_ids (l: CopyJob list) =
   let ids = l |> List.map (fun j -> j.Id)
   
   printMethod ids
   ids = ["j1"; "j2"; "j3" ]

let expected_item_count (l: CopyJob list) =
   let counts = l |> List.map (fun j -> j.ItemsAbsolute.Length)

   printMethod counts
   counts = [3; 2; 2]

let expected_item_count_by_type (l: CopyJob list) =
   let intOfBool = function | true -> 1 | false -> 0
   let countsByType (job: CopyJob) =
      job.ItemsAbsolute
      |> List.map (fun e -> 
            e |> CopyItem.isCopy |> intOfBool, 
            e |> CopyItem.isLink |> intOfBool, 
            e |> CopyItem.isYank |> intOfBool)
      |> List.fold (fun (cacc,lacc,yacc) (c,l,y) -> cacc+c, lacc+l, yacc+y) (0,0,0)
   let counts = l |> List.map countsByType

   printMethod counts
   counts = [0, 3, 0; 1, 0, 1; 2, 0, 0]

let expected_exclude_items (l: CopyJob list) =
   let excludeCounts (job: CopyJob) =
      job.ItemsAbsolute
      |> List.map (CopyItem.map (fun _ _ _ e -> e.Length) (fun _ _ _ e -> e.Length) (fun _ -> 0))
      |> List.sum
   let counts = l |> List.map excludeCounts

   printMethod counts
   counts = [1; 2]

[<Scenario>]
let ``Can find the jobs files`` () =
   Given paths
   |> When checking_existence
   |> It should be all_found
   |> Verify

[<Scenario>]
let ``Can read all jobs in file`` () =
   Given file2
   |> When parsing
   |> It should have (length 3)
   |> Verify

[<Scenario>]
let ``Can populate the ids of all parsed jobs`` () =
   Given file2
   |> When parsing
   |> It should have expected_ids
   |> Verify

[<Scenario>]
let ``Can populate the copy items for each parsed job`` () =
   Given file2
   |> When parsing
   |> It should have expected_item_count
   |> Verify

[<Scenario>]
let ``Can populate the correct copy item type`` () =
   Given file2
   |> When parsing
   |> It should have expected_item_count_by_type
   |> Verify

[<Scenario>]
let ``Can populate the copy exclude items`` () =
   Given file4
   |> When parsing
   |> It should have expected_exclude_items
   |> Verify

[<Scenario>]
[<FailsWithType (typeof<ParseException>)>]
let ``Job id is a required attribute`` () =
   Given file3
   |> When parsing
   |> Verify

// TODO: add a test to verify that the items (to/from) get populated