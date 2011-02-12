namespace GabeSoft.FOPS.Core

open System
open System.Text
open System.Text.RegularExpressions

/// Module used to convert a wildcard pattern 
/// into a regex pattern.
/// Directory matching rules:
/// \*\               - zero or more levels
/// \?*\              - one level
/// \?*\*\ or \*\?*\  - one or more level
/// Directory and file name matching rules:
/// * matches zero or more characters and 
/// ? matches one character            
module Wildcard =
   /// Escapes the regular expression characters
   /// in the specified input.
   let private escape input = Regex.Escape (input)

   /// <summary>
   /// Normalizes all path separators.
   /// <para>
   /// Preconditions:
   /// <para> - the input is not escaped  </para>
   /// </para>
   /// </summary>
   let private normalize (input:string) = 
      (new StringBuilder(input))
         .Replace('/', Path.separator)
         .Replace('\\', Path.separator)
         .ToString()

   /// <summary>
   /// Prepares the specified input for regex conversion.
   /// Performs the following replacements:
   ///   1. *.*          -> *
   ///   2. /*$          -> />>
   ///   3. /*/*/*/../*  -> /*
   /// <para>
   /// Preconditions: 
   /// <para> - the input is normalized   </para>
   /// <para> - the input is not escaped  </para>
   /// </para>
   /// </summary>
   let private prepare input = 
      let pat1, repl1 = escape "*.*", "*"
      let pat2, repl2 =
         sprintf "%s$" ("/*" |> normalize |> escape), ("/>>" |> normalize)
      let pat3, repl3 = 
         let pat = "/*" |> normalize
         sprintf "(%s)+" (escape pat), pat
      let replace pat (repl:string) text = Regex.Replace(text, pat, repl)
      let trim (text:string) = text.Trim()
      input 
      |> trim
      |> replace pat1 repl1 
      |> replace pat2 repl2         
      |> replace pat3 repl3
   
   /// <summary>
   /// Converts the specified input into a 
   /// regular expression.
   /// <para>
   /// Preconditions:
   /// <para> - the input is normalized                     </para>
   /// <para> - the input is escaped                        </para>
   /// <para> - the input has been prepared for conversion  </para>
   /// </para>
   /// </summary>
   let private convert (input: string) =
      // the following replacements are performed in order
      // \>>   -> \\[^\\]+
      // \*\   -> \\(.>><<\\)<<
      // \?*\  -> \\[^\\]+\\
      // *     -> [^\\]*
      // ?     -> [^\\]
      // >>    -> *
      // <<    -> ?

      let separator = Path.separator.ToString() |> escape
      (new StringBuilder(input))
         .Replace("/>>" |> normalize |> escape, 
                  sprintf "%s[^%s]+" separator separator)
         .Replace("/*/" |> normalize |> escape, 
                  sprintf "%s(<<:.>><<%s)<<" separator separator)
         .Replace("/?*/" |> normalize |> escape, 
                  sprintf "%s[^%s]+%s" separator separator separator)
         .Replace("*" |> escape, sprintf "[^%s]*" separator)
         .Replace("?" |> escape, sprintf "[^%s]" separator)
         .Replace(">>", "*")
         .Replace("<<", "?")
         .Insert(0, "^")
         .Append("$")
         .ToString()

   /// Converts the specified wildcard pattern 
   /// into a regular expression.
   let toRegex pattern =
      pattern   
      |> normalize
      |> prepare
      |> escape
      |> convert

   /// Finds the longest directory path in the given 
   /// wildcard pattern that does not contain a wildcard. 
   /// This directory can be used as the search origin.
   let root pattern = 
      let rec find (pattern:string) = 
         if pattern.IndexOfAny([|'?'; '*'|]) > -1 
         then find (Path.directory pattern)
         else pattern
      if Path.root pattern = pattern 
      then pattern
      else find (Path.directory pattern)
            
   /// Returns a value that indicates whether the wildcard pattern is 
   /// folder recursive or matches only items in the parent folder.
   let isRecursive pattern =
      root pattern <> Path.directory pattern 

   /// Returns a regular expression that matches all the files
   /// in all directories in the specified folder.
   let matchAll folder = 
      let clean (s: string) = s.TrimEnd([|'/'; '\\'|])
      let pattern = sprintf "%s/*/*" (folder |> clean) 
      toRegex pattern
