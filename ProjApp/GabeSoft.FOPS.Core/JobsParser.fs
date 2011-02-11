namespace GabeSoft.FOPS.Core

open System
open System.IO
open System.Xml
open System.Xml.Linq

open GabeSoft.FOPS.Core

type ParseException (message:string, ?innerException:Exception) =
   inherit Exception (
      message, 
      match innerException with | Some ex -> ex | None -> null)

module JobsParser =
   let fail message = raise (ParseException(message))
   let lname (elem: XElement) = elem.Name.LocalName
   let xname name = XName.Get (name)
   let xattr name (elem: XElement) = 
      let attr = elem.Attribute(xname name)
      if attr = null then None else Some attr.Value
   let xelem name (parent: XContainer) =
      let elem = parent.Element(xname name)
      if elem = null then None else Some elem
   let xvalue (elem: option<XElement>) = 
      elem |> Option.map(fun e -> Some e.Value) 
   let xsons name (parent: XContainer) = 
      parent.Elements (xname name)

   let getRequired (v: option<string>) errorIfMissing = 
      match v with
      | None   -> fail errorIfMissing
      | Some e -> e

   let getOptional (v: option<string>) =
      match v with
      | None   -> String.Empty
      | Some e -> e

   let getAttr name elem required =
      match required with
      | true   -> getRequired (xattr name elem) 
                              (sprintf "'%s' is a required attribute on '%s' element" name (lname elem))
      | false  -> getOptional (xattr name elem) 

   let toBool def text = 
      match bool.TryParse(text) with
      | true, v   -> v
      | false, _  -> def

   let parseExclude (elem: XElement) = 
      getAttr "path" elem true

   let parseCopy f (elem: XElement) =
      let excludes = xsons "exclude" elem 
                     |> Seq.map parseExclude
                     |> Seq.toList
      let from = getAttr "from" elem true
      let to' = getAttr "to" elem true
      let overwrite = getAttr "overwrite" elem false
      f (from, to', overwrite |> toBool true, excludes)

   let parseYank (elem: XElement) =
      Yank (getAttr "from" elem true)

   let parseItem (elem: XElement) = 
      match lname elem with
      | "copy"       -> parseCopy (Item.copy Pattern) elem
      | "copy-file"  -> parseCopy (Item.copy File) elem
      | "copy-dir"   -> parseCopy (Item.copy Folder) elem
      | "link"       -> parseCopy (Item.link Pattern) elem
      | "link-file"  -> parseCopy (Item.link File) elem
      | "link-dir"   -> parseCopy (Item.link Folder) elem
      | "yank"       -> parseYank elem
      | n            -> fail (sprintf "unknown element %s" n)

   let parseJob (elem: XElement) =
      let id = getAttr "id" elem true
      let basePath = getAttr "basePath" elem false 
      let items = xsons "copy" elem      
                  |> Seq.append (xsons "copy-file" elem)
                  |> Seq.append (xsons "copy-dir" elem)
                  |> Seq.append (xsons "link" elem)
                  |> Seq.append (xsons "link-file" elem)
                  |> Seq.append (xsons "link-dir" elem)
                  |> Seq.append (xsons "yank" elem)
                  |> Seq.map parseItem
                  |> Seq.toList
      new Job(items, id, basePath)

   let parseDocument (xdoc: XDocument) = 
      xdoc.Root
      |> xsons "job" 
      |> Seq.map parseJob
      |> Seq.toList

   let parseFile filePath =
      use file  = File.OpenRead(filePath)
      let xdoc = XDocument.Load(file)
      parseDocument xdoc

