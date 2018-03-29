open FSharp.Data
open System

type SpiderUri = 
    { 
        Address : Uri;
        Text : string option;
        parent : SpiderUri option 
    }

type SpiderResult =
    {
        htmlNode : HtmlNode;
        href : HtmlAttribute;
    }

[<EntryPoint>]
let main args =

    let parseAnchors (uri : SpiderUri) = 
        HtmlDocument.Load(uri.Address.ToString()).Descendants["a"]

    let parseAnchorLinks (anchors : seq<HtmlNode>) =
        anchors
        |> Seq.choose ( fun htmlNode -> 
            htmlNode.TryGetAttribute("href")
            |> Option.map(fun h -> { htmlNode = htmlNode; href = h } ))

    let spiderUri ( spiderResult ) ( uribase : SpiderUri ) =
        { Text = Some(spiderResult.htmlNode.InnerText()); Address = Uri(uribase.Address, spiderResult.href.Value()); parent = Some(uribase) }

    let rec crawlDepth spiderUri =
        match spiderUri.parent with
        | None -> 0
        | Some a -> crawlDepth a + 1

    let spiderAgent = MailboxProcessor.Start(fun uriQueue->
        let rec crawlLoop() = async {
            let! uri = uriQueue.Receive()
            
            parseAnchors(uri) 
            |> parseAnchorLinks
            |> Seq.map (fun f -> spiderUri f uri)
            |> Seq.iter (fun f -> 
                printfn "%A %d" f.Address (crawlDepth f);
                if crawlDepth f < 2 then 
                    uriQueue.Post f
                )

            return! crawlLoop()
        }

        crawlLoop()
    )

    spiderAgent.Post { Address = Uri("http://appthem.com"); parent = None; Text = None }
    //spiderAgent.Post { Address = Uri("https://www.bikesales.com.au"); crawlDepth = 0; Text = None }
    
    Console.ReadLine() |> ignore

    0 // return an integer exit code

