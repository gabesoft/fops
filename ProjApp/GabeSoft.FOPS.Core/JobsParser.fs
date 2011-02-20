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
  let private defaults = Map.ofList [ "force", true ]
  let private fail message = raise (new ParseException(message))
  let private lname (elem: XElement) = elem.Name.LocalName
  let private xname name = XName.Get (name)
  let private xattr name (elem: XElement) = 
    let attr = elem.Attribute(xname name)
    if attr = null then None else Some attr.Value
  let private xelem name (parent: XContainer) =
    let elem = parent.Element(xname name)
    if elem = null then None else Some elem
  let private xvalue (elem: option<XElement>) = 
    elem |> Option.map(fun e -> Some e.Value) 
  /// Returns the children elements with the 
  /// specified name of the given parent element.
  let private xcn name (parent: XContainer) =  parent.Elements (xname name)
  /// Returns all children elements of the given parent element.
  let private xall (parent: XContainer) = parent.Elements()

  let private getRequired (v: option<string>) errorIfMissing = 
    match v with
    | None   -> fail errorIfMissing
    | Some e -> e

  let private getOptional (v: option<string>) =
    match v with
    | None   -> String.Empty
    | Some e -> e

  let private getAttr name elem required =
    match required with
    | true   -> getRequired (xattr name elem) 
                            (sprintf "'%s' is a required attribute on '%s' element" name (lname elem))
    | false  -> getOptional (xattr name elem) 

  let private toBool def text = 
    match bool.TryParse(text) with
    | true, v   -> v
    | false, _  -> def

  let private parseExclude (elem: XElement) = 
    getAttr "src" elem true

  let private parseCopy f (elem: XElement) =
    let excludes = xcn "exclude" elem 
                    |> Seq.map parseExclude
                    |> Seq.toList
    let src = getAttr "src" elem true
    let dst = getAttr "dst" elem true
    let force = getAttr "force" elem false
    f (src, dst, force |> toBool defaults.["force"], excludes)

  let private parseYank f (elem: XElement) =
    let src = getAttr "src" elem true
    f (getAttr "src" elem true)

  let private parseItem (elem: XElement) = 
    match lname elem with
    | "copy"        -> [ parseCopy (Item.copy PatternMode) elem ]
    | "copy-file"   -> [ parseCopy (Item.copy FileMode) elem ]
    | "copy-dir"    -> [ parseCopy (Item.copy DirectoryMode) elem ]
    | "link"        -> [ parseCopy (Item.link PatternMode) elem ]
    | "link-file"   -> [ parseCopy (Item.link FileMode) elem ]
    | "link-dir"    -> [ parseCopy (Item.link DirectoryMode) elem ]
    | "delete"      -> [ parseYank (Item.yank PatternMode) elem ]
    | "delete-dir"  -> [ parseYank (Item.yank DirectoryMode) elem ]
    | "move-file"   -> [ parseCopy (Item.copy FileMode) elem
                         parseYank (Item.yank FileMode) elem ]
    | "move-dir"    -> [ parseCopy (Item.copy DirectoryMode) elem
                         parseYank (Item.yank DirectoryMode) elem ]
    | n             -> fail (sprintf "unknown element %s" n)

  let parseJob (elem: XElement) =
    let id = getAttr "id" elem true
    let baseSrc = getAttr "base-src" elem false 
    let baseDst = getAttr "base-dst" elem false 
    let items = xall elem
                |> Seq.map parseItem
                |> Seq.concat
                |> Seq.toList                
    new Job(items, id, baseSrc, baseDst)

  let parseDocument (xdoc: XDocument) = 
    xdoc.Root
    |> xcn "job" 
    |> Seq.map parseJob
    |> Seq.toList

  let parseFile filePath =
    use file  = File.OpenRead(filePath)
    let xdoc = XDocument.Load(file)
    parseDocument xdoc

