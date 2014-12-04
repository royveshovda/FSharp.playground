open System
open System.IO
open Calculator



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
    
let file = ("..\\..\\..\\burma14.tsp", 14, 8)
//let file = ("..\\..\\..\\mona-lisa100K.tsp", 100000, 6)


let printSoultion (solution:Point list) =
    List.iter (fun (p:Point) -> printfn "%f %f" p.X p.Y) (solution)

[<EntryPoint>]
let main argv = 
    argv |> ignore

    let arr = [|
        new Point(16.47, 96.10);
        new Point(16.47, 94.44);
        new Point(20.09, 94.55);
        new Point(20.09, 92.54);
        new Point(22.39, 93.37);
        new Point(25.23, 97.24);
        new Point(22.00, 96.05);
        new Point(21.52, 95.59);
        new Point(20.47, 97.02);
        new Point(19.41, 97.13);
        new Point(17.20, 96.29);
        new Point(16.53, 97.38);
        new Point(16.30, 97.38);
        new Point(14.05, 98.12)|]

    let tempLength1 = length arr
    let tempLength2 = length2 arr

    printfn "Length1: %f" tempLength1
    printfn "Length2: %f" tempLength2
    
    Console.ReadKey() |> ignore

    0 // return an integer exit code
