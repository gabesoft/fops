
module FilterTests 

open System
open System.Diagnostics
open System.Text
open System.Text.RegularExpressions
open System.Globalization

open NaturalSpec
open NUnit.Framework

open GabeSoft.FOPS.Core
open GabeSoft.FOPS.Test

let paths = 
    FilterTestsData.patterns1
    |> List.map (fun (_, pl) -> pl |> List.map (fun (p, _) -> p))
    |> List.concat

let accepting (path, values) =
    let pattern = Wildcard.toRegex path
    printMethod (path, pattern)

    let actual = values |> List.map fst |> List.filter (Filter.allowed pattern)
    let expected = values |> List.filter snd |> List.map fst
    expected, actual

let matches (expected: string list, actual: string list) =
    printMethod (expected.Length, actual.Length)
    expected = actual

[<ScenarioTemplate(0)>]
[<ScenarioTemplate(1)>]
[<ScenarioTemplate(2)>]
[<ScenarioTemplate(3)>]
[<ScenarioTemplate(4)>]
[<ScenarioTemplate(5)>]
[<ScenarioTemplate(6)>]
[<ScenarioTemplate(7)>]
[<ScenarioTemplate(8)>]
[<ScenarioTemplate(9)>]
[<ScenarioTemplate(10)>]
[<ScenarioTemplate(11)>]
[<ScenarioTemplate(12)>]
let ``Applying a filter should accept only valid paths`` (index) =
    Given FilterTestsData.patterns1.[index]
    |> When accepting
    |> It should have matches
    |> Verify

let io_server paths = 
    let provider = new TestIOProvider(paths)
    new IOServer(provider)

let create_node_and_filter path excludes data =
    let server = io_server (data |> List.map fst)
    let node = Filter.apply { Pattern = (Wildcard.toRegex path) 
                              Exclude = (excludes |> List.map Wildcard.toRegex)
                              Recursive = Wildcard.isRecursive path }
                            (server.Node (Wildcard.root path))
    node

let filtering (path, excludes, data) =
    printMethod path
    let node = create_node_and_filter path excludes data
    let expected = data |> List.filter snd |> List.map fst
    let actual = node.AllFiles |> Seq.map (fun f -> f.Path) |> Seq.toList
    expected, actual

let applying_file_filter (path, data) =
    printMethod path
    Wildcard.isRecursive path, create_node_and_filter path [] data

let no_folders_if_recursive (isRecursive, node:IONode) =
    printMethod (isRecursive, node.Folders |> Seq.length)
    let noFolders = (0 = (node.Folders |> Seq.length))
    not isRecursive = noFolders

let allowed_matches (expected: string list, actual: string list) =
    printMethod (expected.Length, actual.Length)
    expected = actual

[<Scenario>]
let ``Applying a filter should keep only valid paths`` () =
    Given FilterTestsData.patterns2.[0]
    |> When filtering
    |> It should have allowed_matches
    |> Verify

[<ScenarioTemplate(0)>]
[<ScenarioTemplate(1)>]
[<ScenarioTemplate(2)>]
[<ScenarioTemplate(3)>]
[<ScenarioTemplate(4)>]
[<ScenarioTemplate(5)>]
[<ScenarioTemplate(6)>]
[<ScenarioTemplate(7)>]
[<ScenarioTemplate(8)>]
[<ScenarioTemplate(9)>]
[<ScenarioTemplate(10)>]
[<ScenarioTemplate(11)>]
[<ScenarioTemplate(12)>]
let ``Non recursive filters should leave no folders on parents`` (index) =
    Given FilterTestsData.patterns1.[index]
    |> When applying_file_filter
    |> It should have no_folders_if_recursive
    |> Verify