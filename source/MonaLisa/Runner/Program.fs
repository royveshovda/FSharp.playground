open System
open System.IO

type Point =
        struct
            val X:float
            val Y:float
            new (x:float,y:float) = { X=x; Y=y }
        end


type TspFilePart =
    | NamePart of name:string
    | TypePart of fileType:string
    | CommentPart of comment:string
    | EdgeWeightTypePart of edgeWeightType:string
    | DimensionPart of dimension:int
    | PointsPart of points:Point[]

type TspFile = {
    Name: string;
    Type: string;
    Comment: string;
    EdgeWeightType: string;
    Dimension: int;
    Points: Point[]}

let readFile (filename:string) size linesToSkip =
    use fs = File.OpenText(filename)
    let text = fs.ReadToEnd()
    //TODO: Improve parsing here
    let points =
        text.Split ('\n')
        |> Seq.skip linesToSkip
        |> Seq.take size
        |> Seq.map (fun line -> line.TrimStart().TrimEnd())
        //|> Seq.map (fun line -> line.Replace('.', ','))
        |> Seq.map (fun line -> line.Replace("  ", " ").Replace("  ", " ").Replace("  ", " ").Replace("  ", " ").Replace("  ", " ").Replace("  ", " ").Replace("  ", " "))
        |> Seq.map (fun line ->
            let fields = line.Split(' ')
            Point(float fields.[1],float fields.[2]))
        |> Seq.toArray
    points
    
let file = ("C:\\TestData\\burma14.tsp", 14, 8)
//let file = ("C:\\TestData\\mona-lisa100K.tsp", 100000, 6)

[<EntryPoint>]
let main argv = 
    argv |> ignore
    let (filename, size, linesToSkip) = file
    let points = readFile filename size linesToSkip

    printfn "Point read: %i" points.Length
    Console.ReadKey() |> ignore

    0 // return an integer exit code
