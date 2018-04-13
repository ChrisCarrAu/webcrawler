open System
open System.Collections.Generic
open FSharp.Data

type SpiderNode = 
    { 
        Address : Uri
        Text : string option 
        Parent : SpiderNode option
    }

type SpiderResult =
    {
        htmlNode : HtmlNode
        href : HtmlAttribute
    }

let parseAnchors (uri : SpiderNode) = 
    try
        HtmlDocument.Load(uri.Address.ToString()).Descendants["a"]
    with
        | :? System.ArgumentException as ex -> Seq.empty
        | :? System.NotSupportedException as ex -> Seq.empty
        | :? System.Net.WebException as ex -> Seq.empty

let parseAnchorLinks (anchors : seq<HtmlNode>) =
    anchors
    |> Seq.choose ( fun htmlNode -> 
        htmlNode.TryGetAttribute("href")
        |> Option.map(fun h -> { htmlNode = htmlNode; href = h } ))

let spiderNode ( spiderResult ) ( uribase : SpiderNode ) =
    { 
        Text = Some(spiderResult.htmlNode.InnerText())
        Address = Uri(uribase.Address, spiderResult.href.Value())
        Parent = Some(uribase) 
    }

let rec crawlDepth spiderUri =
    match spiderUri.Parent with
    | None -> 0
    | Some a -> crawlDepth a + 1

let spiderAgent (crawledSet : HashSet<string>) = MailboxProcessor.Start(fun uriQueue ->
    let rec crawlLoop() = async {
        let! uri = uriQueue.Receive()

        printfn "%A %d" uri.Address (crawlDepth uri);
            
        parseAnchors(uri) 
        |> parseAnchorLinks
        |> Seq.map (fun f -> spiderNode f uri)
        |> Seq.iter (fun f -> 
            if crawlDepth f < 4 then 
                if crawledSet.Add(f.Address.GetLeftPart(UriPartial.Query)) then
                    uriQueue.Post f
            )
        return! crawlLoop()
    }
    crawlLoop()
)

[<EntryPoint>]
let main args =

    let crawledSet = HashSet<string>()

    //spiderAgent.Post { Address = Uri("http://appthem.com"); parent = None; Text = None }
    spiderAgent(crawledSet).Post { Address = Uri("http://ozhog.com.au"); Parent = None; Text = None }
    //spiderAgent.Post { Address = Uri("https://www.bikesales.com.au"); parent = None; Text = None }
    
    Console.ReadLine() |> ignore

    0 // return an integer exit code

