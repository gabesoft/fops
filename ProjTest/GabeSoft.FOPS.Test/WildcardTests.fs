module WildcardTests

open System
open System.IO
open System.Diagnostics
open System.Text
open System.Text.RegularExpressions
open System.Globalization

open NaturalSpec
open NUnit.Framework

open GabeSoft.FOPS.Core

let values = [
   @"C:\*.*", @"^C:\\[^\\]+$"
   @"C:\*", @"^C:\\[^\\]+$"
   @"C:\*\*", @"^C:\\(?:.*?\\)?[^\\]+$"
   @"C:\?*\*", @"^C:\\[^\\]+\\[^\\]+$"
   @"C:\a\*", @"^C:\\a\\[^\\]+$"
   @"C:\a\?", @"^C:\\a\\[^\\]$"
   @"C:\a\b*\*", @"^C:\\a\\b[^\\]*\\[^\\]+$"
   @"C:\a\b*\*.pdf", @"^C:\\a\\b[^\\]*\\[^\\]*\.pdf$"
   @"C:\*foo", @"^C:\\[^\\]*foo$" 
   @"C:\a*foo", @"^C:\\a[^\\]*foo$" 
   @"C:\a?foo", @"^C:\\a[^\\]foo$" 
   @"C:\*\f.txt", @"^C:\\(?:.*?\\)?f\.txt$"
   @"C:\?*\f.txt", @"^C:\\[^\\]+\\f\.txt$"
   @"C:\a\*\b\c.txt", @"^C:\\a\\(?:.*?\\)?b\\c\.txt$"
   @"C:\a\*\*\*.*\b\c.txt", @"^C:\\a\\(?:.*?\\)?b\\c\.txt$"
   @"C:\a\?*\b\c.txt", @"^C:\\a\\[^\\]+\\b\\c\.txt$"
   @"C:\a\?*\*\b\c.txt", @"^C:\\a\\[^\\]+\\(?:.*?\\)?b\\c\.txt$" 
   @"C:\a\b\c\f.bat", @"^C:\\a\\b\\c\\f\.bat$"
   @"C:\a\*\bin\*\*.dll", @"^C:\\a\\(?:.*?\\)?bin\\(?:.*?\\)?[^\\]*\.dll$"
   @"C:\a\*.*\*.*\*\c\*.*", @"^C:\\a\\(?:.*?\\)?c\\[^\\]+$" ]

let converting_to (pattern, expected) =
   let actual = Wildcard.toRegex pattern
   printMethod actual
   expected, actual

let outcome (expected, actual) =
   printMethod expected
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
[<ScenarioTemplate(13)>]
[<ScenarioTemplate(14)>]
[<ScenarioTemplate(15)>]
[<ScenarioTemplate(16)>]
[<ScenarioTemplate(17)>]
[<ScenarioTemplate(18)>]
[<ScenarioTemplate(19)>]
let ``Converting a wildcard pattern yields proper regex`` (index) =
   Given values.[index]
   |> When converting_to
   |> It should have outcome
   |> Verify