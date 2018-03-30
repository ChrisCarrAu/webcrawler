open FSharp.Data
open System
open System.Collections.Generic

type SpiderNode = 
    { 
        Address : Uri;
        Text : string option;
        parent : SpiderNode option 
    }

type SpiderResult =
    {
        htmlNode : HtmlNode;
        href : HtmlAttribute;
    }

[<EntryPoint>]
let main args =

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

    let spiderUri ( spiderResult ) ( uribase : SpiderNode ) =
        { Text = Some(spiderResult.htmlNode.InnerText()); Address = Uri(uribase.Address, spiderResult.href.Value()); parent = Some(uribase) }

    let rec crawlDepth spiderUri =
        match spiderUri.parent with
        | None -> 0
        | Some a -> crawlDepth a + 1

    let crawledSet = HashSet<string>()

    let spiderAgent = MailboxProcessor.Start(fun uriQueue->
        let rec crawlLoop() = async {
            let! uri = uriQueue.Receive()

            printfn "%A %d" uri.Address (crawlDepth uri);
            
            parseAnchors(uri) 
            |> parseAnchorLinks
            |> Seq.map (fun f -> spiderUri f uri)
            |> Seq.iter (fun f -> 
                if crawlDepth f < 4 then 
                    if crawledSet.Add(f.Address.GetLeftPart(UriPartial.Path)) then
                        uriQueue.Post f
                )

            return! crawlLoop()
        }

        crawlLoop()
    )

    spiderAgent.Post { Address = Uri("http://appthem.com"); parent = None; Text = None }
    //spiderAgent.Post { Address = Uri("https://www.bikesales.com.au"); parent = None; Text = None }
    
    Console.ReadLine() |> ignore

    0 // return an integer exit code

