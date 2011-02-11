namespace GabeSoft.FOPS.Core

open System
open System.Collections.Generic
open System.Text
open System.Text.RegularExpressions

/// Filter specification.
type FilterSpec = {
   /// A regex pattern that determines which paths to accept.
   Pattern: string
   /// A list of regex patterns that match items to 
   /// be excluded from the accepted paths.
   Exclude: string list
   /// A flag indicating whether the accept regex pattern is 
   /// folder recursive (matches items in child folders).
   Recursive: bool }

module Filter =
   let private isMatch pattern input = 
      Regex.IsMatch(input, pattern, RegexOptions.IgnoreCase)

   let private allow (accept: string) (reject: string list) path =
      let accepted = isMatch accept path 
      let rejected = reject |> List.exists (fun p -> isMatch p path)
      accepted && not rejected

   /// Determines if a path is allowed according to the
   /// specified regex pattern.
   let allowed pattern path = isMatch pattern path

   /// <summary>
   /// Applies the specified accept pattern and exclude patterns to 
   /// the given node.
   /// </summary>
   /// <param name="spec">A filter specification.</param>
   /// <param name="node">The io node on which to apply the given filters.</param>
   let rec apply (spec: FilterSpec) (node:IONode) = 
      { node with
         Files = node.Files 
                  |> Seq.filter (fun n -> allow spec.Pattern spec.Exclude n.Path)
                  |> Seq.cache
         Folders = match spec.Recursive with
                     | false  -> Seq.empty<_>
                     | true   -> node.Folders 
                                 |> Seq.map (apply spec) 
                                 |> Seq.cache }