module OptionsValidatorTests

open System
open System.IO
open System.Diagnostics

open NaturalSpec
open NUnit.Framework

open GabeSoft.FOPS.Core
open GabeSoft.FOPS.Test

let validating opts =
  let log = new LogImpl()
  let validator = new OptionsValidator(log)
  validator.CreateJobs opts

[<Scenario>]
let ``From file - creates all jobs`` () =
  failwith "not implemented"

[<Scenario>]
let ``From file - creates only specified job`` () =
  failwith "not implemented"

[<Scenario>]  
let ``From file - overrides base-src and base-dst`` () =
  failwith "not implemented"
