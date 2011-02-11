module CopyEngineTests

open System
open System.IO
open System.Diagnostics

open NaturalSpec
open NUnit.Framework

open GabeSoft.FOPS.Core

let paths = [
   @"C:\Temp\*\Dir1\*.txt"
   @"C:\Temp\Source\Dir1\doc2.txt"
   @"C:\*.*"
   @"C:\Temp\f*.*x" ]

let allowed = [
   [ @"C:\Temp\Source1\Source2\Dir1\doc1.txt"
     @"C:\Temp\Source1\Dir1\doc2.txt"
     @"C:\Temp\Source1\Source2\Dir1\file.txt" ] 
   [ @"C:\Temp\Source\Dir1\doc2.txt" ] 
   [ @"C:\file1.pdf"; @"C:\file2.txt"; @"C:\Dir1\file1.pdf"; @"C:\Dir1\Dir2\file1.pdf" ] 
   [ @"C:\Temp\file1.ax"; @"C:\Temp\file2.x"] ]

let files = [
   fun () -> [ @"C:\Temp\Source1\Source2\Dir1\doc1.txt"
               @"C:\Temp\Source1\Source2\Dir2\doc1.txt"
               @"C:\Temp\Source1\Source2\Dir1\doc1.pdf"
               @"C:\Temp\Source1\Dir1\doc2.txt"
               @"C:\Temp\Source1\Dir2\doc2.txt"
               @"C:\Temp\Dir1\doc3.txt"
               @"C:\Temp\Source\doc3.txt"
               @"C:\Temp\Source1\Source2\Dir1\file.txt" ] 
   fun () -> [ @"C:\Temp\Source\Dir1\doc2.txt" ] 
   fun () -> [ @"C:\file1.pdf"; @"C:\file2.txt"; @"C:\Dir1\file1.pdf"; @"C:\Dir1\Dir2\file1.pdf" ] 
   fun () -> [ @"C:\Temp\file1.ax"
               @"C:\Temp\file2.x"
               @"C:\Temp2\file3.x"
               @"C:\Temp\file1.axd"
               @"C:\Temp\Dir\file1.ax"
               @"C:\Temp2\file1.axd" ] ]

let same expected actual =
   let e = List.ofSeq expected
   let a = List.ofSeq actual
   printMethod (e.Length, a.Length)
   e = a

//let reading_files index =
//   let path = paths.[index]
//   let read = files.[index]
//   printMethod path
//   CopyEngine.getFiles path read
//
//[<ScenarioTemplate(0)>]
//[<ScenarioTemplate(1)>]
//[<ScenarioTemplate(2)>]
//[<ScenarioTemplate(3)>]
//let ``When reading files should allow according to wildcards`` (index) =
//   Given index
//   |> When reading_files
//   |> It should have (same allowed.[index]) 
//   |> Verify

// TODO: test the copy engine by using mocks!
